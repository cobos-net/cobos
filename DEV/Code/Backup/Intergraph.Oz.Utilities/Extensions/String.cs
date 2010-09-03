using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Security;

namespace Intergraph.Oz.Utilities.Extensions
{
	public static class String
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static string CamelCase( this string s )
		{
			StringBuilder result = new StringBuilder( s.Length );

			string[] tokens = s.Split( ' ', '\n', '/', '\\', '<', '>', '&', '?', '#', '!', '%', '\"', '\'' );

			foreach ( string t in tokens )
			{
				if ( string.IsNullOrEmpty( t ) )
				{
					continue;
				}
				result.AppendFormat( "{0}{1}", char.ToUpper( t[ 0 ] ), t.Substring( 1 ) );
			}

			return result.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static string EscapeXml( this string s )
		{
			if ( s == string.Empty )
			{
				return s;
			}

			return SecurityElement.Escape( s );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static string UnescapeXml( this string s )
		{
			return HttpUtility.HtmlDecode( s );
		}

		/// <summary>
		/// deserialize from Json
		/// </summary>
		/// <param name="str">The 'this' string reference</param>
		/// <returns>Deserialized object</returns>
		public static object FromJson( this string str )
		{
			JavaScriptSerializer serializer = new JavaScriptSerializer();
			return serializer.DeserializeObject( str );
		}

		/// <summary>
		/// Deserialize from Json as a specified type
		/// </summary>
		/// <typeparam name="T">The type to cast to</typeparam>
		/// <param name="str">The 'this' string reference</param>
		/// <param name="example">An instance of an anonymous type</param>
		/// <returns>A reference to the anonymous type.  If the cast fails, then null.</returns>
		public static T FromJsonAs<T>( this string str, T example )
		{
			JavaScriptSerializer serializer = new JavaScriptSerializer();
			object json = serializer.DeserializeObject( str );

			if ( json is Dictionary<string, object> )
			{
				return ((Dictionary<string, object>)json).CastByExample( example );
			}
			else
			{
				// this may or may not work depending on the object type
				return json.CastByExample( example );
			}
		}
	}
}
