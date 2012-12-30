// ============================================================================
// Filename: CurrentCursorTest.cs
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
using Cobos.Core.UI;

namespace Cobos.Core.Tests.UI
{
	public class CurrentCursorTest : ICurrentCursor
	{
		public string CurrentCursorValue = "Arrow";

		public CursorType Type
		{
			set
			{
				switch ( value )
				{
				case CursorType.AppStarting:
					CurrentCursorValue = "AppStarting";
					break;
				case CursorType.ArrowCD:
					CurrentCursorValue = "ArrowCD";
					break;
				case CursorType.Arrow:
					CurrentCursorValue = "Arrow";
					break;
				case CursorType.Cross:
					CurrentCursorValue = "Cross";
					break;
				case CursorType.HandCursor:
					CurrentCursorValue = "Hand";
					break;
				case CursorType.Help:
					CurrentCursorValue = "Help";
					break;
				case CursorType.IBeam:
					CurrentCursorValue = "IBeam";
					break;
				case CursorType.No:
					CurrentCursorValue = "No";
					break;
				case CursorType.None:
					CurrentCursorValue = "None";
					break;
				case CursorType.Pen:
					CurrentCursorValue = "Pen";
					break;
				case CursorType.ScrollSE:
					CurrentCursorValue = "ScrollSE";
					break;
				case CursorType.ScrollWE:
					CurrentCursorValue = "ScrollWE";
					break;
				case CursorType.SizeAll:
					CurrentCursorValue = "SizeAll";
					break;
				case CursorType.SizeNESW:
					CurrentCursorValue = "SizeNESW";
					break;
				case CursorType.SizeNS:
					CurrentCursorValue = "SizeNS";
					break;
				case CursorType.SizeNWSE:
					CurrentCursorValue = "SizeNWSE";
					break;
				case CursorType.SizeWE:
					CurrentCursorValue = "SizeWE";
					break;
				case CursorType.UpArrow:
					CurrentCursorValue = "UpArrow";
					break;
				case CursorType.WaitCursor:
					CurrentCursorValue = "Wait";
					break;
				case CursorType.Custom:
					CurrentCursorValue = "CustomCursor";
					break;
				default:
					break;
				}
			}
		}

		public void SetDefault()
		{
			CurrentCursorValue = "Arrow";
		}
	}
}
