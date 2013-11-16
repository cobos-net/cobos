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

namespace Cobos.Utilities.Xml
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Xml;
    using Cobos.Utilities.Extensions;

    /// <summary>
    /// Helper class to resolve the path to an XSLT embedded resource.
    /// </summary>
    public class XslResolver : XmlResolver
    {
        /// <summary>
        /// The namespace to use.
        /// </summary>
        private string namespaceUri;
        
        /// <summary>
        /// The assembly to look in.
        /// </summary>
        private Assembly assembly;

        /// <summary>
        /// Initializes a new instance of the <see cref="XslResolver"/> class.
        /// </summary>
        /// <param name="assembly">The assembly to look in.</param>
        /// <param name="ns">The namespace to use.</param>
        public XslResolver(Assembly assembly, string ns)
        {
            this.assembly = assembly;
            this.namespaceUri = ns;
        }

        /// <summary>
        /// Sets a System.Net.ICredentials object.
        /// </summary>
        /// <remarks>
        /// When overridden in a derived class, sets the credentials used to authenticate
        /// Web requests.
        /// </remarks>
        public override System.Net.ICredentials Credentials
        {
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// When overridden in a derived class, maps a URI to an object containing the actual resource.
        /// </summary>
        /// <param name="absoluteUri">The URI returned from System.Xml.XmlResolver.ResolveUri(System.Uri,System.String).</param>
        /// <param name="role">The current version does not use this parameter when resolving URIs.</param>
        /// <param name="typeOfObjectToReturn">The type of object to return.</param>
        /// <returns>A System.IO.Stream object or null if a type other than stream is specified.</returns>
        public override object GetEntity(Uri absoluteUri, string role, Type typeOfObjectToReturn)
        {
            // The absoluteUri is the path to the stylesheet from the current working directory.
            // We need to resolve this path with respect to the embedded resource.
            Uri currentDirectoryUri = new Uri(Directory.GetCurrentDirectory());

            // The segments don't include the scheme (e.g. FILE) and is of the form: /C:/Folder/SubFolder
            // The segments array is tokenised as follows: '/', "C:/", "Folder/", "SubFolder"
            // So we need to slice the array omitting the first element:
            string currentDirectory = string.Join(string.Empty, currentDirectoryUri.Segments.Slice(1, null));

            // Subtract the filename of the stylesheet from the context of the assembly's codebase and get rid of the / file seperators.
            string filename = absoluteUri.AbsolutePath.Replace(currentDirectory, string.Empty).Replace('/', '.');
            string resourcepath = this.namespaceUri + filename;

            return this.assembly.GetManifestResourceStream(resourcepath);
        }
    }
}
