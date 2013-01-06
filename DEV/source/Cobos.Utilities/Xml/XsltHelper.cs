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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using System.IO;
using System.Reflection;

namespace Cobos.Utilities.Xml
{
    public static class XsltHelper
    {
        public static XslCompiledTransform Load(string filename, string resNamespace)
        {
            XslCompiledTransform xslt = new XslCompiledTransform();
            xslt.Load(filename, null, new XslResolver(Assembly.GetCallingAssembly(), resNamespace));
            return xslt;
        }

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
