// ----------------------------------------------------------------------------
// <copyright file="XsltHelper.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Utilities.Xml
{
    using System.IO;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Xsl;

    /// <summary>
    /// Helper methods for XSLT transform operations.
    /// </summary>
    public static class XsltHelper
    {
        /// <summary>
        /// Load the specified XSLT transform that is an embedded resource in an assembly.
        /// </summary>
        /// <param name="filename">The name of the XSLT transform to load.</param>
        /// <param name="resNamespace">The resource namespace.</param>
        /// <returns>The loaded style-sheet if found; otherwise null.</returns>
        public static XslCompiledTransform Load(string filename, string resNamespace)
        {
            XslCompiledTransform xslt = new XslCompiledTransform();
            xslt.Load(filename, null, new XslResolver(Assembly.GetCallingAssembly(), resNamespace));
            return xslt;
        }

        /// <summary>
        /// Transform an XML document using the specified XSLT transform.
        /// </summary>
        /// <param name="xsltResource">The URI of the embedded XSLT resource.</param>
        /// <param name="input">The XML input document.</param>
        /// <param name="output">The output stream for the transformed result.</param>
        public static void Transform(string xsltResource, XmlReader input, TextWriter output)
        {
            XmlTextReader xmlReader = null;

            try
            {
                // load the embedded stylesheet resource
                Assembly assembly = Assembly.GetCallingAssembly();
                xmlReader = new XmlTextReader(assembly.GetManifestResourceStream(xsltResource));

                XslCompiledTransform xslt = new XslCompiledTransform();
                xslt.Load(xmlReader);

                xslt.Transform(input, null, output);
            }
            finally
            {
                if (xmlReader != null)
                {
                    xmlReader.Close();
                }
            }
        }

        /// <summary>
        /// Transform an XML document using the specified XSLT transform.
        /// </summary>
        /// <param name="xsltResource">The URI of the embedded XSLT resource.</param>
        /// <param name="xmlPath">The path to the XML input document.</param>
        /// <param name="outputPath">The path to the transformed output.</param>
        public static void Transform(string xsltResource, string xmlPath, string outputPath)
        {
            XmlTextReader xmlReader = null;

            try
            {
                // load the embedded stylesheet resource
                Assembly assembly = Assembly.GetCallingAssembly();
                xmlReader = new XmlTextReader(assembly.GetManifestResourceStream(xsltResource));

                XslCompiledTransform xslt = new XslCompiledTransform();
                xslt.Load(xmlReader);
                xslt.Transform(xmlPath, outputPath);
            }
            finally
            {
                if (xmlReader != null)
                {
                    xmlReader.Close();
                }
            }
        }
    }
}
