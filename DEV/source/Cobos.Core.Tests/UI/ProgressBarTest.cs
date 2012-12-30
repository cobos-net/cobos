// ============================================================================
// Filename: ProgressBarTest.cs
// Description: 
// ----------------------------------------------------------------------------
// Created by: N.Davis                          Date: 21-Nov-09
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
using System.Diagnostics;
using Cobos.Core.UI;

namespace Cobos.Core.Tests.UI
{
	public class ProgressBarTest : IProgressBar
	{
		/// <summary>
		/// Increment the progress bar
		/// </summary>
		public void PerformStep()
		{
			if ( Value < Maximum )
			{
				++_value;
			}
			Debug.WriteLine( string.Format( "Progress Current = {0} of {1}", Value, Maximum ) );
		}

		/// <summary>
		/// Set the current message prompt
		/// </summary>
		public string Prompt
		{
			get
			{
				return _prompt;
			}
			set
			{
				_prompt = value;

				Debug.WriteLine( string.Format( "Progress Prompt = {0}", _prompt ) );
			}
		}

		string _prompt;

		/// <summary>
		/// Set the maximum limit
		/// </summary>
		public int Maximum
		{
			get
			{
				return _maximum;
			}
			set
			{
				_maximum = value;
				Debug.WriteLine( string.Format( "Progress Maximum = {0}", _maximum ) );
			}
		}

		int _maximum;

		/// <summary>
		/// Set the current value
		/// </summary>
		public int Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
				Debug.WriteLine( string.Format( "Progress Value = {0}", _value ) );
			}
		}

		int _value;

		/// <summary>
		/// Hide/show the progress bar
		/// </summary>
		public bool Visible
		{
			get
			{
				return _visible;
			}
			set
			{
				_visible = value;
				Debug.WriteLine( string.Format( "Progress Visible = {0}", _visible ) );
			}
		}

		bool _visible;
	}
}
