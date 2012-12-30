// ============================================================================
// Filename: RegExHelper.cs
// Description: 
// ----------------------------------------------------------------------------
// Created by: N.Davis                          Date: 27-Nov-09
// Updated by:                                  Date:
// ============================================================================
// Copyright (c) 2009-2012 Nicholas Davis		nick@cobos.co.uk
//
// Cobos Software Development Kit
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Cobos.Utilities.Text
{
	public static class RegExHelper
	{
		/// <summary>
		/// Guid pattern matching.
		/// </summary>
		public static readonly string GuidPattern = @"\{[A-F0-9]{8}-[A-F0-9]{4}-[A-F0-9]{4}-[A-F0-9]{4}-[A-F0-9]{12}\}";

		public static readonly Regex GuidExpression = new Regex( GuidPattern );

		/// <summary>
		/// Phone number pattern matching.  Generic phone, not country specific.
		/// </summary>
		public static readonly string PhonePattern = @"\b(\(?\+?[0-9]*\)?)?[0-9 ()-]{3,}\b";

		public static readonly Regex PhoneExpression = new Regex( PhonePattern );

		/// <summary>
		/// Time pattern matching.
		/// </summary>
		public static readonly string TimePattern = @"(([0-1]?[0-9])|([2][0-3])):([0-5]?[0-9])(:([0-5]?[0-9]))?";

		public static readonly Regex TimeExpression = new Regex( TimePattern );

		/// <summary>
		/// Numeric pattern matching.
		/// </summary>
		public static readonly string NumericPattern = @"^[-+]?([0-9]{1,3}[,]?)?([0-9]{3}[,]?)*[.]?[0-9]*$";

		public static readonly Regex NumericExpression = new Regex( NumericPattern );
	}
}
