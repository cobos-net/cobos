// ----------------------------------------------------------------------------
// <copyright file="XsltHelper.cs" company="Cobos SDK">
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

namespace Cobos.Utilities.Xml
{
    using System;
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
                    xmlReader = null;
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
                    xmlReader = null;
                }
            }
        }
    }
}
