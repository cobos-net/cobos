// ----------------------------------------------------------------------------
// <copyright file="ArrayExtensions.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Utilities.Extensions
{
    using System;
    using System.Linq;

    /// <summary>
    /// Extension methods for the System.Array class.
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// Appends the contents of a2 into a1.
        /// </summary>
        /// <typeparam name="T">The type of the element contained in the array.</typeparam>
        /// <param name="self">The 'this' object reference.</param>
        /// <param name="append">The array to append.</param>
        /// <returns>The new a1 array with the appended result.</returns>
        public static T[] Append<T>(this T[] self, T[] append)
        {
            return self.Concat(append).Cast<T>().ToArray();
        }

        /// <summary>
        /// Appends the object into an array.
        /// </summary>
        /// <typeparam name="T">The type of the element contained in the array.</typeparam>
        /// <param name="self">The 'this' object reference.</param>
        /// <param name="obj">The object to append.</param>
        /// <returns>The new a1 array with the appended result.</returns>
        public static T[] Append<T>(this T[] self, T obj)
        {
            return self.Concat(new T[] { obj }).Cast<T>().ToArray();
        }

        /// <summary>
        /// Concatenates a multi-dimensional array into a single dimension array.
        /// </summary>
        /// <typeparam name="T">The type of the element contained in the array.</typeparam>
        /// <param name="self">The 'this' object reference.</param>
        /// <returns>The concatenated result.</returns>
        public static T[] ConcatenateAll<T>(this T[][] self)
        {
            int totalLength = 0;

            for (int m = 0; m < self.Length; ++m)
            {
                T[] current = self[m];

                if (current != null)
                {
                    totalLength += current.Length;
                }
            }

            if (totalLength == 0)
            {
                return null;
            }

            T[] result = new T[totalLength];

            int index = 0;

            for (int m = 0; m < self.Length; ++m)
            {
                T[] current = self[m];

                if (current != null)
                {
                    int currentLength = current.Length;

                    Array.Copy(current, 0, result, index, currentLength);

                    index += currentLength;
                }
            }

            return result;
        }

        /// <summary>
        /// Find the index of the specified object in the array.
        /// </summary>
        /// <typeparam name="T">The type of the element contained in the array.</typeparam>
        /// <param name="self">The 'this' object reference.</param>
        /// <param name="obj">The object to find.</param>
        /// <returns>The index of the object in the array.</returns>
        public static int IndexOf<T>(this T[] self, T obj)
        {
            for (int i = 0; i < self.Length; ++i)
            {
                if (object.ReferenceEquals(self[i], obj))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Returns a one-level deep copy of a portion of an array.
        /// </summary>
        /// <typeparam name="T">The type of the element contained in the array.</typeparam>
        /// <param name="self">The 'this' object reference.</param>
        /// <param name="begin">Zero-based index at which to begin extraction.
        /// As a negative index, start indicates an offset from the end of the sequence. slice(-2) extracts the second-to-last element and the last element in the sequence.</param>
        /// <param name="end">Zero-based index at which to end extraction. slice extracts up to but not including end.
        /// As a negative index, end indicates an offset from the end of the sequence. slice(2,-1) extracts the third element through the second-to-last element in the sequence.
        /// If end is null, slice extracts to the end of the sequence.</param>
        /// <returns>A portion of an array.</returns>
        public static T[] Slice<T>(this T[] self, int begin, int? end)
        {
            if (begin >= self.Length)
            {
                throw new System.ArgumentOutOfRangeException();
            }

            // normalise begin
            if (begin < 0)
            {
                begin = self.Length + begin;

                if (begin < 0)
                {
                    begin = 0;
                }
            }

            // normalise end
            if (end == null)
            {
                end = self.Length;
            }
            else if (end < 0)
            {
                end = self.Length + end;

                if (end < 0)
                {
                    end = 0;
                }
            }
            else if (end > self.Length)
            {
                end = self.Length;
            }

            if (end <= begin)
            {
                return null; // nothing to copy
            }

            // create the copy
            T[] copy = new T[(int)end - begin];

            System.Array.Copy(self, begin, copy, 0, (int)end - begin);

            return copy;
        }

        /// <summary>
        /// Changes the content of an array, adding new elements while removing old elements.
        /// </summary>
        /// <typeparam name="T">The type of the element contained in the array.</typeparam>
        /// <param name="self">The 'this' object reference.</param>
        /// <param name="index">Index at which to start changing the array. If negative, will begin that many elements from the end.</param>
        /// <param name="howMany">An integer indicating the number of old array elements to remove.
        /// If howMany is 0, no elements are removed. In this case, you should specify at least one new element.</param>
        /// <param name="elements">The elements to add to the array. If you don't specify any elements, splice simply removes elements from the array.</param>
        /// <returns>The modified array a.</returns>
        public static T[] Splice<T>(this T[] self, int index, int howMany, params T[] elements)
        {
            if (index >= self.Length)
            {
                throw new System.ArgumentOutOfRangeException();
            }

            // normalise howMany
            if (howMany < 0)
            {
                howMany = 0;
            }

            if (howMany == 0 && elements.Length == 0)
            {
                return self;
            }

            // normalise index
            if (index < 0)
            {
                index = self.Length + index;

                if (index < 0)
                {
                    index = 0;
                }
            }

            // calculate the start and end indices for removal
            int removeStart = index, removeEnd = index + howMany;

            if (removeEnd > self.Length)
            {
                removeEnd = self.Length - index;
            }

            // calculate the new size of the array
            int newSize = self.Length - (removeEnd - removeStart) + elements.Length;

            if (newSize == 0)
            {
                return null;
            }

            T[] newArray = new T[newSize];

            if (removeStart > 0)
            {
                System.Array.Copy(self, 0, newArray, 0, removeStart);
            }

            if (elements.Length > 0)
            {
                System.Array.Copy(elements, 0, newArray, removeStart, elements.Length);
            }

            if (self.Length - removeEnd > 0)
            {
                System.Array.Copy(self, removeEnd, newArray, removeStart + elements.Length, self.Length - removeEnd);
            }

            return newArray;
        }
    }
}
