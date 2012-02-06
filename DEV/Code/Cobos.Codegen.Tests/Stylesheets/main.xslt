<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
	xmlns:t="tempuri"	
>
	<xsl:import href="default.xslt"/>
	<xsl:output method="xml" indent="yes"/>

	<xsl:template match="/">
		<Processed xmlns="tempuri">
			<xsl:apply-templates select="//t:Object"/>
			<xsl:apply-templates select="//t:Object" mode="testmode"/>
			<!--xsl:apply-templates select="//t:Object[ @name = 'obj1' ]" mode="testmode"/-->
		</Processed>
	</xsl:template>


	<xsl:template match="t:Object">
		<MainObject>
			<xsl:copy-of select="."/>
		</MainObject>
	</xsl:template>

	<xsl:template match="t:Object[ @name = 'obj1' ]">
		<Object1>
			<xsl:apply-imports/>
		</Object1>
	</xsl:template>

	<xsl:template match="t:Object" mode="testmode">
		<xsl:apply-imports/>
	</xsl:template>
	
	

</xsl:stylesheet>
