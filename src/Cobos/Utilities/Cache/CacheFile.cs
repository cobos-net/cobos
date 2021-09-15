// ----------------------------------------------------------------------------
// <copyright file="CacheFile.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Utilities.Cache
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Very simple cache file for storing values grouped in sections.
    /// The structure is similar to an INI file using section values.
    /// </summary>
    /// <typeparam name="T">The type of object stored in the cache.</typeparam>
    public class CacheFile<T>
    {
        /// <summary>
        /// Regular expression for matching section header names.
        /// </summary>
        private static readonly Regex RegexSectionHeader = new Regex(@"\[(?<name>\w*)\]");

        /// <summary>
        /// The section values for the cache file.
        /// </summary>
        private readonly Dictionary<string, HashSet<T>> sectionValues = new Dictionary<string, HashSet<T>>(StringComparer.CurrentCultureIgnoreCase);

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheFile{T}"/> class.
        /// </summary>
        /// <param name="path">The path to the cache file.</param>
        public CacheFile(string path)
        {
            this.Path = path;
        }

        /// <summary>
        /// Gets the path to the cache file.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Gets an array of values for the specified section.
        /// </summary>
        /// <param name="sectionName">The name of the section.</param>
        /// <returns>An array of values in the section.</returns>
        public T[] this[string sectionName]
        {
            get
            {
                if (!this.sectionValues.TryGetValue(sectionName, out HashSet<T> values))
                {
                    values = this.CreateValueCollection(null);
                    this.sectionValues[sectionName] = values;
                }

                return values.ToArray();
            }

            set
            {
                this.sectionValues[sectionName] = this.CreateValueCollection(value);
            }
        }

        /// <summary>
        /// Open the cache file.
        /// </summary>
        public void Open()
        {
            this.sectionValues.Clear();

            if (!System.IO.File.Exists(this.Path))
            {
                return;
            }

            // The cache will never be that large, so read all into memory rather than a line at a time
            using (TextReader reader = new StreamReader(this.Path))
            {
                HashSet<T> section = null;
                string line;

                Type theType = typeof(T);

                while ((line = reader.ReadLine()) != null)
                {
                    Match match = RegexSectionHeader.Match(line);

                    if (match.Success)
                    {
                        section = this.CreateValueCollection(null);
                        this.sectionValues[match.Groups["name"].Value] = section;
                    }
                    else
                    {
                        if (section != null)
                        {
                            T value = (T)TypeDescriptor.GetConverter(theType).ConvertFrom(line);
                            section.Add(value);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Save the cache file.
        /// </summary>
        public void Save()
        {
            using (TextWriter writer = new StreamWriter(this.Path))
            {
                foreach (string key in this.sectionValues.Keys)
                {
                    writer.WriteLine("[" + key + "]");

                    HashSet<T> values = this.sectionValues[key];

                    foreach (T value in values)
                    {
                        writer.WriteLine(value);
                    }
                }
            }
        }

        /// <summary>
        /// Add a value into a section.
        /// </summary>
        /// <param name="sectionName">The name of the section.</param>
        /// <param name="value">The value to add.</param>
        public void Add(string sectionName, T value)
        {
            if (!this.sectionValues.TryGetValue(sectionName, out HashSet<T> values))
            {
                values = this.CreateValueCollection(null);
                this.sectionValues[sectionName] = values;
            }

            values.Add(value);
        }

        /// <summary>
        /// Add a collection of values into a section.
        /// </summary>
        /// <param name="sectionName">The name of the section.</param>
        /// <param name="value">The values to add.</param>
        public void Add(string sectionName, T[] value)
        {
            if (!this.sectionValues.TryGetValue(sectionName, out HashSet<T> values))
            {
                values = this.CreateValueCollection(null);
                this.sectionValues[sectionName] = values;
            }

            foreach (T v in value)
            {
                values.Add(v);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the section contains a specific value.
        /// </summary>
        /// <param name="sectionName">The name of the section.</param>
        /// <param name="value">The value to look for.</param>
        /// <returns>true if the section contains the value; otherwise false.</returns>
        public bool Contains(string sectionName, T value)
        {
            if (!this.sectionValues.ContainsKey(sectionName))
            {
                return false;
            }

            HashSet<T> values = this.sectionValues[sectionName];

            return values.Contains(value);
        }

        /// <summary>
        /// Helper class to create an empty value collection.
        /// </summary>
        /// <param name="values">The values to create a collection for.</param>
        /// <returns>An object representing a hash set containing the values.</returns>
        private HashSet<T> CreateValueCollection(IEnumerable<T> values)
        {
            if (values == null)
            {
                return new HashSet<T>(this.GetComparer());
            }
            else
            {
                return new HashSet<T>(values, this.GetComparer());
            }
        }

        /// <summary>
        /// Pseudo-specialization for string generic types.
        /// </summary>
        /// <returns>A comparer for the type.</returns>
        private IEqualityComparer<T> GetComparer()
        {
            if (typeof(T) == typeof(string))
            {
                return (IEqualityComparer<T>)StringComparer.CurrentCultureIgnoreCase;
            }
            else
            {
                return EqualityComparer<T>.Default;
            }
        }
    }
}
