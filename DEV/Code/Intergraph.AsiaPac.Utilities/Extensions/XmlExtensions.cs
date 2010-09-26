using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Intergraph.AsiaPac.Utilities.Extensions
{
	public static class XmlExtensions
	{
		/// <summary>
		/// Utility function to extract attribute values
		/// </summary>
		/// <param name="attributes"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static string GetAnyAttributeValue( this System.Xml.XmlAttribute[] attributes, string name )
		{
			foreach ( XmlAttribute attribute in attributes )
			{
				if ( attribute.Name == name )
				{
					return attribute.Value;
				}
			}
			return null;
		}
	}
}
