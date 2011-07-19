<xsl:stylesheet version="1.0"
						xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
						xmlns:msxsl="urn:schemas-microsoft-com:xslt"
						exclude-result-prefixes="msxsl"
						xmlns="http://schemas.intergraph.com/asiapac/cad/datamodel/1.0.0"
						xmlns:cad="http://schemas.intergraph.com/asiapac/cad/datamodel/1.0.0"
						xmlns:xsd="http://www.w3.org/2001/XMLSchema"
						xmlns:msdata="urn:schemas-microsoft-com:xml-msdata"
						xmlns:codegen="urn:schemas-microsoft-com:xml-msprop"
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

	<xsl:key name="dbTableKey" match="cad:Object|cad:Property" use="@dbTable"/>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Process the data model into a dataset schema
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="/cad:DataModel">

		<xsl:call-template name="generatedXmlWarning"/>

		<xsl:variable name="name">
			<xsl:apply-templates select="." mode="className"/>
		</xsl:variable>

		<xsd:schema targetNamespace="http://schemas.intergraph.com/asiapac/cad/datamodel/1.0.0"
				elementFormDefault="qualified" 
				xmlns="http://schemas.intergraph.com/asiapac/cad/datamodel/1.0.0"
				xmlns:cad="http://schemas.intergraph.com/asiapac/cad/datamodel/1.0.0"
				xmlns:xsd="http://www.w3.org/2001/XMLSchema"
				xmlns:msdata="urn:schemas-microsoft-com:xml-msdata">
			<xsl:element name="xsd:element">
				<xsl:attribute name="name">
					<xsl:value-of select="$name"/>
				</xsl:attribute>
				<xsl:attribute name="msdata:IsDataSet">true</xsl:attribute>
				<xsd:complexType>
					<xsd:choice maxOccurs="unbounded">
						<xsl:apply-templates select="cad:Object"/>
					</xsd:choice>
				</xsd:complexType>
				<!-- copy the key constraints that apply to the tables referenced by this data model -->
				<xsl:apply-templates select="cad:Object" mode="constraints"/>
				<!-- generate unique and keyref constraints for the reference objects -->
				<xsl:apply-templates select="//cad:Reference"/>
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

	<!-- Match a processed object -->
	<xsl:template match="cad:Object">
		<xsl:element name="xsd:element">
			<xsl:attribute name="name">
				<xsl:value-of select="@className"/>
			</xsl:attribute>
			<!--
			<xsl:attribute name="codegen:typedName">
				<xsl:value-of select="@name"/>
			</xsl:attribute>
			-->
			<xsd:complexType>
				<xsd:sequence>
					<xsl:apply-templates select=".//cad:Property"/>
				</xsd:sequence>
			</xsd:complexType>
		</xsl:element>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Process a property in a top level object
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cad:Property">
		
		<xsl:element name="xsd:element">
			<xsl:variable name="qualifiedName">
				<xsl:apply-templates mode="qualifiedName" select="."/>
			</xsl:variable>
			<xsl:attribute name="name">
				<xsl:value-of select="$qualifiedName"/>
			</xsl:attribute>
			<xsl:attribute name="codegen:typedName">
				<xsl:value-of select="$qualifiedName"/>
			</xsl:attribute>
			<xsl:attribute name="msdata:ColumnName">
				<xsl:apply-templates select="." mode="dbColumn"/>
			</xsl:attribute>
			<xsl:attribute name="type">
				<xsl:value-of select="@dbType"/>
			</xsl:attribute>
			<xsl:copy-of select="@minOccurs"/>
		</xsl:element>
	
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Process a reference
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cad:Reference">
		<xsl:variable name="keyName">
			<xsl:value-of select="../@name"/>
			<xsl:value-of select="@ref"/>
		</xsl:variable>
		<xsl:variable name="keyfield">
			<xsl:value-of select="../cad:Property[ current()/@key ]/@dbColumn"/>
		</xsl:variable>
		<xsl:variable name="keyreffield">
			<xsl:value-of select="/cad:DataModel/cad:Object[ current()/@ref ]/cad:Property[ current()/@refer ]/@dbColumn"/>
		</xsl:variable>
		<xsd:unique name="{$keyName}Constraint" msdata:PrimaryKey="true">
			<xsd:selector xpath=".//{../@name}"/>
			<xsd:field xpath="{$keyfield}"/>
		</xsd:unique>
		<xsd:keyref name="{$keyName}" refer="{$keyName}Constraint" codegen:typedParent="{../@name}" codegen:typedChildren="Get{@name}">
			<xsd:selector xpath=".//{@ref}"/>
			<xsd:field xpath="{$keyreffield}"/>
		</xsd:keyref>
	</xsl:template>


	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Process a top level object's constraints
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cad:Object" mode="constraints">
		<xsl:variable name="object" select="."/>
		
		<xsl:for-each select="$databaseConstraintsNodeSet/*[ self::xsd:key | self::xsd:unique | self::xsd:keyref ][ xsd:selector/@xpath = concat( './/', $object/@dbTable ) ]">
			
			<xsl:element name="{name()}">
				<xsl:attribute name="name">
					<xsl:value-of select="concat( $object/@name, '_', @name )"/>
				</xsl:attribute>
				<xsl:apply-templates select="." mode="constraintAttributes"/>
				<xsd:selector xpath=".//{$object/@name}"/>
				<xsl:copy-of select="xsd:field"/>
			</xsl:element>

		</xsl:for-each>
	
	</xsl:template>
	
	<!-- Add extra dataset processing attributes to key constraints -->

	<xsl:template match="xsd:key" mode="constraintAttributes">
		<xsl:attribute name="msdata:PrimaryKey">true</xsl:attribute>
	</xsl:template>

	<xsl:template match="xsd:unique" mode="constraintAttributes">
	</xsl:template>

	<xsl:template match="xsd:keyref" mode="constraintAttributes">
	</xsl:template>

</xsl:stylesheet>
