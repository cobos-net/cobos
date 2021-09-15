// ----------------------------------------------------------------------------
// <copyright file="DebugUtility.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data.Utilities
{
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
        /// See: <c>http://www.codeproject.com/Tips/405938/Debugging-DataSet-Constraint-Errors</c>.
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
