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
  ============================================================================
  Created by: N.Davis                        Date: 2010-04-09
  Modified by:                               Date:
  ============================================================================
  Notes: 
	
	
  ============================================================================
  -->
					 
  <!--
  ============================================================================
  UpdateRows method body: Update all in-memory modified rows into the database.
  ============================================================================
	

	
  ============================================================================
  -->

  <xsl:template match="cobos:Object" mode="updateRowsMethodBody">
        /// &lt;summary&gt;
        /// Update all in-memory modified rows into the database.
        /// &lt;/summary&gt;
        private void UpdateRows(global::System.Data.IDbConnection connection, <xsl:apply-templates select="." mode="listDeclDataRow"/> changed)
        {
            if (changed.Count == 0)
            {
                return;
            }

            using (var command = connection.CreateCommand())
            {
                command.Connection = connection;

                foreach (<xsl:value-of select="@datasetRowType"/> row in changed)
                {
                    var buffer = new global::System.Text.StringBuilder(1024);
                    
                    buffer.Append("UPDATE <xsl:value-of select="@dbTable"/> SET ");
                    this.AppendUpdateValues(buffer, row);
                    buffer.Append(" WHERE " + <xsl:apply-templates select="." mode="sqlUpdateWhere"/>);

                    command.CommandText = buffer.ToString();
                    command.ExecuteNonQuery();
                }
            }
        }

        /// &lt;summary&gt;
        /// Append the SQL update fragment for this row.
        /// &lt;/summary&gt;
        private void AppendUpdateValues(global::System.Text.StringBuilder buffer, <xsl:value-of select="@datasetRowType"/> row)
        {<xsl:apply-templates select=".//cobos:Property" mode="sqlUpdateValue"/>
        }
  </xsl:template>
</xsl:stylesheet>