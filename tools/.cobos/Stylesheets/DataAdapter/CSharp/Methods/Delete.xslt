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
  Delete method body: Remove an object from the database.
  ============================================================================
	
  ============================================================================
  -->
  <xsl:template match="cobos:Object" mode="deleteMethodBody">
        /// &lt;summary&gt;
        /// Delete an object from the database.
        /// &lt;/summary&gt;
        /// &lt;param name="value"&gt;The object to delete.&lt;/param&gt;
        public void Delete<xsl:value-of select="@className"/>(<xsl:value-of select="@className"/> value)
        {
            value.DataRowSource.Delete();
        }
  </xsl:template>
  <!--
  ============================================================================
  Remove method body: Remove an object from the table.
  ============================================================================
	
  ============================================================================
  -->
  <xsl:template match="cobos:Object" mode="removeMethodBody">
        /// &lt;summary&gt;
        /// Remove an object from the table.
        /// &lt;/summary&gt;
        /// &lt;param name="value"&gt;The object to remove.&lt;/param&gt;
        public void Remove<xsl:value-of select="@className"/>(<xsl:value-of select="@className"/> value)
        {
            this.Table.Remove<xsl:value-of select="@className"/>Row(value.DataRowSource);
        }
  </xsl:template>
  <!--
  ============================================================================
  DeleteRows method body: Delete all in-memory deleted rows from the database.
  ============================================================================
	
  ============================================================================
  -->
  <xsl:template match="cobos:Object" mode="deleteRowsMethodBody">
        /// &lt;summary&gt;
        /// Delete all in-memory deleted rows from the database.
        /// &lt;/summary&gt;
        /// &lt;param name="connection"&gt;An object representing the database connecction.&lt;/param&gt;
        /// &lt;param name="changed"&gt;The rows that are to be deleted.&lt;/param&gt;
        private void DeleteRows(global::System.Data.IDbConnection connection, <xsl:apply-templates select="." mode="listDeclDataRow"/> changed)
        {
            if (changed.Any() == false)
            {
                return;
            }

            var buffer = new global::System.Text.StringBuilder(1024);
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

            using (var command = connection.CreateCommand())
            {
                command.Connection = connection;
                command.CommandText = buffer.ToString();
                command.ExecuteNonQuery();
            }
        }
  </xsl:template>
</xsl:stylesheet>