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
	INNER/OUTER JOIN
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	
		static readonly string[] _innerJoin = new string[]{ "EVENT ON AEVEN.eid = EVENT.eid" };
	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

  <!-- Find all inner join tables -->
  <xsl:template match="cobos:Object[./cobos:Metadata/cobos:Joins/*]" mode="sqlJoin">
    <xsl:text>new string[] { </xsl:text>
    <xsl:apply-templates mode="sqlJoin" select="./cobos:Metadata/cobos:Joins/*[self::cobos:InnerJoin | self::cobos:OuterJoin]"/>
    <xsl:text> }</xsl:text>
  </xsl:template>

  <xsl:template match="cobos:Object[not(./cobos:Metadata/cobos:Joins/*)]" mode="sqlJoin">
    <xsl:text>null</xsl:text>
  </xsl:template>

  <!-- INNER JOIN and OUTER JOIN clauses -->
  <xsl:template match="cobos:InnerJoin | cobos:OuterJoin" mode="sqlJoin">
    <xsl:text>"</xsl:text>
    <xsl:value-of select="@references"/>
    <xsl:text> ON </xsl:text>
    <xsl:value-of select="ancestor::cobos:Object[1]/@dbTable"/>
    <xsl:text>.</xsl:text>
    <xsl:value-of select="@foreignKey"/>
    <xsl:text> = </xsl:text>
    <xsl:value-of select="@references"/>
    <xsl:text>.</xsl:text>
    <xsl:value-of select="@referenceKey"/>
    <xsl:text>"</xsl:text>
    <xsl:if test="not(position() = last())">, </xsl:if>
  </xsl:template>

</xsl:stylesheet>