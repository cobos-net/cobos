using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Cobos.WpfApplication.Views
{
	public interface IViewPresentation
	{
		string DisplayName
		{
			get;
		}

		Size? InitialSize
		{
			get;
		}
	}
}
