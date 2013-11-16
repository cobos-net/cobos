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
  Filename: .xslt
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
  DeleteRows method body: Delete all in-memory deleted rows from the database.
  ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	
  ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
  -->
  <xsl:template match="cobos:Object" mode="deleteRowsMethodBody">
        <![CDATA[/// <summary>
        /// Delete all in-memory deleted rows from the database. crap.
        /// </summary>
        /// <param name="connection">An object representing the database connecction.</param>
        /// <param name="changed">The rows that are to be deleted.</param>]]>
        private void DeleteRows(IDbConnection connection, <xsl:apply-templates select="." mode="listDeclDataRow"/> changed)
        {
            if (changed.Count == 0)
            {
                return;
            }

            StringBuilder buffer = new StringBuilder(1024);
            buffer.Append("DELETE FROM <xsl:value-of select="@dbTable"/> WHERE ");

            bool first = true;

            foreach (<xsl:value-of select="@datasetRowType"/> row in changed)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    buffer.Append(" OR ");
                }

                buffer.Append(<xsl:apply-templates select="." mode="sqlDeleteWhere"/>);
            }

            using (IDbCommand command = this.Database.GetCommand())
            {
                command.Connection = connection;
                command.CommandText = buffer.ToString();
                command.ExecuteNonQuery();
            }
        }
  </xsl:template>
</xsl:stylesheet>