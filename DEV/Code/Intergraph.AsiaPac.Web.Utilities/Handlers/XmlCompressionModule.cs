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

namespace Intergraph.AsiaPac.Web.Utilities.Handlers
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
