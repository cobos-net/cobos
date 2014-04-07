<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
					 xmlns:msxsl="urn:schemas-microsoft-com:xslt"
					 xmlns:xsd="http://www.w3.org/2001/XMLSchema"
					 xmlns="http://schemas.cobos.co.uk/datamodel/1.0.0"
					 xmlns:cobos="http://schemas.cobos.co.uk/datamodel/1.0.0"
>
	<xsl:output method="xml" indent="yes" encoding="utf-8"/>
  <xsl:include href="../Utilities/Boilerplate.xslt"/>
  <xsl:include href="../Utilities/Nodes.xslt"/>
  <!-- 
	============================================================================
	Filename: merge.xslt
	Description: Merge a database schema document into an XSLT.
	============================================================================
	Created by:	N.Davis						Date: 2010-08-10
	Updated by:								Date:
	============================================================================
	Notes: The output XSLT includes variables containing the elements in the
	input schema.  This is used when processing other XML documents, since
	the XSLT can be included.
	
	============================================================================
	-->
	<xsl:template name="nsdecl">
		<xsl:variable name="nsdecl">
			<nsdecl xmlns:msxsl="urn:schemas-microsoft-com:xslt"
						xmlns="http://schemas.cobos.co.uk/datamodel/1.0.0"
						xmlns:cobos="http://schemas.cobos.co.uk/datamodel/1.0.0"
						xmlns:xsd="http://www.w3.org/2001/XMLSchema"/>
		</xsl:variable>
		<xsl:apply-templates mode="copyAttributesAndNamespace" select="msxsl:node-set($nsdecl)"/>
	</xsl:template>	
	<!-- 
	============================================================================
	Create the stylesheet containing the processing variables
	============================================================================
	-->
	<xsl:template match="/xsd:schema">
		<!-- Boilerplate warning -->
    <xsl:call-template name="generatedXmlWarning"/>
		<!-- Stylesheet element -->
    <xsl:element name="xsl:stylesheet">
			<xsl:call-template name="nsdecl"/>
			<xsl:attribute name="version">1.0</xsl:attribute>
			<xsl:attribute name="exclude-result-prefixes">msxsl</xsl:attribute>
			<!-- Output processing statement -->
      <!--
      <xsl:element name="xsl:output">
				<xsl:attribute name="method">xml</xsl:attribute>
				<xsl:attribute name="indent">yes</xsl:attribute>
				<xsl:attribute name="encoding">utf-8</xsl:attribute>
			</xsl:element>
      -->
      <!-- Database types variable -->
			<xsl:element name="xsl:variable">
				<xsl:attribute name="name">databaseTypes</xsl:attribute>
				<xsl:copy-of select="xsd:simpleType"/>
			</xsl:element>
      <!-- Database types node-set -->
			<xsl:element name="xsl:variable">
				<xsl:attribute name="name">databaseTypesNodeSet</xsl:attribute>
				<xsl:attribute name="select">msxsl:node-set($databaseTypes)</xsl:attribute>
			</xsl:element>
      <!-- Database tables variable -->
			<xsl:element name="xsl:variable">
				<xsl:attribute name="name">databaseTables</xsl:attribute>
				<xsl:copy-of select="xsd:element/xsd:complexType/xsd:sequence/xsd:element" />
			</xsl:element>
      <!-- Database tables node-set -->
			<xsl:element name="xsl:variable">
				<xsl:attribute name="name">databaseTablesNodeSet</xsl:attribute>
				<xsl:attribute name="select">msxsl:node-set($databaseTables)</xsl:attribute>
			</xsl:element>
      <!-- Database contraints variable -->
			<xsl:element name="xsl:variable">
				<xsl:attribute name="name">databaseConstraints</xsl:attribute>
				<xsl:copy-of select="xsd:element//*[self::xsd:key | self::xsd:unique | self::xsd:keyref]" />
			</xsl:element>
      <!-- Database constraints node-set -->
			<xsl:element name="xsl:variable">
				<xsl:attribute name="name">databaseConstraintsNodeSet</xsl:attribute>
				<xsl:attribute name="select">msxsl:node-set($databaseConstraints)</xsl:attribute>
			</xsl:element>
		</xsl:element>
	</xsl:template>
</xsl:stylesheet>
