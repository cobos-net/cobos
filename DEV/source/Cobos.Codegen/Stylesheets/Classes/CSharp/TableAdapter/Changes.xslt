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
		/// <xsl:text disable-output-escaping="yes"><![CDATA[<summary>]]></xsl:text>
		/// Commit all changes to the database.
		/// <xsl:text disable-output-escaping="yes"><![CDATA[</summary>]]></xsl:text>
		public void AcceptChanges()
		{
			<xsl:apply-templates select="." mode="listDeclDataRow"/> inserted = new <xsl:apply-templates select="." mode="listDeclDataRow"/>();
			<xsl:apply-templates select="." mode="listDeclDataRow"/> updated = new <xsl:apply-templates select="." mode="listDeclDataRow"/>();
			<xsl:apply-templates select="." mode="listDeclDataRow"/> deleted = new <xsl:apply-templates select="." mode="listDeclDataRow"/>();

			foreach ( <xsl:value-of select="@datasetRowType"/> row in _table.Rows )
			{
				if ( row.RowState == DataRowState.Added )
				{
					inserted.Add( row );
				}
				else if ( row.RowState == DataRowState.Modified )
				{
					updated.Add( row );
				}
				else if ( row.RowState == DataRowState.Deleted )
				{
					deleted.Add( row );
				}
			}

			if ( <xsl:text disable-output-escaping="yes"><![CDATA[inserted.Count == 0 && updated.Count == 0 && deleted.Count == 0]]></xsl:text> )
			{
				return;
			}

			using ( IDbTransaction transaction = _connection.BeginTransaction() )
			{
				try
				{
					InsertRows( inserted );
					UpdateRows( updated );
					DeleteRows( deleted );

					transaction.Commit();

					_table.AcceptChanges();
				}
				catch ( Exception )
				{
					transaction.Rollback();
					throw;
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
		/// <xsl:text disable-output-escaping="yes"><![CDATA[<summary>]]></xsl:text>
		/// Check whether this has changes.
		/// <xsl:text disable-output-escaping="yes"><![CDATA[</summary>]]></xsl:text>
		public bool HasChanges()
		{
			return (_table.GetChanges() != null);
		}
	</xsl:template>
				 
					 
	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	RejectChanges method body: Undo all in-memory changes.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	
	

	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Object" mode="rejectChangesMethodBody">
		/// <xsl:text disable-output-escaping="yes"><![CDATA[<summary>]]></xsl:text>
		/// Undo all in-memory changes.
		/// <xsl:text disable-output-escaping="yes"><![CDATA[</summary>]]></xsl:text>
		public void RejectChanges()
		{
			_table.RejectChanges();
		}
	</xsl:template>
					 
</xsl:stylesheet>