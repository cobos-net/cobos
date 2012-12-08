using System;
using System.Diagnostics;
using Cobos.Core.UI;

namespace Cobos.Core.Tests.UI
{
	public class ProgressBarTest : IProgressBar
	{
		/// <summary>
		/// Increment the progress bar
		/// </summary>
		public void PerformStep()
		{
			if ( Value < Maximum )
			{
				++_value;
			}
			Debug.WriteLine( string.Format( "Progress Current = {0} of {1}", Value, Maximum ) );
		}

		/// <summary>
		/// Set the current message prompt
		/// </summary>
		public string Prompt
		{
			get
			{
				return _prompt;
			}
			set
			{
				_prompt = value;

				Debug.WriteLine( string.Format( "Progress Prompt = {0}", _prompt ) );
			}
		}

		string _prompt;

		/// <summary>
		/// Set the maximum limit
		/// </summary>
		public int Maximum
		{
			get
			{
				return _maximum;
			}
			set
			{
				_maximum = value;
				Debug.WriteLine( string.Format( "Progress Maximum = {0}", _maximum ) );
			}
		}

		int _maximum;

		/// <summary>
		/// Set the current value
		/// </summary>
		public int Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
				Debug.WriteLine( string.Format( "Progress Value = {0}", _value ) );
			}
		}

		int _value;

		/// <summary>
		/// Hide/show the progress bar
		/// </summary>
		public bool Visible
		{
			get
			{
				return _visible;
			}
			set
			{
				_visible = value;
				Debug.WriteLine( string.Format( "Progress Visible = {0}", _visible ) );
			}
		}

		bool _visible;
	}
}
