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
	Match property types
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Property[ @dbType = 'xsd:string' or (contains( @dbType, 'string_' ) and not( @stringFormat )) ]" mode="propertyType">
		<xsl:text>string </xsl:text>
	</xsl:template>

	<xsl:template match="cobos:Property[ @dbType = 'xsd:integer' ]" mode="propertyType">
		<xsl:text>long</xsl:text>
		<xsl:apply-templates select="@minOccurs" mode="propertyType"/>
	</xsl:template>

	<xsl:template match="cobos:Property[ @dbType = 'xsd:float' ]" mode="propertyType">
		<xsl:text>float</xsl:text>
		<xsl:apply-templates select="@minOccurs" mode="propertyType"/>
	</xsl:template>

	<xsl:template match="cobos:Property[ @dbType = 'xsd:dateTime' ]" mode="propertyType">
		<xsl:text>DateTime</xsl:text>
		<xsl:apply-templates select="@minOccurs" mode="propertyType"/>
	</xsl:template>

	<xsl:template match="cobos:Property[ contains( @dbType, 'hexBinary_' ) ]" mode="propertyType">
		<xsl:text>byte[] </xsl:text>
	</xsl:template>

	<xsl:template match="@minOccurs[ . = 0 ]" mode="propertyType">
		<xsl:text>? </xsl:text>
	</xsl:template>

	<xsl:template match="@minOccurs[ . = 1 ]" mode="propertyType">
		<xsl:text> </xsl:text>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Match object types - may be overriding abstract class
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Object[ ancestor-or-self::cobos:Object[ @implements ] ]" mode="propertyType">
		<!-- get the interface object the parent class implements -->
		<xsl:variable name="interface" select="/cobos:DataModel/cobos:Interface[ @name = current()/ancestor-or-self::cobos:Object[ @implements ]/@implements ]"/>
		<!-- only apply if this property is defined in the abstract class -->
		<xsl:variable name="property" select="$interface//cobos:Object[ substring-after( @qualifiedName, '.' ) = substring-after( current()/@qualifiedName, '.' ) ]"/>
		<xsl:choose>
			<xsl:when test="$property">
				<xsl:value-of select="$property/@qualifiedTypeName"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="@typeName"/>
			</xsl:otherwise>
		</xsl:choose>
		<xsl:text> </xsl:text>
	</xsl:template>

	<xsl:template match="cobos:Object[ not( ancestor-or-self::cobos:Object[ @implements ] ) ]" mode="propertyType">
		<xsl:value-of select="@typeName"/>
		<xsl:text> </xsl:text>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Match reference types
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Reference[ not( @isCollection ) ]" mode="propertyType">
		<xsl:value-of select="@ref"/>
		<xsl:text> </xsl:text>
	</xsl:template>

	<xsl:template match="cobos:Reference[ @isCollection ]" mode="propertyType">
		<xsl:apply-templates select="." mode="listDecl"/>
		<xsl:text> </xsl:text>
	</xsl:template>
					 
</xsl:stylesheet>