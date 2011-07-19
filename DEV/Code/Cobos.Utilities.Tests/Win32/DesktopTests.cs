using System;
using Xunit;
using Intergraph.AsiaPac.Utilities.Win32;

namespace Intergraph.AsiaPac.Utilities.Tests.Win32
{
	public class DesktopTests
	{
		[Fact]
		public void Can_set_desktop_wallpaper()
		{
			Desktop.SetWallpaper( TestManager.TestFilesLocation + @"\Images\wallpaper.bmp", Desktop.WallpaperStyleEnum.Centered );
		}
	}
}
