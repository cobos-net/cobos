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
  Loads the data from the database.
  ============================================================================
  
  

  
  ============================================================================
  -->
  <xsl:template match="cobos:Object" mode="fillMethodBody">
        /// &lt;summary&gt;
        /// Fills the table with data from the database.
        /// &lt;/summary&gt;
        /// &lt;param name="where"&gt;The where clauses.  May be null.&lt;/param&gt;
        /// &lt;param name="orderBy"&gt;The order by clause.  May be null.&lt;/param&gt;
        public void Fill(string[] where, string orderBy)
        {
            var factory = this.ProviderFactory;
        
            using (var connection = factory.CreateConnection())
            {
                connection.ConnectionString = this.ConnectionString;
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.Connection = connection;
                    command.CommandText = SelectTemplate.ToString(where, null, orderBy);

                    using (var adapter = factory.CreateDataAdapter())
                    {
                        adapter.SelectCommand = command;
                            
                        try
                        {
                            this.Table.BeginLoadData();
                            adapter.Fill(this.Table);
                        }
                        finally
                        {
                            this.Table.EndLoadData();
                        }
                    }
                }
            }
        }
  </xsl:template>
</xsl:stylesheet>