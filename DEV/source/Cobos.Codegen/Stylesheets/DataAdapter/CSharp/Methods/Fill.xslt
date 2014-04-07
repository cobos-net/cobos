<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
					 xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
					 xmlns:msxsl="urn:schemas-microsoft-com:xslt"
					 xmlns:cobos="http://schemas.cobos.co.uk/datamodel/1.0.0"
					 xmlns="http://schemas.cobos.co.uk/datamodel/1.0.0"
					 xmlns:xsd="http://www.w3.org/2001/XMLSchema"
					 exclude-result-prefixes="msxsl">
  <!-- 
	=============================================================================
	Filename: Fill.xslt
	Description: 
	============================================================================
	Created by: N.Davis                        Date: 2010-04-09
	Modified by:                               Date:
	============================================================================
	Notes: 
	
	
	============================================================================
	-->
  <!--
	============================================================================
	Fill.
	============================================================================
	
	

	
	============================================================================
	-->
  <xsl:template match="cobos:Object" mode="fillMethodBody">
        /// &lt;summary&gt;
        /// Fill the data table from the database.
        /// &lt;/summary&gt;
        /// &lt;param name="dataAdapter"&gt;The data adapter to fill the table.&lt;/param&gt;
        /// &lt;param name="where"&gt;The where clauses.&lt;/param&gt;
        /// &lt;param name="orderBy"&gt;The order by clause.&lt;/param&gt;
        /// &lt;remarks&gt;
        /// Loses all in-memory changes that aren't committed.
        /// &lt;/remarks&gt;
        public void Fill(global::System.Data.IDbDataAdapter dataAdapter, string[] where, string orderBy)
        {
            this.Table.Clear();
			
            using (var connection = this.databaseConnection())
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.Connection = connection;
                    command.CommandText = SelectTemplate.ToString(where, null, orderBy);

                    using (var adapter = (global::System.Data.Common.DbDataAdapter)dataAdapter)
                    {
                        ((global::System.Data.IDbDataAdapter)adapter).SelectCommand = command;
                        adapter.Fill(this.Table);
                    }
                }
            }
        }

        /// &lt;summary&gt;
        /// Fill the data table from the database.
        /// &lt;/summary&gt;
        /// &lt;remarks&gt;
        /// Loses all in-memory changes that aren't committed.
        /// &lt;/remarks&gt;
        public void Fill(global::System.Data.IDbDataAdapter dataAdapter)
        {
            this.Fill(dataAdapter, null, null);
        }
  </xsl:template>
</xsl:stylesheet>