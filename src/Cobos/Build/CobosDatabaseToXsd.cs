// ----------------------------------------------------------------------------
// <copyright file="CobosDatabaseToXsd.cs" company="Nicholas Davis">
// Copyright (c) Nicholas Davis. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Cobos.Build
{
    using System;
    using System.IO;
    using System.Linq;
    using Cobos.Data;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    /// <summary>
    /// MSBuild task to create an XML schema from a database.
    /// </summary>
    public partial class CobosDatabaseToXsd : Task
    {
        /// <summary>
        /// The targeted database platform.
        /// </summary>
        private DatabasePlatformEnum databasePlatform;

        /// <summary>
        /// Select the database platform.
        /// Currently only Oracle is supported.
        /// </summary>
        public enum DatabasePlatformEnum
        {
            /// <summary>
            /// Oracle Database.
            /// </summary>
            Oracle,

            /// <summary>
            /// MySQL Database.
            /// </summary>
            MySQL,

            /// <summary>
            /// <c>PostgreSQL</c> Database.
            /// </summary>
            PostgreSQL,

            /// <summary>
            /// SQL Server Database.
            /// </summary>
            SqlServer,
        }

        /// <summary>
        /// Gets or sets the Database connection string.
        /// </summary>
        [Required]
        public string ConnectionString
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the database platform.
        /// </summary>
        [Required]
        public string DatabasePlatform
        {
            get
            {
                return this.databasePlatform.ToString();
            }

            set
            {
                this.databasePlatform = (DatabasePlatformEnum)Enum.Parse(typeof(DatabasePlatformEnum), value, true);
            }
        }

        /// <summary>
        /// Gets or sets the database schema to generate the metadata from.
        /// </summary>
        [Required]
        public string DatabaseSchema
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the database tables to query the metadata for.
        /// </summary>
        [Required]
        public ITaskItem[] DatabaseTables
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the required output file name.
        /// </summary>
        [Required]
        public string OutputFile
        {
            get;
            set;
        }

        /// <summary>
        /// Execute the task.
        /// </summary>
        /// <returns>true if the task successfully executed; otherwise, false.</returns>
        public override bool Execute()
        {
            var database = DatabaseAdapterFactory.Instance.TryCreate(
                Enum.GetName(typeof(DatabasePlatformEnum), this.databasePlatform),
                this.ConnectionString);

            if (database == null)
            {
                throw new InvalidOperationException("An invalid Database Platform was specified.");
            }

            // Get the tables collection that we are querying metadata for.
            string[] tables = (from item in this.DatabaseTables select item.ItemSpec).ToArray();

            try
            {
                // Create the output schema.
                string message = "Getting table schema(s) for " + string.Join(",", tables) + " from schema " + this.DatabaseSchema;
                BuildMessageEventArgs args = new BuildMessageEventArgs(message, string.Empty, "Getting Schema", MessageImportance.Normal);
                this.BuildEngine.LogMessageEvent(args);

                using (FileStream fstream = new FileStream(this.OutputFile, FileMode.Create))
                {
                    database.GetTableSchema(this.DatabaseSchema, tables, fstream);
                }
            }
            finally
            {
                if (database is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }

            return true;
        }
    }
}
