// ----------------------------------------------------------------------------
// <copyright file="CurrentCursor.cs" company="Cobos SDK">
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

namespace Cobos.WpfApplication.UI
{
    using Cobos.WpfApplication.Interfaces;
    using Cobos.WpfApplication.Utilities;
    using System;
    using System.Windows.Input;

    using CobosCursorType = Cobos.WpfApplication.Utilities.CursorType;
    using WindowsCursors = System.Windows.Input.Cursors;

	public class CurrentCursor : ICurrentCursor
	{
		public CobosCursorType Type
		{
			set
			{
				switch ( value )
				{
				case CobosCursorType.AppStarting:
					Mouse.OverrideCursor = WindowsCursors.AppStarting;
					break;
				case CobosCursorType.ArrowCD:
					Mouse.OverrideCursor = WindowsCursors.ArrowCD;
					break;
				case CobosCursorType.Arrow:
					Mouse.OverrideCursor = WindowsCursors.Arrow;
					break;
				case CobosCursorType.Cross:
					Mouse.OverrideCursor = WindowsCursors.Cross;
					break;
				case CobosCursorType.HandCursor:
					Mouse.OverrideCursor = WindowsCursors.Hand;
					break;
				case CobosCursorType.Help:
					Mouse.OverrideCursor = WindowsCursors.Help;
					break;
				case CobosCursorType.IBeam:
					Mouse.OverrideCursor = WindowsCursors.IBeam;
					break;
				case CobosCursorType.No:
					Mouse.OverrideCursor = WindowsCursors.No;
					break;
				case CobosCursorType.None:
					Mouse.OverrideCursor = WindowsCursors.None;
					break;
				case CobosCursorType.Pen:
					Mouse.OverrideCursor = WindowsCursors.Pen;
					break;
				case CobosCursorType.ScrollSE:
					Mouse.OverrideCursor = WindowsCursors.ScrollSE;
					break;
				case CobosCursorType.ScrollWE:
					Mouse.OverrideCursor = WindowsCursors.ScrollWE;
					break;
				case CobosCursorType.SizeAll:
					Mouse.OverrideCursor = WindowsCursors.SizeAll;
					break;
				case CobosCursorType.SizeNESW:
					Mouse.OverrideCursor = WindowsCursors.SizeNESW;
					break;
				case CobosCursorType.SizeNS:
					Mouse.OverrideCursor = WindowsCursors.SizeNS;
					break;
				case CobosCursorType.SizeNWSE:
					Mouse.OverrideCursor = WindowsCursors.SizeNWSE;
					break;
				case CobosCursorType.SizeWE:
					Mouse.OverrideCursor = WindowsCursors.SizeWE;
					break;
				case CobosCursorType.UpArrow:
					Mouse.OverrideCursor = WindowsCursors.UpArrow;
					break;
				case CobosCursorType.WaitCursor:
					Mouse.OverrideCursor = WindowsCursors.Wait;
					break;
				//case PhoneViewCursorType.Custom:
				//   Mouse.OverrideCursor = CustomCursor;
				//   break;
				default:
					break;
				}
			}
		}

		public void SetDefault()
		{
			Mouse.OverrideCursor = WindowsCursors.Arrow;
		}
	}
}
