using System;

namespace Cobos.Core.UI
{
	public interface IProgressBar
	{
		/// <summary>
		/// Increment the progress bar
		/// </summary>
		void PerformStep();

		/// <summary>
		/// Set the current message prompt
		/// </summary>
		string Prompt
		{
			get;
			set;
		}

		/// <summary>
		/// Set the maximum limit
		/// </summary>
		int Maximum
		{
			get;
			set;
		}

		/// <summary>
		/// Set the current value
		/// </summary>
		int Value
		{
			get;
			set;
		}

		/// <summary>
		/// Hide/show the progress bar
		/// </summary>
		bool Visible
		{
			get;
			set;
		}
	}
}
