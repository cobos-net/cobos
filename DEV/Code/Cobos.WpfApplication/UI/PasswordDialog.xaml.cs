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
using System.Windows.Shapes;

namespace Intergraph.AsiaPac.WpfApplication.UI
{
	/// <summary>
	/// Interaction logic for PasswordView.xaml
	/// </summary>
	public partial class PasswordDialog : Window
	{
		public PasswordDialog()
		{
			InitializeComponent();
		}

		private void OK_Click( object sender, RoutedEventArgs e )
		{
			this.DialogResult = true;
		}

		private void Cancel_Click( object sender, RoutedEventArgs e )
		{
			this.DialogResult = false;
		}
	}
}
