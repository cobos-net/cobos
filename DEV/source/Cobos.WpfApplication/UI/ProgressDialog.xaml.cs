// ============================================================================
// Filename: ProgressDialog.xaml.cs
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
using Cobos.Core.UI;

namespace Cobos.WpfApplication.UI
{
	/// <summary>
	/// Interaction logic for ProgressDialog.xaml
	/// </summary>
	public partial class ProgressDialog : Window, IProgressBar 
	{
		private delegate void UpdateProgressBarDelegate( System.Windows.DependencyProperty dp, Object value );

		UpdateProgressBarDelegate _updateProgress;

		public ProgressDialog()
		{
			InitializeComponent();

			// Initialise the progress bar, we use a delegate
			// function to update the progress so that the 
			// progress bar is always updated when the application
			// is busy working.
			_progress.Minimum = 0.0;
			_progress.Maximum = 10.0;
			_progress.Value = 0.0;

			_updateProgress = new UpdateProgressBarDelegate( _progress.SetValue );
		}

		#region IProgresBar implementation

		/// <summary>
		/// Increment the progress bar
		/// </summary>
		public void PerformStep()
		{
			double nextValue = _progress.Value + 1.0;

			Dispatcher.Invoke( _updateProgress, 
										System.Windows.Threading.DispatcherPriority.Background,
										new object[] { ProgressBar.ValueProperty, nextValue } );
		}

		/// <summary>
		/// Set the current message prompt
		/// </summary>
		public string Prompt
		{
			get
			{
				return _prompt.Text;
			}
			set
			{
				_prompt.Text = value;
			}
		}

		/// <summary>
		/// Set the maximum limit
		/// </summary>
		public int Maximum
		{
			get
			{
				return (int)Math.Floor( _progress.Maximum );
			}
			set
			{
				_progress.Maximum = (double)value;
			}
		}

		/// <summary>
		/// Set the current value
		/// </summary>
		public int Value
		{
			get
			{
				return (int)Math.Floor( _progress.Value );
			}
			set
			{
				Dispatcher.Invoke( _updateProgress,
											System.Windows.Threading.DispatcherPriority.Background,
											new object[] { ProgressBar.ValueProperty, Math.Floor( (double)value ) } );
			}
		}

		/// <summary>
		/// Hide/show the progress bar
		/// </summary>
		public bool Visible
		{
			get
			{
				return this.IsVisible;
			}
			set
			{
				if ( value )
				{
					this.Show();
				}
				else
				{
					this.Hide();
				}
			}
		}

		#endregion

	}
}
