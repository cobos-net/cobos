// ----------------------------------------------------------------------------
// <copyright file="DataContractHelper.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
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
                return default;
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
