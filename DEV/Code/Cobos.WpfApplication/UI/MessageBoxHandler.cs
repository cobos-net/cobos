using System;
using System.Windows;
using Cobos.Core.UI;
using Cobos.WpfApplication.Controls;

namespace Cobos.WpfApplication.UI
{
	class MessageBoxHandler : IMessageHandler 
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="caption"></param>
		public void ShowInfo( string message, string caption )
		{
			MessageBox.Show( message, caption, MessageBoxButton.OK, MessageBoxImage.Information );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="caption"></param>
		public void ShowError( string message, string caption )
		{
			MessageBox.Show( message, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="exception"></param>
		/// <param name="caption"></param>
		public void ShowError( Exception exception, string caption )
		{
			string message = exception.Message;
			
			while ( (exception = exception.InnerException) != null )
			{
				message += "\n\n" + exception.Message;
			}
			
			ShowError( message, caption );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="caption"></param>
		/// <returns></returns>
		public MessageHandlerResult ShowQuestion( string message, string caption )
		{
			return (MessageHandlerResult)MessageBox.Show( message, caption, MessageBoxButton.YesNo, MessageBoxImage.Question );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="options"></param>
		/// <returns>A null value if no choice was made, otherwise the index of options that was chosen</returns>
		public int? ShowChoices( string message, string[] options )
		{
			return MessageChoice.Show( message, options );
		}
	}
}
