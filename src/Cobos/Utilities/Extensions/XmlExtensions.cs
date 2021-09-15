// ----------------------------------------------------------------------------
// <copyright file="XmlExtensions.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Utilities.Extensions
{
    using System.Xml;

    /// <summary>
    /// Extension methods for XML objects.
    /// </summary>
    public static class XmlExtensions
    {
        /// <summary>
        /// Utility method to extract attribute values.
        /// </summary>
        /// <param name="self">The 'this' object reference.</param>
        /// <param name="name">The name of the attribute.</param>
        /// <returns>The attribute value if found; otherwise null.</returns>
        public static string GetAnyAttributeValue(this System.Xml.XmlAttribute[] self, string name)
        {
            foreach (XmlAttribute attribute in self)
            {
                if (attribute.Name == name)
                {
                    return attribute.Value;
                }
            }

            return null;
        }
    }
}
