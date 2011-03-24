<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
						xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
						xmlns:msxsl="urn:schemas-microsoft-com:xslt"
						exclude-result-prefixes="msxsl util"
						xmlns="http://schemas.intergraph.com/asiapac/cad/datamodel/1.0.0"
						xmlns:cad="http://schemas.intergraph.com/asiapac/cad/datamodel/1.0.0"
						xmlns:xsd="http://www.w3.org/2001/XMLSchema"
						xmlns:util="http://schemas.phoneview.com/casemanagement/utilities"
>
	<xsl:output method="xml" indent="yes"/>

	<!-- 
	=============================================================================
	Filename: DBSchema.xslt
	Description: XSLT for creation of Xsd from DB Schema
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Created by: N.Davis                        Date: 2010-04-09
	Modified by:                               Date:
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Notes: 


	============================================================================
	-->

	<xsl:include href="Utilities.xslt"/>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Key for building the XSD string types using a length restriction
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~3
	-->

	<xsl:key name="stringLengths" match="COLUMN[./DATA_TYPE = 'VARCHAR2']" use="CHAR_LENGTH"/>
	<xsl:key name="rawLengths" match="COLUMN[./DATA_TYPE = 'RAW']" use="DATA_LENGTH"/>
	<xsl:key name="tableNames" match="COLUMN" use="TABLE_NAME"/>
	
	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~3
	-->

	<xsl:template match="/TABLE_COLUMNS">

		<xsl:call-template name="generatedXmlWarning"/>

		<xsd:schema targetNamespace="http://schemas.intergraph.com/asiapac/cad/datamodel/1.0.0"
				elementFormDefault="qualified"
				xmlns="http://schemas.intergraph.com/asiapac/cad/datamodel/1.0.0"
				xmlns:xsd="http://www.w3.org/2001/XMLSchema">

			<xsd:simpleType name="char">
				<xsd:restriction base="xsd:string">
					<xsd:maxLength value="1"/>
				</xsd:restriction>
			</xsd:simpleType>

			<xsl:for-each select="//COLUMN[./DATA_TYPE = 'VARCHAR2'][generate-id() = generate-id(key('stringLengths',CHAR_LENGTH)[1])]">
				<xsl:sort select="CHAR_LENGTH" data-type="number"/>
				<xsl:variable name="length">
					<xsl:value-of select="CHAR_LENGTH"/>
				</xsl:variable>
				<xsd:simpleType name="string_{$length}">
					<xsd:restriction base="xsd:string">
						<xsd:maxLength value="{$length}"/>
					</xsd:restriction>
				</xsd:simpleType>
			</xsl:for-each>

			<xsl:for-each select="//COLUMN[./DATA_TYPE = 'RAW'][generate-id() = generate-id(key('rawLengths',DATA_LENGTH)[1])]">
				<xsl:sort select="DATA_LENGTH" data-type="number"/>
				<xsl:variable name="length">
					<xsl:value-of select="DATA_LENGTH"/>
				</xsl:variable>
				<xsd:simpleType name="hexBinary_{$length}">
					<xsd:restriction base="xsd:hexBinary">
						<xsd:length value="{$length}"/>
					</xsd:restriction>
				</xsd:simpleType>
			</xsl:for-each>

			<xsl:for-each select="//COLUMN[generate-id() = generate-id(key('tableNames',TABLE_NAME)[1])]">
				<xsl:variable name="tableName" select="TABLE_NAME"/>
				<xsd:element name="{TABLE_NAME}">
					<xsd:complexType>
						<xsd:sequence>
							<xsl:apply-templates select="//COLUMN[./TABLE_NAME = $tableName]"/>
						</xsd:sequence>
					</xsd:complexType>
				</xsd:element>
			</xsl:for-each>
		
		</xsd:schema>

	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="COLUMN">
		<xsl:element name="xsd:element">
			<xsl:attribute name="name">
				<xsl:value-of select="COLUMN_NAME"/>
			</xsl:attribute>
			<xsl:attribute name="type">
				<xsl:apply-templates select="DATA_TYPE"/>
			</xsl:attribute>
			<xsl:attribute name="minOccurs">
				<xsl:apply-templates select="NULLABLE"/>
			</xsl:attribute>
		</xsl:element>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Match the database data types
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="DATA_TYPE[. = 'CHAR']">
		<xsl:text>char</xsl:text>
	</xsl:template>

	<xsl:template match="DATA_TYPE[. = 'VARCHAR2']">
		<xsl:value-of select="concat( 'string_', ../CHAR_LENGTH )"/>
	</xsl:template>

	<xsl:template match="DATA_TYPE[. = 'NUMBER']">
		<xsl:text>xsd:integer</xsl:text>
	</xsl:template>

	<xsl:template match="DATA_TYPE[. = 'FLOAT']">
		<xsl:text>xsd:float</xsl:text>
	</xsl:template>

	<xsl:template match="DATA_TYPE[. = 'DATE']">
		<xsl:text>xsd:dateTime</xsl:text>
	</xsl:template>

	<xsl:template match="DATA_TYPE[. = 'RAW']">
		<xsl:value-of select="concat( 'hexBinary_', ../DATA_LENGTH )"/>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Set the multiplicity
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="NULLABLE[. = 'Y']">
		<xsl:text>0</xsl:text>
	</xsl:template>

	<xsl:template match="NULLABLE[. = 'N']">
		<xsl:text>1</xsl:text>
	</xsl:template>

</xsl:stylesheet>
