// ----------------------------------------------------------------------------
// <copyright file="HttpXmlHelper.cs" company="Cobos SDK">
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

namespace Cobos.Web.Utilities.Xml
{
    using System;
    using System.Text;
    using System.Web;
    using Cobos.Utilities.Xml;

    /// <summary>
    /// Helper class for serializing and de-serializing objects to HTTP contexts.
    /// </summary>
    /// <typeparam name="T">The type object.</typeparam>
    public static class HttpXmlHelper<T>
    {
        /// <summary>
        /// Serialize an object to an HTTP response.
        /// </summary>
        /// <param name="entity">The object to serialize.</param>
        /// <param name="context">An HTTP context.</param>
        public static void Serialize(T entity, HttpContext context)
        {
            context.Response.ContentType = "text/xml";
            context.Response.Cache.SetNoStore();
            context.Response.Cache.SetNoServerCaching();
            context.Response.ContentEncoding = Encoding.UTF8;
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            XmlHelper.Serialize<T>(entity, context.Response.OutputStream);
        }

        /// <summary>
        /// Deserialize an HTTP request to an object representation.
        /// </summary>
        /// <param name="context">An HTTP context.</param>
        /// <returns>The deserialized object.</returns>
        public static T Deserialize(HttpContext context)
        {
            return XmlHelper.Deserialize<T>(context.Request.InputStream);
        }

        /// <summary>
        /// Serialize an object to the HTTP response.
        /// </summary>
        /// <param name="context">An HTTP context.</param>
        /// <param name="entity">The entity to serialize.</param>
        /// <param name="prefix">The prefix associated with an XML namespace.</param>
        /// <param name="ns">An XML namespace.</param>
        public static void Serialize(HttpContext context, T entity, string prefix, string ns)
        {
            context.Response.ContentType = "text/xml";
            context.Response.Cache.SetNoStore();
            XmlHelper.Serialize<T>(entity, context.Response.OutputStream, prefix, ns);
        }
    }
}
