using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Intergraph.AsiaPac.Utilities.Xml
{
	public class XslResolver : XmlResolver
	{
		string _namespace;
		Assembly _assembly;

		public XslResolver( Assembly assembly, string ns )
		{
			_assembly = assembly;
			_namespace = ns;
		}

		public override System.Net.ICredentials Credentials
		{
			set { throw new Exception( "The method or operation is not implemented." ); }
		}

		public override object GetEntity( Uri absoluteUri, string role, Type ofObjectToReturn )
		{
			// get the file name of the style sheet to load
			string filename = absoluteUri.Segments[ absoluteUri.Segments.GetUpperBound( 0 ) ];
			// now build the resource path
			string resourcepath = _namespace + "." + filename;
			// and open the stream to the resource
			return _assembly.GetManifestResourceStream( resourcepath );
		}
	}
}
