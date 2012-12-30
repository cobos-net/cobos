// ============================================================================
// Filename: PhoneViewApplicationTest.cs
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
using System.Text;
using NUnit.Framework;
using Cobos.Core.UI;

using PVCursor = Cobos.Core.UI.CursorType;

namespace Cobos.Core.Tests.UI
{
	[TestFixture]
	public class CobosApplicationTest : IDisposable
	{
		public CobosApplicationTest()
		{
			InitialiseTest();
		}

		public void Dispose()
		{
			DisposeTest();
		}

		public static void InitialiseTest()
		{
			CobosApplication theApp = CobosApplication.Instance;
			theApp.Initialise( new CurrentCursorTest(),
										new MessageHandlerTest(),
										new ProgressBarTest(),
										new CurrentUserTest(),
										TestManager.TestFilesFolder );
		}

		public static void DisposeTest()
		{
			CobosApplication.Instance.Dispose();
		}

		[TestCase]
		public void Invalid_startup_parameters_throw_exception()
		{
			Assert.DoesNotThrow( delegate { CobosApplication.Instance.Dispose(); } );
			CobosApplication theApp = CobosApplication.Instance;

			Assert.Throws<Exception>( delegate { theApp.Initialise( null, null, null, null, null ); } );

			CurrentCursorTest cursor = new CurrentCursorTest();
			Assert.Throws<Exception>( delegate { theApp.Initialise( cursor, null, null, null, null ); } );

			MessageHandlerTest message = new MessageHandlerTest();
			Assert.Throws<Exception>( delegate { theApp.Initialise( cursor, message, null, null, null ); } );

			ProgressBarTest progress = new ProgressBarTest();
			Assert.Throws<Exception>( delegate { theApp.Initialise( cursor, message, progress, null, null ); } );
	
			CurrentUserTest user = new CurrentUserTest();
			Assert.Throws<Exception>( delegate { theApp.Initialise( cursor, message, progress, user, null ); } );

			string startupPath = "";
			Assert.Throws<Exception>( delegate { theApp.Initialise( cursor, message, progress, user, startupPath ); } );

			startupPath = TestManager.TestFilesFolder;
			Assert.DoesNotThrow( delegate { theApp.Initialise( cursor, message, progress, user, startupPath ); } );
		}

		[TestCase]
		public void Cursor_type_can_be_changed()
		{
			CobosApplication theApp = CobosApplication.Instance;

			theApp.Cursor.Type = CursorType.SizeWE;
			Assert.AreEqual( ((CurrentCursorTest)theApp.Cursor).CurrentCursorValue, "SizeWE" );

			theApp.Cursor.SetDefault();
			Assert.AreEqual( ((CurrentCursorTest)theApp.Cursor).CurrentCursorValue, "Arrow" );
		}

		[TestCase]
		public void User_can_logon()
		{
			CobosApplication theApp = CobosApplication.Instance;
		}
	
		[TestCase]
		public void Message_handler_works()
		{
			CobosApplication theApp = CobosApplication.Instance;
			MessageHandlerTest theMessage = (MessageHandlerTest)theApp.Message;

			Assert.DoesNotThrow( delegate { theApp.Message.ShowError( "This is an error", "Test error" ); } );
			Assert.DoesNotThrow( delegate { theApp.Message.ShowInfo( "This is information", "Test information" ); } );

			try
			{
				throw new Exception( "This is a thrown exception" );
			}
			catch ( Exception e )
			{
				Assert.DoesNotThrow( delegate { theApp.Message.ShowError( e, "Test exception" ); } );
			}

			theMessage.NextShowQuestionResult = MessageHandlerResult.No;
			Assert.AreEqual( theApp.Message.ShowQuestion( "Dumb question", "What" ), MessageHandlerResult.No );

			theMessage.NextShowQuestionResult = MessageHandlerResult.Yes;
			Assert.AreEqual( theApp.Message.ShowQuestion( "Sensible question", "What" ), MessageHandlerResult.Yes );

			string[] choices = new string[] { "Zero", "One", "Two" };
			int? choice;

			theMessage.NextShowChoicesResult = null;
			Assert.False( theApp.Message.ShowChoices( "Select one", choices ).HasValue );

			theMessage.NextShowChoicesResult = 1;
			choice = theApp.Message.ShowChoices( "Select one", choices );
			Assert.True( choice.HasValue );
			Assert.AreEqual( choice.Value, 1 );
		}

		[TestCase]
		public void Progress_bar_works()
		{
			CobosApplication theApp = CobosApplication.Instance;
			ProgressBarTest theProgress = (ProgressBarTest)theApp.ProgressBar;

			theProgress.Maximum = 10;
			theProgress.Value = 0;

			for ( int i = 0; i < 11; ++i )
			{
				theProgress.PerformStep();
			}

			Assert.AreEqual( theProgress.Value, 10 );
		}
	}

}
