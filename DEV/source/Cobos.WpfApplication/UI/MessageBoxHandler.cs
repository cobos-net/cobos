// ============================================================================
// Filename: MessageBoxHandler.cs
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
