<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt"
    xmlns:wix="http://schemas.microsoft.com/wix/2006/wi"
    xmlns:util="http://schemas.microsoft.com/wix/UtilExtension"
    xmlns:i="http://www.cobos.co.uk/build/ignore"
    exclude-result-prefixes="msxsl wix i"
>
  <!--
	============================================================================
	Filename: CodegenFiles.xslt
	Description: Custom ignore defintions.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Created by: N.Davis             Date: 2012-09-04
	Updated by:                     Date:
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Notes: 
	
	

	============================================================================
	-->

  <!-- Include the base stylesheet containing all of the processing rules -->
  <xsl:include href="\QualitySystem\BUILD\INSTALL\Stylesheets\heat-ignore-default.xslt"/>
  <xsl:include href="\QualitySystem\BUILD\INSTALL\Stylesheets\heat-install-xml.xslt"/>

  <!-- Set the xsl:import href path -->
  <xsl:template name="setImportPath">
    <xsl:param name="stylesheetPath"/>
    <xsl:call-template name="wixXmlAttribute">
      <xsl:with-param name="id">1</xsl:with-param>
      <xsl:with-param name="action">setValue</xsl:with-param>
      <xsl:with-param name="elementPath">/*[\[]local-name() = 'stylesheet'[\]]/*[\[]local-name() = 'import'[\]]</xsl:with-param>
      <xsl:with-param name="attributeName">href</xsl:with-param>
      <xsl:with-param name="value">[INSTALLDIR]CODEGEN\Stylesheets\<xsl:value-of select="$stylesheetPath"/></xsl:with-param>
      <xsl:with-param name="sequence">1</xsl:with-param>
    </xsl:call-template>
  </xsl:template>

  <!-- DataModelAdapter.xslt -->
  <xsl:template match="wix:File[@Source='$(var.CodegenFiles)\Examples\DataModelAdapter.xslt']">
    <xsl:call-template name="setImportPath">
      <xsl:with-param name="stylesheetPath">DataAdapter\CSharp\DataModelAdapter.xslt</xsl:with-param>
    </xsl:call-template>
  </xsl:template>

  <!-- DataObject.xslt -->
  <xsl:template match="wix:File[@Source='$(var.CodegenFiles)\Examples\DataObject.xslt']">
    <xsl:call-template name="setImportPath">
      <xsl:with-param name="stylesheetPath">DataObject\CSharp\DataObject.xslt</xsl:with-param>
    </xsl:call-template>
  </xsl:template>

  <!-- DataObjectAdapter.xslt -->
  <xsl:template match="wix:File[@Source='$(var.CodegenFiles)\Examples\DataObjectAdapter.xslt']">
    <xsl:call-template name="setImportPath">
      <xsl:with-param name="stylesheetPath">DataAdapter\CSharp\DataObjectAdapter.xslt</xsl:with-param>
    </xsl:call-template>
  </xsl:template>

  <!-- DataSet.xslt -->
  <xsl:template match="wix:File[@Source='$(var.CodegenFiles)\Examples\DataSet.xslt']">
    <xsl:call-template name="setImportPath">
      <xsl:with-param name="stylesheetPath">DataModel\Dataset.xslt</xsl:with-param>
    </xsl:call-template>
  </xsl:template>

  <!-- Process.xslt -->
  <xsl:template match="wix:File[@Source='$(var.CodegenFiles)\Examples\Process.xslt']">
    <xsl:call-template name="setImportPath">
      <xsl:with-param name="stylesheetPath">DataModel\Process.xslt</xsl:with-param>
    </xsl:call-template>
  </xsl:template>

  <!-- Schema.xslt -->
  <xsl:template match="wix:File[@Source='$(var.CodegenFiles)\Examples\Schema.xslt']">
    <xsl:call-template name="setImportPath">
      <xsl:with-param name="stylesheetPath">DataModel\Schema.xslt</xsl:with-param>
    </xsl:call-template>
  </xsl:template>

</xsl:stylesheet>
