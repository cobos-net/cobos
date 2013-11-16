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
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Created by: N.Davis                        Date: 2010-04-09
	Modified by:                               Date:
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Notes: 
	
	
	============================================================================
	-->
  <!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Fill.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	
	

	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->
  <xsl:template match="cobos:Object" mode="fillMethodBody">
        <![CDATA[/// <summary>
        /// Fill the data table from the database.
        /// </summary>
        /// <param name="where">The where clauses.</param>
        /// <param name="orderBy">The order by clause.</param>
        /// <remarks>
        /// Loses all in-memory changes that aren't committed.
        /// </remarks>]]>
        public void Fill(string[] where, string orderBy)
        {
            this.Table.Clear();
            this.Database.Fill(SelectTemplate.ToString(where, null, orderBy), this.Table);
        }

        /// <![CDATA[/// <summary>
        /// Fill the data table from the database.
        /// </summary>
        /// <remarks>
        /// Loses all in-memory changes that aren't committed.
        /// </remarks>]]>
        public void Fill()
        {
            this.Fill(null, null);
        }
  </xsl:template>
</xsl:stylesheet>