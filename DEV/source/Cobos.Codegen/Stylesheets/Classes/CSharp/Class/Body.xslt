<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
					 xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
					 xmlns:msxsl="urn:schemas-microsoft-com:xslt"
					 xmlns:cobos="http://schemas.cobos.co.uk/datamodel/1.0.0"
					 xmlns="http://schemas.cobos.co.uk/datamodel/1.0.0"
					 xmlns:xsd="http://www.w3.org/2001/XMLSchema"
					 exclude-result-prefixes="msxsl">

  <!-- 
	=============================================================================
	Filename: .xslt
	Description: Create a class definition for each object.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Created by: N.Davis                        Date: 2010-04-09
	Modified by:                               Date:
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Notes: 
	
	
	============================================================================
	-->

  <!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Class body definition
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

  <xsl:template match="cobos:Interface|cobos:Object" mode="classBody">
    <!-- add member object declarations (Members.xslt) -->
    <xsl:apply-templates select="." mode="classMemberDefinition"/>

    <!-- add the constructor declaration (Constructor.xslt) -->
    <xsl:apply-templates select="." mode="classConstructorDeclaration"/>

    <!-- add property declarations (..\Properties\Properties.xslt) -->
    <xsl:apply-templates select="child::cobos:Property[ not( @hidden ) ]|child::cobos:Object|child::cobos:Reference" mode="propertyDefinition"/>

    <!-- add nested classes (Class.xslt) -->
    <xsl:apply-templates select="child::cobos:Object" mode="classDefinition"/>
  </xsl:template>

</xsl:stylesheet>