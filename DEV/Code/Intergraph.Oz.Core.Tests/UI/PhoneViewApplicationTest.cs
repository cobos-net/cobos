using System;
using System.Text;
using Xunit;
using Intergraph.Oz.Core.UI;

using PVCursor = Intergraph.Oz.Core.UI.CursorType;

namespace Intergraph.Oz.Core.Tests.UI
{
	public class IntergraphApplicationTest : IDisposable
	{
		public IntergraphApplicationTest()
		{
			InitialiseTest();
		}

		public void Dispose()
		{
			DisposeTest();
		}

		public static void InitialiseTest()
		{
			IntergraphApplication theApp = IntergraphApplication.Current;
			theApp.Initialise( new CurrentCursorTest(),
										new MessageHandlerTest(),
										new ProgressBarTest(),
										new CurrentUserTest(),
										TestManager.TestFilesFolder );
		}

		public static void DisposeTest()
		{
			IntergraphApplication.Current.Dispose();
		}

		[Fact]
		public void Invalid_startup_parameters_throw_exception()
		{
			Assert.DoesNotThrow( delegate { IntergraphApplication.Current.Dispose(); } );
			IntergraphApplication theApp = IntergraphApplication.Current;

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

		[Fact]
		public void Cursor_type_can_be_changed()
		{
			IntergraphApplication theApp = IntergraphApplication.Current;

			theApp.Cursor.Type = CursorType.SizeWE;
			Assert.Equal( ((CurrentCursorTest)theApp.Cursor).CurrentCursorValue, "SizeWE" );

			theApp.Cursor.SetDefault();
			Assert.Equal( ((CurrentCursorTest)theApp.Cursor).CurrentCursorValue, "Arrow" );
		}

		[Fact]
		public void User_can_logon()
		{
			IntergraphApplication theApp = IntergraphApplication.Current;
			CurrentUserTest theUser = (CurrentUserTest)theApp.User;
			bool? res; 

			theUser.NextShowLoginResult = null;
			Assert.False( theApp.User.ShowLogin( "Login user" ).HasValue );

			theUser.NextShowLoginResult = false;
			res = theApp.User.ShowLogin( null );
			Assert.True( res.HasValue );
			Assert.False( res.Value );

			theUser.NextShowLoginResult = true;
			theUser.Username = "user";
			theUser.Password = "pwd";
			res = theApp.User.ShowLogin( null );
			Assert.True( res.HasValue );
			Assert.True( res.Value );

			Assert.Equal( theApp.User.Username, "user" );
			Assert.Equal( theApp.User.Password, "pwd" );
		}
	
		[Fact]
		public void Message_handler_works()
		{
			IntergraphApplication theApp = IntergraphApplication.Current;
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
			Assert.Equal( theApp.Message.ShowQuestion( "Dumb question", "What" ), MessageHandlerResult.No );

			theMessage.NextShowQuestionResult = MessageHandlerResult.Yes;
			Assert.Equal( theApp.Message.ShowQuestion( "Sensible question", "What" ), MessageHandlerResult.Yes );

			string[] choices = new string[] { "Zero", "One", "Two" };
			int? choice;

			theMessage.NextShowChoicesResult = null;
			Assert.False( theApp.Message.ShowChoices( "Select one", choices ).HasValue );

			theMessage.NextShowChoicesResult = 1;
			choice = theApp.Message.ShowChoices( "Select one", choices );
			Assert.True( choice.HasValue );
			Assert.Equal( choice.Value, 1 );
		}

		[Fact]
		public void Progress_bar_works()
		{
			IntergraphApplication theApp = IntergraphApplication.Current;
			ProgressBarTest theProgress = (ProgressBarTest)theApp.ProgressBar;

			theProgress.Maximum = 10;
			theProgress.Value = 0;

			for ( int i = 0; i < 11; ++i )
			{
				theProgress.PerformStep();
			}

			Assert.Equal( theProgress.Value, 10 );
		}
	}

}
