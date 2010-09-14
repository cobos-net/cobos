using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Intergraph.Oz.WpfApplication.Controls
{
	public enum TriStateEnum
	{
		Yes = 0,
		No = 1,
		NA = 2
	}

	/// <summary>
	/// Interaction logic for TriState.xaml
	/// </summary>
	public partial class TriState : UserControl
	{
		public TriState()
		{
			InitializeComponent();
		}

		public TriStateEnum State
		{
			get
			{
				if ( _Yes.IsChecked.HasValue && _Yes.IsChecked.Value )
				{
					return TriStateEnum.Yes;
				}
				else if ( _No.IsChecked.HasValue && _No.IsChecked.Value )
				{
					return TriStateEnum.No;
				}
				else // _NA or nothing checked
				{
					return TriStateEnum.NA;
				}
			}

			set
			{
				switch ( value )
				{
				case TriStateEnum.Yes:
					_Yes.IsChecked = true;
					break;

				case TriStateEnum.No:
					_No.IsChecked = true;
					break;

				case TriStateEnum.NA:
				default:
					_NA.IsChecked = true;
					break;
				}
			}
		}
		
	}
}
