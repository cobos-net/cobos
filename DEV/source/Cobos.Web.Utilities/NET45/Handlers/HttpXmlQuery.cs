// ----------------------------------------------------------------------------
// <copyright file="HttpXmlQuery.cs" company="Cobos SDK">
//
//      Copyright (c) 2009-2012 Nicholas Davis - nick@cobos.co.uk
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
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Threading;
    using System.Xml;
    using System.Xml.Serialization;
    using Cobos.Utilities.Xml;

    /// <summary>
    /// Manages requesting XML data from a remote server
    /// </summary>
    /// <typeparam name="RequestType">Request object class</typeparam>
    /// <typeparam name="ResponseType">Response object class</typeparam>
    public static class HttpXmlQuery<RequestType, ResponseType>
    {
        /// <summary>
        /// Query the remote server for the response data.
        /// </summary>
        /// <param name="url">Url to send the data to.</param>
        /// <param name="sessionid">The session id on the remote server.</param>
        /// <param name="data">Object to send to the server.</param>
        /// <returns>Object returned by the server.</returns>
        public static ResponseType QueryServer(Uri url, string sessionid, RequestType data)
        {
            return QueryServer(url, sessionid, data, 30000);
        }

        /// <summary>
        /// Query the server for the response data.
        /// </summary>
        /// <param name="url">URL of the remote server.</param>
        /// <param name="sessionid">The session id on the remote server.</param>
        /// <param name="data">Object to send to the server</param>
        /// <param name="timeout">The maximum timeout for the request in milliseconds.</param>
        /// <returns>The response data from the server.</returns>
        public static ResponseType QueryServer(Uri url, string sessionid, RequestType data, int timeout)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.Timeout = timeout;
            request.AutomaticDecompression = DecompressionMethods.GZip;
            request.CookieContainer = new CookieContainer();

            if ((sessionid != null) && (sessionid != string.Empty))
            {
                request.CookieContainer.Add(url, new Cookie("ASP.NET_SessionId", sessionid));
            }

            using (var output = request.GetRequestStream())
            {
                XmlHelper<RequestType>.Serialize(data, output, null, null);
            }
            
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                return XmlHelper<ResponseType>.Deserialize(response.GetResponseStream());
            }
        }

        /// <summary>
        /// Query the remote server for the response data.
        /// </summary>
        /// <param name="url">Url to send the data to.</param>
        /// <param name="sessionid">The session id on the remote server.</param>
        /// <param name="data">Object to send to the server.</param>
        /// <returns>Object returned by the server.</returns>
        public static XmlDocument QueryServerAsXml(Uri url, string sessionid, RequestType data)
        {
            return QueryServerAsXml(url, sessionid, data, 30000);
        }

        /// <summary>
        /// Query the server for the response data.
        /// </summary>
        /// <param name="url">URL of the remote server.</param>
        /// <param name="sessionid">The session id on the remote server.</param>
        /// <param name="data">Object to send to the server</param>
        /// <param name="timeout">The maximum timeout for the request in milliseconds.</param>
        /// <returns>The response data from the server.</returns>
        public static XmlDocument QueryServerAsXml(Uri url, string sessionid, RequestType data, int timeout)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.Timeout = timeout;
            request.AutomaticDecompression = DecompressionMethods.GZip;
            request.CookieContainer = new CookieContainer();

            if ((sessionid != null) && (sessionid != string.Empty))
            {
                request.CookieContainer.Add(url, new Cookie("ASP.NET_SessionId", sessionid));
            }

            using (var output = request.GetRequestStream())
            {
                XmlHelper<RequestType>.Serialize(data, output, null, null);
            }

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(response.GetResponseStream());
                return doc;
            }
        }
    }
}
