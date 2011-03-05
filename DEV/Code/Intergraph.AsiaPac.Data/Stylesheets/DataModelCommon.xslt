<xsl:stylesheet version="1.0"
						xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
						xmlns:msxsl="urn:schemas-microsoft-com:xslt"
						exclude-result-prefixes="msxsl"
						xmlns="http://schemas.intergraph.com/asiapac/cad/datamodel"
						xmlns:cad="http://schemas.intergraph.com/asiapac/cad/datamodel"
						xmlns:xsd="http://www.w3.org/2001/XMLSchema"
>
	<!-- 
	=============================================================================
	Filename: datamodelcommon.xslt
	Description: XSLT for common data model functionality
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Created by: N.Davis                        Date: 2010-04-09
	Modified by:                               Date:
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Notes: In the datamodel, each top level object refers to a single 
	table in the stronly typed dataset.  Nested object hierarchies with references
	are expanded so in order to correctly generated classes and dataset schemas.

	============================================================================
	-->

	<!-- include the generated database schema variables -->
	<xsl:include href="CadDatabase.xslt"/>
	<xsl:include href="Utilities.xslt"/>

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
			<xsl:apply-templates mode="classHierarchyClassName" select="."/>
			<xsl:apply-templates mode="classHierarchyDatasetDefinition" select="."/>
			<xsl:apply-templates select="child::*" mode="classHierarchy"/>
		</xsl:element>
	</xsl:template>

	<!-- match a reference type -->
	<xsl:template match="cad:Object[ @type ]" mode="classHierarchy">
		<xsl:element name="Object">
			<xsl:apply-templates mode="copyAttributesAndNamespace" select="."/>
			<xsl:apply-templates mode="classHierarchyClassName" select="."/>
			<xsl:apply-templates mode="classHierarchyDatasetDefinition" select="."/>
			<xsl:apply-templates select="//cad:Type[ @name = current()/@type ]" mode="classHierarchy"/>
		</xsl:element>
	</xsl:template>

	<!-- tag only the top level object with the derived DataRow id -->
	<xsl:template match="cad:Object[ parent::node()[ self::cad:DataModel ] ]" mode="classHierarchyDatasetDefinition">
		<xsl:variable name="className">
			<xsl:apply-templates mode="className" select="."/>
		</xsl:variable>
		<xsl:variable name="datasetName">
			<xsl:apply-templates mode="className" select="/cad:DataModel"/>
		</xsl:variable>
		<xsl:attribute name="datasetRowType">
			<xsl:value-of select="$datasetName"/>.<xsl:value-of select="$className"/><xsl:text>Row</xsl:text>
		</xsl:attribute>
		<xsl:attribute name="datasetTableType">
			<xsl:value-of select="$datasetName"/>.<xsl:value-of select="$className"/><xsl:text>DataTable</xsl:text>
		</xsl:attribute>		
	</xsl:template>

	<xsl:template match="cad:Object[ not(parent::node()[ self::cad:DataModel ]) ]" mode="classHierarchyDatasetDefinition"/>

	<!-- Expand a type reference into the hierarchy -->
	<xsl:template match="cad:Type" mode="classHierarchy">
		<xsl:attribute name="dbTable">
			<xsl:value-of select="@dbTable"/>
		</xsl:attribute>

		<xsl:apply-templates select="child::*" mode="classHierarchy"/>
	</xsl:template>

	<!-- Copy the property over including the database attributes -->
	<xsl:template match="cad:Property" mode="classHierarchy">
		<xsl:element name="Property">
			<xsl:apply-templates mode="copyAttributesAndNamespace" select="."/>
			<xsl:apply-templates mode="classHierarchy" select="$databaseTablesNodeSet/xsd:element[ @name = translate( current()/../@dbTable, $lowercase, $uppercase ) ]
																				//xsd:element[ @name = translate( current()/@dbColumn, $lowercase, $uppercase ) ]"/>
		</xsl:element>
	</xsl:template>

	<!-- Copy the database type and mulitplicity to the expanded property -->
	<xsl:template match="xsd:element" mode="classHierarchy">
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
	<xsl:template match="cad:DataModel" mode="classHierarchyClassName">
		<xsl:attribute name="className">
			<xsl:apply-templates mode="className" select="."/>
		</xsl:attribute>
	</xsl:template>

	<xsl:template match="cad:DataModel" mode="className">
		<xsl:call-template name="className">
			<xsl:with-param name="tokens" select="@name"/>
		</xsl:call-template>
	</xsl:template>
	
	<!-- class name for concrete object type -->
	<xsl:template match="cad:Object[ parent::node()[ self::cad:DataModel ] ]" mode="classHierarchyClassName">
		<xsl:attribute name="className">
			<xsl:apply-templates mode="className" select="."/>
		</xsl:attribute>
	</xsl:template>

	<xsl:template match="cad:Object[ parent::node()[ self::cad:DataModel ] ]" mode="className">
		<xsl:call-template name="className">
			<xsl:with-param name="tokens" select="@name"/>
		</xsl:call-template>
	</xsl:template>

	<!-- class name for a type reference -->
	<xsl:template match="cad:Object[ @type ][ not(parent::node()[ self::cad:DataModel ]) ]" mode="classHierarchyClassName">
		<xsl:attribute name="className">
			<xsl:apply-templates mode="className" select="."/>
		</xsl:attribute>
	</xsl:template>

	<xsl:template match="cad:Object[ @type ][ not(parent::node()[ self::cad:DataModel ]) ]" mode="className">
		<xsl:call-template name="className">
			<xsl:with-param name="tokens" select="@type"/>
		</xsl:call-template>
	</xsl:template>
	
	<!-- class name for an anonymous nested type, make it up based on the parent name -->
	<xsl:template match="cad:Object[ not( @type ) ][ not(parent::node()[ self::cad:DataModel ]) ]" mode="classHierarchyClassName">
		<xsl:attribute name="className">
			<xsl:apply-templates mode="className" select="."/>
		</xsl:attribute>
	</xsl:template>

	<xsl:template match="cad:Object[ not( @type ) ][ not(parent::node()[ self::cad:DataModel ]) ]" mode="className">
		<xsl:call-template name="className">
			<xsl:with-param name="tokens" select="concat( ../@name, ' ', @name )"/>
		</xsl:call-template>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Helpers
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cad:Object" mode="datasetRowType">
		<xsl:value-of select="ancestor-or-self::cad:Object[ @datasetRowType ]/@datasetRowType"/>
	</xsl:template>

	<xsl:template match="cad:Object" mode="datasetTableType">
		<xsl:value-of select="ancestor-or-self::cad:Object[ @datasetTableType ]/@datasetTableType"/>
	</xsl:template>

</xsl:stylesheet>
