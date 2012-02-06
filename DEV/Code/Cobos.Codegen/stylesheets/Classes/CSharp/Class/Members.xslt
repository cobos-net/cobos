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
		<xsl:apply-templates select="cobos:Object" mode="classMemberDecl"/>
		<xsl:apply-templates select="cobos:Reference" mode="classMemberDecl"/>
		
		public readonly <xsl:value-of select="$datasetRowType"/> ObjectDataRow;
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Class member declaration for a nested object
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Object" mode="classMemberDecl">
		<xsl:value-of select="@typeName"/>
		<xsl:text> _</xsl:text>
		<xsl:value-of select="@name"/>
		<xsl:text>;</xsl:text>
		<xsl:if test="not( position() = last() )">
			<xsl:value-of select="$newlineTab2"/>
		</xsl:if>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Class member declaration for a reference type
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Reference[ not( @isCollection ) ]" mode="classMemberDecl">
		<xsl:value-of select="@ref"/>
		<xsl:text> _</xsl:text>
		<xsl:value-of select="@name"/>
		<xsl:text>;</xsl:text>
		<xsl:if test="not( position() = last() )">
			<xsl:value-of select="$newlineTab2"/>
		</xsl:if>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Class member declaration for reference type that is a collection
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Reference[ @isCollection ]" mode="classMemberDecl">
		<xsl:apply-templates select="." mode="listDecl"/>
		<xsl:text> _</xsl:text>
		<xsl:value-of select="@name"/>
		<xsl:text>;</xsl:text>
		<xsl:if test="not( position() = last() )">
			<xsl:value-of select="$newlineTab2"/>
		</xsl:if>
	</xsl:template>

	
</xsl:stylesheet>