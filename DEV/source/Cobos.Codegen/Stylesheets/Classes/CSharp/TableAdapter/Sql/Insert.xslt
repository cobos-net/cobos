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
	SQL INSERT columns
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

  <xsl:template match="cobos:Property" mode="sqlInsert">
    <xsl:value-of select="@dbColumn"/>
    <xsl:if test="not(position() = last())">, </xsl:if>
  </xsl:template>

  <!-- simple case for non-nullable fields -->
  <xsl:template match="cobos:Property[@minOccurs = 1]" mode="sqlInsertValue">
    <xsl:apply-templates select="." mode="sqlPropertyRow"/>
    <xsl:if test="not(position() = last())">
      <xsl:value-of select="concat($newlineIndent3, 'buffer.Append(', $quot, ', ', $quot, ');')"/>
    </xsl:if>
  </xsl:template>

  <!-- test for null for nullable fields before inserting -->
  <xsl:template match="cobos:Property[@minOccurs = 0]" mode="sqlInsertValue">
    <xsl:apply-templates select="." mode="sqlPropertyRow"/>
    <xsl:if test="not(position() = last())">
      <xsl:value-of select="concat($newlineIndent3, 'buffer.Append(', $quot, ', ', $quot, ');')"/>
    </xsl:if>
  </xsl:template>

</xsl:stylesheet>