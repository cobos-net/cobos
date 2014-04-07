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
	SQL SELECT columns
	============================================================================
	-->

  <xsl:template match="cobos:Property" mode="sqlSelect">
    <xsl:apply-templates select="." mode="sqlSelectColumn"/>
    <xsl:if test="not(position() = last())">, </xsl:if>
  </xsl:template>

  <xsl:template match="cobos:Property" mode="sqlSelectColumn">
    <xsl:value-of select="@dbTable"/>.<xsl:value-of select="@dbColumn"/>
  </xsl:template>

  <xsl:template match="cobos:Property[@dbAlias]" mode="sqlSelectColumn">
    <xsl:value-of select="@dbTable"/>.<xsl:value-of select="@dbColumn"/>
    <xsl:text > AS </xsl:text>
    <xsl:value-of select="@dbAlias"/>
  </xsl:template>

  <xsl:template match="cobos:Property[@dbSelect]" mode="sqlSelectColumn">
    <xsl:value-of select="@dbSelect"/>
    <xsl:text > AS </xsl:text>
    <xsl:value-of select="@dbColumn"/>
  </xsl:template>

</xsl:stylesheet>