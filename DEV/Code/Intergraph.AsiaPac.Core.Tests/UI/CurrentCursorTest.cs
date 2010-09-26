using System;
using Intergraph.AsiaPac.Core.UI;

namespace Intergraph.AsiaPac.Core.Tests.UI
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
