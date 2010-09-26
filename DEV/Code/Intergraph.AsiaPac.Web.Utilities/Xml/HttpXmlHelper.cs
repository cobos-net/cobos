using System;
using System.Text;
using System.Web;
using Intergraph.AsiaPac.Utilities.Xml;

namespace Intergraph.AsiaPac.Web.Utilities.Xml
{
	public static class HttpXmlHelper<T>
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="context"></param>
		public static void Serialize( T entity, HttpContext context )
		{
			//Setup the response
			context.Response.ContentType = "text/xml";
			context.Response.Cache.SetNoStore();
			context.Response.Cache.SetNoServerCaching();
			context.Response.ContentEncoding = Encoding.UTF8;
			context.Response.Cache.SetCacheability( HttpCacheability.NoCache );

			XmlHelper<T>.Serialize( entity, context.Response.OutputStream );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public static T Deserialize( HttpContext context )
		{
			return XmlHelper<T>.Deserialize( context.Request.InputStream );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <param name="response"></param>
		/// <param name="prefix"></param>
		/// <param name="ns"></param>
		public static void Serialize( HttpContext context, T response, string prefix, string ns )
		{
			// Setup the response
			context.Response.ContentType = "text/xml";
			context.Response.Cache.SetNoStore();

			XmlHelper<T>.Serialize( context.Response.OutputStream, response, prefix, ns );
		}
	}
}
