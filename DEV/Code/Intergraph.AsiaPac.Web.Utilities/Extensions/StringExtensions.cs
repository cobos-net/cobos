using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Intergraph.AsiaPac.Utilities.Extensions;

namespace Intergraph.AsiaPac.Web.Utilities.Extensions
{
	public static class StringExtensions
	{
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
