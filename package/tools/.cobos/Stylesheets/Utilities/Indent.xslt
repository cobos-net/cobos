<xsl:stylesheet version="1.0"
						xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
						xmlns:msxsl="urn:schemas-microsoft-com:xslt"
						exclude-result-prefixes="msxsl"
						xmlns="http://schemas.cobos.co.uk/datamodel/1.0.0"
						xmlns:cobos="http://schemas.cobos.co.uk/datamodel/1.0.0"
						xmlns:doc="http://schemas.cobos.co.uk/documentation/1.0.0"
						xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <!-- 
  =============================================================================
  Gets the indent for the Object or Property element.
  =============================================================================
  -->
  <xsl:template match="cobos:Object|cobos:Property|cobos:Reference|cobos:DataModel" mode="newlineIndent">
    <xsl:variable name="indent">
      <xsl:apply-templates select="parent::cobos:Object|parent::cobos:DataModel" mode="indent"/>
    </xsl:variable>
    <xsl:value-of select="concat($newline, $indent)"/>
  </xsl:template>
  <!-- 
  =============================================================================
  Gets the indent plus 1 level for the Object or Property element.
  =============================================================================
  -->
  <xsl:template match="cobos:Object|cobos:Property|cobos:Reference|cobos:DataModel" mode="newlineIndentLevel1">
    <xsl:variable name="indent">
      <xsl:apply-templates select="parent::cobos:Object|parent::cobos:DataModel" mode="indent"/>
    </xsl:variable>
    <xsl:value-of select="concat($newline, $indent, $space)"/>
  </xsl:template>
  <!-- 
  =============================================================================
  Gets the indent plus 2 levels for the Object or Property element.
  =============================================================================
  -->
  <xsl:template match="cobos:Object|cobos:Property|cobos:Reference|cobos:DataModel" mode="newlineIndentLevel2">
    <xsl:variable name="indent">
      <xsl:apply-templates select="parent::cobos:Object|parent::cobos:DataModel" mode="indent"/>
    </xsl:variable>
    <xsl:value-of select="concat($newline, $indent, $space, $space)"/>
  </xsl:template>
  <!-- 
  =============================================================================
  Gets the indent plus 3 levels for the Object or Property element.
  =============================================================================
  -->
  <xsl:template match="cobos:Object|cobos:Property|cobos:Reference|cobos:DataModel" mode="newlineIndentLevel3">
    <xsl:variable name="indent">
      <xsl:apply-templates select="parent::cobos:Object|parent::cobos:DataModel" mode="indent"/>
    </xsl:variable>
    <xsl:value-of select="concat($newline, $indent, $space, $space, $space)"/>
  </xsl:template>
  <!-- 
  =============================================================================
  Gets the indent for the Object or Property element (recursive).
  =============================================================================
  -->
  <xsl:template match="cobos:Object|cobos:DataModel" mode="indent">
    <xsl:variable name="indent">
      <xsl:apply-templates select="parent::cobos:Object|parent::cobos:DataModel" mode="indent"/>
    </xsl:variable>
    <xsl:value-of select="concat($indent, $space)"/>
  </xsl:template>
</xsl:stylesheet>