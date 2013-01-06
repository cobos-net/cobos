// ----------------------------------------------------------------------------
// <copyright file="IDatabaseAdapter.cs" company="Cobos SDK">
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

using System;
using System.Data;
using System.IO;

namespace Cobos.Data
{
    public interface IDatabaseAdapter
    {
        /// <summary>
        /// In some situations this may result in performance 
        /// improvements, if read-write access is not required.
        /// Set via the connection string. 
        /// Not supported on all platforms.
        /// </summary>
        bool ReadOnly
        {
            get;
        }

        DataTable Fill(string sql, string tableName);

        void Fill(string sql, DataTable result);

        DataTableType Fill<DataTableType>(string sql) where DataTableType : DataTable, new();

        DataSetType Fill<DataSetType>(string sql, string tableName) where DataSetType : DataSet, new();

        void Fill(string sql, string tableName, DataSet dataset);

        void Fill(DatabaseQuery query);

        void Fill(DatabaseQuery[] queries);

        void FillAsynch(DatabaseQuery[] queries);

        void GetTableMetadata(string schema, string[] tables, Stream result);

        void GetTableSchema(string schema, string[] tables, Stream result);

        bool TestConnection();
    }
}
