// ============================================================================
// Filename: ProxyHandler.cs
// Description: 
// ----------------------------------------------------------------------------
// Created by: N.Davis                          Date: 27-Nov-09
// Updated by:                                  Date:
// ============================================================================
// Copyright (c) 2009-2012 Nicholas Davis		nick@cobos.co.uk
//
// Cobos Software Development Kit
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ============================================================================

using System;
using System.Collections.Specialized;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Web.SessionState;
using System.Net;
using System.IO;

namespace Cobos.Web.Utilities.Handlers
{
	public class ProxyHandler : IHttpHandler
	{
		public bool IsReusable
		{
			get
			{
				return true;
			}
		}

		public void ProcessRequest( HttpContext context )
		{
			HttpResponse pageResponse = context.Response;
			HttpRequest pageRequest = context.Request;
			HttpSessionState Session = context.Session;
			string key = "CobosProxyHandler_SessionID";

			try
			{
				Uri url = new Uri( ParseUrl( context.Request.Url.AbsoluteUri ) );

				HttpWebRequest proxyrequest = (HttpWebRequest)HttpWebRequest.Create( url );
				
				proxyrequest.ContentType = pageRequest.ContentType;
				proxyrequest.Method = pageRequest.RequestType;
				proxyrequest.AutomaticDecompression = DecompressionMethods.GZip;
				proxyrequest.CookieContainer = new CookieContainer();
				
				string sessionid = Session[ key ] as string;
				if ( (sessionid != null) && (sessionid != string.Empty) )
				{
					proxyrequest.CookieContainer.Add( url, new Cookie( "ASP.NET_SessionId", sessionid ) );
				}

				CopyHeaders( proxyrequest, pageRequest );

				if ( pageRequest.RequestType.ToUpper() == "POST" )
				{
					//Create the first set of binary readers/writers
					BinaryReader reader1 = new BinaryReader( pageRequest.InputStream );
					Stream requeststream = proxyrequest.GetRequestStream();
					requeststream.Write( reader1.ReadBytes( (int)pageRequest.ContentLength ), 0, (int)pageRequest.ContentLength );
					requeststream.Close();
				}

				// Get the data
				HttpWebResponse proxyresponse = (HttpWebResponse)proxyrequest.GetResponse();

				try
				{
					pageResponse.ContentType = proxyresponse.ContentType;
					pageResponse.Cache.SetNoServerCaching();
					pageResponse.Cache.SetNoStore();
					pageResponse.Cache.SetCacheability( HttpCacheability.NoCache );

					// Get the session id
					Cookie sessioncookie = proxyresponse.Cookies[ "ASP.NET_SessionId" ];
					if ( sessioncookie != null )
					{
						Session[ key ] = sessioncookie.Value;
					}

					try
					{
						pageResponse.Headers.Add( proxyresponse.Headers );
					}
					catch ( PlatformNotSupportedException )
					{
						// This operation is only supported in IIS 7.0
						// Integrated pipeline mode with at least .NET 3.0.
						// http://msdn.microsoft.com/en-us/library/system.web.httpresponse.headers.aspx
					}

					BinaryReader reader2 = new BinaryReader( proxyresponse.GetResponseStream() );
					BinaryWriter writer2 = new BinaryWriter( pageResponse.OutputStream );

					// Copy from one stream to the other
					writer2.Write( reader2.ReadBytes( (int)proxyresponse.ContentLength ) );
					reader2.Close();

					// Copy the page headers
					//foreach ( string headerKey in proxyresponse.Headers.Keys )
					//{
					//   pageResponse.Headers[ headerKey ] = proxyresponse.Headers[ headerKey ];
					//}

				}
				finally
				{
					proxyresponse.Close();
				}
			}
			catch ( Exception ex )
			{
				pageResponse.StatusCode = 500;
				pageResponse.StatusDescription = String.Format( "InternalServerError: {0}\n{1}", ex.Message, pageRequest[ "url" ] );
				//pageResponse.Close();
			}
		}

		public string ParseUrl( string url )
		{
			int pos = url.ToLower().IndexOf( "__proxy__" );

			if ( pos > 0 )
			{
				return url.Substring( pos ).Replace( "__proxy__", "http:/" );
			}
			else
			{
				return url;
			}
		}

		/// <summary>
		/// Not all HTTP headers can be set via the Headers Name/Value collection,
		/// they are considered restricted and can only be set via the API or not
		/// at all.
		/// http://msdn.microsoft.com/en-us/library/system.net.webheadercollection.aspx
		/// </summary>
		/// <param name="target"></param>
		/// <param name="source"></param>
		public static void CopyHeaders( HttpWebRequest target, HttpRequest source )
		{
			NameValueCollection headers = source.Headers;

			foreach ( string name in headers.Keys )
			{
				switch ( name )
				{
				case "Accept":
					target.Accept = headers[ name ];
					break;

				case "Connection":
					//target.Connection = headers[ name ];
					break;

				case "Content-Length":
					target.ContentLength = source.ContentLength;
					break;

				case "Content-Type":
					target.ContentType = source.ContentType;
					break;

				case "Date":
					target.Date = DateTime.Parse( headers[ name ] );
					break;

				case "Expect":
					target.Expect = headers[ name ];
					break;

				case "Host":
					target.Host = headers[ name ];
					break;

				case "If-Modified-Since":
					target.IfModifiedSince = DateTime.Parse( headers[ name ] );
					break;

				case "Range":
					// no such property
					break;

				case "Referer":
					target.Referer = headers[ name ];
					break;

				case "Transfer-Encoding":
					target.TransferEncoding = headers[ name ];
					break;

				case "User-Agent":
					target.UserAgent = headers[ name ];
					break;

				case "Proxy-Connection":
					// no such property
					break;

				default:
					target.Headers.Add( name, headers[ name ] );
					break;
				}

			}
		}

		/// <summary>
		/// Not all HTTP headers can be set via the Headers Name/Value collection,
		/// they are considered restricted and can only be set via the API or not
		/// at all.
		/// http://msdn.microsoft.com/en-us/library/system.net.webheadercollection.aspx
		/// </summary>
		/// <param name="target"></param>
		/// <param name="source"></param>
		public static void CopyHeaders( HttpResponse target, HttpWebResponse source )
		{
			NameValueCollection headers = source.Headers;

			foreach ( string name in headers.Keys )
			{
				switch ( name )
				{
				case "Accept":
					// no such property
					break;

				case "Connection":
					// no such property
					break;

				case "Content-Length":
					break;

				case "Content-Type":
					break;

				case "Date":
					break;

				case "Expect":
					break;

				case "Host":
					break;

				case "If-Modified-Since":
					break;

				case "Range":
					break;

				case "Referer":
					break;

				case "Transfer-Encoding":
					break;

				case "User-Agent":
					break;

				case "Proxy-Connection":
					break;

				default:
					target.Headers.Add( name, source.Headers[ name ] );
					break;
				}

			}

		}
	}
}
