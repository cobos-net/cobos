// ============================================================================
// Filename: WindowExtensions.cs
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
