<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
						xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
						xmlns:msxsl="urn:schemas-microsoft-com:xslt"
						exclude-result-prefixes="msxsl"
						xmlns="http://schemas.cobos.co.uk/datamodel/1.0.0"
						xmlns:cobos="http://schemas.cobos.co.uk/datamodel/1.0.0"
						xmlns:xsd="http://www.w3.org/2001/XMLSchema"
>
  <xsl:output method="xml" cdata-section-elements="cobos:SchemaType cobos:CodeType cobos:PropertyGet cobos:PropertySet cobos:ConvertTo"/>
  <xsl:include href="../Database/DatabaseVariables.def"/>
  <xsl:include href="../Utilities/Utilities.inc"/>
  <!-- 
	=============================================================================
	Filename: Process.xslt
	Description: Pre-processes a data model for further processing.
	=============================================================================
	Created by: N.Davis                        Date: 2010-06-16
	Modified by:                               Date:
	=============================================================================
	Notes: Expands the data model with inline definitions such as type names,
	       database type declarations, multiplicity etc.

	=============================================================================
	-->
  <!--
	=============================================================================
	Preprocess the data model
	=============================================================================
	-->
  <xsl:template match="/cobos:DataModel">
    <xsl:call-template name="generatedXmlWarning"/>
    <xsl:element name="DataModel">
      <xsl:apply-templates mode="copyAttributesAndNamespace" select="."/>
      <xsl:apply-templates select="cobos:Object|cobos:Interface|cobos:TableObject" mode="expand"/>
    </xsl:element>
  </xsl:template>
  <!--
	=============================================================================
	Expand the type definition, reference types are inlined into the nodeset
	and database attributes are copied to the properties.
	=============================================================================
	-->
  <!-- match a concrete type -->
  <xsl:template match="cobos:Object[not(@type)]" mode="expand">
    <xsl:element name="Object">
      <xsl:apply-templates mode="copyAttributesAndNamespace" select="."/>
      <xsl:if test="not(@dbTable)">
        <xsl:attribute name="dbTable">
          <xsl:apply-templates mode="getDbTable" select="."/>
        </xsl:attribute>
      </xsl:if>
      <xsl:apply-templates mode="expandName" select="."/>
      <xsl:apply-templates mode="expandDataset" select="."/>
      <xsl:apply-templates select="child::*[not(self::cobos:Metadata)]" mode="expand"/>
      <xsl:copy-of select="./cobos:Metadata"/>
    </xsl:element>
  </xsl:template>

  <!-- match an interface -->
  <xsl:template match="cobos:Interface" mode="expand">
    <xsl:element name="Interface">
      <xsl:apply-templates mode="copyAttributesAndNamespace" select="."/>
      <xsl:apply-templates mode="expandName" select="."/>
      <xsl:apply-templates select="child::*" mode="expand"/>
    </xsl:element>
  </xsl:template>

  <!-- match a reference type -->
  <xsl:template match="cobos:Object[@type]" mode="expand">
    <xsl:element name="Object">
      <xsl:apply-templates mode="copyAttributesAndNamespace" select="."/>
      <xsl:apply-templates mode="expandName" select="."/>
      <xsl:apply-templates mode="expandDataset" select="."/>
      <xsl:apply-templates select="//cobos:Type[@name = current()/@type]" mode="expand"/>
      <xsl:copy-of select="./cobos:Metadata"/>
    </xsl:element>
  </xsl:template>

  <!-- match a table type -->
  <xsl:template match="cobos:TableObject" mode="expand">
    <xsl:element name="Object">
      <xsl:apply-templates mode="copyAttributesAndNamespace" select="."/>
      <xsl:apply-templates mode="expandName" select="."/>
      <xsl:apply-templates mode="expandDataset" select="."/>
      <xsl:variable name="dbTable" select="@dbTable"/>
      <xsl:for-each select="$databaseTablesNodeSet/xsd:element[translate(@name, $lowercase, $uppercase) = translate(current()/@dbTable, $lowercase, $uppercase)]//xsd:element">
        <xsl:element name="Property">
          <xsl:apply-templates mode="tableObjectProperties" select="."/>
          <xsl:attribute name="dbTable">
            <xsl:value-of select="$dbTable"/>
          </xsl:attribute>
        </xsl:element>
      </xsl:for-each>
      <xsl:copy-of select="./cobos:Metadata"/>
    </xsl:element>
  </xsl:template>

  <!-- match a reference type -->
  <xsl:template match="cobos:Reference" mode="expand">
    <xsl:element name="Reference">
      <xsl:apply-templates mode="expandDataset" select="/cobos:DataModel/cobos:Object[@name = current()/@ref]"/>
      <xsl:copy-of select="@*"/>
      <xsl:attribute name="fieldName">
        <xsl:apply-templates mode="fieldName" select="."/>
      </xsl:attribute>
      <xsl:copy-of select="./cobos:Metadata"/>
    </xsl:element>
  </xsl:template>

  <!-- tag only the top level object with the derived DataRow id -->
  <xsl:template match="cobos:DataModel/cobos:TableObject|cobos:DataModel/cobos:Object" mode="expandDataset">
    <xsl:variable name="className">
      <xsl:apply-templates mode="className" select="."/>
    </xsl:variable>
    <xsl:variable name="datasetName">
      <xsl:apply-templates mode="className" select="/cobos:DataModel"/>
    </xsl:variable>
    <xsl:attribute name="dataModelType">
      <xsl:value-of select="$datasetName"/>
    </xsl:attribute>
    <xsl:attribute name="datasetRowType">
      <xsl:value-of select="$datasetName"/>.<xsl:value-of select="$className"/><xsl:text>Row</xsl:text>
    </xsl:attribute>
    <xsl:attribute name="datasetTableType">
      <xsl:value-of select="$datasetName"/>.<xsl:value-of select="$className"/><xsl:text>DataTable</xsl:text>
    </xsl:attribute>
    <xsl:attribute name="datasetTableName">
      <xsl:value-of select="$className"/>
    </xsl:attribute>
  </xsl:template>

  <xsl:template match="cobos:Object[not(parent::node()[self::cobos:DataModel])]" mode="expandDataset"/>

  <!-- Expand a type reference into the hierarchy -->
  <xsl:template match="cobos:Type" mode="expand">
    <xsl:attribute name="dbTable">
      <xsl:apply-templates mode="getDbTable" select="."/>
    </xsl:attribute>
    <xsl:apply-templates select="child::*" mode="expand"/>
  </xsl:template>

  <!-- Copy the property over including the database attributes -->
  <xsl:template match="cobos:Property" mode="expand">
    <xsl:element name="Property">
      <xsl:apply-templates mode="copyAttributesAndNamespace" select="."/>
      <xsl:apply-templates mode="expandName" select="."/>
      <xsl:variable name="dbTable">
        <xsl:apply-templates mode="getDbTable" select="."/>
      </xsl:variable>
      <xsl:if test="not(@dbTable)">
        <xsl:attribute name="dbTable">
          <xsl:value-of select="$dbTable"/>
        </xsl:attribute>
      </xsl:if>
      <xsl:apply-templates mode="expand" select="$databaseTablesNodeSet/xsd:element[translate(@name, $lowercase, $uppercase) = translate($dbTable, $lowercase, $uppercase)]
                           //xsd:element[translate(@name, $lowercase, $uppercase) = translate(current()/@dbColumn, $lowercase, $uppercase)]"/>
      <xsl:apply-templates mode="stringFormat" select="."/>
    </xsl:element>
  </xsl:template>

  <!-- apply the dbTable attribute from either this node or an ancestor that defines it -->
  <xsl:template match="cobos:Object|cobos:Type|cobos:Property" mode="getDbTable">
    <xsl:value-of select="ancestor-or-self::*[self::cobos:Object|self::cobos:Interface|self::cobos:Type|self::cobos:Property][@dbTable][1]/@dbTable"/>
  </xsl:template>

  <!-- Copy the database type and mulitplicity to the expanded property -->
  <xsl:template match="xsd:element" mode="expand">
    <xsl:attribute name="dbType">
      <xsl:value-of select="@type"/>
    </xsl:attribute>
    <xsl:copy-of select="@minOccurs"/>
  </xsl:template>

  <!-- Create a property from the database type for TableObject types -->
  <xsl:template match="xsd:element" mode="tableObjectProperties">
    <xsl:attribute name="name">
      <xsl:call-template name="capsUnderscoreToTypeName">
        <xsl:with-param name="tokens" select="@name"/>
      </xsl:call-template>
    </xsl:attribute>
    <xsl:attribute name="dbColumn">
      <xsl:value-of select="@name"/>
    </xsl:attribute>
    <xsl:attribute name="dbType">
      <xsl:value-of select="@type"/>
    </xsl:attribute>
    <xsl:copy-of select="@minOccurs"/>
  </xsl:template>

  <xsl:template match="cobos:Property[@stringFormat]" mode="stringFormat">
    <xsl:if test="not(/cobos:DataModel/cobos:StringFormats/cobos:StringFormat[@name = current()/@stringFormat])">
      <xsl:value-of select="concat('Error: no string format found for ', @stringFormat)"/>
    </xsl:if>
    <xsl:copy-of select="/cobos:DataModel/cobos:StringFormats/cobos:StringFormat[@name = current()/@stringFormat]"/>
  </xsl:template>

  <xsl:template match="cobos:Property[not(@stringFormat)]" mode="stringFormat">
  </xsl:template>

  <!--
	=============================================================================
	Class naming conventions
	=============================================================================
	-->

  <!-- class name for the datamodel -->
  <xsl:template match="cobos:DataModel|cobos:TableObject|cobos:Interface" mode="expandName">
    <xsl:attribute name="className">
      <xsl:apply-templates mode="className" select="."/>
    </xsl:attribute>
    <xsl:attribute name="typeName">
      <xsl:apply-templates mode="typeName" select="."/>
    </xsl:attribute>
    <xsl:attribute name="fieldName">
      <xsl:apply-templates mode="fieldName" select="."/>
    </xsl:attribute>
  </xsl:template>

  <!-- class name for concrete object type -->
  <xsl:template match="cobos:Object[parent::cobos:DataModel]" mode="expandName">
    <xsl:attribute name="className">
      <xsl:apply-templates mode="className" select="."/>
    </xsl:attribute>
    <xsl:attribute name="typeName">
      <xsl:apply-templates mode="typeName" select="."/>
    </xsl:attribute>
    <xsl:attribute name="fieldName">
      <xsl:apply-templates mode="fieldName" select="."/>
    </xsl:attribute>
  </xsl:template>

  <!-- class name for a type reference -->
  <xsl:template match="cobos:Object[@type and not(parent::cobos:DataModel)]" mode="expandName">
    <xsl:attribute name="className">
      <xsl:apply-templates mode="className" select="."/>
    </xsl:attribute>
    <xsl:attribute name="typeName">
      <xsl:apply-templates mode="typeName" select="."/>
    </xsl:attribute>
    <xsl:attribute name="fieldName">
      <xsl:apply-templates mode="fieldName" select="."/>
    </xsl:attribute>
    <xsl:attribute name="fullName">
      <xsl:apply-templates mode="fullName" select="."/>
    </xsl:attribute>
    <xsl:attribute name="navigationProperty">
      <xsl:apply-templates mode="navigationProperty" select="."/>
    </xsl:attribute>
  </xsl:template>

  <!-- class name for an anonymous nested type, make it up based on the parent name -->
  <xsl:template match="cobos:Object[not(@type) and not(parent::cobos:DataModel)]" mode="expandName">
    <xsl:attribute name="className">
      <xsl:apply-templates mode="className" select="."/>
    </xsl:attribute>
    <xsl:attribute name="typeName">
      <xsl:apply-templates mode="typeName" select="."/>
    </xsl:attribute>
    <xsl:attribute name="fieldName">
      <xsl:apply-templates mode="fieldName" select="."/>
    </xsl:attribute>
    <xsl:attribute name="fullName">
      <xsl:apply-templates mode="fullName" select="."/>
    </xsl:attribute>
    <xsl:attribute name="navigationProperty">
      <xsl:apply-templates mode="navigationProperty" select="."/>
    </xsl:attribute>
  </xsl:template>

  <!-- property name -->
  <xsl:template match="cobos:Property" mode="expandName">
    <xsl:attribute name="fieldName">
      <xsl:apply-templates mode="fieldName" select="."/>
    </xsl:attribute>
    <xsl:attribute name="fullName">
      <xsl:apply-templates select="ancestor::*[self::cobos:Object|self::cobos:Interface]" mode="fullNameForNestedType"/>
      <xsl:call-template name="titleCaseName">
        <xsl:with-param name="name">
          <xsl:value-of select="@name"/>
        </xsl:with-param>
      </xsl:call-template>
    </xsl:attribute>
  </xsl:template>

</xsl:stylesheet>
