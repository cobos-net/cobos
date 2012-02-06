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
	Create the property get/set function declarations
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Object|cobos:Reference|cobos:Property[ ancestor::cobos:Interface ]" mode="propertyBody">
		<xsl:text>get;
			set;</xsl:text>
	</xsl:template>

	<xsl:template match="cobos:Object[ not( ancestor::cobos:Interface ) ]" mode="propertyBody">
		<xsl:text>get { return _</xsl:text><xsl:value-of select="@name"/><xsl:text>; }
			set { }</xsl:text>
	</xsl:template>
	
	<xsl:template match="cobos:Reference[ not( ancestor::cobos:Interface ) ]" mode="propertyBody">
		<xsl:text>get { </xsl:text>
		<xsl:apply-templates select="." mode="propertyGet"/>
		<xsl:text> }
			set { }</xsl:text>
	</xsl:template>

	<xsl:template match="cobos:Property[ not( ancestor::cobos:Interface ) ][ @minOccurs = 1 ]" mode="propertyBody">
		<xsl:text>get { </xsl:text>
		<xsl:apply-templates select="." mode="propertyGet"/>
		<xsl:text> }
			set { </xsl:text>
		<xsl:apply-templates select="." mode="propertySet"/>
		<xsl:text> } </xsl:text>
	</xsl:template>

	<xsl:template match="cobos:Property[ not( ancestor::cobos:Interface ) ][ @minOccurs = 0 ]" mode="propertyBody">
		<xsl:text>get
			{ </xsl:text>
				if ( ObjectDataRow.Is<xsl:apply-templates mode="qualifiedName" select="."/>Null() )
				{
					return null;
				}
				<xsl:apply-templates select="." mode="propertyGet"/>
			}
			set 
			{
				if ( value == null )
				{
					ObjectDataRow.Set<xsl:apply-templates mode="qualifiedName" select="."/>Null();
				}
				else
				{
					<xsl:apply-templates select="." mode="propertySet"/>
				} <xsl:text>
			} </xsl:text>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Create the property get function body implementation
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<!-- basic return type -->
	<xsl:template match="cobos:Property[ not( @stringFormat ) ]" mode="propertyGet">
		<xsl:variable name="columnName">
			<xsl:apply-templates mode="qualifiedName" select="."/>
		</xsl:variable>
		<xsl:value-of select="concat( 'return ObjectDataRow.', $columnName, ';' )"/>
	</xsl:template>

	<xsl:template match="cobos:Reference" mode="propertyGet">
		<xsl:value-of select="concat( 'return _', @name, ';' )"/>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Create the property set function body implementation
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<!-- basic type -->
	<xsl:template match="cobos:Property[ not( @stringFormat ) ]" mode="propertySet">
		<xsl:variable name="columnName">
			<xsl:apply-templates mode="qualifiedName" select="."/>
		</xsl:variable>
		<xsl:variable name="value">
			<xsl:apply-templates mode="propertySetValue" select="."/>
		</xsl:variable>
		<xsl:value-of select="concat( 'ObjectDataRow.', $columnName, ' = ', $value, ';' )"/>
	</xsl:template>

	<!-- simple case for any non-nullable type -->
	<xsl:template match="cobos:Property[ @minOccurs = 1 ]" mode="propertySetValue">
		<xsl:text>value</xsl:text>
	</xsl:template>

	<!-- Strings can be set to null -->
	<xsl:template match="cobos:Property[ @minOccurs = 0 and @dbType = 'xsd:string' ]" mode="propertySetValue">
		<xsl:text>value</xsl:text>
	</xsl:template>

	<!-- any other string length that is not a special formatted value -->
	<xsl:template match="cobos:Property[ @minOccurs = 0 and (contains( @dbType, 'string_' ) and not( @stringFormat )) ]" mode="propertySetValue">
		<xsl:text>value</xsl:text>
	</xsl:template>

	<!-- Nullable value types -->
	<xsl:template match="cobos:Property[ @minOccurs = 0 and ( @dbType = 'xsd:integer' or @dbType = 'xsd:float' or @dbType = 'xsd:dateTime' ) ]" mode="propertySetValue">
		<xsl:text>value.Value</xsl:text>
	</xsl:template>
	
</xsl:stylesheet>