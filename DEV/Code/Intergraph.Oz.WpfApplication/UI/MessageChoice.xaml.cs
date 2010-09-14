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

namespace Intergraph.Oz.WpfApplication.UI
{
	/// <summary>
	/// Interaction logic for MessageChoice.xaml
	/// </summary>
	public partial class MessageChoice : Window
	{
		public MessageChoice( string message, string[] options )
		{
			InitializeComponent();

			_prompt.Text = message;

			_okButton.IsEnabled = false;

			_combo.ItemsSource = options;

			_combo.SelectionChanged +=new SelectionChangedEventHandler(_combo_SelectionChanged);
		}

		/// <summary>
		/// Close the dialog
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OK_Click( object sender, RoutedEventArgs e )
		{
			this.DialogResult = true;
		}

		/// <summary>
		/// Close the dialog
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Cancel_Click( object sender, RoutedEventArgs e )
		{
			this.DialogResult = false;
		}

		/// <summary>
		/// Enable/disable the OK button based on a valid selection
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void _combo_SelectionChanged( object sender, SelectionChangedEventArgs e )
		{
			_okButton.IsEnabled = _combo.SelectedIndex > -1;
		}

		/// <summary>
		/// Static utility analogous to MessageBox.Show
		/// </summary>
		/// <param name="message"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public static int? Show( string message, string[] options )
		{
			MessageChoice choice = new MessageChoice( message, options );

			bool? result = choice.ShowDialog();

			if ( !result.HasValue || result == false )
			{
				return null;
			}

			int index = choice._combo.SelectedIndex;

			return index > -1 ? (int?)index : null;
		}
	}
}
