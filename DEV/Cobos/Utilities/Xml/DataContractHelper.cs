// ----------------------------------------------------------------------------
// <copyright file="DataContractHelper.cs" company="Cobos SDK">
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

namespace Cobos.Utilities.Xml
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;

    /// <summary>
    /// Class specification and implementation of <see cref="DataContractHelper"/>.
    /// </summary>
    public class DataContractHelper
    {
        /// <summary>
        /// Default namespace.
        /// </summary>
        public const string DefaultNamespace = "http://schemas.cobos.co.uk/datamodel/1.0.0";

        /// <summary>
        /// Serializes a type to XML.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="value">The value to serialize.</param>
        /// <param name="knownTypes">Extra known types.</param>
        /// <returns>The XML string representation of the object.</returns>
        public static string Serialize<T>(T value, Type[] knownTypes)
        {
            if (value == null)
            {
                return null;
            }

            if (knownTypes == null)
            {
                knownTypes = new Type[0];
            }

            using (var stream = new MemoryStream())
            {
                var serializer = new DataContractSerializer(typeof(T), knownTypes);
                serializer.WriteObject(stream, value);
                stream.Seek(0, SeekOrigin.Begin);

                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Deserializes a type from XML.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="xml">The XML string representation.</param>
        /// <param name="knownTypes">The extra types that the serializer needs to know about.</param>
        /// <returns>The deserialized object representation of the XML string.</returns>
        public static T Deserialize<T>(string xml, Type[] knownTypes)
        {
            if (string.IsNullOrEmpty(xml))
            {
                return default(T);
            }

            if (knownTypes == null)
            {
                knownTypes = new Type[0];
            }

            using (var stream = new MemoryStream())
            {
                byte[] data = System.Text.Encoding.UTF8.GetBytes(xml);
                stream.Write(data, 0, data.Length);
                stream.Position = 0;

                var serializer = new DataContractSerializer(typeof(T), knownTypes);
                return (T)serializer.ReadObject(stream);
            }
        }
    }
}
