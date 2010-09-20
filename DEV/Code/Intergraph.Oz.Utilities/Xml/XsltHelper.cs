using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using System.IO;
using System.Reflection;

namespace Intergraph.Oz.Utilities.Xml
{
	public static class XsltHelper
	{
		public static XslCompiledTransform Load( string filename, string resNamespace )
		{
			XslCompiledTransform xslt = new XslCompiledTransform();
			xslt.Load( filename, null, new XslResolver( Assembly.GetCallingAssembly(), resNamespace ) );
			return xslt;
		}

		public static void Transform( string xsltResource, XmlReader input, TextWriter output )
		{
			XmlTextReader xmlReader = null;

			try
			{
				// load the embedded stylesheet resource
				Assembly assembly = Assembly.GetCallingAssembly();
				xmlReader = new XmlTextReader( assembly.GetManifestResourceStream( xsltResource ) );

				XslCompiledTransform xslt = new XslCompiledTransform();
				xslt.Load( xmlReader );

				xslt.Transform( input, null, output );
			}
			finally
			{
				if ( xmlReader != null )
				{
					xmlReader.Close();
					xmlReader = null;
				}
			}
		}

		public static void Transform( string xsltResource, string xmlPath, string outputPath )
		{
			XmlTextReader xmlReader = null;

			try
			{
				// load the embedded stylesheet resource
				Assembly assembly = Assembly.GetCallingAssembly();
				xmlReader = new XmlTextReader( assembly.GetManifestResourceStream( xsltResource ) );

				XslCompiledTransform xslt = new XslCompiledTransform();
				xslt.Load( xmlReader );
				xslt.Transform( xmlPath, outputPath );
			}
			finally
			{
				if ( xmlReader != null )
				{
					xmlReader.Close();
					xmlReader = null;
				}
			}
		}

	}
}
