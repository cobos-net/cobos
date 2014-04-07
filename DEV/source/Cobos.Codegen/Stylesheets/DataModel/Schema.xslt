<xsl:stylesheet version="1.0"
						xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
						xmlns:msxsl="urn:schemas-microsoft-com:xslt"
						exclude-result-prefixes="msxsl cobos"
						xmlns="http://schemas.cobos.co.uk/datamodel/1.0.0"
						xmlns:cobos="http://schemas.cobos.co.uk/datamodel/1.0.0"
						xmlns:xsd="http://www.w3.org/2001/XMLSchema"
>
	<xsl:output method="xml" indent="yes"/>
	<xsl:strip-space elements="xsl:attribute"/>
	<!-- 
	=============================================================================
	Filename: Schema.xslt
	Description: XSLT for creation of Xsd definitions from data model
	=============================================================================
	Created by: N.Davis                        Date: 2010-04-09
	Modified by:                               Date:
	=============================================================================
	Notes: 


	============================================================================
	-->
	<xsl:include href="../Utilities/Utilities.inc"/>
	<!--
	=============================================================================
	Default namespace for the generated schema.
	Dynamically add the user specified namespace to a dummy element so that
	we can copy the xmlns: attributes for copying to the output nodes.
	=============================================================================
	-->
	<xsl:param name="xmlNamespace"/>
  <!--
	=============================================================================
	=============================================================================
	-->
  <xsl:variable name="ns-uri" select="string($xmlNamespace)"/>
	<xsl:variable name="ns-container">
		<xsl:element name="dummy" namespace="{$ns-uri}"/>
	</xsl:variable>
	<!--
	=============================================================================
	Controls how the minOccurs attributes are set for nested objects and types.
	
	strict:		 nested objects are always mandatory, nested types are set 
					   according to the database schema definition.  (default)
	
	optional:	 every nested object and type has minOccurs="0"
	
	mandatory: every nested object and type has minOccurs="1"
	=============================================================================
	-->
	<xsl:param name="multiplicityMode"/>
	<!--
	=============================================================================
	Set the root node name if you want to contain all of the top level elements 
	within a containing element.
	=============================================================================
	-->
	<xsl:param name="rootNodeName"/>
	<!--
	=============================================================================
	Process the data model into an Xml schema
	=============================================================================
	-->
	<xsl:template match="/cobos:DataModel">
		<xsl:call-template name="generatedXmlWarning"/>
		<xsl:element name="xsd:schema">
			<xsl:copy-of select="msxsl:node-set($ns-container)/*/namespace::*[. = $ns-uri]" />
			<xsl:attribute name="targetNamespace">
				<xsl:value-of select="$xmlNamespace"/>
			</xsl:attribute>
			<xsl:attribute name="elementFormDefault">qualified</xsl:attribute>
			<xsl:choose>
				<xsl:when test="$rootNodeName != ''">
					<xsl:element name="xsd:element">
						<xsl:attribute name="name">
							<xsl:value-of select="$rootNodeName"/>
						</xsl:attribute>
						<xsl:element name="xsd:complexType">
							<xsl:element name="xsd:sequence">
								<xsl:apply-templates select="child::cobos:Object|child::cobos:TableObject"/>
							</xsl:element>
						</xsl:element>
					</xsl:element>
				</xsl:when>
				<xsl:otherwise>
					<xsl:apply-templates select="child::cobos:Object|child::cobos:TableObject"/>
				</xsl:otherwise>
			</xsl:choose>
			<xsl:apply-templates select="child::cobos:Type"/>
			<xsl:apply-templates select="child::cobos:Enumeration"/>
			<!-- copy the database type definitions -->
			<xsl:copy-of select="$databaseTypesNodeSet"/>
		</xsl:element>
	</xsl:template>
  <!--
	=============================================================================
	=============================================================================
	-->
  <xsl:template match="cobos:Object[not(parent::cobos:DataModel)]" mode="minOccurs">
		<xsl:attribute name="minOccurs">
			<xsl:choose>
				<xsl:when test="$multiplicityMode = 'optional'">
					<xsl:value-of select="0"/>
				</xsl:when>
				<xsl:when test="$multiplicityMode = 'mandatory'">
					<xsl:value-of select="1"/>
				</xsl:when>
				<xsl:otherwise>
					<!-- strict -->
					<xsl:value-of select="1"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:attribute>
	</xsl:template>
  <!--
	=============================================================================
	=============================================================================
	-->
  <xsl:template match="cobos:Object[parent::cobos:DataModel]|cobos:TableObject" mode="minOccurs">
		<xsl:if test="$rootNodeName != ''">
			<xsl:attribute name="minOccurs">0</xsl:attribute>
		</xsl:if>
	</xsl:template>
	<!--
	=============================================================================
	Process a concrete object type
	=============================================================================
	-->
	<xsl:template match="cobos:Object[not(@type)]">
		<xsl:element name="xsd:element">
			<xsl:attribute name="name">
				<xsl:value-of select="@name"/>
			</xsl:attribute>
			<xsl:apply-templates select="." mode="minOccurs"/>
			<xsl:element name="xsd:complexType">
				<xsl:element name="xsd:sequence">
					<xsl:apply-templates select="child::cobos:Object|child::cobos:Property[not(@hidden)]|child::cobos:Reference|child::cobos:XsdProperty"/>
				</xsl:element>
			</xsl:element>
		</xsl:element>
	</xsl:template>
	<!--
	=============================================================================
	Process object references
	=============================================================================
	-->
	<xsl:template match="cobos:Object[@type]">
		<xsl:element name="xsd:element">
			<xsl:attribute name="name">
				<xsl:value-of select="@name"/>
			</xsl:attribute>
			<xsl:attribute name="type">
				<xsl:value-of select="@type"/>
			</xsl:attribute>
			<xsl:apply-templates select="." mode="minOccurs"/>
		</xsl:element>
	</xsl:template>
	<!--
	=============================================================================
	Process object types
	=============================================================================
	-->
	<xsl:template match="cobos:Type">
		<xsd:complexType name="{@name}">
			<xsd:sequence>
				<xsl:apply-templates select="child::cobos:Object|child::cobos:Property[not(@hidden)]|child::cobos:XsdProperty"/>
			</xsd:sequence>
		</xsd:complexType>
	</xsl:template>
	<!--
	=============================================================================
	Process a table object
	=============================================================================
	-->
	<xsl:template match="cobos:TableObject">
		<xsl:element name="xsd:element">
			<xsl:attribute name="name">
				<xsl:value-of select="@name"/>
			</xsl:attribute>
			<xsl:apply-templates select="." mode="minOccurs"/>
			<xsl:element name="xsd:complexType">
				<xsl:element name="xsd:sequence">
					<xsl:apply-templates select="$databaseTablesNodeSet/xsd:element[translate(@name, $lowercase, $uppercase) = translate(current()/@dbTable, $lowercase, $uppercase)]//xsd:element" mode="tableObjectElements"/>
				</xsl:element>
			</xsl:element>
		</xsl:element>
	</xsl:template>
	<!--
	=============================================================================
	Process a reference object
	=============================================================================
	-->
	<xsl:template match="cobos:Reference[not(@isCollection)]">
		<xsd:element ref="{@ref}" minOccurs="0" maxOccurs="1"/>
	</xsl:template>
  <!--
	=============================================================================
	=============================================================================
	-->
  <xsl:template match="cobos:Reference[@isCollection]">
		<xsd:element name="{@name}">
			<xsd:complexType>
				<xsd:sequence>
					<xsd:element ref="{@ref}" minOccurs="0" maxOccurs="unbounded"/>
				</xsd:sequence>
			</xsd:complexType>
		</xsd:element>
	</xsl:template>
	<!--
	=============================================================================
	Process a property
	=============================================================================
	-->
	<xsl:template match="cobos:Property">
		<xsl:element name="xsd:element">
			<xsl:attribute name="name">
				<xsl:value-of select="@name"/>
			</xsl:attribute>
			<xsl:apply-templates select="." mode="setDataAttributes"/>
		</xsl:element>
	</xsl:template>
  <!--
	=============================================================================
	Set the data attributes for a standard type (i.e. not string encoded data)
  =============================================================================
	-->
	<xsl:template match="cobos:Property[not(@stringFormat)]" mode="setDataAttributes">
		<xsl:variable name="dbTable">
			<xsl:apply-templates mode="getDbTable" select="."/>
		</xsl:variable>
		<xsl:apply-templates select="$databaseTablesNodeSet/xsd:element[translate(@name, $lowercase, $uppercase) = translate($dbTable, $lowercase, $uppercase)]//xsd:element[translate(@name, $lowercase, $uppercase) = translate(current()/@dbColumn, $lowercase, $uppercase)]" mode="copyDbAttributes"/>
	</xsl:template>
  <!--
	=============================================================================
	=============================================================================
	-->
  <xsl:template match="cobos:Property[@stringFormat = 'CadDts']" mode="setDataAttributes">
		<xsl:attribute name="type">xsd:dateTime</xsl:attribute>
		<xsl:apply-templates select="." mode="setDataMultiplicity"/>
	</xsl:template>
  <!--
	=============================================================================
	=============================================================================
	-->
  <xsl:template match="cobos:Property[@stringFormat = 'CadBoolean']" mode="setDataAttributes">
		<xsl:attribute name="type">xsd:boolean</xsl:attribute>
		<xsl:apply-templates select="." mode="setDataMultiplicity"/>
	</xsl:template>
  <!--
	=============================================================================
	=============================================================================
	-->
  <xsl:template match="cobos:Property[@stringFormat = 'Separator']" mode="setDataAttributes">
		<xsl:attribute name="type">xsd:string</xsl:attribute>
		<xsl:apply-templates select="." mode="setDataMultiplicity"/>
	</xsl:template>
  <!--
	=============================================================================
	=============================================================================
	-->
  <xsl:template match="cobos:Property" mode="setDataMultiplicity">
		<xsl:choose>
			<xsl:when test="$multiplicityMode = 'optional'">
				<xsl:attribute name="minOccurs">0</xsl:attribute>
			</xsl:when>
			<xsl:when test="$multiplicityMode = 'mandatory'">
				<xsl:attribute name="minOccurs">1</xsl:attribute>
			</xsl:when>
			<xsl:otherwise>
				<!-- strict -->
				<xsl:variable name="dbTable">
					<xsl:apply-templates mode="getDbTable" select="."/>
				</xsl:variable>
				<xsl:copy-of select="$databaseTablesNodeSet/xsd:element[translate(@name, $lowercase, $uppercase) = translate($dbTable, $lowercase, $uppercase)]//xsd:element[translate(@name, $lowercase, $uppercase) = translate(current()/@dbColumn, $lowercase, $uppercase)]/@minOccurs"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
  <!--
	=============================================================================
	=============================================================================
	-->
  <xsl:template match="cobos:XsdProperty">
		<xsl:element name="xsd:element">
			<xsl:copy-of select="@*"/>
		</xsl:element>
	</xsl:template>
	<!--
	=============================================================================
	Copy attributes from the database schema
	=============================================================================
	-->
	<xsl:template match="xsd:element" mode="tableObjectElements">
		<xsl:element name="xsd:element">
			<xsl:attribute name="name">
				<xsl:call-template name="capsUnderscoreToClassName">
					<xsl:with-param name="tokens" select="@name"/>
				</xsl:call-template>
			</xsl:attribute>
			<xsl:apply-templates select="." mode="copyDbAttributes"/>
		</xsl:element>
	</xsl:template>
  <!--
	=============================================================================
	=============================================================================
	-->
  <xsl:template match="xsd:element" mode="copyDbAttributes">
		<xsl:copy-of select="@type"/>
		<xsl:choose>
			<xsl:when test="$multiplicityMode = 'optional'">
				<xsl:attribute name="minOccurs">0</xsl:attribute>
			</xsl:when>
			<xsl:when test="$multiplicityMode = 'mandatory'">
				<xsl:attribute name="minOccurs">1</xsl:attribute>
			</xsl:when>
			<xsl:otherwise> 
				<!-- strict -->
				<xsl:copy-of select="@minOccurs"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!--
	=============================================================================
	Enumerations
	=============================================================================
	-->
	<xsl:template match="cobos:Enumeration">
		<xsd:simpleType name="{@name}">
			<xsd:restriction base="{@base}">
				<xsl:apply-templates select="cobos:Item"/>
			</xsd:restriction>
		</xsd:simpleType>
	</xsl:template>
  <!--
	=============================================================================
	=============================================================================
	-->
  <xsl:template match="cobos:Item">
		<xsd:enumeration value="{text()}"/>
	</xsl:template>	
</xsl:stylesheet>
