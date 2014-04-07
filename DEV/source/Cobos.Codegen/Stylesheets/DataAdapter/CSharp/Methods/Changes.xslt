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
	AcceptChanges method body: Commit all changes to the database
	============================================================================
	
	

	
	============================================================================
	-->
  <xsl:template match="cobos:Object" mode="acceptChangesMethodBody">
        /// &lt;summary&gt;
        /// Commit all changes to the database.
        /// &lt;/summary&gt;
        public void AcceptChanges()
        {
            var inserted = new <xsl:apply-templates select="." mode="listDeclDataRow"/>();
            var updated = new <xsl:apply-templates select="." mode="listDeclDataRow"/>();
            var deleted = new <xsl:apply-templates select="." mode="listDeclDataRow"/>();

            foreach (<xsl:value-of select="@datasetRowType"/> row in this.Table.Rows)
            {
                if (row.RowState == global::System.Data.DataRowState.Added)
                {
                    inserted.Add(row);
                }
                else if (row.RowState == global::System.Data.DataRowState.Modified)
                {
                    updated.Add(row);
                }
                else if (row.RowState == global::System.Data.DataRowState.Deleted)
                {
                    deleted.Add(row);
                }
            }

            if (inserted.Count == 0 &amp;&amp; updated.Count == 0 &amp;&amp; deleted.Count == 0)
            {
                return;
            }
            
            using (var connection = this.databaseConnection())
            {
                connection.Open();
                
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        this.InsertRows(connection, inserted);
                        this.UpdateRows(connection, updated);
                        this.DeleteRows(connection, deleted);
                
                        transaction.Commit();
                        this.Table.AcceptChanges();
                    }
                    catch (global::System.Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
  </xsl:template>
  <!--
	============================================================================
	HasChanges method body: Check whether this has been modified.
	============================================================================
	
	

	
	============================================================================
	-->
  <xsl:template match="cobos:Object" mode="hasChangesMethodBody">
        /// &lt;summary&gt;
        /// Check whether this has changes.
        /// &lt;/summary&gt;
        /// &lt;returns&gt;true if the table has changes; otherwise false.&lt;/returns&gt;
        public bool HasChanges()
        {
            return this.Table.GetChanges() != null;
        }
  </xsl:template>
  <!--
  ============================================================================
  RejectChanges method body: Undo all in-memory changes.
  ============================================================================
	
	

	
  ============================================================================
  -->
  <xsl:template match="cobos:Object" mode="rejectChangesMethodBody">
        /// &lt;summary&gt;
        /// Reject all in-memory changes.
        /// &lt;/summary&gt;
        public void RejectChanges()
        {
            this.Table.RejectChanges();
        }
  </xsl:template>
</xsl:stylesheet>