<xsl:stylesheet version="1.0"
						xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
						xmlns:msxsl="urn:schemas-microsoft-com:xslt"
						exclude-result-prefixes="msxsl"
						xmlns="http://schemas.intergraph.com/asiapac/cad/datamodel"
						xmlns:cad="http://schemas.intergraph.com/asiapac/cad/datamodel"
						xmlns:xsd="http://www.w3.org/2001/XMLSchema"
						xmlns:msdata="urn:schemas-microsoft-com:xml-msdata"
						xmlns:msprop="urn:schemas-microsoft-com:xml-msprop"
>
	<xsl:output method="xml" indent="yes"/>

	<!-- 
	=============================================================================
	Filename: dataset.xslt
	Description: XSLT for creation of DataSet Xsd definitions
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Created by: N.Davis                        Date: 2010-04-09
	Modified by:                               Date:
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Notes: We expand the object hierarchy so that each top level object is 
	represented by a single query and single data table.

	============================================================================
	-->

	<xsl:include href="DataModelCommon.xslt"/>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Process the data model into a dataset schema
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="/cad:DataModel">

		<xsl:call-template name="generatedXmlWarning"/>

		<xsd:schema targetNamespace="http://schemas.intergraph.com/asiapac/cad/datamodel"
				elementFormDefault="qualified"
				xmlns="http://schemas.intergraph.com/asiapac/cad/datamodel"
				xmlns:cad="http://schemas.intergraph.com/asiapac/cad/datamodel"
				xmlns:xsd="http://www.w3.org/2001/XMLSchema"
				xmlns:msdata="urn:schemas-microsoft-com:xml-msdata">
			<xsl:element name="xsd:element">
				<xsl:attribute name="name">
					<xsl:apply-templates select="." mode="className"/>
				</xsl:attribute>
				<xsl:attribute name="msdata:IsDataSet">true</xsl:attribute>
				<xsd:complexType>
					<xsd:choice maxOccurs="unbounded">
						<xsl:apply-templates select="cad:Object" mode="expandTopLevel"/>
					</xsd:choice>
				</xsd:complexType>
			</xsl:element>
			<!-- copy the database type definitions -->
			<xsl:copy-of select="$databaseTypesNodeSet"/>
		</xsd:schema>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Process a top level object 
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<!-- Match a top level object and expand the object hierarchy -->
	<xsl:template match="cad:Object" mode="expandTopLevel">
		<xsl:variable name="classHierarchy">
			<xsl:apply-templates select="." mode="classHierarchy"/>
		</xsl:variable>
		<xsl:variable name="classHierarchyNodeset" select="msxsl:node-set( $classHierarchy )"/>
		<xsl:apply-templates select="$classHierarchyNodeset"/>
	</xsl:template>

	<!-- Match a processed object -->
	<xsl:template match="cad:Object">
		<xsl:element name="xsd:element">
			<xsl:attribute name="name">
				<xsl:value-of select="@className"/>
			</xsl:attribute>
			<xsl:attribute name="msdata:ColumnName">
				<xsl:value-of select="@name"/>
			</xsl:attribute>
			<xsd:complexType>
				<xsd:sequence>
					<xsl:apply-templates select=".//cad:Property"/>
				</xsd:sequence>
			</xsd:complexType>
		</xsl:element>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Process a property
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<!-- Process a property in a top level object -->
	<xsl:template match="cad:Property">
		
		<xsl:element name="xsd:element">
			<xsl:attribute name="name">
				<xsl:apply-templates mode="qualifiedName" select="."/>
			</xsl:attribute>
			<xsl:attribute name="msdata:ColumnName">
				<xsl:value-of select="@dbColumn"/>
			</xsl:attribute>
			<xsl:attribute name="type">
				<xsl:value-of select="@dbType"/>
			</xsl:attribute>
			<xsl:copy-of select="@minOccurs"/>
		</xsl:element>
	
	</xsl:template>
	
	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Process a property name - nested types are flattened in our DB model
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cad:Property" mode="qualifiedName">
		<xsl:apply-templates select="ancestor::cad:Object[ not( position() = last() ) ]" mode="qualifiedName"/>
		<xsl:value-of select="@name"/>
	</xsl:template>

	<xsl:template match="cad:Object" mode="qualifiedName">
		<xsl:value-of select="@name"/>
		<xsl:text>_</xsl:text>
	</xsl:template>

</xsl:stylesheet>
