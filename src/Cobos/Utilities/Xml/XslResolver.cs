// ----------------------------------------------------------------------------
// <copyright file="XslResolver.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
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
        private readonly string namespaceUri;

        /// <summary>
        /// The assembly to look in.
        /// </summary>
        private readonly Assembly assembly;

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
