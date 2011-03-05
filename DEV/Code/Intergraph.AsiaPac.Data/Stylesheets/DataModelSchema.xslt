<xsl:stylesheet version="1.0"
						xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
						xmlns:msxsl="urn:schemas-microsoft-com:xslt"
						exclude-result-prefixes="msxsl"
						xmlns="http://schemas.intergraph.com/asiapac/cad/datamodel"
						xmlns:cad="http://schemas.intergraph.com/asiapac/cad/datamodel"
						xmlns:xsd="http://www.w3.org/2001/XMLSchema"
>
	<xsl:output method="xml" indent="yes"/>
	<xsl:strip-space elements="xsl:attribute"/>

	<!-- 
	=============================================================================
	Filename: datamodelschema.xslt
	Description: XSLT for creation of Xsd definitions from data model
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Created by: N.Davis                        Date: 2010-04-09
	Modified by:                               Date:
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Notes: 


	============================================================================
	-->

	<!-- include the generated database schema variables -->
	<xsl:include href="CadDatabase.xslt"/>
	<xsl:include href="Utilities.xslt"/>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Process the data model into an Xml schema
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="/cad:DataModel">

		<xsl:call-template name="generatedXmlWarning"/>

		<xsd:schema targetNamespace="http://schemas.intergraph.com/asiapac/cad/datamodel"
				elementFormDefault="qualified"
				xmlns="http://schemas.intergraph.com/asiapac/cad/datamodel"
				xmlns:cad="http://schemas.intergraph.com/asiapac/cad/datamodel"
				xmlns:xsd="http://www.w3.org/2001/XMLSchema">

			<xsl:apply-templates select="child::cad:Object | child::cad:Type"/>

			<!-- copy the database type definitions -->
			<xsl:copy-of select="$databaseTypesNodeSet"/>

		</xsd:schema>

	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Process a concrete object type
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cad:Object[ not( @type ) ]">

		<xsd:element name="{@name}">
			<xsd:complexType>
				<xsd:sequence>
					<xsl:apply-templates select="./*"/>
				</xsd:sequence>
			</xsd:complexType>
		</xsd:element>

	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Process object references
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cad:Object[ @type ]">
		
		<xsd:element name="{@name}" type="{@type}"/>
	
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Process object types
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cad:Type">

		<xsd:complexType name="{@name}">
			<xsd:sequence>
				<xsl:apply-templates select="./*"/>
			</xsd:sequence>
		</xsd:complexType>

	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Process a property
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cad:Property">

		<xsl:element name="xsd:element">
			<xsl:attribute name="name">
				<xsl:value-of select="@name"/>
			</xsl:attribute>
			<xsl:apply-templates select="$databaseTablesNodeSet/xsd:element[ @name = translate( current()/../@dbTable, $lowercase, $uppercase ) ]
													//xsd:element[ @name = translate( current()/@dbColumn, $lowercase, $uppercase ) ]" 
										mode="copyDbAttributes"/>
		</xsl:element>
		
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Copy attributes from the database schema
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="xsd:element" mode="copyDbAttributes">
		<xsl:copy-of select="@type"/>
		<xsl:copy-of select="@minOccurs"/>
	</xsl:template>


</xsl:stylesheet>
