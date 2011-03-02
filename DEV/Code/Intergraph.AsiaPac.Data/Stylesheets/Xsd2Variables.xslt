<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
					 xmlns:msxsl="urn:schemas-microsoft-com:xslt"
					 xmlns:xsd="http://www.w3.org/2001/XMLSchema"
					 xmlns="http://www.w3.org/2001/XMLSchema"
					 xmlns:cad="http://schemas.intergraph.com/asiapac/cad/datamodel"
					 exclude-result-prefixes="cad"
>
	<xsl:output method="xml" indent="yes" encoding="utf-8"/>

	<!-- 
	============================================================================
	Filename: Xsd2Variables.xslt
	Description: Creates another Xslt for merging a schema.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Created by:	N.Davis						Date: 2010-08-10
	Updated by:									Date:
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Notes: Takes a schema as an input and creates a merging Xslt for further 
	processing
	
	============================================================================
	-->

	<xsl:include href="Utilities.xslt"/>

	<xsl:template name="nsdecl">
		<xsl:variable name="nsdecl">
			<nsdecl xmlns:msxsl="urn:schemas-microsoft-com:xslt"
						xmlns="http://www.w3.org/2001/XMLSchema"
						xmlns:xsd="http://www.w3.org/2001/XMLSchema"/>
		</xsl:variable>
		<xsl:apply-templates mode="nsdecl" select="msxsl:node-set($nsdecl)"/>
	</xsl:template>
	
	<!-- 
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Match the root element and create the stylesheet
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->
	
	<xsl:template match="/xsd:schema">

		<xsl:element name="xsl:stylesheet">
			<xsl:call-template name="nsdecl"/>
			<xsl:attribute name="version">1.0</xsl:attribute>
			<xsl:element name="xsl:output">
				<xsl:attribute name="method">xml</xsl:attribute>
				<xsl:attribute name="indent">yes</xsl:attribute>
				<xsl:attribute name="encoding">utf-8</xsl:attribute>
			</xsl:element>
			<xsl:apply-templates select="./*" />
		</xsl:element>

	</xsl:template>

	<!-- 
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Merge the Excel worksheets into Sections
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="xsd:element | xsd:simpleType">

		<xsl:element name="xsl:variable">
			<xsl:attribute name="name">
				<xsl:value-of select="@name"/>
			</xsl:attribute>
			<xsl:copy-of select="."/>
		</xsl:element>
		
	</xsl:template>

</xsl:stylesheet>
