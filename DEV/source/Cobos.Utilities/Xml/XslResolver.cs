// ----------------------------------------------------------------------------
// <copyright file="XslResolver.cs" company="Cobos SDK">
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
using System.Reflection;
using System.IO;
using System.Text;
using System.Xml;

using Cobos.Utilities.Extensions;

namespace Cobos.Utilities.Xml
{
    public class XslResolver : XmlResolver
    {
        string _namespace;
        Assembly _assembly;

        public XslResolver(Assembly assembly, string ns)
        {
            _assembly = assembly;
            _namespace = ns;
        }

        public override System.Net.ICredentials Credentials
        {
            set { throw new NotImplementedException(); }
        }

        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            // The absoluteUri is the path to the stylesheet from the current working directory.
            // We need to resolve this path with respect to the embedded resource.
            Uri currentDirectoryUri = new Uri(Directory.GetCurrentDirectory());

            // The segments don't include the scheme (e.g. FILE) and is of the form: /C:/Folder/SubFolder
            // The segments array is tokenised as follows: '/', "C:/", "Folder/", "SubFolder"
            // So we need to slice the array omitting the first element:
            string currentDirectory = string.Join("", currentDirectoryUri.Segments.Slice(1, null));

            // Subtract the filename of the stylesheet from the context of the assembly's codebase and get rid of the / file seperators.
            string filename = absoluteUri.AbsolutePath.Replace(currentDirectory, "").Replace('/', '.');
            string resourcepath = _namespace + filename;

            return _assembly.GetManifestResourceStream(resourcepath);
        }
    }
}
