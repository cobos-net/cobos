// ----------------------------------------------------------------------------
// <copyright file="OracleDatabaseAdapter.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Data.Adapters
{
    using System;
    using Oracle.ManagedDataAccess.Client;

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

        /// <inheritdoc />
        public override Type IntegerType => typeof(decimal);

        /// <inheritdoc />
        public override Type BigIntegerType => typeof(decimal);

        /// <summary>
        /// Test the connection to the database to ensure that the adapter
        /// is correctly configured.
        /// </summary>
        /// <returns>True if the test was successful; Otherwise false.</returns>
        public override bool TestConnection()
        {
            try
            {
                object @object = this.ExecuteScalar("select 1 from dual");

                if (@object != null)
                {
                    return (int)Convert.ChangeType(@object, typeof(int)) == 1;
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
