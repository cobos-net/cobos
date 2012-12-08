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
	UpdateRows method body: Update all in-memory modified rows into the database.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	

	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Object" mode="updateRowsMethodBody">
		/// <xsl:text disable-output-escaping="yes"><![CDATA[<summary>]]></xsl:text>
		/// Update all in-memory modified rows into the database.
		/// <xsl:text disable-output-escaping="yes"><![CDATA[</summary>]]></xsl:text>
		private void UpdateRows( <xsl:apply-templates select="." mode="listDeclDataRow"/> changed )
		{
			if ( changed.Count == 0 )
			{
				return;
			}

			using ( DbCommandType command = new DbCommandType() )
			{
				command.Connection = _connection;

				foreach ( <xsl:value-of select="@datasetRowType"/> row in changed )
				{
					StringBuilder buffer = new StringBuilder( 1024 );
				
					buffer.Append( "UPDATE <xsl:value-of select="@dbTable"/> SET " );

					AppendUpdateValues( buffer, row );

					buffer.Append( " WHERE " + <xsl:apply-templates select="." mode="sqlUpdateWhere"/> );

					command.CommandText = buffer.ToString();
					
					command.ExecuteNonQuery();
				}
			}
		}

		/// <xsl:text disable-output-escaping="yes"><![CDATA[<summary>]]></xsl:text>
		/// Append the SQL update fragment for this row.
		/// <xsl:text disable-output-escaping="yes"><![CDATA[</summary>]]></xsl:text>
		private void AppendUpdateValues( StringBuilder buffer, <xsl:value-of select="@datasetRowType"/> row )
		{
			<xsl:apply-templates select=".//cobos:Property" mode="sqlUpdateValue"/>
		}

	</xsl:template>
					 
</xsl:stylesheet>