<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl igr" xmlns:igr="http://www.intergraph.com/asiapac/core/logger">
	<xsl:output method="html" indent="yes"/>
	<xsl:template match="/igr:LogFile">
		<table class="logFile" cellpadding="0px" cellspacing="0px">
			<xsl:apply-templates select="//igr:Error|//igr:Warning|//igr:Information|//igr:Debug"/>
		</table>
	</xsl:template>
	<xsl:template match="igr:Error|igr:Warning|igr:Information|igr:Debug">
		<tr class="{name()}">
			<td class="messageType">
				<xsl:value-of select="name()"/>
			</td>
			<td class="timestamp">
				<xsl:value-of select="@timestamp"/>
			</td>
			<td class="message">
				<xsl:value-of select="text()"/>
			</td>
		</tr>
	</xsl:template>
</xsl:stylesheet>