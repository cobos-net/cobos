// ----------------------------------------------------------------------------
// <copyright file="AnsiDatabaseAdapter.cs" company="Cobos SDK">
//
//      Copyright (c) 2009-2012 Nicholas Davis - nick@cobos.co.uk
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
    using System.Data.Common;

    /// <summary>
    /// Represents an ANSI compliant database provider.
    /// </summary>
    public class AnsiDatabaseAdapter : DatabaseAdapter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnsiDatabaseAdapter"/> class.
        /// </summary>
        /// <param name="factory">The provider factory.</param>
        protected AnsiDatabaseAdapter(DbProviderFactory factory)
            : base(factory)
        {
        }

        /// <summary>
        /// Test the connection to the database to ensure that the adapter is correctly configured.
        /// </summary>
        /// <returns>true if the test was successful; otherwise false.</returns>
        public override bool TestConnection()
        {
            try
            {
                object result = ExecuteScalar("select 1");

                if (result != null && result is long)
                {
                    return (long)result == 1;
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
                        ordinal_position as ORDINAL_POSITION,
	                    column_default AS COLUMN_DEFAULT,
	                    is_nullable AS IS_NULLABLE, 
	                    UPPER(data_type) AS DATA_TYPE, 
	                    character_maximum_length AS CHARACTER_MAXIMUM_LENGTH, 
	                    character_octet_length AS CHARACTER_OCTET_LENGTH, 
	                    numeric_precision AS NUMERIC_PRECISION, 
	                    numeric_scale AS NUMERIC_SCALE 
                    FROM 
	                    information_schema.columns
                    WHERE
                        UPPER(table_schema) = '" + schema + @"' 
                        AND UPPER(table_name) IN ('" + tableNames + @"') 
                    ORDER BY
	                    table_name, ordinal_position";
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
	                    cols.ordinal_position as ORDINAL_POSITION, 
	                    cons.constraint_type AS CONSTRAINT_TYPE, 
	                    CONCAT(UPPER(cons.table_name), '_', UPPER(cons.constraint_name)) AS CONSTRAINT_NAME, 
	                    'ENABLED' as STATUS 
                    FROM 
	                    information_schema.table_constraints cons, 
	                    information_schema.key_column_usage cols 
                    WHERE 
	                    UPPER(cons.table_schema) = '" + schema + @"'
	                    AND UPPER(cons.table_name) IN ('" + tableNames + @"') 
	                    AND cons.constraint_type IN ('UNIQUE', 'PRIMARY KEY', 'FOREIGN KEY') 
	                    AND cons.constraint_name = cols.constraint_name 
	                    AND cols.table_schema = cons.table_schema
	                    AND cols.table_name = cons.table_name
                    ORDER BY 
	                    cols.table_name, cols.ordinal_position";
        }
    }
}
