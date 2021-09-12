// ----------------------------------------------------------------------------
// <copyright file="TestResource.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Utilities.Tests.Threading.Resource
{
    using System;

    /// <summary>
    /// Dummy resource to simulate work for threading tests.
    /// </summary>
    internal class TestResource
    {
        /// <summary>
        /// The id of the resource.
        /// </summary>
        private readonly long id;

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
        /// Gets the time to work for.
        /// </summary>
        public int WorkPeriodMs { get; }

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

            _ = result + 1;
        }
    }
}
