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
	/// <summary>
	/// Interaction logic for FileLocation.xaml
	/// </summary>
	public partial class FileLocation : UserControl
	{
		public FileLocation()
		{
			InitializeComponent();

			IsFolderBrowser = true;

			Filter = "All files (.*)|*.*";
		}

		public bool IsFolderBrowser
		{
			get;
			set;
		}

		public string Filter
		{
			get;
			set;
		}

		public static readonly DependencyProperty FilePathProperty = DependencyProperty.Register( "FilePath", typeof( string ), typeof( FileLocation ), null );

		public string FilePath
		{
			get
			{
				return (string)GetValue( FilePathProperty );
			}
			set
			{
				SetValue( FilePathProperty, value );
			}
		}

		private void Button_Click( object sender, RoutedEventArgs e )
		{
			if ( IsFolderBrowser )
			{
				BrowseForFolder();
			}
			else
			{
				BrowseForFile();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		void BrowseForFile()
		{
			Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
			dlg.Filter = Filter; // Filter files by extension

			Nullable<bool> result = dlg.ShowDialog();

			if ( result == true )
			{
				FilePath = dlg.FileName;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		void BrowseForFolder()
		{
			System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();

			System.Windows.Forms.DialogResult result = dlg.ShowDialog();

			if ( result == System.Windows.Forms.DialogResult.OK )
			{
				FilePath = dlg.SelectedPath;
			}
		}
	}
}
