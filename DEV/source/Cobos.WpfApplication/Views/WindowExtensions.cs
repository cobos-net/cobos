using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Cobos.WpfApplication.Views
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

			window.Loaded += ( sender, e ) =>
				{
					if ( window.Owner == null )
					{
						Window mainWindow = System.Windows.Application.Current.MainWindow;

						if ( mainWindow != null )
						{
							// centre in main window
							window.Left = mainWindow.Left + (mainWindow.Width - window.ActualWidth) / 2;
							window.Top = mainWindow.Top + (mainWindow.Height - window.ActualHeight) / 2;
						}
						else
						{
							// centre in screen
							Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;
							window.Left = (workingArea.Width - window.ActualWidth) / 2;
							window.Top = (workingArea.Height - window.ActualHeight) / 2;
						}
					}
				};
		}

		public static void Show( this Window window, Window owner )
		{
			window.Owner = owner;
			window.Show();
		}

		public static bool? ShowDialog( this Window window, Window owner )
		{
			window.Owner = owner;
			return window.ShowDialog();
		}
	}
}
