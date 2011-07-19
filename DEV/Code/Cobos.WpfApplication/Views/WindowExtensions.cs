using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Runtime.InteropServices;

namespace Intergraph.AsiaPac.WpfApplication.Views
{
	public static class WindowExtensions
	{
		[DllImport( "user32.dll" )]
		public extern static int SetWindowLong( IntPtr hwnd, int index, int value );

		[DllImport( "user32.dll" )]
		public extern static int GetWindowLong( IntPtr hwnd, int index );

		public static void HideMinimizeAndMaximizeButtons( this Window window )
		{
			const int GWL_STYLE = -16;

			IntPtr hwnd = new System.Windows.Interop.WindowInteropHelper( window ).Handle;
			long value = GetWindowLong( hwnd, GWL_STYLE );

			SetWindowLong( hwnd, GWL_STYLE, (int)(value & -131073 & -65537) );
		}

		public static void InitializeAsDialog( this Window window )
		{
			window.SourceInitialized += ( x, y ) =>
			{
				window.HideMinimizeAndMaximizeButtons();
			};
		}

		public static void Show( this Window window, Window owner )
		{
			window.Owner = owner;
			window.Show();
		}
	}
}
