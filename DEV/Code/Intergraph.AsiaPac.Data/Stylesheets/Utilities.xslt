<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	 xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
	 <xsl:output method="xml" indent="yes"/>

	<!-- 
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Artificially create namespace declarations on a root element
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="*" mode="nsdecl">
		<xsl:copy-of select="namespace::* | @*"/>
	</xsl:template>

	<!-- Example 
	<xsl:template name="nsdecl">
		<xsl:variable name="nsdecl">
			<nsdecl xmlns:cad="http://www.intergraph.com/asiapac/cad/datamodel" 
						xmlns:msxsl="urn:schemas-microsoft-com:xslt"  
						xmlns="http://www.intergraph.com/asiapac/cad/datamodel"
						xmlns:xsd="http://www.w3.org/2001/XMLSchema"/>
		</xsl:variable>
		<xsl:apply-templates mode="nsdecl" select="msxsl:node-set($nsdecl)"/>
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

	<xsl:variable name="lowercase" select="'abcdefghijklmnopqrstuvwxyz'" />
	<xsl:variable name="uppercase" select="'ABCDEFGHIJKLMNOPQRSTUVWXYZ'" />

	<xsl:template name="upperCase">
		<xsl:param name="input"/>
		<xsl:value-of select="translate( $input, $lowercase, $uppercase )"/>
	</xsl:template>

	<xsl:template name="lowerCase">
		<xsl:param name="input"/>
		<xsl:value-of select="translate( $input, $uppercase, $lowercase )"/>
	</xsl:template>

	<!-- 
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Recursive template to camel case an input string
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:variable name="invalidClassNameChar">
		<xsl:text>!@#$%^&amp;*()-+={[}}|\:;"'&lt;&gt;,./?~`</xsl:text>
	</xsl:variable>

	<xsl:template name="className">
		<xsl:param name="tokens"/>
		<xsl:call-template name="camelCase">
			<xsl:with-param name="tokens" select="translate( $tokens, $invalidClassNameChar, '' )"/>
		</xsl:call-template>
	</xsl:template>

	<xsl:template name="camelCase">
		<xsl:param name="tokens"/>

		<xsl:variable name="before" select="substring-before( $tokens, ' ' )"/>

		<xsl:choose>
			<xsl:when test="$before">
				<xsl:value-of select="concat( translate( substring( $before, 1, 1 ), $lowercase, $uppercase  ), substring( $before, 2 ) )" />
				<xsl:call-template name="camelCase">
					<xsl:with-param name="tokens" select="substring-after( $tokens, ' ' )"/>
				</xsl:call-template>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="concat( translate( substring( $tokens, 1, 1 ), $lowercase, $uppercase  ), substring( $tokens, 2 ) )" />
			</xsl:otherwise>
		</xsl:choose>

	</xsl:template>

</xsl:stylesheet>
