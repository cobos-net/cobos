// ----------------------------------------------------------------------------
// <copyright file="XmlCompressionModule.cs" company="Cobos SDK">
//
//      Copyright (c) 2009-2014 Nicholas Davis - nick@cobos.co.uk
//
//      Cobos Software Development Kit
//
//      Permission is hereby granted, free of charge, to any person obtaining
//      a copy of this software and associated documentation files (the
//      "Software"), to deal in the Software without restriction, including
//      without limitation the rights to use, copy, modify, merge, publish,
//      distribute, sublicense, and/or sell copies of the Software, and to
//      permit persons to whom the Software is furnished to do so, subject to
//      the following conditions:
//      
//      The above copyright notice and this permission notice shall be
//      included in all copies or substantial portions of the Software.
//      
//      THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//      EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//      MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//      NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//      LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
//      OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
//      WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Web.Utilities.Handlers
{
    using System;
    using System.Configuration;
    using System.Globalization;
    using System.IO.Compression;
    using System.Web;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;
    using System.Web.UI.WebControls.WebParts;

    /// <summary>
    /// HTTP module to compress the response stream.
    /// </summary>
    public class XmlCompressionModule : IHttpModule
    {
        /// <summary>
        /// Dispose the module
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Initialize the context.
        /// </summary>
        /// <param name="context">The HTTP application context.</param>
        public void Init(HttpApplication context)
        {
            context.PostRequestHandlerExecute += new EventHandler(this.Compress);
        }

        /// <summary>
        /// Delegate called by the HTTP application context.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void Compress(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;
            HttpRequest request = app.Request;
            HttpResponse response = app.Response;

            if (response.ContentType.ToLower(CultureInfo.InvariantCulture).StartsWith("text/xml"))
            {
                string acceptEncoding = request.Headers["Accept-Encoding"];

                if (!string.IsNullOrEmpty(acceptEncoding))
                {
                    acceptEncoding = acceptEncoding.ToLower(CultureInfo.InvariantCulture);

                    if (acceptEncoding.Contains("gzip"))
                    {
                        response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
                        response.AddHeader("Content-encoding", "gzip");
                    }
                    else if (acceptEncoding.Contains("deflate"))
                    {
                        response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
                        response.AddHeader("Content-encoding", "deflate");
                    }
                }
            }
        }
    }
}