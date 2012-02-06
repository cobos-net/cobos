<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	 xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
	
	<!-- 
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Artificially create namespace declarations on a root element
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="*" mode="copyAttributesAndNamespace">
		<xsl:copy-of select="namespace::* | @*"/>
	</xsl:template>

	<!-- Example 
	<xsl:template name="nsdecl">
		<xsl:variable name="nsdecl">
			<nsdecl xmlns:cobos="http://schemas.cobos.co.uk/datamodel/1.0.0" 
						xmlns:msxsl="urn:schemas-microsoft-com:xslt"  
						xmlns="http://schemas.cobos.co.uk/datamodel/1.0.0"
						xmlns:xsd="http://www.w3.org/2001/XMLSchema"/>
		</xsl:variable>
		<xsl:apply-templates mode="copyAttributesAndNamespace" select="msxsl:node-set($nsdecl)"/>
	</xsl:template> -->

	<!-- 
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Copy nodes without copying namespaces
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="*">
		<xsl:element name="{local-name(.)}">
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>

	<xsl:template match="@*">
		<xsl:copy/>
	</xsl:template>

	<!-- 
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Upper/lower case transformations
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:variable name="lowercase" select="'abcdefghijklmnopqrstuvwxyz'" />
	<xsl:variable name="uppercase" select="'ABCDEFGHIJKLMNOPQRSTUVWXYZ'" />

	<xsl:template match="*" mode="upperCase">
		<xsl:value-of select="translate( text(), $lowercase, $uppercase )"/>
	</xsl:template>

	<xsl:template match="*" mode="lowerCase">
		<xsl:value-of select="translate( text(), $uppercase, $lowercase )"/>
	</xsl:template>

	<!-- 
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Title case an input string with whitespace delimited tokens, removing
	invalid characters for class naming conventions.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:variable name="invalidClassNameChar">
		<xsl:text>!@#$%^&amp;*()-+={[}}|\:;"'&lt;&gt;,./?~`</xsl:text>
	</xsl:variable>

	<xsl:template name="tokensToClassName">
		<xsl:param name="tokens"/>
		<xsl:call-template name="titleCaseTokens">
			<xsl:with-param name="tokens" select="translate( $tokens, $invalidClassNameChar, '' )"/>
		</xsl:call-template>
	</xsl:template>

	<!-- 
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Recursive template to title case an input string with whitespace delimited
	tokens.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template name="titleCaseTokens">
		<xsl:param name="tokens"/>

		<xsl:variable name="before" select="substring-before( $tokens, ' ' )"/>

		<xsl:choose>
			<xsl:when test="$before">
				<xsl:value-of select="concat( translate( substring( $before, 1, 1 ), $lowercase, $uppercase  ), substring( $before, 2 ) )" />
				<xsl:call-template name="titleCaseTokens">
					<xsl:with-param name="tokens" select="substring-after( $tokens, ' ' )"/>
				</xsl:call-template>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="concat( translate( substring( $tokens, 1, 1 ), $lowercase, $uppercase  ), substring( $tokens, 2 ) )" />
			</xsl:otherwise>
		</xsl:choose>

	</xsl:template>

	<xsl:template name="titleCaseName">
		<xsl:param name="name"/>
		<xsl:value-of select="concat( translate( substring( $name, 1, 1 ), $lowercase, $uppercase  ), substring( $name, 2 ) )" />
	</xsl:template>

		<!-- 
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Convert a CAPS_UNDERSCORE_TITLE (e.g. database column name) to TitleCase
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template name="capsUnderscoreToClassName">
		<xsl:param name="tokens"/>
		<xsl:call-template name="capsUnderscoreToTitleCase">
			<xsl:with-param name="tokens" select="translate( $tokens, $invalidClassNameChar, '' )"/>
		</xsl:call-template>
	</xsl:template>

	<xsl:template name="capsUnderscoreToTitleCase">
		<xsl:param name="tokens"/>

		<xsl:variable name="before" select="substring-before( $tokens, '_' )"/>

		<xsl:choose>
			<xsl:when test="$before">
				<xsl:value-of select="concat( translate( substring( $before, 1, 1 ), $lowercase, $uppercase ), translate( substring( $before, 2 ), $uppercase, $lowercase ) )" />
				<xsl:call-template name="capsUnderscoreToTitleCase">
					<xsl:with-param name="tokens" select="substring-after( $tokens, '_' )"/>
				</xsl:call-template>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="concat( translate( substring( $tokens, 1, 1 ), $lowercase, $uppercase  ), translate( substring( $tokens, 2 ), $uppercase, $lowercase ) )" />
			</xsl:otherwise>
		</xsl:choose>

	</xsl:template>


	<!-- 
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Boilerplate auto-generation warning messages
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template name="generatedXmlWarning">
		<xsl:text disable-output-escaping="yes"><![CDATA[
<!--
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
 <auto-generated>
	  This code was generated by a tool.

	  Changes to this file may cause incorrect behavior and will be lost if
	  the code is regenerated.
 </auto-generated>
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

This schema was auto-generated by Cobos SDK tools.
-->
]]></xsl:text>
	</xsl:template>

	<xsl:template name="generatedCSharpWarning">
		<xsl:text disable-output-escaping="yes"><![CDATA[
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
//
// This source code was auto-generated by Cobos SDK tools.
//
]]></xsl:text>
	</xsl:template>

	
</xsl:stylesheet>
