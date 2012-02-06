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
	InsertRows method body: Insert all new in-memory rows into the database.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	
		private void InsertRows( List<UnitTestDataModel.NessUnitTestRow> changed )
		{
			if ( changed.Count == 0 )
			{
				return;
			}

			StringBuilder buffer = new StringBuilder( 1024 );

			buffer.Append( "INSERT INTO NESS_UNIT_TESTS " );
			buffer.Append( "( column1, column2, column3, column4, column5, column6 ) " );

			if ( changed.Count == 1 )
			{
				// single row insert - do this the simple way
				buffer.Append( "VALUES (" );
				AppendInsertValues( buffer, changed[ 0 ] );
				buffer.Append( ")" );
			}
			else
			{
				// multi-row insert - do a batch insert for efficiency (non-standard Sql, Oracle only).
				bool first = true;

				foreach ( UnitTestDataModel.NessUnitTestRow row in changed )
				{
					if ( !first )
					{
						buffer.Append( " UNION ALL " );
					}
					else
					{
						first = false;
					}

					buffer.Append( "SELECT " );
					AppendInsertValues( buffer, row );
					buffer.Append( " FROM DUAL " );
				}
			}

			using ( DbCommandType command = new DbCommandType( buffer.ToString(), _connection ) )
			{
				command.ExecuteNonQuery();
			}
		}

		/// <summary>
		/// Append the SQL insert fragment for this row.
		/// </summary>
		private void AppendInsertValues( StringBuilder buffer, UnitTestDataModel.NessUnitTestRow row )
		{
			
			buffer.Append( "'" + row.Col1 + "'" );
			buffer.Append( ", ");
			buffer.Append( row.IsCol2Null() ? "NULL" : row.Col2.ToString() );
			buffer.Append( ", ");
			buffer.Append( row.IsCol3Null() ? "NULL" : "'" + row.Col3 + "'" );
			buffer.Append( ", ");
			buffer.Append( row.IsCol4Null() ? "NULL" : "'" + row.Col4 + "'" );
			buffer.Append( ", ");
			buffer.Append( row.IsCol5Null() ? "NULL" : "'" + row.Col5 + "'" );
			buffer.Append( ", ");
			buffer.Append( row.Col6.ToString() );
		}
	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Object" mode="insertRowsMethodBody">
		/// <xsl:text disable-output-escaping="yes"><![CDATA[<summary>]]></xsl:text>
		/// Insert all new in-memory rows into the database.
		/// <xsl:text disable-output-escaping="yes"><![CDATA[</summary>]]></xsl:text>
		private void InsertRows( <xsl:apply-templates select="." mode="listDeclDataRow"/> changed )
		{
			if ( changed.Count == 0 )
			{
				return;
			}

			StringBuilder buffer = new StringBuilder( 1024 );

			buffer.Append( "INSERT INTO <xsl:value-of select="@dbTable"/> " );
			buffer.Append( "( <xsl:apply-templates select=".//cobos:Property" mode="sqlInsert"/> ) " );

			if ( changed.Count == 1 )
			{
				// single row insert - do this the simple way
				buffer.Append( "VALUES (" );
				AppendInsertValues( buffer, changed[ 0 ] );
				buffer.Append( ")" );
			}
			else
			{
				// multi-row insert - do a batch insert for efficiency (non-standard Sql, Oracle only).
				bool first = true;

				foreach ( <xsl:value-of select="@datasetRowType"/> row in changed )
				{
					if ( !first )
					{
						buffer.Append( " UNION ALL " );
					}
					else
					{
						first = false;
					}

					buffer.Append( "SELECT " );
					AppendInsertValues( buffer, row );
					buffer.Append( " FROM DUAL " );
				}
			}

			using ( DbCommandType command = new DbCommandType() )
			{
				command.Connection = _connection;
				command.CommandText = buffer.ToString();
				command.ExecuteNonQuery();
			}
		}

		/// <xsl:text disable-output-escaping="yes"><![CDATA[<summary>]]></xsl:text>
		/// Append the SQL insert fragment for this row.
		/// <xsl:text disable-output-escaping="yes"><![CDATA[</summary>]]></xsl:text>
		private void AppendInsertValues( StringBuilder buffer, <xsl:value-of select="@datasetRowType"/> row )
		{
			<xsl:apply-templates select=".//cobos:Property" mode="sqlInsertValue"/>
		}
	</xsl:template>
					 
</xsl:stylesheet>