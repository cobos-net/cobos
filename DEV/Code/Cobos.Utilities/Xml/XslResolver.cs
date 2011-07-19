// ============================================================================
// Filename: XslResolver.cs
// Description: 
// ----------------------------------------------------------------------------
// Created by: N.Davis                          Date: 27-Nov-09
// Modified by:                                 Date:
// ============================================================================
// Copyright (c) 2009-2011 Nicholas Davis		nick@cobos.co.uk
//
// Cobos Software Development Kit v0.1
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ============================================================================

// 05-Feb-11 N.Davis
// -----------------
// Rebranded from "Cobos" to "Intergraph.AsiaPac" in preparation for use in the Generic CAD Interoperability project

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;

#if INTERGRAPH_BRANDING
namespace Intergraph.AsiaPac.Utilities.Xml
#else
namespace Cobos.Utilities.Xml
#endif
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
