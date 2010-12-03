using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Xml.Serialization;
using System.IO;
using System.Threading;
using System.Xml;
using Intergraph.AsiaPac.Utilities.Xml;

namespace Intergraph.AsiaPac.Web.Utilities.Handlers
{
	/// <summary>
	/// Manages requesting XML data from a remote server
	/// </summary>
	/// <typeparam name="RequestType">Request object class</typeparam>
	/// <typeparam name="ResponseType">Response object class</typeparam>
	class HttpXmlQuery<RequestType, ResponseType>
	{
		private HttpXmlQuery()
		{
		}

		/// <summary>
		/// Query the server for the response data 
		/// </summary>
		/// <param name="url">Url to send the data to</param>
		/// <param name="request">Object to send to the server</param>
		/// <returns>Object returned by the server</returns>
		/// <exception cref="CentralServerException">Thrown when error communicating with the server occurs</exception>
		public static ResponseType QueryServer( Uri url, string sessionid, RequestType request )
		{
			return QueryServer( url, sessionid, request, 30000 );
		}

		public static ResponseType QueryServer( Uri url, string sessionid, RequestType data, int timeout )
		{
			//Create a new http request
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create( url );
			//We are posting
			request.Method = "POST";
			//Set the time out
			request.Timeout = timeout;
			request.AutomaticDecompression = DecompressionMethods.GZip;
			request.CookieContainer = new CookieContainer();

			if ( (sessionid != null) && (sessionid != string.Empty) )
			{
				request.CookieContainer.Add( url, new Cookie( "ASP.NET_SessionId", sessionid ) );
			}

			Stream output = request.GetRequestStream();
			try
			{
				XmlHelper<RequestType>.Serialize( output, data, null, null );
			}
			finally
			{
				output.Close();
			}

			//Now get the response
			HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			try
			{
				//Deserialise the object in the response.
				return XmlHelper<ResponseType>.Deserialize( response.GetResponseStream() );
			}
			finally
			{
				response.Close();
			}
		}

		public static XmlDocument QueryServerAsXml( Uri url, string sessionid, RequestType request )
		{
			return QueryServerAsXml( url, sessionid, request, 30000 );
		}

		public static XmlDocument QueryServerAsXml( Uri url, string sessionid, RequestType data, int timeout )
		{
			//Create a new http request
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create( url );
			//We are posting
			request.Method = "POST";
			//Set the time out
			request.Timeout = timeout;
			request.AutomaticDecompression = DecompressionMethods.GZip;
			request.CookieContainer = new CookieContainer();

			if ( (sessionid != null) && (sessionid != string.Empty) )
			{
				request.CookieContainer.Add( url, new Cookie( "ASP.NET_SessionId", sessionid ) );
			}

			Stream output = request.GetRequestStream();
			try
			{
				XmlHelper<RequestType>.Serialize( output, data, null, null );
			}
			finally
			{
				output.Close();
			}

			//Now get the response
			HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			try
			{
				//Deserialise the object in the response.
				XmlDocument doc = new XmlDocument();
				doc.Load( response.GetResponseStream() );
				return doc;
			}
			finally
			{
				response.Close();
			}
		}
	}
}
