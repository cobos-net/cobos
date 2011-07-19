using System;
using System.Windows.Input;
using Intergraph.AsiaPac.Core.UI;

using PhoneViewCursorType = Intergraph.AsiaPac.Core.UI.CursorType;
using WindowsCursors = System.Windows.Input.Cursors;

namespace Intergraph.AsiaPac.WpfApplication.UI
{
	public class CurrentCursor : ICurrentCursor 
	{
		public PhoneViewCursorType Type
		{
			set
			{
				switch ( value )
				{
				case PhoneViewCursorType.AppStarting:
					Mouse.OverrideCursor = WindowsCursors.AppStarting;
					break;
				case PhoneViewCursorType.ArrowCD:
					Mouse.OverrideCursor = WindowsCursors.ArrowCD;
					break;
				case PhoneViewCursorType.Arrow:
					Mouse.OverrideCursor = WindowsCursors.Arrow;
					break;
				case PhoneViewCursorType.Cross:
					Mouse.OverrideCursor = WindowsCursors.Cross;
					break;
				case PhoneViewCursorType.HandCursor:
					Mouse.OverrideCursor = WindowsCursors.Hand;
					break;
				case PhoneViewCursorType.Help:
					Mouse.OverrideCursor = WindowsCursors.Help;
					break;
				case PhoneViewCursorType.IBeam:
					Mouse.OverrideCursor = WindowsCursors.IBeam;
					break;
				case PhoneViewCursorType.No:
					Mouse.OverrideCursor = WindowsCursors.No;
					break;
				case PhoneViewCursorType.None:
					Mouse.OverrideCursor = WindowsCursors.None;
					break;
				case PhoneViewCursorType.Pen:
					Mouse.OverrideCursor = WindowsCursors.Pen;
					break;
				case PhoneViewCursorType.ScrollSE:
					Mouse.OverrideCursor = WindowsCursors.ScrollSE;
					break;
				case PhoneViewCursorType.ScrollWE:
					Mouse.OverrideCursor = WindowsCursors.ScrollWE;
					break;
				case PhoneViewCursorType.SizeAll:
					Mouse.OverrideCursor = WindowsCursors.SizeAll;
					break;
				case PhoneViewCursorType.SizeNESW:
					Mouse.OverrideCursor = WindowsCursors.SizeNESW;
					break;
				case PhoneViewCursorType.SizeNS:
					Mouse.OverrideCursor = WindowsCursors.SizeNS;
					break;
				case PhoneViewCursorType.SizeNWSE:
					Mouse.OverrideCursor = WindowsCursors.SizeNWSE;
					break;
				case PhoneViewCursorType.SizeWE:
					Mouse.OverrideCursor = WindowsCursors.SizeWE;
					break;
				case PhoneViewCursorType.UpArrow:
					Mouse.OverrideCursor = WindowsCursors.UpArrow;
					break;
				case PhoneViewCursorType.WaitCursor:
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
