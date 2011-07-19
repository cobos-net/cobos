<xsl:stylesheet version="1.0"
						xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
						xmlns:msxsl="urn:schemas-microsoft-com:xslt"
						exclude-result-prefixes="msxsl"
						xmlns="http://schemas.intergraph.com/asiapac/cad/datamodel/1.0.0"
						xmlns:cad="http://schemas.intergraph.com/asiapac/cad/datamodel/1.0.0"
						xmlns:xsd="http://www.w3.org/2001/XMLSchema"
>
	<!-- 
	=============================================================================
	Filename: datamodelcommon.xslt
	Description: XSLT for common data model functionality
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Created by: N.Davis                        Date: 2010-04-09
	Modified by:                               Date:
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Common includes and definitions

	============================================================================
	-->

	<!-- include the generated database schema variables -->
	<xsl:include href="CadDatabase.xslt"/>
	<xsl:include href="Utilities.xslt"/>

	<!-- useful for formatting text output -->
	<xsl:variable name="newline" select="string('&#xD;&#xA;')"/>
	<xsl:variable name="tab" select="string('&#9;')"/>
	<xsl:variable name="newlineTab" select="concat( $newline, $tab )"/>
	<xsl:variable name="newlineTab2" select="concat( $newline, $tab, $tab )"/>
	<xsl:variable name="newlineTab3" select="concat( $newline, $tab, $tab, $tab )"/>
	<xsl:variable name="newlineTab4" select="concat( $newline, $tab, $tab, $tab, $tab )"/>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Class naming conventions
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<!-- class name for the datamodel -->
	<xsl:template match="cad:DataModel|cad:TableObject|cad:Interface" mode="className">
		<xsl:call-template name="tokensToClassName">
			<xsl:with-param name="tokens" select="@name"/>
		</xsl:call-template>
	</xsl:template>

	<!-- class name for concrete object type -->
	<xsl:template match="cad:Object[ parent::cad:DataModel ]" mode="className">
		<xsl:call-template name="tokensToClassName">
			<xsl:with-param name="tokens" select="@name"/>
		</xsl:call-template>
	</xsl:template>

	<!-- class name for a type reference -->
	<xsl:template match="cad:Object[ @type ][ not( parent::cad:DataModel ) ]" mode="className">
		<xsl:call-template name="tokensToClassName">
			<xsl:with-param name="tokens" select="@type"/>
		</xsl:call-template>
	</xsl:template>

	<!-- class name for an anonymous nested type, make it up based on the parent name -->
	<xsl:template match="cad:Object[ not( @type ) ][ not( parent::cad:DataModel ) ]" mode="className">
		<xsl:call-template name="tokensToClassName">
			<xsl:with-param name="tokens" select="concat( ../@name, ' ', @name )"/>
		</xsl:call-template>
	</xsl:template>

	<xsl:template match="cad:Property[ not( @dbAlias ) ]" mode="dbColumn">
		<xsl:value-of select="@dbColumn"/>
	</xsl:template>

	<xsl:template match="cad:Property[ @dbAlias ]" mode="dbColumn">
		<xsl:value-of select="@dbAlias"/>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Class names - qualified names
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cad:Object" mode="qualifiedName">
		<xsl:apply-templates select="ancestor::*[ self::cad:Object | self::cad:Interface ]" mode="qualifiedNameForClass"/>
		<xsl:call-template name="titleCaseName">
			<xsl:with-param name="name">
				<xsl:value-of select="@name"/>
			</xsl:with-param>
		</xsl:call-template>
		<xsl:text>Type</xsl:text>
	</xsl:template>

	<xsl:template match="cad:Object|cad:Interface" mode="qualifiedNameForClass">
		<xsl:call-template name="titleCaseName">
			<xsl:with-param name="name">
				<xsl:value-of select="@name"/>
			</xsl:with-param>
		</xsl:call-template>
		<xsl:if test="not( position() = 1 )">
			<xsl:text>Type</xsl:text>
		</xsl:if>
		<xsl:text>.</xsl:text>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Property names - nested types are flattened in our DB model
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cad:Property" mode="qualifiedName">
		<xsl:apply-templates select="ancestor::cad:Object[ not( position() = last() ) ]" mode="qualifiedNameForProperty"/>
		<xsl:call-template name="titleCaseName">
			<xsl:with-param name="name">
				<xsl:value-of select="@name"/>
			</xsl:with-param>
		</xsl:call-template>
	</xsl:template>

	<xsl:template match="cad:Object" mode="qualifiedNameForProperty">
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

	<xsl:template match="cad:Object" mode="datasetRowType">
		<xsl:value-of select="ancestor-or-self::cad:Object[ @datasetRowType ]/@datasetRowType"/>
	</xsl:template>

	<xsl:template match="cad:Object" mode="datasetTableType">
		<xsl:value-of select="ancestor-or-self::cad:Object[ @datasetTableType ]/@datasetTableType"/>
	</xsl:template>

</xsl:stylesheet>
