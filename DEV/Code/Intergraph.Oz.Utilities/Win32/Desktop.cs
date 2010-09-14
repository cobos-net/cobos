using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Intergraph.Oz.Utilities.Win32
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
