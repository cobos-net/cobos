// ============================================================================
// Filename: Desktop.cs
// Description: 
// ----------------------------------------------------------------------------
// Created by: N.Davis                          Date: 27-Nov-09
// Modified by:                                 Date:
// ============================================================================
// Copyright (c) 2009-2011 Nicholas Davis		nick@cobos.co.uk
//
// Cobos Software Development Kit v0.1
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
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Cobos.Utilities.Win32
{
	public static class Desktop
	{
		#region Win32 imports

		[DllImport( "user32.dll", CharSet = CharSet.Auto )]
		static extern int SystemParametersInfo( int uAction, int uParam, string lpvParam, int fuWinIni );

		const int SPI_SETDESKWALLPAPER = 20;
		const int SPIF_UPDATEINIFILE = 0x01;
		const int SPIF_SENDWININICHANGE = 0x02;
		
		#endregion
		
		#region Public enums

		public enum WallpaperStyleEnum
		{
			Tiled,
			Centered,
			Stretched
		}

		#endregion

		/// <summary>
		/// Set the windows desktop wallpaper
		/// </summary>
		/// <param name="path"></param>
		/// <param name="style"></param>
		public static void SetWallpaper( string path, WallpaperStyleEnum style )
		{
			WallpaperStyle = style;

			SystemParametersInfo( SPI_SETDESKWALLPAPER, 0, path, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE );
		}

		/// <summary>
		/// Set the current wallpaper style
		/// </summary>
		public static WallpaperStyleEnum WallpaperStyle
		{
			set
			{
				using ( RegistryKey key = Registry.CurrentUser.OpenSubKey( "Control Panel\\Desktop", true ) )
				{
					switch ( value )
					{
					case WallpaperStyleEnum.Stretched:
						key.SetValue( @"WallpaperStyle", "2" );
						key.SetValue( @"TileWallpaper", "0" );
						break;

					case WallpaperStyleEnum.Centered:
						key.SetValue( @"WallpaperStyle", "1" );
						key.SetValue( @"TileWallpaper", "0" );
						break;

					case WallpaperStyleEnum.Tiled:
						key.SetValue( @"WallpaperStyle", "1" );
						key.SetValue( @"TileWallpaper", "1" );
						break;
					}
				}
			}
		}
		
	}
}
