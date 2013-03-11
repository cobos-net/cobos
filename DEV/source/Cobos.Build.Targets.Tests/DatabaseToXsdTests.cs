// ----------------------------------------------------------------------------
// <copyright file="DatabaseToXsdTests.cs" company="Cobos SDK">
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

namespace Cobos.Build.Targets.Tests
{
    using System;
    using System.IO;
    using Cobos.Build.Targets;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using NUnit.Framework;
    using NSubstitute;

    [TestFixture]
    public class DatabaseToXsdTests
    {
        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. 
        /// </summary>
        [Test]
        public void Can_generate_oracle_schema()
		{
            const string outputFile = @"C:\temp\database.xsd";

            if (File.Exists(outputFile))
            {
                File.Delete(outputFile);
            }

			DatabaseToXsd target = new DatabaseToXsd();
			
            target.BuildEngine = Substitute.For<IBuildEngine>();
            target.ConnectionString = "Data Source=vea795db2.world;User Id=eadev;Password=eadev";
            target.DatabasePlatform = "Oracle";
            target.DatabaseSchema = "EADEV";
            target.DatabaseTables = new TaskItem[] { new TaskItem("AEVEN"), new TaskItem("EVENT") };
            target.OutputFile = outputFile;

            Assert.True(target.Execute());
            Assert.True(File.Exists(outputFile));

            //File.Delete(outputFile);
		}
    }
}
