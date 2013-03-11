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
	Object member variables 
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Object[ not( ancestor::cobos:Interface ) ]" mode="classMemberDefinition">
    <xsl:variable name="datasetRowType">
      <xsl:apply-templates select="." mode="datasetRowType"/>
    </xsl:variable>
    <xsl:value-of select="concat($indent2, 'public readonly ', $datasetRowType, ' ObjectDataRow;')" />
    <xsl:apply-templates select="cobos:Object" mode="classMemberDecl"/>
    <xsl:apply-templates select="cobos:Reference" mode="classMemberDecl"/>
  </xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Class member declaration for a nested object
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Object" mode="classMemberDecl">
    <xsl:value-of select="$newlineIndent2"/>
    <xsl:value-of select="concat('private ', @typeName, ' ', @memberName, ';')" />
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Class member declaration for a reference type
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Reference[not(@isCollection)]" mode="classMemberDecl">
    <xsl:value-of select="$newlineIndent2"/>
    <xsl:value-of select="concat('private ', @ref, ' ', @memberName, ';')" />
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Class member declaration for reference type that is a collection
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Reference[ @isCollection ]" mode="classMemberDecl">
    <xsl:value-of select="$newlineIndent2"/>
    <xsl:variable name="listDecl">
      <xsl:apply-templates select="." mode="listDecl"/>
    </xsl:variable>
    <xsl:value-of select="concat('private ', $listDecl, ' ', @memberName, ';')" />
	</xsl:template>

	
</xsl:stylesheet>