// ============================================================================
// Filename: StringExtensions.cs
// Description: 
// ----------------------------------------------------------------------------
// Created by: N.Davis                          Date: 27-Nov-09
// Modified by:                                 Date:
// ============================================================================
// Copyright (c) 2009-2011 Nicholas Davis		nick@cobos.co.uk
//
// Cobos Software Development Kit v0.1
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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Security;
using Cobos.Utilities.Text;

namespace Cobos.Utilities.Extensions
{
	public static class StringExtensions
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static string CamelCase( this string s )
		{
			StringBuilder result = new StringBuilder( s.Length );

			string[] tokens = s.Split( ' ', '\n', '/', '\\', '<', '>', '&', '?', '#', '!', '%', '\"', '\'' );

			foreach ( string t in tokens )
			{
				if ( string.IsNullOrEmpty( t ) )
				{
					continue;
				}
				result.AppendFormat( "{0}{1}", char.ToUpper( t[ 0 ] ), t.Substring( 1 ) );
			}

			return result.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static string EscapeXml( this string s )
		{
			if ( s == string.Empty )
			{
				return s;
			}

			return SecurityElement.Escape( s );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static string UnescapeXml( this string s )
		{
			return HttpUtility.HtmlDecode( s );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static byte[] ConvertToByteArray( this string s )
		{
			return new System.Text.UTF8Encoding().GetBytes( s );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="bytes"></param>
		/// <returns></returns>
		public static string ConvertToString( this byte[] bytes )
		{
			return new System.Text.UTF8Encoding().GetString( bytes );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool IsQuoted( this string value )
		{
			return (value.StartsWith( "'" ) && value.EndsWith( "'" ));
		}

		/// <summary>
		/// NOTE: NOT SQL OR DATABASE SAFE. --> Use SQLQuote (and family)
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string Quote( this string value )
		{
			// is it already quoted ?
			if ( value.StartsWith( "'" ) && value.EndsWith( "'" ) )
			{
				return value;
			}
			return "'" + value + "'";
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <param name="maxLength"></param>
		/// <returns></returns>
		public static string SQLQuote( this string value, int maxLength )
		{
			if ( value.Length == 0 )
			{
				return "''";
			}
			// return SQL friendly quoted text 
			StringBuilder newValue = new StringBuilder( value );

			// if the string is already quoted, remove quotes so we can check for special characters
			if ( value.StartsWith( "'" ) && value.EndsWith( "'" ) && value.Length > 1 )  // disallow match on only single quote
			{
				newValue.Remove( value.Length - 1, 1 );
				newValue.Remove( 0, 1 );
				value = newValue.ToString();
			}

			// search the string for these characters

			// single quote, backslash, double quote or question mark
			char[] lookFor = new char[] { '\'', '\\', '"', '?' };

			// must be delimited with ...
			// another single quote, backslash before real backslash, backslash before double quoute, backslash before questionmark
			char[] appendTo = new char[] { '\'', '\\', '\\', '\\' };

			// TODO: perhaps could use regex here.  Not sure if that will be quicker tho ... 
			int k = 0;
			for ( int i = 0; i < value.Length; ++i, ++k )
			{
				for ( int j = 0; j < lookFor.Length; ++j )
				{
					if ( value[ i ] == lookFor[ j ] )
					{
						newValue.Insert( k++, appendTo[ j ] );
					}
				}
			}

			if ( maxLength != 0 )
			{
				if ( newValue.Length > maxLength )
					newValue.Length = maxLength;
			}

			newValue.Insert( 0, '\'' );
			newValue.Append( '\'' );
			return newValue.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string SQLQuoteID( this string value )
		{
			if ( value.IsQuoted() )
			{
				return value;
			}
			// if theres guaranteed no funny chars, like an ID field - do it the easy way
			return "'" + value + "'";
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string SQLQuoteOrNULL( this string value )
		{
			if ( string.IsNullOrEmpty( value ) )
			{
				return "NULL";
			}
			else
			{
				return SQLQuote( value, 0 );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <param name="maxLength"></param>
		/// <returns></returns>
		public static string SQLQuoteOrNULL( this string value, int maxLength )
		{
			if ( string.IsNullOrEmpty( value ) )
			{
				return "NULL";
			}
			else
			{
				return SQLQuote( value, maxLength );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string SQLQuote( this string value )
		{
			return SQLQuote( value, 0 );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="list"></param>
		/// <returns></returns>
		public static string[] GetQuotedValues( this List<string> list )
		{
			List<string> quotedids = list.ConvertAll(
				 new Converter<string, string>( delegate( string id )
				 {
					 return id.SQLQuote();
				 } ) );

			return quotedids.ToArray();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool IsPhoneNumber( this string value )
		{
			return RegExHelper.PhoneExpression.IsMatch( value );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool IsTime( this string value )
		{
			return RegExHelper.TimeExpression.IsMatch( value );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool IsNumeric( this string value )
		{
			return RegExHelper.NumericExpression.IsMatch( value );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <param name="name"></param>
		public static void ValidateEmptyField( this string value, string name )
		{
			if ( string.IsNullOrEmpty( value ) )
			{
				throw new Exception( "Error: " + name + " cannot be an empty field" );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="values"></param>
		/// <param name="names"></param>
		public static void ValidateEmptyFields( this string[] values, string[] names )
		{
			if ( values.Length != names.Length )
			{
				string exceptionString = "Arrays must have identical length in function ValidateEmptyFields.";
				throw new Exception( exceptionString );
			}

			for ( int i = 0; i < values.Length; ++i )
			{
				if ( string.IsNullOrEmpty( values[ i ] ) )
				{
					string exceptionString = "Error: " + names[ i ] + " cannot be an empty field";
					throw new Exception( exceptionString );
				}
			}
		}

		/// <summary>
		/// Minify the string, removing \r\n\t and excess whitespace
		/// Particularly useful when reading CDATA from Xml.
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static string RemoveWhitespace( this string s )
		{
			char[] arr = s.ToCharArray();

			for ( int i = 0; i < arr.Length; i++ )
			{
				switch ( arr[ i ] )
				{
				case '\t':
				case '\r':
				case '\n':
					arr[ i ] = ' ';
					break;
				}
			}

			s = new string( arr );

			s = Regex.Replace( s, @"\s+", " " );

			return s.Trim();
		}

	}
}
