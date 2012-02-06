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
	
		private void DeleteRows( List<UnitTestDataModel.NessUnitTestRow> changed )
		{
			if ( changed.Count == 0 )
			{
				return;
			}
			
			StringBuilder buffer = new StringBuilder( 1024 );
			buffer.Append( "DELETE FROM NESS_UNIT_TESTS WHERE " );
			
			bool first = true;
			
			foreach ( UnitTestDataModel.NessUnitTestRow row in changed )
			{
				if ( first )
				{
					first = false;
				}
				else
				{
					buffer.Append( " OR " );
				}
				
				buffer.Append( "(column1 = '" + row[ "column1", DataRowVersion.Original ].ToString() + "')" );
			}
			
			using ( DbCommandType command = new DbCommandType( buffer.ToString(), _connection ) )
			{
				command.ExecuteNonQuery();
			}
		}
	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Object" mode="deleteRowsMethodBody">
		/// <xsl:text disable-output-escaping="yes"><![CDATA[<summary>]]></xsl:text>
		/// Delete all in-memory deleted rows from the database.
		/// <xsl:text disable-output-escaping="yes"><![CDATA[</summary>]]></xsl:text>
		private void DeleteRows( <xsl:apply-templates select="." mode="listDeclDataRow"/> changed )
		{
			if ( changed.Count == 0 )
			{
				return;
			}
			
			StringBuilder buffer = new StringBuilder( 1024 );
			buffer.Append( "DELETE FROM <xsl:value-of select="@dbTable"/> WHERE " );
			
			bool first = true;
			
			foreach ( <xsl:value-of select="@datasetRowType"/> row in changed )
			{
				if ( first )
				{
					first = false;
				}
				else
				{
					buffer.Append( " OR " );
				}
				
				buffer.Append( <xsl:apply-templates select="." mode="sqlDeleteWhere"/> );
			}
			
			using ( DbCommandType command = new DbCommandType() )
			{
				command.Connection = _connection;
				command.CommandText = buffer.ToString();
				command.ExecuteNonQuery();
			}
		}
	</xsl:template>
					 
</xsl:stylesheet>