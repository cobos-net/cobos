// ============================================================================
// Filename: CacheFileTests.cs
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
using System.Collections.Generic;
using System.IO;
using System.Text;
using Cobos.Utilities.Cache;
using NUnit.Framework;

namespace Cobos.Utilities.Tests
{
	[TestFixture]
	public class CacheFileTests
	{
		[TestCase]
		public void Can_cache_items_and_read_back()
		{
			// Strategy:
			// ---------
			// 1. Delete any old cache file.
			// 2. Create a new cache file.
			// 3. Add 3 sections with 10 values each.
			// 4. Save the cache file.
			// 5. Re-open the cache file.
			// 6. Confirm that the values are read back in.

			string path = TestManager.ResolvePath( "test.cache" );

			if ( System.IO.File.Exists( path ) )
			{
				System.IO.File.Delete( path );
			}

			CacheFile<string> cache = new CacheFile<string>( path );

			Assert.DoesNotThrow( delegate { cache.Open(); } );

			///////////////////////////////////////////////////////////////////
			// Populate the cache
			
			Assert.DoesNotThrow(
				delegate
				{
					for ( int i = 0; i < 10; ++i )
					{
						cache.Add( "Section1", i.ToString() );
					}
				} );

			Assert.DoesNotThrow(
				delegate
				{
					for ( double i = 0.0; i < 10.0; ++i )
					{
						cache.Add( "Section2", (i + 0.5).ToString() );
					}
				} );


			Assert.DoesNotThrow(
				delegate
				{
					string[] stringValues = new string[ 5 ];

					for ( int i = 0; i < 5; ++i )
					{
						stringValues[ i ] = "String" + i;
					}

					cache[ "Section3" ] = stringValues;

					stringValues = new string[ 5 ];

					for ( int i = 0; i < 5; ++i )
					{
						stringValues[ i ] = "String" + (i + 5).ToString();
					}

					cache.Add( "Section3", stringValues );
					cache.Add( "Section4", stringValues );

				} );

			// Empty sections
			Assert.DoesNotThrow(
				delegate
				{
					cache[ "Section5" ] = null;
					cache[ "Section6" ] = null;
				} );

			// Try adding duplicates to make sure they aren't cached more than once
			for ( int i = 0; i < 10; ++i )
			{
				cache.Add( "Section3", "String" + i );
			}

			// Try adding lowercase duplicates to make sure they aren't cached more than once
			for ( int i = 0; i < 10; ++i )
			{
				cache.Add( "Section3", ("String" + i).ToLower() );
			}

			///////////////////////////////////////////////////////////////////
			// Save the cache and re-open

			Assert.DoesNotThrow( delegate { cache.Save(); } );
			Assert.True( System.IO.File.Exists( path ) );

			cache = new CacheFile<string>( path );

			Assert.DoesNotThrow( delegate { cache.Open(); } );

			///////////////////////////////////////////////////////////////////
			// Confirm the values were read

			string[] values;

			values = cache[ "Section1" ];
			Assert.NotNull( values );
			Assert.AreEqual( 10, values.Length );

			for ( int i = 0; i < 10; ++i )
			{
				Assert.AreEqual( i.ToString(), values[ i ] );
			}

			values = cache[ "Section2" ];
			Assert.NotNull( values );
			Assert.AreEqual( 10, values.Length );

			for ( double i = 0.0; i < 10.0; ++i )
			{
				Assert.AreEqual( (i + 0.5).ToString(), values[ (int)i ] );
			}

			values = cache[ "Section3" ];
			Assert.NotNull( values );
			Assert.AreEqual( 10, values.Length );

			for ( int i = 0; i < 10; ++i )
			{
				Assert.AreEqual( "String" + i, values[ i ] );
			}

			values = cache[ "Section4" ];
			Assert.NotNull( values );
			Assert.AreEqual( 5, values.Length );

			for ( int i = 0; i < 5; ++i )
			{
				Assert.AreEqual( "String" + (i + 5).ToString(), values[ i ] );
			}

			// Empty sections previously added
			values = cache[ "Section5" ];
			Assert.NotNull( values );
			Assert.IsEmpty( values );

			values = cache[ "Section6" ];
			Assert.NotNull( values );
			Assert.IsEmpty( values );

			// Empty section not previously added
			values = cache[ "Section7" ];
			Assert.NotNull( values );
			Assert.IsEmpty( values );

			// Search the contents of the cache
			Assert.True( cache.Contains( "Section3", "String1" ) );
			Assert.True( cache.Contains( "Section3", "String8" ) );
			Assert.False( cache.Contains( "Section3", "String999" ) );
			Assert.False( cache.Contains( "Section999", "String999" ) );
		}
	}
}
