<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
						xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
						xmlns:msxsl="urn:schemas-microsoft-com:xslt"
						exclude-result-prefixes="msxsl"
						xmlns="http://schemas.intergraph.com/asiapac/cad/datamodel/1.0.0"
						xmlns:cad="http://schemas.intergraph.com/asiapac/cad/datamodel/1.0.0"
						xmlns:xsd="http://www.w3.org/2001/XMLSchema"
>

	<!-- 
	=============================================================================
	Filename: datamodelexpand.xslt
	Description: XSLT for common data model functionality
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Created by: N.Davis                        Date: 2010-06-16
	Modified by:                               Date:
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Notes: In the datamodel, each top level object refers to a single 
	table in the stronly typed dataset.  Nested object hierarchies with references
	are expanded so in order to correctly generated classes and dataset schemas.

	============================================================================
	-->

	<xsl:include href="DataModelCommon.xslt"/>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Process the data model into CSharp classes
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="/cad:DataModel">
		<xsl:call-template name="generatedXmlWarning"/>
		<xsl:element name="DataModel">
			<xsl:apply-templates mode="copyAttributesAndNamespace" select="."/>
			<xsl:apply-templates select="cad:Object|cad:TableObject" mode="classHierarchy"/>
		</xsl:element>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Expand the class hierarchy, reference types are inlined into the nodeset
	and database attributes are copied to the properties.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<!-- match a concrete type -->
	<xsl:template match="cad:Object[ not( @type ) ]" mode="classHierarchy">
		<xsl:element name="Object">
			<xsl:apply-templates mode="copyAttributesAndNamespace" select="."/>
			<xsl:if test="not( @dbTable )">
				<xsl:attribute name="dbTable">
					<xsl:apply-templates mode="getDbTable" select="."/>
				</xsl:attribute>
			</xsl:if>
			<xsl:apply-templates mode="classHierarchyClassName" select="."/>
			<xsl:apply-templates mode="classHierarchyDatasetDefinition" select="."/>
			<xsl:apply-templates select="child::*" mode="classHierarchy"/>
			<xsl:copy-of select="./cad:Metadata"/>
		</xsl:element>
	</xsl:template>

	<!-- match a reference type -->
	<xsl:template match="cad:Object[ @type ]" mode="classHierarchy">
		<xsl:element name="Object">
			<xsl:apply-templates mode="copyAttributesAndNamespace" select="."/>
			<xsl:apply-templates mode="classHierarchyClassName" select="."/>
			<xsl:apply-templates mode="classHierarchyDatasetDefinition" select="."/>
			<xsl:apply-templates select="//cad:Type[ @name = current()/@type ]" mode="classHierarchy"/>
			<xsl:copy-of select="./cad:Metadata"/>
		</xsl:element>
	</xsl:template>

	<!-- match a table type -->
	<xsl:template match="cad:TableObject" mode="classHierarchy">
		<xsl:element name="Object">
			<xsl:apply-templates mode="copyAttributesAndNamespace" select="."/>
			<xsl:apply-templates mode="classHierarchyClassName" select="."/>
			<xsl:apply-templates mode="classHierarchyDatasetDefinition" select="."/>
			<xsl:variable name="dbTable" select="@dbTable"/>
			<xsl:for-each select="$databaseTablesNodeSet/xsd:element[ @name = translate( current()/@dbTable, $lowercase, $uppercase ) ]//xsd:element">
				<xsl:element name="Property">
					<xsl:apply-templates mode="tableObjectProperties" select="."/>
					<xsl:attribute name="dbTable">
						<xsl:value-of select="$dbTable"/>
					</xsl:attribute>
				</xsl:element>
			</xsl:for-each>
			<xsl:copy-of select="./cad:Metadata"/>
		</xsl:element>
	</xsl:template>

	<!-- match a reference type -->
	<xsl:template match="cad:Reference" mode="classHierarchy">
		<xsl:element name="Reference">
			<xsl:apply-templates mode="classHierarchyDatasetDefinition" select="/cad:DataModel/cad:Object[ @name = current()/@ref ]"/>
			<xsl:copy-of select="@*"/>
			<xsl:copy-of select="./cad:Metadata"/>
		</xsl:element>
	</xsl:template>

	<!-- tag only the top level object with the derived DataRow id -->
	<xsl:template match="cad:TableObject|cad:Object[ parent::node()[ self::cad:DataModel ] ]" mode="classHierarchyDatasetDefinition">
		<xsl:variable name="className">
			<xsl:apply-templates mode="className" select="."/>
		</xsl:variable>
		<xsl:variable name="datasetName">
			<xsl:apply-templates mode="className" select="/cad:DataModel"/>
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

	<xsl:template match="cad:Object[ not(parent::node()[ self::cad:DataModel ]) ]" mode="classHierarchyDatasetDefinition"/>

	<!-- Expand a type reference into the hierarchy -->
	<xsl:template match="cad:Type" mode="classHierarchy">
		<xsl:attribute name="dbTable">
			<xsl:apply-templates mode="getDbTable" select="."/>
		</xsl:attribute>
		<xsl:apply-templates select="child::*" mode="classHierarchy"/>
	</xsl:template>

	<!-- Copy the property over including the database attributes -->
	<xsl:template match="cad:Property" mode="classHierarchy">
		<xsl:element name="Property">
			<xsl:apply-templates mode="copyAttributesAndNamespace" select="."/>
			<xsl:variable name="dbTable">
				<xsl:apply-templates mode="getDbTable" select="."/>
			</xsl:variable>
			<xsl:if test="not( @dbTable )">
				<xsl:attribute name="dbTable">
					<xsl:value-of select="$dbTable"/>
				</xsl:attribute>
			</xsl:if>
			<xsl:apply-templates mode="classHierarchy" select="$databaseTablesNodeSet/xsd:element[ @name = translate( $dbTable, $lowercase, $uppercase ) ]
																				//xsd:element[ @name = translate( current()/@dbColumn, $lowercase, $uppercase ) ]"/>
		</xsl:element>
	</xsl:template>

	<!-- apply the dbTable attribute from either this node or an ancestor that defines it -->
	<xsl:template match="cad:Object|cad:Type|cad:Property" mode="getDbTable">
		<xsl:value-of select="ancestor-or-self::*[ self::cad:Object | self::cad:Type | self::cad:Property ][ @dbTable ][ 1 ]/@dbTable"/>
	</xsl:template>

	<!-- Copy the database type and mulitplicity to the expanded property -->
	<xsl:template match="xsd:element" mode="classHierarchy">
		<xsl:attribute name="dbType">
			<xsl:value-of select="@type"/>
		</xsl:attribute>
		<xsl:copy-of select="@minOccurs"/>
	</xsl:template>

	<!-- Create a property from the database type for TableObject types -->
	<xsl:template match="xsd:element" mode="tableObjectProperties">
		<xsl:attribute name="name">
			<xsl:call-template name="capsUnderscoreToClassName">
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

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Class naming conventions
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<!-- class name for the datamodel -->
	<xsl:template match="cad:DataModel|cad:TableObject" mode="classHierarchyClassName">
		<xsl:attribute name="className">
			<xsl:apply-templates mode="className" select="."/>
		</xsl:attribute>
	</xsl:template>

	<!-- class name for concrete object type -->
	<xsl:template match="cad:Object[ parent::node()[ self::cad:DataModel ] ]" mode="classHierarchyClassName">
		<xsl:attribute name="className">
			<xsl:apply-templates mode="className" select="."/>
		</xsl:attribute>
	</xsl:template>

	<!-- class name for a type reference -->
	<xsl:template match="cad:Object[ @type ][ not(parent::node()[ self::cad:DataModel ]) ]" mode="classHierarchyClassName">
		<xsl:attribute name="className">
			<xsl:apply-templates mode="className" select="."/>
		</xsl:attribute>
	</xsl:template>

	<!-- class name for an anonymous nested type, make it up based on the parent name -->
	<xsl:template match="cad:Object[ not( @type ) ][ not(parent::node()[ self::cad:DataModel ]) ]" mode="classHierarchyClassName">
		<xsl:attribute name="className">
			<xsl:apply-templates mode="className" select="."/>
		</xsl:attribute>
	</xsl:template>

</xsl:stylesheet>
