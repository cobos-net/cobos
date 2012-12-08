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
	FindBy method body: Find the object using the primary key fields.
	Only add this method if the table has primary key or unique key constraints.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	
		public LineupUnit FindByLineupNameUnitId( string LineupName, string UnitId )
		{
			UnitDataModel.LineupUnitRow found = _table.FindByLineupNameUnitId( LineupName, UnitId );

			if ( found == null || found.RowState == DataRowState.Deleted )
			{
				return null;
			}

			return new LineupUnit( found );
		}
	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Object" mode="findByMethodBody">
		<xsl:if test="$databaseConstraintsNodeSet/*[ self::xsd:key | self::xsd:unique ][ xsd:selector/@xpath = concat( './/', current()/@dbTable ) ]">
		/// <xsl:text disable-output-escaping="yes"><![CDATA[<summary>]]></xsl:text>
		/// Find the object using the primary key fields.
		/// <xsl:text disable-output-escaping="yes"><![CDATA[</summary>]]></xsl:text>
		public <xsl:value-of select="@className"/> FindBy<xsl:apply-templates select="." mode="findByMethodName"/>( <xsl:apply-templates select="." mode="findByMethodArgumentList"/> )
		{
			<xsl:apply-templates select="." mode="findByMethodImpl"/>
		}
		</xsl:if>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Argument list: string LineupName, string UnitId.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->
	
	<xsl:template match="cobos:Object" mode="findByMethodArgumentList">
		<xsl:variable name="object" select="."/>
		<xsl:for-each select="$databaseConstraintsNodeSet//xsd:key//xsd:field[ ../xsd:selector/@xpath = concat( './/', $object/@dbTable ) ]">
			<!-- Get the [ 1 ]st node since we will get multiple node results for db fields 
					that are decomposed into more than property, e.g. stringFormat="Seperator" -->
			<xsl:variable name="property" select="$object//cobos:Property[ translate( @dbColumn, $lowercase, $uppercase ) = current()/@xpath ][ 1 ]"/>
			<xsl:variable name="propertyType">
				<xsl:apply-templates select="$property" mode="propertyType"/>
			</xsl:variable>
			<xsl:value-of select="$propertyType"/>
			<xsl:value-of select="$property/@name"/>
			<xsl:if test="not( position() = last() )">
				<xsl:text>, </xsl:text>
			</xsl:if>
		</xsl:for-each>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Method body: 
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Object" mode="findByMethodImpl">
		<xsl:value-of select="@datasetRowType"/> found = _table.FindBy<xsl:apply-templates select="." mode="findByMethodName"/>( <xsl:apply-templates select="." mode="findByMethodParams"/> );

			if ( found == null || found.RowState == DataRowState.Deleted )
			{
				return null;
			}

			return new <xsl:value-of select="@className"/>( found );
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Method name: FindByLineupNameUnitId.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Object" mode="findByMethodName">
		<xsl:variable name="object" select="."/>
		<xsl:for-each select="$databaseConstraintsNodeSet//xsd:key//xsd:field[ ../xsd:selector/@xpath = concat( './/', $object/@dbTable ) ]">
			<xsl:variable name="property" select="$object//cobos:Property[ translate( @dbColumn, $lowercase, $uppercase ) = current()/@xpath ]"/>
			<xsl:value-of select="$property/@name"/>
		</xsl:for-each>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Method params: LineupName, UnitId.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Object" mode="findByMethodParams">
		<xsl:variable name="object" select="."/>
		<xsl:for-each select="$databaseConstraintsNodeSet//xsd:key//xsd:field[ ../xsd:selector/@xpath = concat( './/', $object/@dbTable ) ]">
			<xsl:apply-templates select="$object//cobos:Property[ translate( @dbColumn, $lowercase, $uppercase ) = current()/@xpath ]" mode="findByMethodParamValue"/>
			<xsl:if test="not( position() = last() )">
				<xsl:text>, </xsl:text>
			</xsl:if>
		</xsl:for-each>
	</xsl:template>

	<xsl:template match="cobos:Property" mode="findByMethodParamValue">
		<xsl:value-of select="@name"/>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	FindAll method body: Get all objects stored in memory.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	
	

	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Object" mode="findAllMethodBody">
		/// <xsl:text disable-output-escaping="yes"><![CDATA[<summary>]]></xsl:text>
		/// Get all objects stored in memory.
		/// <xsl:text disable-output-escaping="yes"><![CDATA[</summary>]]></xsl:text>
		public <xsl:apply-templates select="." mode="listDecl"/> FindAll()
		{
			int numRows = _table.Rows.Count;

			<xsl:apply-templates select="." mode="listDecl"/> results = new <xsl:apply-templates select="." mode="listDecl"/>( numRows );

			<xsl:text disable-output-escaping="yes"><![CDATA[for ( int row = 0; row < numRows; ++row )]]></xsl:text>
			{
				if ( _table[ row ].RowState != DataRowState.Deleted )
				{
					results.Add( new <xsl:value-of select="@className"/>( _table[ row ] ) );
				}
			}

			return results;
		}
	</xsl:template>
					 
</xsl:stylesheet>