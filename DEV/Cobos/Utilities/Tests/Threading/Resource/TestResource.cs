// ----------------------------------------------------------------------------
// <copyright file="TestResource.cs" company="Cobos SDK">
//
//      Copyright (c) 2009-2014 Nicholas Davis - nick@cobos.co.uk
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

namespace Cobos.Utilities.Tests.Threading.Resource
{
    using System;
    using System.Threading;
    using Cobos.Utilities.Threading.Resource;

    /// <summary>
    /// Dummy resource to simulate work for threading tests.
    /// </summary>
    internal class TestResource
    {
        /// <summary>
        /// The time to work for.
        /// </summary>
        public readonly int WorkPeriodMs;

        /// <summary>
        /// The id of the resource.
        /// </summary>
        private long id;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestResource"/> class.
        /// </summary>
        /// <param name="i">The id of the resource.</param>
        /// <param name="workPeriodMs">The time to work for.</param>
        public TestResource(long i, int workPeriodMs)
        {
            this.id = i;

            // make the threads work within a tolerance of the work period 
            // to avoid the situation where all threads execute in an 
            // orderly fashion.  We want to introduce as much uncertainty 
            // as possible.
            double tolerance = (double)workPeriodMs * 0.15; // +/- 15% max

            Random rand = new Random();

            int delta = rand.Next((int)Math.Floor(tolerance));

            if (rand.Next(1) == 1)
            {
                this.WorkPeriodMs = workPeriodMs + delta;
            }
            else
            {
                this.WorkPeriodMs = workPeriodMs - delta;
            }
        }

        /// <summary>
        /// Do some work.
        /// </summary>
        public void DoWork()
        {
            Console.WriteLine("Working with resource {0}", this.id);

            DateTime end = DateTime.Now.Add(new TimeSpan(0, 0, 0, 0, this.WorkPeriodMs));
            Random rand = new Random();

            int result = 0;

            while (DateTime.Now < end)
            {
                int value = rand.Next(10);
                result = value / (rand.Next(2) + 1);
            }

            result = result + 1;
        }
    }
}
