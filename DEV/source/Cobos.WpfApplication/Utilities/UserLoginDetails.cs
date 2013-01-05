// ----------------------------------------------------------------------------
// <copyright file="UserLoginDetails.cs" company="Cobos SDK">
//
//      Copyright (c) 2009-2012 Nicholas Davis - nick@cobos.co.uk
//
//      Cobos Software Development Kit
//
//      Permission is hereby granted, free of charge, to any person obtaining
//      a copy of this software and associated documentation files (the
//      "Software"), to deal in the Software without restriction, including
//      without limitation the rights to use, copy, modify, merge, publish,
//      distribute, sublicense, and/or sell copies of the Software, and to
//      permit persons to whom the Software is furnished to do so, subject to
//      the following conditions:
//      
//      The above copyright notice and this permission notice shall be
//      included in all copies or substantial portions of the Software.
//      
//      THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//      EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//      MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//      NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//      LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
//      OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
//      WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.WpfApplication.Utilities
{
    /// <summary>
    /// User Login information.
    /// </summary>
    public class UserLoginDetails
    {
        /// <summary>
        /// The username.
        /// </summary>
        public string Username;

        /// <summary>
        /// The user password.
        /// </summary>
        public string Password;

        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The user password.</param>
        public UserLoginDetails(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}
