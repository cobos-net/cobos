<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
					 xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
					 xmlns:msxsl="urn:schemas-microsoft-com:xslt"
					 xmlns:cobos="http://schemas.cobos.co.uk/datamodel/1.0.0"
					 xmlns="http://schemas.cobos.co.uk/datamodel/1.0.0"
					 xmlns:xsd="http://www.w3.org/2001/XMLSchema"
					 exclude-result-prefixes="msxsl">
					 
	<xsl:include href="./Delegates.xslt"/>
	<xsl:include href="./New.xslt"/>
	<xsl:include href="./Find.xslt"/>
	<xsl:include href="./Changes.xslt"/>
	<xsl:include href="./Select.xslt"/>
	<xsl:include href="./Insert.xslt"/>
	<xsl:include href="./Update.xslt"/>
	<xsl:include href="./Delete.xslt"/>
	<xsl:include href="./Sql/Sql.xslt"/>

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
	Create data adapter
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Object" mode="tableAdapter">
	public class <xsl:value-of select="@className"/>TableAdapter<xsl:text disable-output-escaping="yes"><![CDATA[<DbConnectionType,DbCommandType,DataAdapterType>]]></xsl:text>
		where DbConnectionType : <xsl:value-of select="$dbConnectionType"/>, IDisposable, new()
		where DbCommandType : <xsl:value-of select="$dbCommandType"/>, IDisposable, new()
		where DataAdapterType : <xsl:value-of select="$dataAdapterType"/>, IDisposable, new()
	{
		#region Instance Data

		DbConnectionType _connection;

		<xsl:value-of select="@datasetTableType"/> _table = new <xsl:value-of select="@datasetTableType"/>();

		const string _select = "<xsl:apply-templates select=".//cobos:Property" mode="sqlSelect"/>";
		
		const string _from = "<xsl:value-of select="@dbTable"/>";
		
		static readonly string[] _innerJoin = <xsl:apply-templates mode="sqlJoin" select="."/>;

		static readonly string[] _where = <xsl:apply-templates mode="sqlWhere" select="."/>;

		const string _groupBy = <xsl:apply-templates mode="sqlGroupBy" select="."/>;

		const string _orderBy = <xsl:apply-templates mode="sqlOrderBy" select="."/>;

		#endregion

		#region Construction

		public <xsl:value-of select="@className"/>TableAdapter( DbConnectionType connection )
		{
			_connection = connection;
			_table.RowChanging += new DataRowChangeEventHandler( _table_RowChanging );
			_table.RowChanged += new DataRowChangeEventHandler( _table_RowChanged );
			_table.RowDeleting += new DataRowChangeEventHandler( _table_RowDeleting );
			_table.RowDeleted += new DataRowChangeEventHandler( _table_RowDeleted );
			_table.TableNewRow += new DataTableNewRowEventHandler( _table_TableNewRow );
		}

		#endregion

		#region Public properties

		public static readonly SqlSelectTemplate SelectTemplate = new SqlSelectTemplate( _select, _from, _innerJoin, _where, _groupBy, _orderBy, true );

		#endregion

		#region Public methods

		<xsl:apply-templates select="." mode="acceptChangesMethodBody"/>

		<xsl:apply-templates select="." mode="addNewMethodBody"/>

		<xsl:apply-templates select="." mode="createNewMethodBody"/>

		<xsl:apply-templates select="." mode="findByMethodBody"/>

		<xsl:apply-templates select="." mode="findAllMethodBody"/>

		<xsl:apply-templates select="." mode="hasChangesMethodBody"/>

		<xsl:apply-templates select="." mode="rejectChangesMethodBody"/>

		<xsl:apply-templates select="." mode="selectMethodBody"/>

		#endregion

		#region Events

		<xsl:apply-templates select="." mode="delegatesDeclarations"/>

		#endregion

		#region Private methods

		<xsl:apply-templates select="." mode="insertRowsMethodBody"/>

		<xsl:apply-templates select="." mode="updateRowsMethodBody"/>

		<xsl:apply-templates select="." mode="deleteRowsMethodBody"/>

		#endregion

		#region User extensions

		<xsl:apply-templates select="." mode="userTableAdapterExtensions"/>

		#endregion
	}
	</xsl:template>
	
	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Common templates for generating generic List<> declarations.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Object" mode="listDecl">
		<xsl:text disable-output-escaping="yes"><![CDATA[List<]]></xsl:text>
		<xsl:value-of select="@className"/>
		<xsl:text disable-output-escaping="yes"><![CDATA[>]]></xsl:text>
	</xsl:template>

	<xsl:template match="cobos:Object" mode="listDeclDataRow">
		<xsl:text disable-output-escaping="yes"><![CDATA[List<]]></xsl:text>
		<xsl:value-of select="@datasetRowType"/>
		<xsl:text disable-output-escaping="yes"><![CDATA[>]]></xsl:text>
	</xsl:template>

	<xsl:template match="cobos:Reference" mode="listDecl">
		<xsl:text disable-output-escaping="yes"><![CDATA[List<]]></xsl:text>
		<xsl:value-of select="@ref"/>
		<xsl:text disable-output-escaping="yes"><![CDATA[>]]></xsl:text>
	</xsl:template>

	<xsl:template name="executeStatement">
		<xsl:text disable-output-escaping="yes"><![CDATA[database.Execute<]]></xsl:text>
		<xsl:value-of select="@datasetTableType"/>
		<xsl:text disable-output-escaping="yes"><![CDATA[>]]></xsl:text>
	</xsl:template>
	
</xsl:stylesheet>