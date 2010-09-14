using System;
using System.Diagnostics;
using Intergraph.Oz.Core.UI;

namespace Intergraph.Oz.Core.Tests.UI
{
	public class MessageHandlerTest : IMessageHandler 
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="caption"></param>
		public void ShowInfo( string message, string caption )
		{
			Debug.WriteLine( string.Format( "Information {0}: {1}", caption, message ) );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="caption"></param>
		public void ShowError( string message, string caption )
		{
			Debug.WriteLine( string.Format( "Error {0}: {1}", caption, message ) );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="exception"></param>
		/// <param name="caption"></param>
		public void ShowError( Exception exception, string caption )
		{
			Debug.WriteLine( string.Format( "Exception {0}: {1}", caption, exception.Message ) );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="caption"></param>
		/// <returns></returns>
		public MessageHandlerResult ShowQuestion( string message, string caption )
		{
			Debug.WriteLine( string.Format( "Question {0}: {1}", caption, message ) );
			return NextShowQuestionResult;
		}

		public MessageHandlerResult NextShowQuestionResult = MessageHandlerResult.Yes;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="options"></param>
		/// <returns>A null value if no choice was made, otherwise the index of options that was chosen</returns>
		public int? ShowChoices( string message, string[] options )
		{
			Debug.WriteLine( string.Format( "Choice {0}:", message ) );

			foreach ( string option in options )
			{
				Debug.WriteLine( option );
			}

			return NextShowChoicesResult;
		}

		public int? NextShowChoicesResult = null;
	}
}
