// ----------------------------------------------------------------------------
// <copyright file="DatabaseSettingDetails.cs" company="Cobos SDK">
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
    /// Struture to hold Database settings.
    /// </summary>
    public class DatabaseSettingsDetails
    {
        /// <summary>
        /// The database host.
        /// </summary>
        public string Hostname;

        /// <summary>
        /// The port that the database server is running on.
        /// </summary>
        public int Port;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="hostname">The database host.</param>
        /// <param name="port">The port that the database server is running on.</param>
        public DatabaseSettingsDetails(string hostname, int port)
        {
            Hostname = hostname;
            Port = port;
        }
    }
}
