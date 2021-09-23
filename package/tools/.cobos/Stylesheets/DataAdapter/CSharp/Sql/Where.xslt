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
	============================================================================
	Created by: N.Davis                        Date: 2010-04-09
	Modified by:                               Date:
	============================================================================
	Notes: 
	
	
	============================================================================
	-->
  <!--
	============================================================================
	WHERE clause
	============================================================================

	============================================================================
	-->
  <xsl:template match="cobos:Object[cobos:Metadata/cobos:Filter]" mode="sqlWhere">
    <xsl:text>new string[] { </xsl:text>
    <xsl:apply-templates select="cobos:Metadata/cobos:Filter" mode="sqlWhere"/>
    <xsl:text> }</xsl:text>
  </xsl:template>
  <!--
	============================================================================
	-->
  <xsl:template match="cobos:Object" mode="sqlWhere">
    <xsl:text>null</xsl:text>
  </xsl:template>
  <!--
	============================================================================
	-->
  <xsl:template match="cobos:Filter" mode="sqlWhere">
    <xsl:text>"</xsl:text>
    <xsl:value-of select="."/>
    <xsl:text>"</xsl:text>
    <xsl:if test="not(position() = last())">, </xsl:if>
  </xsl:template>
</xsl:stylesheet>