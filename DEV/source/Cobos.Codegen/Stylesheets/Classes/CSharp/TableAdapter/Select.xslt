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
	Select method body: Refresh the data table from the database.  Loses
	all in-memory changes that aren't committed.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	
	

	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Object" mode="selectMethodBody">
		/// <xsl:text disable-output-escaping="yes"><![CDATA[<summary>]]></xsl:text>
		/// Refresh the data table from the database.
		/// Loses all in-memory changes that aren't committed.
		/// <xsl:text disable-output-escaping="yes"><![CDATA[</summary>]]></xsl:text>
		public void Select( string[] where, string orderBy )
		{
			_table.Clear();

			using ( DbCommandType command = new DbCommandType() )
			{
				command.Connection = _connection;
				command.CommandText = SelectTemplate.ToString( where, null, orderBy );
			
				using ( DataAdapterType adapter = new DataAdapterType() )
				{
					<xsl:choose>
						<xsl:when test="$dataAdapterType = 'IDbDataAdapter' and $dbCommandType = 'IDbCommand'">
					// special case for data adapters and commands that implement the System.Data interfaces
					// but not the System.Data.Common common implementations.  The IDataAdapter/IDbDataAdapter
					// interfaces do not provide a Fill( DataTable table ) method, only a Fill( DataSet dataset)
					// method.  Rather than implementing the method here, temporarily add the table to a new
					// DataSet and then use the existing implementation.
					using ( DataSet dataset = new DataSet() )
					{
						string tableName = _table.TableName;
						_table.TableName = "Table";
						dataset.Tables.Add( _table );

						adapter.SelectCommand = command;
						adapter.Fill( dataset );

						dataset.Tables.Remove( _table );
						_table.TableName = tableName;
					}
						</xsl:when>
						<xsl:otherwise>
					adapter.SelectCommand = command;
					adapter.Fill( dataset );
						</xsl:otherwise>
					</xsl:choose>
				}
			}
		}

		public void Select()
		{
			Select( null, null );
		}
	</xsl:template>
					 
</xsl:stylesheet>