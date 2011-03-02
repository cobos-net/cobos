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
   <xsl:strip-space elements="xsl:attribute"/>

   <!-- 
   =============================================================================
   Filename: dataset.xslt
   Description: XSLT for creation of DataSet Xsd definitions
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
   Process the data model into a dataset schema
   ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
   -->

   <xsl:template match="/cad:DataModel">

      <xsd:schema targetNamespace="http://schemas.intergraph.com/asiapac/cad/datamodel"
            elementFormDefault="qualified"
            xmlns="http://schemas.intergraph.com/asiapac/cad/datamodel"
            xmlns:cad="http://schemas.intergraph.com/asiapac/cad/datamodel"
            xmlns:xsd="http://www.w3.org/2001/XMLSchema"
            xmlns:msdata="urn:schemas-microsoft-com:xml-msdata">

         <xsl:element name="xsd:element">
            <xsl:attribute name="name">
               <xsl:value-of select="@name"/>
            </xsl:attribute>
            <xsl:attribute name="msdata:IsDataSet">true</xsl:attribute>
            <xsd:complexType>
               <xsd:choice maxOccurs="unbounded">
                  <xsl:apply-templates select="cad:Object"/>
               </xsd:choice>
            </xsd:complexType>
         </xsl:element>
         
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

      <xsl:element name="xsd:element">
         <xsl:attribute name="name">
            <xsl:value-of select="@name"/>
         </xsl:attribute>
         <xsl:attribute name="msdata:ColumnName">
            <xsl:value-of select="@name"/>
         </xsl:attribute>
         <xsd:complexType>
            <xsd:sequence>
               <xsl:apply-templates select="./*"/>
            </xsd:sequence>
         </xsd:complexType>
      </xsl:element>

   </xsl:template>

   <!--
   ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
   Process object references
   ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
   -->

   <xsl:template match="cad:Object[ not( @type ) ]" mode="nested">
      <xsl:apply-templates select="./*"/>
   </xsl:template>

   <xsl:template match="cad:Object[ @type ]">
      <xsl:variable name="objectType" select="@type"/>
      <xsl:apply-templates select="//cad:Object[ @name = $objectType ]"/>
   </xsl:template>

   <!--
   ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
   Process a property
   ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
   -->

   <xsl:template match="cad:Property">
      
      <xsl:variable name="upperTableName">
         <xsl:value-of select="translate( ../@dbTable, $lowercase, $uppercase )"/>
      </xsl:variable>
      
      <xsl:variable name="upperColumnName">
         <xsl:value-of select="translate( @dbColumn, $lowercase, $uppercase )"/>
      </xsl:variable>

      <xsl:element name="xsd:element">
         <xsl:attribute name="name">
            <xsl:value-of select="@name"/>
         </xsl:attribute>
         <xsl:attribute name="msdata:ColumnName">
            <xsl:value-of select="@dbColumn"/>
         </xsl:attribute>
         <xsl:apply-templates select="$databaseTablesNodeSet/xsd:element[ @name = $upperTableName ]//xsd:element[ @name = $upperColumnName ]" mode="copyDbAttributes"/>
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
