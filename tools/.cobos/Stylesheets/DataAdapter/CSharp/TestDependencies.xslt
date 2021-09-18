<xsl:stylesheet version="1.0"
						xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
						xmlns:msxsl="urn:schemas-microsoft-com:xslt"
						exclude-result-prefixes="msxsl"
						xmlns="http://schemas.cobos.co.uk/datamodel/1.0.0"
						xmlns:cobos="http://schemas.cobos.co.uk/datamodel/1.0.0"
						xmlns:doc="http://schemas.cobos.co.uk/documentation/1.0.0"
						xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <xsl:output method="text" omit-xml-declaration="yes"/>
  <xsl:strip-space elements="*"/>
  <!-- 
  =============================================================================
  Stylesheet parameters.
  =============================================================================
  -->
  <xsl:param name="codeNamespace"/>
  <xsl:param name="xmlNamespace"/>
  <!-- 
  =============================================================================
  Stylesheet includes.
  =============================================================================
  -->
  <xsl:include href="../../Utilities/Utilities.inc"/>
  <!-- 
  =============================================================================
  Code namespace definition.
  =============================================================================
  -->
  <xsl:template match="/cobos:DataModel">
    <xsl:call-template name="generatedCSharpWarning"/>
    <xsl:value-of select="concat('namespace ', $codeNamespace)"/>
    <xsl:value-of select="concat($newline, '{')"/>
    <xsl:apply-templates select="." mode="dataAdapter"/>
    <xsl:value-of select="concat($newline, '}')"/>
  </xsl:template>
  <!-- 
  =============================================================================
  Data Model adapter definition.
  =============================================================================
  -->
  <xsl:template match="cobos:DataModel" mode="dataAdapter">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndentLevel2"/>
    </xsl:variable>
    <xsl:variable name="objectDependencies">
      <xsl:apply-templates select="." mode="objectDependencies"/>
    </xsl:variable>
    <xsl:apply-templates select="msxsl:node-set($objectDependencies)/cobos:Object" mode="acceptChanges"/>
  </xsl:template>
  <!-- ==================================================================== -->
  <xsl:template match="cobos:Object" mode="acceptChanges">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndentLevel3"/>
    </xsl:variable>
    <xsl:if test="not(position() = 1)">
      <xsl:value-of select="$indent"/>
    </xsl:if>
    <xsl:value-of select="concat($indent, '    this.', @className, '.AcceptChanges();')"/>
  </xsl:template>
  <!-- 
  =============================================================================
  Find references and order by dependency.
  =============================================================================
  -->
  <xsl:template match="cobos:DataModel" mode="objectDependencies">
    <xsl:variable name="objectDependencies">
      <xsl:apply-templates select="cobos:Object" mode="objectDependencies"/>
    </xsl:variable>
    <!--<xsl:copy-of select="msxsl:node-set($objectDependencies)/cobos:Object[not(@className = preceding::cobos:Object/@className)]"/>-->
    <xsl:copy-of select="msxsl:node-set($objectDependencies)/cobos:Object[not(@className = preceding::cobos:Object/@className)]"/>
  </xsl:template>
  <!-- ==================================================================== -->
  <xsl:template match="cobos:Object" mode="objectDependencies">
    <xsl:apply-templates select="/cobos:DataModel/cobos:Object[cobos:Reference/@ref = current()/@name]" mode="objectDependencies"/>
    <xsl:apply-templates select="." mode="copyObjectDependency"/>
  </xsl:template>
  <!-- ==================================================================== -->
  <xsl:template match="cobos:Object" mode="copyObjectDependency">
    <xsl:copy-of select="."/>
  </xsl:template>
</xsl:stylesheet>