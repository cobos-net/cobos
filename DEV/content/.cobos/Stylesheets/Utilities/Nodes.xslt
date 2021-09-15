<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <!-- 
  =============================================================================
  Filename: Nodes.xslt
  Description: Node related utilities.
  =============================================================================
  Created by: N.Davis                        Date: 2010-04-09
  Modified by:                               Date:
  =============================================================================
  
  
  =============================================================================
  -->
  <!-- 
  =============================================================================
  Artificially create namespace declarations on a root element
  =============================================================================
  Example:

  <xsl:template name="nsdecl">
    <xsl:variable name="nsdecl">
      <nsdecl xmlns:cobos="http://schemas.cobos.co.uk/datamodel/1.0.0" 
            xmlns:msxsl="urn:schemas-microsoft-com:xslt"  
            xmlns="http://schemas.cobos.co.uk/datamodel/1.0.0"
            xmlns:xsd="http://www.w3.org/2001/XMLSchema"/>
    </xsl:variable>
    <xsl:apply-templates mode="copyAttributesAndNamespace" select="msxsl:node-set($nsdecl)"/>
  </xsl:template> 
  =============================================================================
  -->
  <xsl:template match="*" mode="copyAttributesAndNamespace">
    <xsl:copy-of select="namespace::* | @*"/>
  </xsl:template>
  <!-- 
  =============================================================================
  Copy nodes without copying namespaces
  =============================================================================
  -->
  <xsl:template match="*">
    <xsl:element name="{local-name(.)}">
      <xsl:apply-templates/>
    </xsl:element>
  </xsl:template>
  <xsl:template match="@*">
    <xsl:copy/>
  </xsl:template>
  <!-- 
  =============================================================================
  Simple serialization of elements
  =============================================================================
  -->
  <xsl:template match="node()" mode="serialize">
    <xsl:param name="namespace"/>
    <xsl:variable name="namespaceDecl">
      <xsl:if test="$namespace != ''">
        <xsl:value-of select="concat(' xmlns=&quot;&quot;', $namespace, '&quot;&quot; xmlns:xsi=&quot;&quot;http://www.w3.org/2001/XMLSchema-instance&quot;&quot;')"/>
      </xsl:if>
    </xsl:variable>
    <xsl:variable name="attributes">
      <xsl:apply-templates select="@*" mode="serialize"/>
    </xsl:variable>
    <xsl:variable name="content">
      <xsl:apply-templates select="node()" mode="serialize"/>
    </xsl:variable>
    <xsl:value-of select="concat('&lt;', name(), $namespaceDecl, $attributes, '&gt;', $content, '&lt;/', name(), '&gt;')"/>
  </xsl:template>
  <!-- 
  =============================================================================
  -->
  <xsl:template match="@*" mode="serialize">
    <xsl:value-of select="concat(' ', name(), '=&quot;&quot;', ., '&quot;&quot;')"/>
  </xsl:template>
  <!-- 
  =============================================================================
  -->
  <xsl:template match="text()" mode="serialize">
    <xsl:value-of select="."/>
  </xsl:template>
  <!-- 
  =============================================================================
  -->
  <xsl:template match="comment()" mode="serialize">
  </xsl:template>
</xsl:stylesheet>
