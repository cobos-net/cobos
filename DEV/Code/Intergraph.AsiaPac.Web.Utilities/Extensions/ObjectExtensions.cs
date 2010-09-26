using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace Intergraph.AsiaPac.Web.Utilities.Extensions
{
	public static class ObjectExtensions
	{
		/// <summary>
		/// serialize the object to Json
		/// </summary>
		/// <param name="obj">The 'this' object reference</param>
		/// <returns>String representation of the object in JSON notation</returns>
		public static string ToJson( this object obj )
		{
			JavaScriptSerializer serializer = new JavaScriptSerializer();
			return serializer.Serialize( obj );
		}
	}
}
