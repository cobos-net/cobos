<xsl:stylesheet version="1.0"
            xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
            xmlns:msxsl="urn:schemas-microsoft-com:xslt"
            exclude-result-prefixes="msxsl"
            xmlns="http://schemas.cobos.co.uk/datamodel/1.0.0"
            xmlns:cobos="http://schemas.cobos.co.uk/datamodel/1.0.0"
            xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <!-- 
  =============================================================================
  Filename: Names.xslt
  Description: Data Object naming conventions
  =============================================================================
  Created by: N.Davis                        Date: 2010-04-09
  Modified by:                               Date:
  =============================================================================

  
  =============================================================================
  -->
  <!--
  =============================================================================
  Class name for the datamodel.
  =============================================================================
  -->
  <!--  -->
  <xsl:template match="cobos:DataModel|cobos:TableObject|cobos:Interface" mode="className">
    <xsl:call-template name="tokensToClassName">
      <xsl:with-param name="tokens" select="@name"/>
    </xsl:call-template>
  </xsl:template>
  <!--
  =============================================================================
  Class name for concrete object type.
  =============================================================================
  -->
  <xsl:template match="cobos:Object[parent::cobos:DataModel]" mode="className">
    <xsl:call-template name="tokensToClassName">
      <xsl:with-param name="tokens" select="@name"/>
    </xsl:call-template>
  </xsl:template>
  <!--
  =============================================================================
  Class name for a type reference.
  =============================================================================
  -->
  <xsl:template match="cobos:Object[@type][not(parent::cobos:DataModel)]" mode="className">
    <xsl:call-template name="tokensToClassName">
      <xsl:with-param name="tokens" select="@type"/>
    </xsl:call-template>
  </xsl:template>
  <!--
  =============================================================================
  Class name for an anonymous nested type, based on the parent name.
  =============================================================================
  -->
  <xsl:template match="cobos:Object[not(@type)][not(parent::cobos:DataModel)]" mode="className">
    <xsl:call-template name="tokensToClassName">
      <xsl:with-param name="tokens" select="concat(../@name, ' ', @name)"/>
    </xsl:call-template>
  </xsl:template>
  <!--
  =============================================================================
  Property name for a DB column.
  =============================================================================
  -->
  <xsl:template match="cobos:Property[not(@dbAlias)]" mode="dbColumn">
    <xsl:value-of select="@dbColumn"/>
  </xsl:template>
  <!--
  =============================================================================
  Property name for a DB column alias.
  =============================================================================
  -->
  <xsl:template match="cobos:Property[@dbAlias]" mode="dbColumn">
    <xsl:value-of select="@dbAlias"/>
  </xsl:template>
  <!--
  =============================================================================
  =============================================================================
  Class names - qualified names.
  =============================================================================
  =============================================================================
  -->
  <!--
  =============================================================================
  Qualified class name - valid identifier name.
  =============================================================================
  -->
  <xsl:template match="cobos:Object" mode="qualifiedName">
    <xsl:apply-templates select="ancestor::*[self::cobos:Object | self::cobos:Interface]" mode="qualifiedNameForClass"/>
    <xsl:call-template name="titleCaseName">
      <xsl:with-param name="name">
        <xsl:value-of select="@name"/>
      </xsl:with-param>
    </xsl:call-template>
  </xsl:template>
  <!--
  =============================================================================
  Fully qualified class name.
  =============================================================================
  -->
  <xsl:template match="cobos:Object|cobos:Interface" mode="qualifiedNameForClass">
    <xsl:call-template name="titleCaseName">
      <xsl:with-param name="name">
        <xsl:value-of select="@name"/>
      </xsl:with-param>
    </xsl:call-template>
    <xsl:text>.</xsl:text>
  </xsl:template>
  <!--
  =============================================================================
  =============================================================================
  Type names - for objects and types
  =============================================================================
  =============================================================================
  -->
  <!--
  =============================================================================
  Gets the type name for a top level data model element.
  =============================================================================
  -->
  <xsl:template match="cobos:DataModel|cobos:TableObject|cobos:Interface|cobos:Object[parent::cobos:DataModel]" mode="typeName">
    <xsl:value-of select="@name"/>
  </xsl:template>
  <!--
  =============================================================================
  Gets the type name for a nested object.
  =============================================================================
  -->
  <xsl:template match="cobos:Object[not(@type) and not(parent::cobos:DataModel)]" mode="typeName">
    <xsl:value-of select="concat(../@name, @name)"/>
  </xsl:template>
  <!--
  =============================================================================
  Gets the type name for a nested object that is a type reference.
  =============================================================================
  -->
  <xsl:template match="cobos:Object[@type and not(parent::cobos:DataModel)]" mode="typeName">
    <xsl:value-of select="concat(parent::cobos:Object/@name, @name)"/>
  </xsl:template>
  <!--
  =============================================================================
  =============================================================================
  Member variable names - for all objects, types and properties
  =============================================================================
  =============================================================================
  -->
  <!--
  =============================================================================
  Gets the memeber name for any element.
  =============================================================================
  -->
  <xsl:template match="cobos:DataModel|cobos:TableObject|cobos:Interface|cobos:Object|cobos:Reference|cobos:Property" mode="memberName">
    <xsl:value-of select="concat(translate(substring(@name, 1, 1), $uppercase, $lowercase), substring(@name, 2), 'Field')" />
  </xsl:template>
  <!--
  =============================================================================
  =============================================================================
  Qualified type names.
  =============================================================================
  =============================================================================
  -->
  <!--
  =============================================================================
  Gets the qualified type name for an Object element.
  =============================================================================
  -->
  <xsl:template match="cobos:Object" mode="qualifiedTypeName">
    <xsl:apply-templates select="ancestor::*[self::cobos:Object | self::cobos:Interface]" mode="qualifiedTypeNameForClass"/>
    <xsl:call-template name="titleCaseName">
      <xsl:with-param name="name">
        <xsl:apply-templates select="." mode="typeName"/>
      </xsl:with-param>
    </xsl:call-template>
  </xsl:template>
  <!--
  =============================================================================
  Gets the fully qualified type name for an Object or Interface element.
  =============================================================================
  -->
  <xsl:template match="cobos:Object|cobos:Interface" mode="qualifiedTypeNameForClass">
    <xsl:call-template name="titleCaseName">
      <xsl:with-param name="name">
        <xsl:apply-templates select="." mode="typeName"/>
      </xsl:with-param>
    </xsl:call-template>
    <xsl:text>.</xsl:text>
  </xsl:template>
  <!--
  =============================================================================
  =============================================================================
  Property names - nested types are flattened in our DB model
  =============================================================================
  =============================================================================
  -->
  <!--
  =============================================================================
  Qualified name for a scalar property type.
  =============================================================================
  -->
  <xsl:template match="cobos:Property" mode="qualifiedName">
    <xsl:apply-templates select="ancestor::cobos:Object[not(position() = last())]" mode="qualifiedNameForProperty"/>
    <xsl:call-template name="titleCaseName">
      <xsl:with-param name="name">
        <xsl:value-of select="@name"/>
      </xsl:with-param>
    </xsl:call-template>
  </xsl:template>
  <!--
  =============================================================================
  Qualified name for an object reference property type.
  =============================================================================
  -->
  <xsl:template match="cobos:Object" mode="qualifiedNameForProperty">
    <xsl:call-template name="titleCaseName">
      <xsl:with-param name="name">
        <xsl:value-of select="@name"/>
      </xsl:with-param>
    </xsl:call-template>
  </xsl:template>
  <!--
  =============================================================================
  DataSet name - Row type
  =============================================================================
  -->
  <xsl:template match="cobos:Object" mode="datasetRowType">
    <xsl:value-of select="ancestor-or-self::cobos:Object[@datasetRowType]/@datasetRowType"/>
  </xsl:template>
  <!--
  =============================================================================
  DataSet name - Table type
  =============================================================================
  -->
  <xsl:template match="cobos:Object" mode="datasetTableType">
    <xsl:value-of select="ancestor-or-self::cobos:Object[@datasetTableType]/@datasetTableType"/>
  </xsl:template>
</xsl:stylesheet>
