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
	AcceptChanges method body: Commit all changes to the database
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	
	

	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->
  <xsl:template match="cobos:Object" mode="acceptChangesMethodBody">
        <![CDATA[/// <summary>
        /// Commit all changes to the database.
        /// </summary>]]>
        public void AcceptChanges()
        {
            var inserted = new <xsl:apply-templates select="." mode="listDeclDataRow"/>();
            var updated = new <xsl:apply-templates select="." mode="listDeclDataRow"/>();
            var deleted = new <xsl:apply-templates select="." mode="listDeclDataRow"/>();

            foreach (<xsl:value-of select="@datasetRowType"/> row in this.Table.Rows)
            {
                if (row.RowState == DataRowState.Added)
                {
                    inserted.Add(row);
                }
                else if (row.RowState == DataRowState.Modified)
                {
                    updated.Add(row);
                }
                else if (row.RowState == DataRowState.Deleted)
                {
                    deleted.Add(row);
                }
            }

            if (<![CDATA[inserted.Count == 0 && updated.Count == 0 && deleted.Count == 0]]>)
            {
                return;
            }
            
            using (var connection = this.Database.GetConnection())
            {
                connection.Open();
                
                using (IDbTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        this.InsertRows(connection, inserted);
                        this.UpdateRows(connection, updated);
                        this.DeleteRows(connection, deleted);
                
                        transaction.Commit();
                        this.Table.AcceptChanges();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
  </xsl:template>
  <!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	HasChanges method body: Check whether this has been modified.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	
	

	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->
  <xsl:template match="cobos:Object" mode="hasChangesMethodBody">
        <![CDATA[/// <summary>
        /// Check whether this has changes.
        /// </summary>
        /// <returns>true if the table has changes; otherwise false.</returns>]]>
        public bool HasChanges()
        {
            return this.Table.GetChanges() != null;
        }
  </xsl:template>
  <!--
  ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
  RejectChanges method body: Undo all in-memory changes.
  ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	
	

	
  ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
  -->
  <xsl:template match="cobos:Object" mode="rejectChangesMethodBody">
        <![CDATA[/// <summary>
        /// Reject all in-memory changes.
        /// </summary>]]>
        public void RejectChanges()
        {
            this.Table.RejectChanges();
        }
  </xsl:template>
</xsl:stylesheet>