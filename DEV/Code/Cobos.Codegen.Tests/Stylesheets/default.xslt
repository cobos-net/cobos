<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
	xmlns:t="tempuri"					 
>
	<xsl:output method="xml" indent="yes"/>

	<xsl:template match="t:Object">
		<DefaultObject>
			<xsl:copy-of select="."/>
		</DefaultObject>
	</xsl:template>

	<xsl:template match="t:Object[ @name = 'obj1' ]">
		<DefaultObject1>
			<xsl:copy-of select="."/>
		</DefaultObject1>
	</xsl:template>

	<xsl:template match="t:Object" mode="testmode">
		<TestObject>
			<xsl:copy-of select="."/>
		</TestObject>
	</xsl:template>

</xsl:stylesheet>
