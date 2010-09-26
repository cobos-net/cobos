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
using Intergraph.AsiaPac.WpfApplication.ViewModels;

namespace Intergraph.AsiaPac.WpfApplication.Views
{
	/// <summary>
	/// Interaction logic for PresentationWindow.xaml
	/// </summary>
	public partial class ViewDialog : Window
	{
		public ViewDialog()
		{
			this.InitializeAsDialog();

			InitializeComponent();
		}

		/// <summary>
		/// 
		/// </summary>
		public UserControl UserControl
		{
			set
			{
				value.Width = double.NaN;
				value.Height = double.NaN;

				this._control.Children.Add( value );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public ViewModelBase ViewModel
		{
			set
			{
				_viewModel = value;

				IViewPresentation presentation = value as IViewPresentation;

				if ( presentation != null )
				{
					Title = presentation.DisplayName;

					Size? size = presentation.InitialSize;

					if ( size.HasValue )
					{
						Height = size.Value.Height;
						Width = size.Value.Width;
					}
				}

				IViewModelTransactional transactional = value as IViewModelTransactional;

				if ( transactional != null )
				{
					DataContext = transactional.DataSource;
				}
			}
		}

		ViewModelBase _viewModel;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OK_Click( object sender, RoutedEventArgs e )
		{
			IViewModelTransactional model = _viewModel as IViewModelTransactional;

			if ( model != null )
			{
				if ( model.TryCommit() )
				{
					this.DialogResult = true;
				}
				else
				{
					return;  // let the user try again
				}
			}
			else
			{
				this.DialogResult = true;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Cancel_Click( object sender, RoutedEventArgs e )
		{
			IViewModelTransactional model = _viewModel as IViewModelTransactional;

			if ( model != null )
			{
				model.TryRollback();
			}

			this.DialogResult = false;  // always make sure that we can quit out of a dialog.
		}

		/// <summary>
		/// Utility function to show a dialog with a particular view/view model definition.
		/// </summary>
		/// <typeparam name="ViewType"></typeparam>
		/// <typeparam name="ViewModelType"></typeparam>
		/// <returns></returns>
		public static bool? ShowDialog<ViewType>( object dataSource )
			where ViewType : UserControl, new()
		{
			ViewDialog dialog = new ViewDialog();
			dialog.UserControl = new ViewType();

			ViewModelBase viewModel = dataSource as ViewModelBase;

			if ( viewModel != null )
			{
				dialog.ViewModel = viewModel;
			}
			else
			{
				dialog.DataContext = dataSource;
			}

			return dialog.ShowDialog();
		}

		/// <summary>
		/// Utility function to show a dialog with a particular view/view model definition.
		/// </summary>
		/// <typeparam name="ViewType"></typeparam>
		/// <typeparam name="ViewModelType"></typeparam>
		public static void Show<ViewType>( object dataSource )
			where ViewType : UserControl, new()
		{
			ViewDialog dialog = new ViewDialog();
			dialog.UserControl = new ViewType();

			ViewModelBase viewModel = dataSource as ViewModelBase;

			if ( viewModel != null )
			{
				dialog.ViewModel = viewModel;
			}
			else
			{
				dialog.DataContext = dataSource;
			}

			dialog.Show();
		}
	}
}
