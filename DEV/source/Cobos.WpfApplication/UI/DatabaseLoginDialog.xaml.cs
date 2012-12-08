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
using Cobos.WpfApplication.Views;

namespace Cobos.WpfApplication.UI
{
	/// <summary>
	/// Interaction logic for DatabaseConnectDialog.xaml
	/// </summary>
	public partial class DatabaseLoginDialog : Window
	{
		public DatabaseLoginDialog()
		{
			this.InitializeAsDialog();

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
