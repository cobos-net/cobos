﻿using System;
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
using Intergraph.AsiaPac.WpfApplication.Views;
using Intergraph.AsiaPac.WpfApplication.ViewModels;


namespace Intergraph.AsiaPac.WpfApplication.Controls
{
	/// <summary>
	/// Interaction logic for Attachments.xaml
	/// </summary>
	public partial class EditableGrid : UserControl
	{
		public EditableGrid()
		{
			InitializeComponent();
		}

		public DataGrid DataGrid
		{
			get { return _DataGrid; }
		}
	}
}