// ----------------------------------------------------------------------------
// <copyright file="CacheFile.cs" company="Cobos SDK">
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Cobos.Utilities.Cache
{
    /// <summary>
    /// Very simple cache file for storing values grouped in sections.
    /// The structure is similar to an ini file using section values.
    /// </summary>
    public class CacheFile<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public readonly string Path;

        /// <summary>
        /// 
        /// </summary>
        private readonly Dictionary<string, HashSet<T>> SectionValues = new Dictionary<string, HashSet<T>>(StringComparer.CurrentCultureIgnoreCase);

        /// <summary>
        /// 
        /// </summary>
        private readonly Regex RegexSectionHeader = new Regex(@"\[(?<name>\w*)\]");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public CacheFile(string path)
        {
            Path = path;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Open()
        {
            SectionValues.Clear();

            if (!System.IO.File.Exists(Path))
            {
                return;
            }

            // The cache will never be that large, so read all into memory rather than a line at a time
            using (TextReader reader = new StreamReader(Path))
            {
                HashSet<T> section = null;
                string line;

                Type theType = typeof(T);

                while ((line = reader.ReadLine()) != null)
                {
                    Match match = RegexSectionHeader.Match(line);

                    if (match.Success)
                    {
                        // found a new section 
                        section = CreateValueCollection(null);
                        SectionValues[match.Groups["name"].Value] = section;
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
        /// 
        /// </summary>
        public void Save()
        {
            using (TextWriter writer = new StreamWriter(Path))
            {
                foreach (string key in SectionValues.Keys)
                {
                    writer.WriteLine("[" + key + "]");

                    HashSet<T> values = SectionValues[key];

                    foreach (T value in values)
                    {
                        writer.WriteLine(value);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public T[] this[string sectionName]
        {
            get
            {
                HashSet<T> values;

                if (!SectionValues.TryGetValue(sectionName, out values))
                {
                    values = CreateValueCollection(null);
                    SectionValues[sectionName] = values;
                }

                return values.ToArray();
            }
            set
            {
                SectionValues[sectionName] = CreateValueCollection(value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="value"></param>
        public void Add(string sectionName, T value)
        {
            HashSet<T> values;

            if (!SectionValues.TryGetValue(sectionName, out values))
            {
                values = CreateValueCollection(null);
                SectionValues[sectionName] = values;
            }

            values.Add(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="value"></param>
        public void Add(string sectionName, T[] value)
        {
            HashSet<T> values;

            if (!SectionValues.TryGetValue(sectionName, out values))
            {
                values = CreateValueCollection(null);
                SectionValues[sectionName] = values;
            }

            foreach (T v in value)
            {
                values.Add(v);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Contains(string sectionName, T value)
        {
            if (!SectionValues.ContainsKey(sectionName))
            {
                return false;
            }

            HashSet<T> values = SectionValues[sectionName];

            return values.Contains(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        private HashSet<T> CreateValueCollection(IEnumerable<T> values)
        {
            if (values == null)
            {
                return new HashSet<T>(GetComparer());
            }
            else
            {
                return new HashSet<T>(values, GetComparer());
            }
        }

        /// <summary>
        /// Pseudo-specialisation for string generic types.
        /// </summary>
        /// <returns></returns>
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
