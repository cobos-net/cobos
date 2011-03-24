using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xunit;

namespace Intergraph.AsiaPac.Data.Tests.Events
{
	public class CommentsTests
	{
		[Fact]
		public void Can_query_comments_from_database()
		{
			List<EventComment> comments = null;

			Assert.DoesNotThrow(
				delegate
				{
					using ( DatabaseConnection dbconn = new DatabaseConnection( TestManager.DatabaseConnection ) )
					{
						EventCommentDataAdapter adapter = new EventCommentDataAdapter( dbconn );
						comments = adapter.GetData( null, null, null );
					}
				} );

			Assert.NotNull( comments );
			Assert.NotEmpty( comments );

			Debug.Print( "Number of comments found: " + comments.Count.ToString() );
		}

	}
}
