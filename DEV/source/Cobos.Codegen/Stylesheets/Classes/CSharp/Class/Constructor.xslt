<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
					 xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
					 xmlns:msxsl="urn:schemas-microsoft-com:xslt"
					 xmlns:cobos="http://schemas.cobos.co.uk/datamodel/1.0.0"
					 xmlns="http://schemas.cobos.co.uk/datamodel/1.0.0"
					 xmlns:xsd="http://www.w3.org/2001/XMLSchema"
					 exclude-result-prefixes="msxsl">
  <!-- 
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Top level classes.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->
  <xsl:template match="cobos:DataModel/cobos:Object" mode="classConstructorDeclaration">

        <![CDATA[/// <summary>
        /// Initializes a new instance of the <see cref="]]><xsl:value-of select="@typeName"/><![CDATA["/> class.
        /// </summary>
        /// <param name="dataRow">The data row for the instance.</param>]]>
        public <xsl:value-of select="@typeName"/>(<xsl:value-of select="@datasetRowType"/> dataRow)
        {
    <xsl:text>        this.ObjectDataRow = dataRow;</xsl:text>
    <xsl:apply-templates select="cobos:Object" mode="classConstructorBody"/>
        }
  </xsl:template>
  <!-- 
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Nested classes.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->
  <xsl:template match="cobos:Object/cobos:Object[not(ancestor::cobos:Interface)]" mode="classConstructorDeclaration">
    <xsl:variable name="datasetRowType">
      <xsl:apply-templates select="." mode="datasetRowType"/>
    </xsl:variable>
        
        <![CDATA[/// <summary>
        /// Initializes a new instance of the <see cref="]]><xsl:value-of select="@typeName"/><![CDATA["/> class.
        /// </summary>
        /// <param name="dataRow">The data row for the instance.</param>]]>
        public <xsl:value-of select="@typeName"/>(<xsl:value-of select="$datasetRowType"/> dataRow)
        {
    <xsl:text>        this.ObjectDataRow = dataRow;</xsl:text>
    <xsl:apply-templates select="cobos:Object" mode="classConstructorBody"/>
        }
  </xsl:template>
  <!-- 
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Construct the nested objects.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->
  <xsl:template match="cobos:Object" mode="classConstructorBody">
    <xsl:value-of select="concat($newlineIndent3, 'this.', @memberName, ' = new ', @typeName, '(this.ObjectDataRow);')"/>
  </xsl:template>
</xsl:stylesheet>