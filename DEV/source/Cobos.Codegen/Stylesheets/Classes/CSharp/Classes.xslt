<xsl:stylesheet version="1.0"
						xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
						xmlns:msxsl="urn:schemas-microsoft-com:xslt"
						exclude-result-prefixes="msxsl"
						xmlns="http://schemas.cobos.co.uk/datamodel/1.0.0"
						xmlns:cobos="http://schemas.cobos.co.uk/datamodel/1.0.0"
						xmlns:xsd="http://www.w3.org/2001/XMLSchema"
>
  <xsl:include href="../../Common.xslt"/>
  <xsl:include href="Custom.xslt"/>
  <xsl:include href="./Class/Class.xslt"/>
  <xsl:include href="./Properties/Properties.xslt"/>
  <xsl:include href="./TableAdapter/TableAdapter.xslt"/>

  <xsl:output method="text" indent="yes" omit-xml-declaration="yes"/>
  <xsl:strip-space elements="*"/>

  <!-- 
	=============================================================================
	Filename: Classes.xslt
	Description: XSLT for creation of C# code from data model
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Created by: N.Davis                        Date: 2010-04-09
	Modified by:                               Date:
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Notes: In the datamodel, each top level object refers to a single 
	table in the stronly typed dataset.  Nested object hierarchies are 
	flattened for more efficient database access and post-processing.

	============================================================================
	-->

  <!-- C# namespace -->
  <xsl:param name="codeNamespace"/>
  <!-- System.Runtime.Serialization.DataContract namespace -->
  <xsl:param name="xmlNamespace"/>

  <!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Process the data model into CSharp classes
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

  <xsl:template match="/cobos:DataModel">
		<xsl:call-template name="generatedCSharpWarning"/>
namespace <xsl:value-of select="$codeNamespace"/>
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Runtime.Serialization;
    using System.Text;
    <xsl:call-template name="customNamespaceDeclarations"/>
    <xsl:apply-templates select="cobos:Object|cobos:Interface" mode="classDefinition"/>
    <xsl:apply-templates select="cobos:Object[ not( @createTableAdapter = 'false' ) ]" mode="tableAdapter"/>
    <xsl:apply-templates select="cobos:Object" mode="customCodeExtensions"/>
}<xsl:text />
  </xsl:template>

</xsl:stylesheet>