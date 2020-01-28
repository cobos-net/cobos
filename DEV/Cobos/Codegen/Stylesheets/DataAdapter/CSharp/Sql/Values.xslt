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
  Filename: Values.xslt
  Description: SQL column value fragments for inserting and updating rows
  =============================================================================
  Created by: N.Davis                        Date: 2010-04-09
  Modified by:                               Date:
  =============================================================================
  Notes: 
  
  
  =============================================================================
  -->
  <!--
  =============================================================================
  Simple case for non-nullable fields.
  =============================================================================
  -->
  <xsl:template match="cobos:Property[@minOccurs = 1]" mode="sqlPropertyRow">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndentLevel3"/>
    </xsl:variable>
    <xsl:variable name="columnValue">
      <xsl:apply-templates mode="sqlPropertyValue" select="."/>
    </xsl:variable>
    <xsl:value-of select="concat($indent, 'buffer.Append(', $columnValue, ');')"/>
    <xsl:if test="not(position() = last())">
      <xsl:value-of select="concat($indent, 'buffer.Append(', $quot, ', ', $quot, ');')"/>
    </xsl:if>
  </xsl:template>
  <!--
  =============================================================================
  Test for null for nullable fields before inserting.
  =============================================================================
  -->
  <xsl:template match="cobos:Property[@minOccurs = 0]" mode="sqlPropertyRow">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndentLevel3"/>
    </xsl:variable>
    <xsl:variable name="columnName">
      <xsl:apply-templates mode="fullName" select="."/>
    </xsl:variable>
    <xsl:variable name="columnValue">
      <xsl:apply-templates mode="sqlPropertyValue" select="."/>
    </xsl:variable>
    <xsl:value-of select="concat($indent, 'buffer.Append(row.Is', $columnName, 'Null() ? ', $quot, 'NULL', $quot, ' : ', $columnValue, ');')"/>
    <xsl:if test="not(position() = last())">
      <xsl:value-of select="concat($indent, 'buffer.Append(', $quot, ', ', $quot, ');')"/>
    </xsl:if>
  </xsl:template>
  <!--
  =============================================================================
  Simple property value for non-string and date types.
  =============================================================================
  -->
  <xsl:template match="cobos:Property[not(@dbType = 'xsd:string' or @dbType = 'xsd:dateTime' or contains(@dbType, 'string_'))]" mode="sqlPropertyValue">
    <xsl:variable name="columnName">
      <xsl:apply-templates mode="fullName" select="."/>
    </xsl:variable>
    <xsl:value-of select="concat('row.', $columnName, '.ToString()')"/>
  </xsl:template>
  <!--
  =============================================================================
  Get a quoted value for the string property.
  =============================================================================
  -->
  <xsl:template match="cobos:Property[@dbType = 'xsd:string' or contains(@dbType, 'string_')]" mode="sqlPropertyValue">
    <xsl:variable name="columnName">
      <xsl:apply-templates mode="fullName" select="."/>
    </xsl:variable>
    <xsl:value-of select="concat($quot, $apos, $quot, ' + row.', $columnName, '.Replace(', $quot, $apos, $quot, ', ', $quot, $apos, $apos, $quot, ') + ', $quot, $apos, $quot)"/>
  </xsl:template>
  <!--
  =============================================================================
  Get a quoted value for the dateTime property.
  =============================================================================
  -->
  <xsl:template match="cobos:Property[@dbType = 'xsd:dateTime']" mode="sqlPropertyValue">
    <xsl:variable name="columnName">
      <xsl:apply-templates mode="fullName" select="."/>
    </xsl:variable>
    <xsl:value-of select="concat($quot, $apos, $quot, ' + row.', $columnName, '.ToLocalTime().ToString(', $quot, 'yyyy-MM-dd HH:mm:ss.fff', $quot, ') + ', $quot, $apos, $quot)"/>
  </xsl:template>
  <!--
  ============================================================================
  ORDER/GROUP BY value
  ============================================================================
  -->
  <xsl:template match="cobos:By">
    <xsl:variable name="property" select="ancestor::cobos:Object[1]//cobos:Property[@name = current()/text()]"/>
    <xsl:value-of select="$property/@dbTable"/>
    <xsl:text>.</xsl:text>
    <xsl:value-of select="$property/@dbColumn"/>
    <xsl:if test="not(position() = last())">, </xsl:if>
  </xsl:template>
</xsl:stylesheet>