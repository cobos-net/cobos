// ============================================================================
// Filename: XmlCompressionModule.cs
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
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO.Compression;
using System.Globalization;

namespace Cobos.Web.Utilities.Handlers
{
	public class XmlCompressionModule : IHttpModule
	{
		#region IHttpModule Members

		public void Dispose()
		{
		}

		public void Init( HttpApplication context )
		{
			context.PostRequestHandlerExecute += new EventHandler( Compress );
		}

		#endregion

		private void Compress( object sender, EventArgs e )
		{
			HttpApplication app = (HttpApplication)sender;
			HttpRequest request = app.Request;
			HttpResponse response = app.Response;

			// Are we returning XML?
			if ( response.ContentType.ToLower( CultureInfo.InvariantCulture ).StartsWith( "text/xml" ) )
			{
				// Get the encoding accepted by the client
				string acceptEncoding = request.Headers[ "Accept-Encoding" ];
				// Make sure an encoding is specified
				if ( !string.IsNullOrEmpty( acceptEncoding ) )
				{
					// Make the encoding lower case
					acceptEncoding = acceptEncoding.ToLower( CultureInfo.InvariantCulture );
					// Does it accept GZIP?
					if ( acceptEncoding.Contains( "gzip" ) )
					{
						response.Filter = new GZipStream( response.Filter, CompressionMode.Compress );
						response.AddHeader( "Content-encoding", "gzip" );
					}
					// Does it accept deflate?
					else if ( acceptEncoding.Contains( "deflate" ) )
					{
						response.Filter = new DeflateStream( response.Filter, CompressionMode.Compress );
						response.AddHeader( "Content-encoding", "deflate" );
					}
				}
			}
		}
	}
}
