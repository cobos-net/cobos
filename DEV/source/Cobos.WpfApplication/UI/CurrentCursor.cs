using System;
using System.Windows.Input;
using Cobos.Core.UI;

using CobosCursorType = Cobos.Core.UI.CursorType;
using WindowsCursors = System.Windows.Input.Cursors;

namespace Cobos.WpfApplication.UI
{
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
