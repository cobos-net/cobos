// ----------------------------------------------------------------------------
// <copyright file="PasswordHash.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Utilities.Authentication
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// PasswordHash - A salted password hashing library <c>https://defuse.ca/</c>.
    /// </summary>
    /// <remarks>
    /// Use 'HashPassword' to create the initial hash, store that in your DB
    /// Then use 'ValidatePassword' with the hash from the DB to verify a password
    /// NOTE: Salting happens automatically, there is no need for a separate salt field in the DB.
    /// </remarks>
    public static class PasswordHash
    {
        /// <summary>
        /// Hashes a password.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <returns>The hashed password as a 128 character hex string.</returns>
        public static string HashPassword(string password)
        {
            string salt = GetRandomSalt();
            string hash = Sha256Hex(salt + password);
            return salt + hash;
        }

        /// <summary>
        /// Validates a password.
        /// </summary>
        /// <param name="password">The password to test.</param>
        /// <param name="correctHash">The hash of the correct password.</param>
        /// <returns>True if password is the correct password, false otherwise.</returns>
        public static bool ValidatePassword(string password, string correctHash)
        {
            if (correctHash.Length < 128)
            {
                throw new ArgumentException("correctHash must be 128 hex characters!");
            }

            string salt = correctHash.Substring(0, 64);
            string validHash = correctHash.Substring(64, 64);
            string passHash = Sha256Hex(salt + password);
            return string.Compare(validHash, passHash) == 0;
        }

        /// <summary>
        /// Returns the SHA256 hash of a string, formatted in hex.
        /// </summary>
        /// <param name="toHash">The string value to hash.</param>
        /// <returns>The hash of the string.</returns>
        private static string Sha256Hex(string toHash)
        {
            SHA256Managed hash = new SHA256Managed();
            byte[] utf8 = UTF8Encoding.UTF8.GetBytes(toHash);
            return BytesToHex(hash.ComputeHash(utf8));
        }

        /// <summary>
        /// Returns a random 64 character hex string (256 bits).
        /// </summary>
        /// <returns>The random salt value.</returns>
        private static string GetRandomSalt()
        {
            RNGCryptoServiceProvider random = new RNGCryptoServiceProvider();
            byte[] salt = new byte[32]; // 256 bits
            random.GetBytes(salt);
            return BytesToHex(salt);
        }

        /// <summary>
        /// Converts a byte array to a hex string.
        /// </summary>
        /// <param name="toConvert">The byte array to convert.</param>
        /// <returns>The converted hex string.</returns>
        private static string BytesToHex(byte[] toConvert)
        {
            StringBuilder s = new StringBuilder(toConvert.Length * 2);

            foreach (byte b in toConvert)
            {
                s.Append(b.ToString("x2"));
            }

            return s.ToString();
        }
    }
}