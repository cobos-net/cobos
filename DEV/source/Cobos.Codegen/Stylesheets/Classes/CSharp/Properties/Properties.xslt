<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
					 xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
					 xmlns:msxsl="urn:schemas-microsoft-com:xslt"
					 xmlns:cobos="http://schemas.cobos.co.uk/datamodel/1.0.0"
					 xmlns="http://schemas.cobos.co.uk/datamodel/1.0.0"
					 xmlns:xsd="http://www.w3.org/2001/XMLSchema"
					 exclude-result-prefixes="msxsl">

  <xsl:include href="./Types.xslt"/>
  <xsl:include href="./Body.xslt"/>

  <!-- 
	=============================================================================
	Filename: .xslt
	Description: 
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Created by: N.Davis                        Date: 2010-04-09
	Modified by:                               Date:
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Notes: 
	
	
	============================================================================
	-->

  <!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Create property declarations
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

  <!-- output a simple property type, nested object or reference -->
  <xsl:template match="cobos:Property|cobos:Object|cobos:Reference" mode="propertyDefinition">
        [System.Runtime.Serialization.DataMember(Order = <xsl:value-of select="position() - 1"/>)]
    <xsl:apply-templates select="." mode="propertyQualifiers"/> <xsl:apply-templates select="." mode="propertyType"/> <xsl:value-of select="@name"/>
        {
    <xsl:apply-templates select="." mode="propertyBody"/>
        }
  </xsl:template>

  <!-- output a reference to a parent record (foreign key parent) -->
  <xsl:template match="cobos:Reference" mode="parentReferencePropertyDefinition">
        [System.Runtime.Serialization.IgnoreDataMember]
    <xsl:apply-templates select="." mode="propertyQualifiers"/> <xsl:value-of select="../@name"/><xsl:text> </xsl:text><xsl:value-of select="../@name"/>
        {
    <xsl:apply-templates select="." mode="parentReferencePropertyBody"/>
        }
  </xsl:template>

  <!-- Property qualifiers in abstract classes -->
  <xsl:template match="*[self::cobos:Reference|self::cobos:Property|self::cobos:Object][ancestor::cobos:Interface[@isAbstractClass = 'true']]" mode="propertyQualifiers">
    <xsl:text>    public abstract </xsl:text>
  </xsl:template>

  <!-- Property qualifiers in interfaces -->
  <xsl:template match="*[self::cobos:Reference|self::cobos:Property|self::cobos:Object][ancestor::cobos:Interface[not(@isAbstractClass = 'true')]]" mode="propertyQualifiers"/>

  <!-- Property qualifiers in concrete classes -->
  <xsl:template match="*[self::cobos:Reference|self::cobos:Property|self::cobos:Object][not(ancestor::cobos:Interface)]" mode="propertyQualifiers">
    <xsl:text>    public </xsl:text>
    <xsl:apply-templates select="." mode="propertyQualifiersForInheritance"/>
  </xsl:template>

  <!-- Property qualifiers for inherited properties -->
  <xsl:template match="*[self::cobos:Reference|self::cobos:Property|self::cobos:Object][ancestor::cobos:Object[@implements]]" mode="propertyQualifiersForInheritance">
    <!-- get the interface object the parent class implements -->
    <xsl:variable name="interface" select="/cobos:DataModel/cobos:Interface[@name = current()/ancestor-or-self::cobos:Object[@implements]/@implements]"/>
    <!-- only apply if this property is defined in the abstract class -->
    <xsl:apply-templates select="$interface//*[self::cobos:Object|self::cobos:Property][substring-after(@qualifiedName, '.') = substring-after(current()/@qualifiedName, '.')]" mode="propertyQualifiersForInheritanceOverride"/>
  </xsl:template>

  <xsl:template match="cobos:Reference|cobos:Property|cobos:Object" mode="propertyQualifiersForInheritanceOverride">
    <xsl:text>override </xsl:text>
  </xsl:template>

</xsl:stylesheet>