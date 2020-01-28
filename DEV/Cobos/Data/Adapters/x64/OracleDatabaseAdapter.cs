// ----------------------------------------------------------------------------
// <copyright file="OracleDatabaseAdapter.cs" company="Cobos SDK">
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

namespace Cobos.Data.Adapters
{
    using System;
    using System.Data;
    using Cobos.Data.Adapters;
    using Oracle.DataAccess.Client;

    /// <summary>
    /// Represents a connection to an Oracle database.
    /// </summary>
    public class OracleDatabaseAdapter : DatabaseAdapter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OracleDatabaseAdapter"/> class.
        /// </summary>
        public OracleDatabaseAdapter()
            : base(OracleClientFactory.Instance)
        {
        }

        /// <summary>
        /// Test the connection to the database to ensure that the adapter 
        /// is correctly configured.
        /// </summary>
        /// <returns>True if the test was successful; Otherwise false.</returns>
        public override bool TestConnection()
        {
            try
            {
                object @object = ExecuteScalar("select 1 from dual");

                if (@object != null && @object is int)
                {
                    return (int)@object == 1;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// SQL to query the DB metadata to get the columns for the specified tables.
        /// </summary>
        /// <param name="schema">The schema name.</param>
        /// <param name="tableNames">The table names to query for.</param>
        /// <returns>A platform specific SQL query.</returns>
        protected override string GetMetadataColumnsSQL(string schema, string tableNames)
        {
            return @"SELECT 
                        table_name AS TABLE_NAME, 
                        column_name AS COLUMN_NAME,
                        column_id AS ORDINAL_POSITION,
                        data_default AS COLUMN_DEFAULT,
                        CASE
                            WHEN nullable = 'Y' THEN 'YES'  
                            WHEN nullable = 'N' THEN 'NO'
                        END AS IS_NULLABLE,
                        UPPER(data_type) AS DATA_TYPE, 
                        char_length AS CHARACTER_MAXIMUM_LENGTH,
                        data_length AS CHARACTER_OCTET_LENGTH, 
                        data_precision AS NUMERIC_PRECISION, 
                        data_scale AS NUMERIC_SCALE
                    FROM 
                        all_tab_columns 
                    WHERE 
                        UPPER(owner) = '" + schema + @"' 
                        AND UPPER(table_name) IN ('" + tableNames + @"') 
                    ORDER BY 
                        table_name, column_id";
        }

        /// <summary>
        /// SQL to query the DB metadata to get the constraints for the specified tables.
        /// </summary>
        /// <param name="schema">The schema name.</param>
        /// <param name="tableNames">The table names to query for.</param>
        /// <returns>A platform specific SQL query.</returns>
        protected override string GetMetadataConstraintsSQL(string schema, string tableNames)
        {
            return @"SELECT 
                        cols.table_name AS TABLE_NAME, 
                        cols.column_name AS COLUMN_NAME, 
                        cols.position AS ORDINAL_POSITION, 
                        CASE 
                            WHEN cons.constraint_type = 'P' THEN 'PRIMARY KEY'
                            WHEN cons.constraint_type = 'R' THEN 'FOREIGN KEY'
                            WHEN cons.constraint_type = 'U' THEN 'UNIQUE'
                        END AS CONSTRAINT_TYPE, 
                        cons.constraint_name AS CONSTRAINT_NAME, 
                        cons.status AS STATUS 
                    FROM 
                        all_constraints cons, 
                        all_cons_columns cols 
                    WHERE 
                        UPPER(cons.owner) = '" + schema + @"' 
                        AND UPPER(cons.table_name) IN ('" + tableNames + @"')  
                        AND cons.constraint_type IN ('P', 'R', 'U') 
                        AND cons.constraint_name = cols.constraint_name 
                        AND cons.owner = cols.owner 
                        AND cons.table_name = cols.table_name
                    ORDER BY 
                        cols.table_name, cols.position";
        }
    }
}
