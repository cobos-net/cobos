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
  Filename: Types.xslt
  Description: Type Definitions for CSharp types.
  =============================================================================
  Created by: N.Davis                        Date: 2010-04-09
  Modified by:                               Date:
  =============================================================================
  Notes: 
  
  
  ============================================================================
  -->
  <!--
  =============================================================================
  Property Types
  =============================================================================
  -->
  <xsl:template match="cobos:Property[not(@converter)]" mode="propertyType">
    <xsl:variable name="dataType">
      <xsl:apply-templates select="@dbType" mode="propertyType"/>
    </xsl:variable>
    <xsl:value-of select="$dataType"/>
    <xsl:if test="$dataType != 'string' and not (contains($dataType, '[]') or contains($dataType, 'System.Collections'))">
      <xsl:apply-templates select="@minOccurs" mode="propertyType"/>
    </xsl:if>
  </xsl:template>

  <xsl:template match="cobos:Property[@converter]" mode="propertyType">
    <xsl:variable name="targetType">
      <xsl:apply-templates select="@converterTargetType" mode="propertyType"/>
    </xsl:variable>
    <xsl:value-of select="$targetType"/>
    <xsl:if test="$targetType != 'string' and not (contains($targetType, '[]') or contains($targetType, 'System.Collections'))">
      <xsl:apply-templates select="@minOccurs" mode="propertyType"/>
    </xsl:if>
  </xsl:template>

  <xsl:template match="cobos:Property[@converterParameter]" mode="converterParameter">
    <xsl:value-of select="concat($quot, @converterParameter, $quot)"/>
  </xsl:template>

  <xsl:template match="cobos:Property[not(@converterParameter)]" mode="converterParameter">
    <xsl:text>null</xsl:text>
  </xsl:template>
  <!--
  =============================================================================
  String
  =============================================================================
  -->
  <xsl:template match="@*[. = 'xsd:string' or contains(., 'string_')]" mode="propertyType">
    <xsl:text>string</xsl:text>
  </xsl:template>
  <xsl:template match="@*[. = 'cobos:char']" mode="propertyType">
    <xsl:text>char</xsl:text>
  </xsl:template>
  <!--
  =============================================================================
  Numeric - Integer
  =============================================================================
  -->
  <xsl:template match="@*[. = 'xsd:integer']" mode="propertyType">
    <xsl:text>long</xsl:text>
  </xsl:template>
  <xsl:template match="@*[. = 'xsd:byte']" mode="propertyType">
    <xsl:text>sbyte</xsl:text>
  </xsl:template>
  <xsl:template match="@*[. = 'xsd:unsignedByte']" mode="propertyType">
    <xsl:text>byte</xsl:text>
  </xsl:template>
  <xsl:template match="@*[. = 'xsd:short']" mode="propertyType">
    <xsl:text>short</xsl:text>
  </xsl:template>
  <xsl:template match="@*[. = 'xsd:unsignedShort']" mode="propertyType">
    <xsl:text>ushort</xsl:text>
  </xsl:template>
  <xsl:template match="@*[. = 'xsd:int']" mode="propertyType">
    <xsl:text>int</xsl:text>
  </xsl:template>
  <xsl:template match="@*[. = 'xsd:unsignedInt']" mode="propertyType">
    <xsl:text>uint</xsl:text>
  </xsl:template>
  <xsl:template match="@*[. = 'xsd:long']" mode="propertyType">
    <xsl:text>long</xsl:text>
  </xsl:template>
  <xsl:template match="@*[. = 'xsd:unsignedLong']" mode="propertyType">
    <xsl:text>ulong</xsl:text>
  </xsl:template>
  <!--
  =============================================================================
  Numeric - Fixed point
  =============================================================================
  -->
  <xsl:template match="@*[. = 'xsd:decimal']" mode="propertyType">
    <xsl:text>decimal</xsl:text>
  </xsl:template>
  <!--
  =============================================================================
  Numeric - Floating point
  =============================================================================
  -->
  <xsl:template match="@*[. = 'xsd:float']" mode="propertyType">
    <xsl:text>float</xsl:text>
  </xsl:template>
  <xsl:template match="@*[. = 'xsd:double']" mode="propertyType">
    <xsl:text>double</xsl:text>
  </xsl:template>
  <!--
  =============================================================================
  Date and Time.
  =============================================================================
  -->
  <xsl:template match="@*[. = 'xsd:dateTime' or . = 'xsd:date' or . = 'cobos:timestamp']" mode="propertyType">
    <xsl:text>global::System.DateTime</xsl:text>
  </xsl:template>
  <xsl:template match="@*[. = 'xsd:gYear']" mode="propertyType">
    <xsl:text>byte</xsl:text>
  </xsl:template>
  <xsl:template match="@*[. = 'xsd:time' or @dbType = 'xsd:duration']" mode="propertyType">
    <xsl:text>global::System.TimeSpan</xsl:text>
  </xsl:template>
  <!--
  =============================================================================
  Boolean
  =============================================================================
  -->
  <xsl:template match="@*[. = 'xsd:boolean']" mode="propertyType">
    <xsl:text>bool</xsl:text>
  </xsl:template>
  <!--
  =============================================================================
  Bit Field
  =============================================================================
  -->
  <xsl:template match="@*[. = 'cobos:bitField']" mode="propertyType">
    <xsl:text>global::System.Collections.BitArray</xsl:text>
  </xsl:template>
  <!--
  =============================================================================
  BLOB
  =============================================================================
  -->
  <xsl:template match="cobos:Property[contains(@dbType, 'hexBinary')]" mode="propertyType">
    <xsl:text>byte[]</xsl:text>
  </xsl:template>
  <!--
  =============================================================================
  Multiplicity
  =============================================================================
  -->
  <xsl:template match="@minOccurs[. = 0]" mode="propertyType">
    <xsl:text>?</xsl:text>
  </xsl:template>
  <xsl:template match="@minOccurs[. = 1]" mode="propertyType">
    <xsl:text></xsl:text>
  </xsl:template>
  <!--
  =============================================================================
  Match object types - may be overriding abstract class
  =============================================================================
  -->
  <xsl:template match="cobos:Object" mode="propertyType">
    <xsl:value-of select="@className"/>
  </xsl:template>
  <!--
  =============================================================================
  Match reference types
  =============================================================================
  -->
  <xsl:template match="cobos:Reference" mode="propertyType">
    <xsl:apply-templates select="." mode="listDecl"/>
    <xsl:text></xsl:text>
  </xsl:template>
  <!--
  =============================================================================
  List<> declarations
  =============================================================================
  -->
  <xsl:template match="cobos:Object" mode="listDecl">
    <xsl:value-of select="concat('global::System.Collections.Generic.IEnumerable&lt;', @className, '&gt;')"/>
  </xsl:template>
  <xsl:template match="cobos:Object" mode="listDeclDataRow">
    <xsl:value-of select="concat('global::System.Collections.Generic.IEnumerable&lt;', @datasetRowType, '&gt;')"/>
  </xsl:template>
  <xsl:template match="cobos:Reference" mode="listDecl">
    <xsl:value-of select="concat('global::System.Collections.Generic.IEnumerable&lt;', @ref, '&gt;')"/>
  </xsl:template>
</xsl:stylesheet>