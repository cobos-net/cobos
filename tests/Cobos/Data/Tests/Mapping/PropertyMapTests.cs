// ----------------------------------------------------------------------------
// <copyright file="PropertyMapTests.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data.Tests.Mapping
{
    using System.Collections.Generic;
    using Cobos.Data.Mapping;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for the <see cref="PropertyMap"/> class.
    /// </summary>
    [TestClass]
    public class PropertyMapTests
    {
        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Create a property map for a known type.
        /// 2. Test that property descriptors can be retrieved.
        /// </summary>
        [TestMethod]
        public void Can_create_and_use_property_map()
        {
            // Arrange.
            PropertyMapRegistry.Instance.RegisterType(typeof(TestClass));

            // Act.
            var map = PropertyMapRegistry.Instance[typeof(TestClass)];
            Assert.IsNotNull(map);

            // Assert.
            var property = map["TestInteger"];
            Assert.IsNotNull(property);
            Assert.AreEqual("Integer", property.Column);
            Assert.AreEqual(typeof(int), property.Property.PropertyType);

            property = map["TestString"];
            Assert.IsNotNull(property);
            Assert.AreEqual("String", property.Column);
            Assert.AreEqual(typeof(string), property.Property.PropertyType);
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Create a property map for a known type.
        /// 2. Test that property descriptors can be retrieved for reference properties.
        /// </summary>
        [TestMethod]
        public void Can_access_reference_properties()
        {
            // Act.
            PropertyMap map = PropertyMapRegistry.Instance[typeof(TestClass)];
            Assert.IsNotNull(map);
            PropertyDescriptor property = map["TestReference1.TestInteger"];
            Assert.IsNotNull(property);
            Assert.AreEqual("Reference1Integer", property.Column);
            Assert.AreEqual(typeof(int), property.Property.PropertyType);

            property = map["TestReference1.TestString"];
            Assert.IsNotNull(property);
            Assert.AreEqual("Reference1String", property.Column);
            Assert.AreEqual(typeof(string), property.Property.PropertyType);
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Create a property map for a known type.
        /// 2. Test that property descriptors can be retrieved for nested reference properties.
        /// </summary>
        [TestMethod]
        public void Can_access_nested_reference_properties()
        {
            // Act.
            PropertyMap map = PropertyMapRegistry.Instance[typeof(TestClass)];
            Assert.IsNotNull(map);
            PropertyDescriptor property = map["TestReference1.TestObject.TestInteger"];
            Assert.IsNotNull(property);
            Assert.AreEqual("Reference2Integer", property.Column);
            Assert.AreEqual(typeof(int), property.Property.PropertyType);

            property = map["TestReference1.TestObject.TestString"];
            Assert.IsNotNull(property);
            Assert.AreEqual("Reference2String", property.Column);
            Assert.AreEqual(typeof(string), property.Property.PropertyType);
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Create a property map for a known type.
        /// 2. Test that property descriptors can be retrieved a property that is a nested type.
        /// </summary>
        [TestMethod]
        public void Can_access_nested_type()
        {
            // Act.
            var map = PropertyMapRegistry.Instance[typeof(TestClass)];
            Assert.IsNotNull(map);

            var property = map["TestNested.TestNestedInteger"];
            Assert.IsNotNull(property);
            Assert.AreEqual("NestedInteger", property.Column);
            Assert.AreEqual(typeof(int), property.Property.PropertyType);

            property = map["TestNested.TestNestedString"];
            Assert.IsNotNull(property);
            Assert.AreEqual("NestedString", property.Column);
            Assert.AreEqual(typeof(string), property.Property.PropertyType);

            property = map["TestNested.TestObject.TestInteger"];
            Assert.IsNotNull(property);
            Assert.AreEqual("Reference1Integer", property.Column);
            Assert.AreEqual(typeof(int), property.Property.PropertyType);

            property = map["TestNested.TestObject.TestString"];
            Assert.IsNotNull(property);
            Assert.AreEqual("Reference1String", property.Column);
            Assert.AreEqual(typeof(string), property.Property.PropertyType);
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Create a property map for a known type.
        /// 2. Test that property descriptors can be retrieved for a property of the same type.
        /// </summary>
        [TestMethod]
        public void Can_access_child_properties()
        {
            // Act.
            PropertyMap map = PropertyMapRegistry.Instance[typeof(TestClass)];
            Assert.IsNotNull(map);
            PropertyDescriptor property = map["TestChild.TestInteger"];
            Assert.IsNotNull(property);
            Assert.AreEqual("Integer", property.Column);
            Assert.AreEqual(typeof(int), property.Property.PropertyType);

            property = map["TestChild.TestString"];
            Assert.IsNotNull(property);
            Assert.AreEqual("String", property.Column);
            Assert.AreEqual(typeof(string), property.Property.PropertyType);

            property = map["TestChild.TestReference1.TestInteger"];
            Assert.IsNotNull(property);
            Assert.AreEqual("Reference1Integer", property.Column);
            Assert.AreEqual(typeof(int), property.Property.PropertyType);

            property = map["TestChild.TestReference1.TestString"];
            Assert.IsNotNull(property);
            Assert.AreEqual("Reference1String", property.Column);
            Assert.AreEqual(typeof(string), property.Property.PropertyType);
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Create a property map for a known type.
        /// 2. Test that property descriptors can be retrieved deeply nested properties.
        /// </summary>
        [TestMethod]
        public void Can_access_deep_nested_properties()
        {
            // Act.
            var map = PropertyMapRegistry.Instance[typeof(TestClass)];
            Assert.IsNotNull(map);

            var property = map["TestChild.TestChild.TestInteger"];
            Assert.IsNotNull(property);
            Assert.AreEqual("Integer", property.Column);
            Assert.AreEqual(typeof(int), property.Property.PropertyType);

            property = map["TestChild.TestChild.TestChild.TestChild.TestChild.TestString"];
            Assert.IsNotNull(property);
            Assert.AreEqual("String", property.Column);
            Assert.AreEqual(typeof(string), property.Property.PropertyType);
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Create a property map for a known type.
        /// 2. Test that property descriptors can be retrieved for references and values without name attributes.
        /// </summary>
        [TestMethod]
        public void Can_access_no_name_properties()
        {
            // Act.
            PropertyMap map = PropertyMapRegistry.Instance[typeof(TestClass)];
            Assert.IsNotNull(map);
            PropertyDescriptor property = map["TestReference1.TestNoColumnName"];
            Assert.IsNotNull(property);
            Assert.AreEqual("TestNoColumnName", property.Column);
            Assert.AreEqual(typeof(float), property.Property.PropertyType);

            property = map["TestNoTableName.TestInteger"];
            Assert.IsNotNull(property);
            Assert.AreEqual("Integer", property.Column);
            Assert.AreEqual(typeof(int), property.Property.PropertyType);

            property = map["TestNoTableName.TestString"];
            Assert.IsNotNull(property);
            Assert.AreEqual("String", property.Column);
            Assert.AreEqual(typeof(string), property.Property.PropertyType);

            property = map["TestNoTableName.TestNoColumnName"];
            Assert.IsNotNull(property);
            Assert.AreEqual("TestNoColumnName", property.Column);
            Assert.AreEqual(typeof(float), property.Property.PropertyType);
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Create a property map for a known type.
        /// 2. Test null-able properties can be accessed.
        /// </summary>
        [TestMethod]
        public void Can_access_nullable_property_types()
        {
            // Act.
            PropertyMap map = PropertyMapRegistry.Instance[typeof(TestClass)];
            Assert.IsNotNull(map);
            PropertyDescriptor property = map["NullableInt"];
            Assert.IsNotNull(property);
            Assert.AreEqual("NullableInt", property.Column);
            Assert.AreEqual(typeof(int?), property.Property.PropertyType);
        }

        /// <summary>
        /// Strategy:
        /// ---------
        /// 1. Create a property map for a known type.
        /// 2. Test collection properties are ignored.
        /// </summary>
        [TestMethod]
        public void Collection_property_types_are_ignored()
        {
            // Act.
            PropertyMap map = PropertyMapRegistry.Instance[typeof(TestClass)];
            Assert.IsNotNull(map);
            PropertyDescriptor property = map["ArrayIgnored"];
            Assert.IsNull(property);
            property = map["ListIgnored"];
            Assert.IsNull(property);
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
