using System;

namespace Cobos.Core.UI
{
	public enum MessageHandlerResult
	{
		// Summary:
		//     The message box returns no result.
		None = 0,
		//
		// Summary:
		//     The result value of the message box is OK.
		OK = 1,
		//
		// Summary:
		//     The result value of the message box is Cancel.
		Cancel = 2,
		//
		// Summary:
		//     The result value of the message box is Yes.
		Yes = 6,
		//
		// Summary:
		//     The result value of the message box is No.
		No = 7,
	}

	public interface IMessageHandler
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="caption"></param>
		void ShowInfo( string message, string caption );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="caption"></param>
		void ShowError( string message, string caption );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="exception"></param>
		/// <param name="caption"></param>
		void ShowError( Exception exception, string caption );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="caption"></param>
		/// <returns></returns>
		MessageHandlerResult ShowQuestion( string message, string caption );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="options"></param>
		/// <returns>A null value if no choice was made, otherwise the index of options that was chosen</returns>
		int? ShowChoices( string message, string[] options );
	}
}
