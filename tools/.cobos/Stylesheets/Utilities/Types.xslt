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
  Custom String Format
  =============================================================================
  -->
  <xsl:template match="cobos:Property[@stringFormat]" mode="propertyType">
    <xsl:variable name="dataType">
      <xsl:value-of select="normalize-space(./cobos:StringFormat/cobos:CodeType)"/>
    </xsl:variable>
    <xsl:value-of select="$dataType"/>
    <xsl:if test="$dataType != 'string'">
      <xsl:apply-templates select="@minOccurs" mode="propertyType"/>
    </xsl:if>
  </xsl:template>
  <!--
  =============================================================================
  String
  =============================================================================
  -->
  <xsl:template match="cobos:Property[@dbType = 'xsd:string' or (contains(@dbType, 'string_') and not(@stringFormat))]" mode="propertyType">
    <xsl:text>string</xsl:text>
  </xsl:template>
  <xsl:template match="cobos:Property[@dbType = 'cobos:char']" mode="propertyType">
    <xsl:text>char</xsl:text>
    <xsl:apply-templates select="@minOccurs" mode="propertyType"/>
  </xsl:template>
  <!--
  =============================================================================
  Numeric - Integer
  =============================================================================
  -->
  <xsl:template match="cobos:Property[@dbType = 'xsd:integer']" mode="propertyType">
    <xsl:text>long</xsl:text>
    <xsl:apply-templates select="@minOccurs" mode="propertyType"/>
  </xsl:template>
  <xsl:template match="cobos:Property[@dbType = 'xsd:byte']" mode="propertyType">
    <xsl:text>sbyte</xsl:text>
    <xsl:apply-templates select="@minOccurs" mode="propertyType"/>
  </xsl:template>
  <xsl:template match="cobos:Property[@dbType = 'xsd:unsignedByte']" mode="propertyType">
    <xsl:text>byte</xsl:text>
    <xsl:apply-templates select="@minOccurs" mode="propertyType"/>
  </xsl:template>
  <xsl:template match="cobos:Property[@dbType = 'xsd:short']" mode="propertyType">
    <xsl:text>short</xsl:text>
    <xsl:apply-templates select="@minOccurs" mode="propertyType"/>
  </xsl:template>
  <xsl:template match="cobos:Property[@dbType = 'xsd:unsignedShort']" mode="propertyType">
    <xsl:text>ushort</xsl:text>
    <xsl:apply-templates select="@minOccurs" mode="propertyType"/>
  </xsl:template>
  <xsl:template match="cobos:Property[@dbType = 'xsd:int']" mode="propertyType">
    <xsl:text>int</xsl:text>
    <xsl:apply-templates select="@minOccurs" mode="propertyType"/>
  </xsl:template>
  <xsl:template match="cobos:Property[@dbType = 'xsd:unsignedInt']" mode="propertyType">
    <xsl:text>uint</xsl:text>
    <xsl:apply-templates select="@minOccurs" mode="propertyType"/>
  </xsl:template>
  <xsl:template match="cobos:Property[@dbType = 'xsd:long']" mode="propertyType">
    <xsl:text>long</xsl:text>
    <xsl:apply-templates select="@minOccurs" mode="propertyType"/>
  </xsl:template>
  <xsl:template match="cobos:Property[@dbType = 'xsd:unsignedLong']" mode="propertyType">
    <xsl:text>ulong</xsl:text>
    <xsl:apply-templates select="@minOccurs" mode="propertyType"/>
  </xsl:template>
  <!--
  =============================================================================
  Numeric - Fixed point
  =============================================================================
  -->
  <xsl:template match="cobos:Property[@dbType = 'xsd:decimal']" mode="propertyType">
    <xsl:text>decimal</xsl:text>
    <xsl:apply-templates select="@minOccurs" mode="propertyType"/>
  </xsl:template>
  <!--
  =============================================================================
  Numeric - Floating point
  =============================================================================
  -->
  <xsl:template match="cobos:Property[@dbType = 'xsd:float']" mode="propertyType">
    <xsl:text>float</xsl:text>
    <xsl:apply-templates select="@minOccurs" mode="propertyType"/>
  </xsl:template>
  <xsl:template match="cobos:Property[@dbType = 'xsd:double']" mode="propertyType">
    <xsl:text>double</xsl:text>
    <xsl:apply-templates select="@minOccurs" mode="propertyType"/>
  </xsl:template>
  <!--
  =============================================================================
  Date and Time.
  =============================================================================
  -->
  <xsl:template match="cobos:Property[@dbType = 'xsd:dateTime' or @dbType = 'xsd:date' or @dbType = 'cobos:timestamp']" mode="propertyType">
    <xsl:text>global::System.DateTime</xsl:text>
    <xsl:apply-templates select="@minOccurs" mode="propertyType"/>
  </xsl:template>
  <xsl:template match="cobos:Property[@dbType = 'xsd:gYear']" mode="propertyType">
    <xsl:text>byte</xsl:text>
    <xsl:apply-templates select="@minOccurs" mode="propertyType"/>
  </xsl:template>
  <xsl:template match="cobos:Property[@dbType = 'xsd:time' or @dbType = 'xsd:duration']" mode="propertyType">
    <xsl:text>global::System.TimeSpan</xsl:text>
    <xsl:apply-templates select="@minOccurs" mode="propertyType"/>
  </xsl:template>
  <!--
  =============================================================================
  Boolean
  =============================================================================
  -->
  <xsl:template match="cobos:Property[@dbType = 'xsd:boolean']" mode="propertyType">
    <xsl:text>bool</xsl:text>
    <xsl:apply-templates select="@minOccurs" mode="propertyType"/>
  </xsl:template>
  <!--
  =============================================================================
  Bit Field
  =============================================================================
  -->
  <xsl:template match="cobos:Property[@dbType = 'cobos:bitField']" mode="propertyType">
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