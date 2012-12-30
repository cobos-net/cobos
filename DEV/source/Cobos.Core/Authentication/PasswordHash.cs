// ============================================================================
// Filename: PasswordHash.cs
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

System;
using System.Text;
using System.Security.Cryptography;

namespace Cobos.Core.Authentication
{
	/*
	 * PasswordHash - A salted password hashing library
	 * WWW: https://defuse.ca/
	 * Use:
	 *      Use 'HashPassword' to create the initial hash, store that in your DB
	 *      Then use 'ValidatePassword' with the hash from the DB to verify a password
	 *      NOTE: Salting happens automatically, there is no need for a separate salt field in the DB
	 */
	public class PasswordHash
	{
		/// <summary>
		/// Hashes a password
		/// </summary>
		/// <param name="password">The password to hash</param>

		/// <returns>The hashed password as a 128 character hex string</returns>
		public static string HashPassword( string password )
		{
			string salt = GetRandomSalt();
			string hash = Sha256Hex( salt + password );
			return salt + hash;
		}

		/// <summary>
		/// Validates a password
		/// </summary>
		/// <param name="password">The password to test</param>

		/// <param name="correctHash">The hash of the correct password</param>
		/// <returns>True if password is the correct password, false otherwise</returns>
		public static bool ValidatePassword( string password, string correctHash )
		{
			if ( correctHash.Length < 128 )
			{
				throw new ArgumentException( "correctHash must be 128 hex characters!" );
			}
			string salt = correctHash.Substring( 0, 64 );
			string validHash = correctHash.Substring( 64, 64 );
			string passHash = Sha256Hex( salt + password );
			return string.Compare( validHash, passHash ) == 0;
		}

		//returns the SHA256 hash of a string, formatted in hex
		private static string Sha256Hex( string toHash )
		{
			SHA256Managed hash = new SHA256Managed();
			byte[] utf8 = UTF8Encoding.UTF8.GetBytes( toHash );
			return BytesToHex( hash.ComputeHash( utf8 ) );
		}

		//Returns a random 64 character hex string (256 bits)
		private static string GetRandomSalt()
		{
			RNGCryptoServiceProvider random = new RNGCryptoServiceProvider();
			byte[] salt = new byte[ 32 ]; //256 bits
			random.GetBytes( salt );
			return BytesToHex( salt );
		}

		//Converts a byte array to a hex string
		private static string BytesToHex( byte[] toConvert )
		{
			StringBuilder s = new StringBuilder( toConvert.Length * 2 );
			foreach ( byte b in toConvert )
			{
				s.Append( b.ToString( "x2" ) );
			}
			return s.ToString();
		}
	}
}