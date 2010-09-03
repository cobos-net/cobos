using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.SourceSafe.Interop;

namespace Intergraph.Oz.Build.Vss
{
	public static class ItemHelper
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="vssItem"></param>
		/// <returns></returns>
		public static bool IsCheckedOut( IVSSItem vssItem )
		{
			return vssItem.IsCheckedOut != (int)VSSFileStatus.VSSFILE_NOTCHECKEDOUT;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="vssItem"></param>
		/// <returns></returns>
		public static bool IsCheckedOutByMe( IVSSItem vssItem )
		{
			return vssItem.IsCheckedOut == (int)VSSFileStatus.VSSFILE_CHECKEDOUT_ME;
		}

	}
}
