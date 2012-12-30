// ============================================================================
// Filename: MessageChoice.xaml.cs
// Description: 
// ----------------------------------------------------------------------------
// Created by: N.Davis                          Date: 21-Nov-09
// Updated by:                                  Date:
// ============================================================================
// Copyright (c) 2009-2012 Nicholas Davis		nick@cobos.co.uk
//
// Cobos Software Development Kit
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ============================================================================

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

namespace Cobos.WpfApplication.UI
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
