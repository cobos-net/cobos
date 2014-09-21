// ----------------------------------------------------------------------------
// <copyright file="PropertyMapTests.cs" company="Cobos SDK">
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

namespace Cobos.Data.Tests.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Cobos.Data.Mapping;
    using NUnit.Framework;

    /// <summary>
    /// Unit tests for the <see cref="PropertyMap"/> class.
    /// </summary>
    [TestFixture]
    public class PropertyMapTests
    {
        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Create a property map for a known type.
        /// 2. Test that property descriptors can be retrieved.
        /// </summary>
        [TestCase]
        public void Can_create_and_use_property_map()
        {
            // Arrange.
            Assert.DoesNotThrow(() =>
                PropertyMapRegistry.Instance.RegisterType(typeof(TestClass)));

            PropertyMap map = null;
            PropertyDescriptor property = null;

            // Act.
            map = PropertyMapRegistry.Instance[typeof(TestClass)];
            Assert.NotNull(map);
            
            // Assert.
            property = map["TestInteger"];
            Assert.NotNull(property);
            Assert.AreEqual("Integer", property.Column);
            Assert.AreEqual(typeof(int), property.Property.PropertyType);

            property = map["TestString"];
            Assert.NotNull(property);
            Assert.AreEqual("String", property.Column);
            Assert.AreEqual(typeof(string), property.Property.PropertyType);
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Create a property map for a known type.
        /// 2. Test that property descriptors can be retrieved for reference properties.
        /// </summary>
        [TestCase]
        public void Can_access_reference_properties()
        {
            PropertyMap map = null;
            PropertyDescriptor property = null;

            // Act.
            map = PropertyMapRegistry.Instance[typeof(TestClass)];
            Assert.NotNull(map);

            property = map["TestReference1.TestInteger"];
            Assert.NotNull(property);
            Assert.AreEqual("Reference1Integer", property.Column);
            Assert.AreEqual(typeof(int), property.Property.PropertyType);

            property = map["TestReference1.TestString"];
            Assert.NotNull(property);
            Assert.AreEqual("Reference1String", property.Column);
            Assert.AreEqual(typeof(string), property.Property.PropertyType);
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Create a property map for a known type.
        /// 2. Test that property descriptors can be retrieved for nested reference properties.
        /// </summary>
        [TestCase]
        public void Can_access_nested_reference_properties()
        {
            PropertyMap map = null;
            PropertyDescriptor property = null;

            // Act.
            map = PropertyMapRegistry.Instance[typeof(TestClass)];
            Assert.NotNull(map);

            property = map["TestReference1.TestObject.TestInteger"];
            Assert.NotNull(property);
            Assert.AreEqual("Reference2Integer", property.Column);
            Assert.AreEqual(typeof(int), property.Property.PropertyType);

            property = map["TestReference1.TestObject.TestString"];
            Assert.NotNull(property);
            Assert.AreEqual("Reference2String", property.Column);
            Assert.AreEqual(typeof(string), property.Property.PropertyType);
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Create a property map for a known type.
        /// 2. Test that property descriptors can be retrieved a property that is a nested type.
        /// </summary>
        [TestCase]
        public void Can_access_nested_type()
        {
            PropertyMap map = null;
            PropertyDescriptor property = null;

            // Act.
            map = PropertyMapRegistry.Instance[typeof(TestClass)];
            Assert.NotNull(map);

            property = map["TestNested.TestNestedInteger"];
            Assert.NotNull(property);
            Assert.AreEqual("NestedInteger", property.Column);
            Assert.AreEqual(typeof(int), property.Property.PropertyType);

            property = map["TestNested.TestNestedString"];
            Assert.NotNull(property);
            Assert.AreEqual("NestedString", property.Column);
            Assert.AreEqual(typeof(string), property.Property.PropertyType);

            property = map["TestNested.TestObject.TestInteger"];
            Assert.NotNull(property);
            Assert.AreEqual("Reference1Integer", property.Column);
            Assert.AreEqual(typeof(int), property.Property.PropertyType);

            property = map["TestNested.TestObject.TestString"];
            Assert.NotNull(property);
            Assert.AreEqual("Reference1String", property.Column);
            Assert.AreEqual(typeof(string), property.Property.PropertyType);
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Create a property map for a known type.
        /// 2. Test that property descriptors can be retrieved for a property of the same type.
        /// </summary>
        [TestCase]
        public void Can_access_child_properties()
        {
            PropertyMap map = null;
            PropertyDescriptor property = null;

            // Act.
            map = PropertyMapRegistry.Instance[typeof(TestClass)];
            Assert.NotNull(map);

            property = map["TestChild.TestInteger"];
            Assert.NotNull(property);
            Assert.AreEqual("Integer", property.Column);
            Assert.AreEqual(typeof(int), property.Property.PropertyType);

            property = map["TestChild.TestString"];
            Assert.NotNull(property);
            Assert.AreEqual("String", property.Column);
            Assert.AreEqual(typeof(string), property.Property.PropertyType);

            property = map["TestChild.TestReference1.TestInteger"];
            Assert.NotNull(property);
            Assert.AreEqual("Reference1Integer", property.Column);
            Assert.AreEqual(typeof(int), property.Property.PropertyType);

            property = map["TestChild.TestReference1.TestString"];
            Assert.NotNull(property);
            Assert.AreEqual("Reference1String", property.Column);
            Assert.AreEqual(typeof(string), property.Property.PropertyType);
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Create a property map for a known type.
        /// 2. Test that property descriptors can be retrieved deeply nested properties.
        /// </summary>
        [TestCase]
        public void Can_access_deep_nested_properties()
        {
            PropertyMap map = null;
            PropertyDescriptor property = null;

            // Act.
            map = PropertyMapRegistry.Instance[typeof(TestClass)];
            Assert.NotNull(map);

            property = map["TestChild.TestChild.TestInteger"];
            Assert.NotNull(property);
            Assert.AreEqual("Integer", property.Column);
            Assert.AreEqual(typeof(int), property.Property.PropertyType);

            property = map["TestChild.TestChild.TestChild.TestChild.TestChild.TestString"];
            Assert.NotNull(property);
            Assert.AreEqual("String", property.Column);
            Assert.AreEqual(typeof(string), property.Property.PropertyType);
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Create a property map for a known type.
        /// 2. Test that property descriptors can be retrieved for references and values without name attributes.
        /// </summary>
        [TestCase]
        public void Can_access_no_name_properties()
        {
            PropertyMap map = null;
            PropertyDescriptor property = null;

            // Act.
            map = PropertyMapRegistry.Instance[typeof(TestClass)];
            Assert.NotNull(map);

            property = map["TestReference1.TestNoColumnName"];
            Assert.NotNull(property);
            Assert.AreEqual("TestNoColumnName", property.Column);
            Assert.AreEqual(typeof(float), property.Property.PropertyType);

            property = map["TestNoTableName.TestInteger"];
            Assert.NotNull(property);
            Assert.AreEqual("Integer", property.Column);
            Assert.AreEqual(typeof(int), property.Property.PropertyType);

            property = map["TestNoTableName.TestString"];
            Assert.NotNull(property);
            Assert.AreEqual("String", property.Column);
            Assert.AreEqual(typeof(string), property.Property.PropertyType);

            property = map["TestNoTableName.TestNoColumnName"];
            Assert.NotNull(property);
            Assert.AreEqual("TestNoColumnName", property.Column);
            Assert.AreEqual(typeof(float), property.Property.PropertyType);
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Create a property map for a known type.
        /// 2. Test null-able properties can be accessed.
        /// </summary>
        [TestCase]
        public void Can_access_nullable_property_types()
        {
            PropertyMap map = null;
            PropertyDescriptor property = null;

            // Act.
            map = PropertyMapRegistry.Instance[typeof(TestClass)];
            Assert.NotNull(map);

            property = map["NullableInt"];
            Assert.NotNull(property);
            Assert.AreEqual("NullableInt", property.Column);
            Assert.AreEqual(typeof(int?), property.Property.PropertyType);
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Create a property map for a known type.
        /// 2. Test collection properties are ignored.
        /// </summary>
        [TestCase]
        public void Collection_property_types_are_ignored()
        {
            PropertyMap map = null;
            PropertyDescriptor property = null;

            // Act.
            map = PropertyMapRegistry.Instance[typeof(TestClass)];
            Assert.NotNull(map);

            property = map["ArrayIgnored"];
            Assert.Null(property);
            property = map["ListIgnored"];
            Assert.Null(property);
        }

        /// <summary>
        /// Test class for property mappings.
        /// </summary>
        [Table(Name = "TestTable")]
        public class TestClass
        {
            /// <summary>
            /// Gets or sets the test property.
            /// </summary>
            [Column(Name = "Integer")]
            public int TestInteger
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the test property.
            /// </summary>
            [Column(Name = "String")]
            public string TestString
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the test property.
            /// </summary>
            [Column(Name = "NullableInt")]
            public int? NullableInt
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the test property.
            /// </summary>
            public Reference1 TestReference1
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the test property.
            /// </summary>
            public Nested TestNested
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the test property.
            /// </summary>
            public TestClass TestChild
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the test property.
            /// </summary>
            public NoTableName TestNoTableName
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the test property that is ignored by the mapper.
            /// </summary>
            public int[] ArrayIgnored
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the test property that is ignored by the mapper.
            /// </summary>
            public List<string> ListIgnored
            {
                get;
                set;
            }

            /// <summary>
            /// Nested type.
            /// </summary>
            [Table(Name = "Nested")]
            public class Nested
            {
                /// <summary>
                /// Gets or sets the test property.
                /// </summary>
                [Column(Name = "NestedInteger")]
                public int TestNestedInteger
                {
                    get;
                    set;
                }

                /// <summary>
                /// Gets or sets the test property.
                /// </summary>
                [Column(Name = "NestedString")]
                public string TestNestedString
                {
                    get;
                    set;
                }

                /// <summary>
                /// Gets or sets the test property.
                /// </summary>
                public Reference1 TestObject
                {
                    get;
                    set;
                }
            }
        }

        /// <summary>
        /// This class is referenced by the <see cref="TestClass"/>.
        /// </summary>
        [Table(Name = "Reference1")]
        public class Reference1
        {
            /// <summary>
            /// Gets or sets the test property.
            /// </summary>
            [Column(Name = "Reference1Integer")]
            public int TestInteger
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the test property.
            /// </summary>
            [Column(Name = "Reference1String")]
            public string TestString
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the value with no name attribute.
            /// </summary>
            public float TestNoColumnName
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the test property.
            /// </summary>
            public Reference2 TestObject
            {
                get;
                set;
            }
        }

        /// <summary>
        /// This class is referenced by the <see cref="TestClass"/>.
        /// </summary>
        [Table(Name = "Reference2")]
        public class Reference2
        {
            /// <summary>
            /// Gets or sets the test property.
            /// </summary>
            [Column(Name = "Reference2Integer")]
            public int TestInteger
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the test property.
            /// </summary>
            [Column(Name = "Reference2String")]
            public string TestString
            {
                get;
                set;
            }
        }

        /// <summary>
        /// Test class with no table name attribute.
        /// </summary>
        public class NoTableName
        {
            /// <summary>
            /// Gets or sets the test property.
            /// </summary>
            [Column(Name = "Integer")]
            public int TestInteger
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the test property.
            /// </summary>
            [Column(Name = "String")]
            public string TestString
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the value with no name.
            /// </summary>
            public float TestNoColumnName
            {
                get;
                set;
            }
        }
    }
}
