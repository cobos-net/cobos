﻿// ============================================================================
// Filename: StringExtensions.cs
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
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

#if INTERGRAPH_BRANDING
using Intergraph.AsiaPac.Utilities.Extensions;

namespace Intergraph.AsiaPac.Web.Utilities.Extensions
#else
using Cobos.Utilities.Extensions;

namespace Cobos.Web.Utilities.Extensions
#endif
{
	public static class StringExtensions
	{
		/// <summary>
		/// deserialize from Json
		/// </summary>
		/// <param name="str">The 'this' string reference</param>
		/// <returns>Deserialized object</returns>
		public static object FromJson( this string str )
		{
			JavaScriptSerializer serializer = new JavaScriptSerializer();
			return serializer.DeserializeObject( str );
		}

		/// <summary>
		/// Deserialize from Json as a specified type
		/// </summary>
		/// <typeparam name="T">The type to cast to</typeparam>
		/// <param name="str">The 'this' string reference</param>
		/// <param name="example">An instance of an anonymous type</param>
		/// <returns>A reference to the anonymous type.  If the cast fails, then null.</returns>
		public static T FromJsonAs<T>( this string str, T example )
		{
			JavaScriptSerializer serializer = new JavaScriptSerializer();
			object json = serializer.DeserializeObject( str );

			if ( json is Dictionary<string, object> )
			{
				return ((Dictionary<string, object>)json).CastByExample( example );
			}
			else
			{
				// this may or may not work depending on the object type
				return json.CastByExample( example );
			}
		}

	}
}