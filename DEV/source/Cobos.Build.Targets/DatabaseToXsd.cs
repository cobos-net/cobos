// ----------------------------------------------------------------------------
// <copyright file="DatabaseToXsd.cs" company="Cobos SDK">
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

namespace Cobos.Build.Targets
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
    public partial class DatabaseToXsd : Task
    {
        #region Fields

        /// <summary>
        /// The targeted database platform.
        /// </summary>
        private DatabasePlatformEnum databasePlatform;

        #endregion

        #region Constructors
        #endregion

        #region Finalizers
        #endregion

        #region Events
        #endregion

        #region Enums

        /// <summary>
        /// Select the database platform.
        /// Currently only Oracle is supported.
        /// </summary>
        public enum DatabasePlatformEnum
        {
            /// <summary>
            /// Oracle Database
            /// </summary>
            Oracle
        }

        #endregion

        #region Interfaces
        #endregion

        #region Properties

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

        #endregion

        #region Indexers
        #endregion

        #region Methods

        /// <summary>
        /// Execute the task
        /// </summary>
        /// <returns>true if the task successfully executed; otherwise, false.</returns>
        public override bool Execute()
        {
            IDatabaseAdapter database = null;

            // Create the database adapter
            switch (this.databasePlatform)
            {
            case DatabasePlatformEnum.Oracle:
                database = new Cobos.Data.Oracle.OracleDatabaseAdapter(this.ConnectionString);
                break;
            }

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
                BuildEngine.LogMessageEvent(args);

                using (FileStream fstream = new FileStream(this.OutputFile, FileMode.Create))
                {
                    database.GetTableSchema(this.DatabaseSchema, tables, fstream);
                }
            }
            finally
            {
                IDisposable toDispose = database as IDisposable;

                if (toDispose != null)
                {
                    toDispose.Dispose();
                }
            }

            return true;
        }

        #endregion

        #region Structs
        #endregion

        #region Classes
        #endregion
    }
}
