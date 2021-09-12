// ----------------------------------------------------------------------------
// <copyright file="CacheFileTests.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Utilities.Tests
{
    using Cobos.Utilities.Cache;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit Tests for the CacheFile class.
    /// </summary>
    [TestClass]
    public class CacheFileTests
    {
        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Delete any old cache file.
        /// 2. Create a new cache file.
        /// 3. Add 3 sections with 10 values each.
        /// 4. Save the cache file.
        /// 5. Re-open the cache file.
        /// 6. Confirm that the values are read back in.
        /// </summary>
        [TestMethod]
        public void Can_cache_items_and_read_back()
        {
            string path = TestManager.ResolvePath("test.cache");

            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

            CacheFile<string> cache = new CacheFile<string>(path);
            cache.Open();

            // Populate the cache
            for (int i = 0; i < 10; ++i)
            {
                cache.Add("Section1", i.ToString());
            }

            for (double i = 0.0; i < 10.0; ++i)
            {
                cache.Add("Section2", (i + 0.5).ToString());
            }

            string[] stringValues = new string[5];

            for (int i = 0; i < 5; ++i)
            {
                stringValues[i] = "String" + i;
            }

            cache["Section3"] = stringValues;

            stringValues = new string[5];

            for (int i = 0; i < 5; ++i)
            {
                stringValues[i] = "String" + (i + 5).ToString();
            }

            cache.Add("Section3", stringValues);
            cache.Add("Section4", stringValues);

            // Empty sections
            cache["Section5"] = null;
            cache["Section6"] = null;

            // Try adding duplicates to make sure they aren't cached more than once
            for (int i = 0; i < 10; ++i)
            {
                cache.Add("Section3", "String" + i);
            }

            // Try adding lowercase duplicates to make sure they aren't cached more than once
            for (int i = 0; i < 10; ++i)
            {
                cache.Add("Section3", ("String" + i).ToLower());
            }

            // Save the cache and re-open
            cache.Save();
            Assert.IsTrue(System.IO.File.Exists(path));

            cache = new CacheFile<string>(path);
            cache.Open();

            // Confirm the values were read
            string[] values;

            values = cache["Section1"];
            Assert.IsNotNull(values);
            Assert.AreEqual(10, values.Length);

            for (int i = 0; i < 10; ++i)
            {
                Assert.AreEqual(i.ToString(), values[i]);
            }

            values = cache["Section2"];
            Assert.IsNotNull(values);
            Assert.AreEqual(10, values.Length);

            for (double i = 0.0; i < 10.0; ++i)
            {
                Assert.AreEqual((i + 0.5).ToString(), values[(int)i]);
            }

            values = cache["Section3"];
            Assert.IsNotNull(values);
            Assert.AreEqual(10, values.Length);

            for (int i = 0; i < 10; ++i)
            {
                Assert.AreEqual("String" + i, values[i]);
            }

            values = cache["Section4"];
            Assert.IsNotNull(values);
            Assert.AreEqual(5, values.Length);

            for (int i = 0; i < 5; ++i)
            {
                Assert.AreEqual("String" + (i + 5).ToString(), values[i]);
            }

            // Empty sections previously added
            values = cache["Section5"];
            Assert.IsNotNull(values);
            Assert.AreEqual(0, values.Length);

            values = cache["Section6"];
            Assert.IsNotNull(values);
            Assert.AreEqual(0, values.Length);

            // Empty section not previously added
            values = cache["Section7"];
            Assert.IsNotNull(values);
            Assert.AreEqual(0, values.Length);

            // Search the contents of the cache
            Assert.IsTrue(cache.Contains("Section3", "String1"));
            Assert.IsTrue(cache.Contains("Section3", "String8"));
            Assert.IsFalse(cache.Contains("Section3", "String999"));
            Assert.IsFalse(cache.Contains("Section999", "String999"));
        }
    }
}
