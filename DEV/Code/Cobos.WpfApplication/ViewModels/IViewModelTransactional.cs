using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intergraph.AsiaPac.WpfApplication.ViewModels
{
	public enum ViewModelTransactionMode
	{
		Insert,
		Update,
		Delete
	}

	/// <summary>
	/// Interface specification for a transactional view model
	/// for interaction with the UI.
	/// </summary>
	public interface IViewModelTransactional
	{
		/// <summary>
		/// Get the data source for use by the view model.
		/// May be a member variable or the view model itself.
		/// </summary>
		object DataSource
		{
			get;
		}

		/// <summary>
		/// Exception safe commit.
		/// </summary>
		/// <returns>True on success, otherwise false.</returns>
		bool TryCommit();

		/// <summary>
		/// Exception safe rollback.
		/// </summary>
		/// <returns>True on success, otherwise false.</returns>
		bool TryRollback();
	}
}
