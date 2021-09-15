// ----------------------------------------------------------------------------
// <copyright file="AnsiDatabaseAdapter.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data.Adapters
{
    using System;
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

        /// <inheritdoc />
        public override Type IntegerType => typeof(int);

        /// <inheritdoc />
        public override Type BigIntegerType => typeof(long);

        /// <summary>
        /// Test the connection to the database to ensure that the adapter is correctly configured.
        /// </summary>
        /// <returns>true if the test was successful; otherwise false.</returns>
        public override bool TestConnection()
        {
            try
            {
                object result = this.ExecuteScalar("select 1");

                if (result != null)
                {
                    return (long)Convert.ChangeType(result, typeof(long)) == 1;
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
