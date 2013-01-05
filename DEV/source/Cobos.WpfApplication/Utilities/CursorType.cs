// ----------------------------------------------------------------------------
// <copyright file="CursorType.cs" company="Cobos SDK">
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

namespace Cobos.WpfApplication.Utilities
{
    /// <summary>
    /// Cursor Type.
    /// </summary>
    public enum CursorType
    {
        /// <summary>
        /// The Cursor that appears when an application is starting.
        /// </summary>
        AppStarting,

        /// <summary>
        /// The arrow with a compact disk Cursor.
        /// </summary>
        ArrowCD,
        
        /// <summary>
        /// The Arrow Cursor.
        /// </summary>
        Arrow,

        /// <summary>
        /// The crosshair Cursor.
        /// </summary>
        Cross,

        /// <summary>
        /// The hand Cursor.
        /// </summary>
        HandCursor,

        /// <summary>
        /// A help Cursor which is a combination of an arrow and a question mark.
        /// </summary>
        Help,

        /// <summary>
        /// An I-beam Cursor, which is used to show where the text cursor appears when the mouse is clicked.
        /// </summary>
        IBeam,

        /// <summary>
        /// A Cursor with which indicates that a particular region is invalid for a given operation.
        /// </summary>
        No,

        /// <summary>
        /// A special cursor that is invisible.
        /// </summary>
        None,
        
        /// <summary>
        /// A pen Cursor.
        /// </summary>
        Pen,

        /// <summary>
        /// A south/east scrolling Cursor.
        /// </summary>
        ScrollSE,
        
        /// <summary>
        /// A west/east scrolling Cursor.
        /// </summary>
        ScrollWE,
        
        /// <summary>
        /// A four-headed sizing Cursor, which consists of four joined arrows that point north, south, east, and west.
        /// </summary>
        SizeAll,

        /// <summary>
        /// A two-headed northwest/southeast sizing Cursor.
        /// </summary>
        SizeNESW,

        /// <summary>
        /// A two-headed north/south sizing Cursor.
        /// </summary>
        SizeNS,

        /// <summary>
        /// A two-headed northwest/southeast sizing Cursor.
        /// </summary>
        SizeNWSE,

        /// <summary>
        /// A two-headed west/east sizing Cursor.
        /// </summary>
        SizeWE,
        
        /// <summary>
        /// An up arrow Cursor, which is typically used to identify an insertion point.
        /// </summary>
        UpArrow,
        
        /// <summary>
        /// A wait (or hourglass) Cursor.
        /// </summary>
        WaitCursor,
        
        /// <summary>
        /// The custom cursor.
        /// </summary>
        Custom
    }

    /// <summary>
    /// Utility class to lookup a cursor type.
    /// </summary>
    public static class CursorTypeLookup
    {
        /// <summary>
        /// Lookup a cursor type by name.
        /// </summary>
        /// <param name="cursorName">The name of the cursor</param>
        /// <returns>The cursor type.</returns>
        public static CursorType GetCursorType(string cursorName)
        {
            CursorType result = CursorType.Arrow;

            switch (cursorName)
            {
            case "AppStarting":
                result = CursorType.AppStarting;
                break;
            case "ArrowCD":
                result = CursorType.ArrowCD;
                break;
            case "Arrow":
                result = CursorType.Arrow;
                break;
            case "Cross":
                result = CursorType.Cross;
                break;
            case "Help":
                result = CursorType.Help;
                break;
            case "IBeam":
                result = CursorType.IBeam;
                break;
            case "No":
                result = CursorType.No;
                break;
            case "None":
                result = CursorType.None;
                break;
            case "Pen":
                result = CursorType.Pen;
                break;
            case "ScrollSE":
                result = CursorType.ScrollSE;
                break;
            case "ScrollWE":
                result = CursorType.ScrollWE;
                break;
            case "SizeAll":
                result = CursorType.SizeAll;
                break;
            case "SizeNESW":
                result = CursorType.SizeNESW;
                break;
            case "SizeNS":
                result = CursorType.SizeNS;
                break;
            case "SizeNWSE":
                result = CursorType.SizeNWSE;
                break;
            case "SizeWE":
                result = CursorType.SizeWE;
                break;
            case "UpArrow":
                result = CursorType.UpArrow;
                break;
            case "WaitCursor":
                result = CursorType.WaitCursor;
                break;
            case "Custom":
                result = CursorType.Custom;
                break;
            default:
                break;
            }

            return result;
        }
    }
}