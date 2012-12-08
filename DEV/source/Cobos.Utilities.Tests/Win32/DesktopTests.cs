using System;
using NUnit.Framework;
using Cobos.Utilities.Win32;

namespace Cobos.Utilities.Tests.Win32
{
	[TestFixture]
	public class DesktopTests
	{
		[TestCase]
		public void Can_set_desktop_wallpaper()
		{
			Desktop.SetWallpaper( TestManager.TestFilesLocation + @"\Images\wallpaper.bmp", Desktop.WallpaperStyleEnum.Centered );
		}
	}
}
