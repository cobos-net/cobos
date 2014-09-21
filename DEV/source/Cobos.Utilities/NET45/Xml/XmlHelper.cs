// ----------------------------------------------------------------------------
// <copyright file="XmlHelper.cs" company="Cobos SDK">
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
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Web;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// Helper class for serializing XML entities.
    /// </summary>
    public static class XmlHelper
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

            XmlSerializer serializer = new XmlSerializer(typeof(T), null, knownTypes, null, DefaultNamespace);

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = new UnicodeEncoding(false, false);
            settings.Indent = false;
            settings.OmitXmlDeclaration = true;

            using (StringWriter textWriter = new StringWriter())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, settings))
                {
                    serializer.Serialize(xmlWriter, value);
                }

                return textWriter.ToString();
            }
        }

        /// <summary>
        /// Deserializes a type from XML.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
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

            XmlSerializer serializer = new XmlSerializer(typeof(T), null, knownTypes, null, DefaultNamespace);
            XmlReaderSettings settings = new XmlReaderSettings();

            using (StringReader textReader = new StringReader(xml))
            {
                using (XmlReader xmlReader = XmlReader.Create(textReader, settings))
                {
                    return (T)serializer.Deserialize(xmlReader);
                }
            }
        }

        /// <summary>
        /// Deserialize an entity from an XML file.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="entity">Receives the deserialized entity.</param>
        /// <param name="filename">The filename containing the entity.</param>
        public static void Deserialize<T>(out T entity, string filename)
        {
            entity = default(T);

            XmlSerializer s = new XmlSerializer(typeof(T));

            using (TextReader r = new StreamReader(filename))
            {
                entity = (T)s.Deserialize(r);
            }
        }

        /// <summary>
        /// Deserialize an entity from a stream.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="stream">The stream object.</param>
        /// <returns>The deserialized entity.</returns>
        public static T Deserialize<T>(Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(stream);
        }

        /// <summary>
        /// Deserialize an entity from a string.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="data">The string representation of the data.</param>
        /// <returns>The deserialized entity.</returns>
        public static T Deserialize<T>(string data)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(data)))
            {
                return Deserialize<T>(stream);
            }
        }

        /// <summary>
        /// Serializes an entity to file.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="entity">The entity to serialize.</param>
        /// <param name="filename">The full path to of the file.</param>
        public static void Serialize<T>(T entity, string filename)
        {
            XmlSerializer s = new XmlSerializer(typeof(T));

            using (TextWriter w = new StreamWriter(filename))
            {
                s.Serialize(w, entity);
            }
        }

        /// <summary>
        /// Serializes an entity to a stream.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="entity">The entity to serialize.</param>
        /// <param name="stream">The stream to serialize to.</param>
        public static void Serialize<T>(T entity, Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(stream, entity);
        }

        /// <summary>
        /// Serialize an entity to a string.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns>The string representation of the entity.</returns>
        public static string Serialize<T>(T entity)
        {
            using (var stream = new MemoryStream(4096))
            {
                Serialize(entity, stream);
                stream.Seek(0, SeekOrigin.Begin);

                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Serialize an entity to a stream.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="entity">The entity to serialize.</param>
        /// <param name="stream">The stream to serialize to.</param>
        /// <param name="prefix">The namespace prefix.</param>
        /// <param name="ns">The full namespace.</param>
        public static void Serialize<T>(T entity, Stream stream, string prefix, string ns)
        {
            XmlSerializerNamespaces namespaceSerializer = null;

            // Now we must write the response to the stream
            if (prefix != null && ns != null)
            {
                namespaceSerializer = new XmlSerializerNamespaces();
                namespaceSerializer.Add(prefix, ns);
            }

            // Create the serializer and write to the stream
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            if (namespaceSerializer != null)
            {
                serializer.Serialize(stream, entity, namespaceSerializer);
            }
            else
            {
                serializer.Serialize(stream, entity);
            }
        }
    }
}
