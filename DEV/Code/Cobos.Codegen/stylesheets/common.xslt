<xsl:stylesheet version="1.0"
						xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
						xmlns:msxsl="urn:schemas-microsoft-com:xslt"
						exclude-result-prefixes="msxsl"
						xmlns="http://schemas.cobos.co.uk/datamodel/1.0.0"
						xmlns:cobos="http://schemas.cobos.co.uk/datamodel/1.0.0"
						xmlns:xsd="http://www.w3.org/2001/XMLSchema"
>
	<!-- 
	=============================================================================
	Filename: common.xslt
	Description: XSLT for common data model functionality
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Created by: N.Davis                        Date: 2010-04-09
	Modified by:                               Date:
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Common includes and definitions

	============================================================================
	-->

	<!-- include the generated database schema variables -->
	<xsl:include href="./database/schema.xslt"/>
	<xsl:include href="utilities.xslt"/>

	<!-- useful for formatting text output -->
	<xsl:variable name="newline" select="string( '&#xD;&#xA;' )"/>
	<xsl:variable name="tab" select="string( '&#9;' )"/>
	<xsl:variable name="newlineTab" select="concat( $newline, $tab )"/>
	<xsl:variable name="newlineTab2" select="concat( $newline, $tab, $tab )"/>
	<xsl:variable name="newlineTab3" select="concat( $newline, $tab, $tab, $tab )"/>
	<xsl:variable name="newlineTab4" select="concat( $newline, $tab, $tab, $tab, $tab )"/>
	
	<!-- XML reserved characters for use in concat function -->
	<xsl:variable name="apos">'</xsl:variable>
	<xsl:variable name="quot">"</xsl:variable>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Class naming conventions
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<!-- class name for the datamodel -->
	<xsl:template match="cobos:DataModel|cobos:TableObject|cobos:Interface" mode="className">
		<xsl:call-template name="tokensToClassName">
			<xsl:with-param name="tokens" select="@name"/>
		</xsl:call-template>
	</xsl:template>

	<!-- class name for concrete object type -->
	<xsl:template match="cobos:Object[ parent::cobos:DataModel ]" mode="className">
		<xsl:call-template name="tokensToClassName">
			<xsl:with-param name="tokens" select="@name"/>
		</xsl:call-template>
	</xsl:template>

	<!-- class name for a type reference -->
	<xsl:template match="cobos:Object[ @type ][ not( parent::cobos:DataModel ) ]" mode="className">
		<xsl:call-template name="tokensToClassName">
			<xsl:with-param name="tokens" select="@type"/>
		</xsl:call-template>
	</xsl:template>

	<!-- class name for an anonymous nested type, make it up based on the parent name -->
	<xsl:template match="cobos:Object[ not( @type ) ][ not( parent::cobos:DataModel ) ]" mode="className">
		<xsl:call-template name="tokensToClassName">
			<xsl:with-param name="tokens" select="concat( ../@name, ' ', @name )"/>
		</xsl:call-template>
	</xsl:template>

	<xsl:template match="cobos:Property[ not( @dbAlias ) ]" mode="dbColumn">
		<xsl:value-of select="@dbColumn"/>
	</xsl:template>

	<xsl:template match="cobos:Property[ @dbAlias ]" mode="dbColumn">
		<xsl:value-of select="@dbAlias"/>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Class names - qualified names
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Object" mode="qualifiedName">
		<xsl:apply-templates select="ancestor::*[ self::cobos:Object | self::cobos:Interface ]" mode="qualifiedNameForClass"/>
		<xsl:call-template name="titleCaseName">
			<xsl:with-param name="name">
				<xsl:value-of select="@name"/>
			</xsl:with-param>
		</xsl:call-template>
	</xsl:template>

	<xsl:template match="cobos:Object|cobos:Interface" mode="qualifiedNameForClass">
		<xsl:call-template name="titleCaseName">
			<xsl:with-param name="name">
				<xsl:value-of select="@name"/>
			</xsl:with-param>
		</xsl:call-template>
		<xsl:text>.</xsl:text>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Type names - for objects and types
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->
	<xsl:template match="cobos:DataModel|cobos:TableObject|cobos:Interface|cobos:Object[ parent::cobos:DataModel ]" mode="typeName">
		<xsl:value-of select="@name"/>
	</xsl:template>

	<xsl:template match="cobos:Object[ not( @type ) and not( parent::cobos:DataModel ) ]" mode="typeName">
		<xsl:value-of select="concat( ../@name, @name )"/>
	</xsl:template>

	<xsl:template match="cobos:Object[ @type and not( parent::cobos:DataModel ) ]" mode="typeName">
		<xsl:value-of select="concat( parent::cobos:Object/@name, @name )"/>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Qualified type names
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Object" mode="qualifiedTypeName">
		<xsl:apply-templates select="ancestor::*[ self::cobos:Object | self::cobos:Interface ]" mode="qualifiedTypeNameForClass"/>
		<xsl:call-template name="titleCaseName">
			<xsl:with-param name="name">
				<xsl:apply-templates select="." mode="typeName"/>
			</xsl:with-param>
		</xsl:call-template>
	</xsl:template>

	<xsl:template match="cobos:Object|cobos:Interface" mode="qualifiedTypeNameForClass">
		<xsl:call-template name="titleCaseName">
			<xsl:with-param name="name">
				<xsl:apply-templates select="." mode="typeName"/>
			</xsl:with-param>
		</xsl:call-template>
		<xsl:text>.</xsl:text>
	</xsl:template>


	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Property names - nested types are flattened in our DB model
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Property" mode="qualifiedName">
		<xsl:apply-templates select="ancestor::cobos:Object[ not( position() = last() ) ]" mode="qualifiedNameForProperty"/>
		<xsl:call-template name="titleCaseName">
			<xsl:with-param name="name">
				<xsl:value-of select="@name"/>
			</xsl:with-param>
		</xsl:call-template>
	</xsl:template>

	<xsl:template match="cobos:Object" mode="qualifiedNameForProperty">
		<xsl:call-template name="titleCaseName">
			<xsl:with-param name="name">
				<xsl:value-of select="@name"/>
			</xsl:with-param>
		</xsl:call-template>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	DataSet names - Helpers
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Object" mode="datasetRowType">
		<xsl:value-of select="ancestor-or-self::cobos:Object[ @datasetRowType ]/@datasetRowType"/>
	</xsl:template>

	<xsl:template match="cobos:Object" mode="datasetTableType">
		<xsl:value-of select="ancestor-or-self::cobos:Object[ @datasetTableType ]/@datasetTableType"/>
	</xsl:template>

</xsl:stylesheet>
