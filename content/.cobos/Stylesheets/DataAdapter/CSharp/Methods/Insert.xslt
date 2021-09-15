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
  InsertRows method body: Insert all new in-memory rows into the database.
  ============================================================================
	
  ============================================================================
  -->
  <xsl:template match="cobos:Object" mode="insertRowsMethodBody">
        /// &lt;summary&gt;
        /// Commit all new rows to the database.
        /// &lt;/summary&gt;
        private void InsertRows(global::System.Data.IDbConnection connection, <xsl:apply-templates select="." mode="listDeclDataRow"/> changed)
        {
            if (changed.Count == 0)
            {
                return;
            }

            var buffer = new global::System.Text.StringBuilder(1024);

            buffer.Append("INSERT INTO <xsl:value-of select="@dbTable"/> ");
            buffer.Append("(<xsl:apply-templates select=".//cobos:Property" mode="sqlInsert"/>) ");

            bool first = true;

            foreach (<xsl:value-of select="@datasetRowType"/> row in changed)
            {
                if (!first)
                {
                    buffer.Append(", ");
                }
                else
                {
                    buffer.Append(" VALUES ");
                    first = false;
                }

                buffer.Append("(");
                this.AppendInsertValues(buffer, row);
                buffer.Append(")");
            }

            using (var command = connection.CreateCommand())
            {
                command.Connection = connection;
                command.CommandText = buffer.ToString();
                command.ExecuteNonQuery();
            }
        }

        /// &lt;summary&gt;
        /// Append the SQL insert fragment for this row.
        /// &lt;/summary&gt;
        private void AppendInsertValues(global::System.Text.StringBuilder buffer, <xsl:value-of select="@datasetRowType"/> row)
        {<xsl:apply-templates select=".//cobos:Property" mode="sqlInsertValue"/>
        }
  </xsl:template>
</xsl:stylesheet>