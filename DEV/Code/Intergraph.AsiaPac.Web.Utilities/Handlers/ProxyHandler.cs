using System;
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

namespace Intergraph.AsiaPac.Web.Utilities.Handlers
{
	public static class ProxyHandler
	{
		static public void Process( Page page, string key )
		{
			HttpResponse pageResponse = page.Response;
			HttpRequest pageRequest = page.Request;
			HttpSessionState Session = page.Session;
			key = key + "_SessionID";

			try
			{
				Uri url = new Uri( pageRequest[ "url" ] );

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

				if ( pageRequest.RequestType.ToUpper() == "POST" )
				{
					//Create the first set of binary readers/writers
					BinaryReader reader1 = new BinaryReader( pageRequest.InputStream );
					Stream requeststream = proxyrequest.GetRequestStream();
					requeststream.Write( reader1.ReadBytes( (int)pageRequest.ContentLength ), 0, (int)pageRequest.ContentLength );
					requeststream.Close();
				}

				//Get the data
				HttpWebResponse proxyresponse = (HttpWebResponse)proxyrequest.GetResponse();
				try
				{
					pageResponse.ContentType = proxyresponse.ContentType;
					pageResponse.Cache.SetNoServerCaching();
					pageResponse.Cache.SetNoStore();
					pageResponse.Cache.SetCacheability( HttpCacheability.NoCache );

					//Get the session id
					Cookie sessioncookie = proxyresponse.Cookies[ "ASP.NET_SessionId" ];
					if ( sessioncookie != null )
					{
						Session[ key ] = sessioncookie.Value;
					}
					BinaryReader reader2 = new BinaryReader( proxyresponse.GetResponseStream() );
					BinaryWriter writer2 = new BinaryWriter( pageResponse.OutputStream );

					//Copy from one stream to the other
					writer2.Write( reader2.ReadBytes( (int)proxyresponse.ContentLength ) );
					reader2.Close();
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
				pageResponse.Close();
			}
		}
	}
}
