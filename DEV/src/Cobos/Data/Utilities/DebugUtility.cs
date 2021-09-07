﻿// ----------------------------------------------------------------------------
// <copyright file="DebugUtility.cs" company="Cobos SDK">
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

namespace Cobos.Data.Utilities
{
    using System;
    using System.Data;

    /// <summary>
    /// Utility class for debugging System.Data errors.
    /// </summary>
    public static class DebugUtility
    {
        /// <summary>
        /// Dump all DataSet errors to <see cref="System.Diagnostics.Debug"/>.
        /// </summary>
        /// <param name="dataset">The dataset to dump.</param>
        /// <remarks>
        /// See: <c>http://www.codeproject.com/Tips/405938/Debugging-DataSet-Constraint-Errors</c>
        /// </remarks>
        public static void DumpDataSetErrors(DataSet dataset)
        {
            foreach (DataTable table in dataset.Tables)
            {
                DataRow[] rowErrors = table.GetErrors();

                if (rowErrors.Length == 0)
                {
                    continue;
                }

                System.Diagnostics.Debug.WriteLine(table.TableName + " Errors: " + rowErrors.Length);

                for (int i = 0; i < rowErrors.Length; i++)
                {
                    System.Diagnostics.Debug.WriteLine(rowErrors[i].RowError);

                    foreach (DataColumn col in rowErrors[i].GetColumnsInError())
                    {
                        System.Diagnostics.Debug.WriteLine(col.ColumnName + ":" + rowErrors[i].GetColumnError(col));
                    }
                }
            }
        }
    }
}