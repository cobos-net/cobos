<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	 xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
					 xmlns:igr="http://www.intergraph.com/asiapac/core/logger"
>
	<xsl:output method="html" indent="yes"/>

	<xsl:template match="/igr:LogFile">

		<html>
			<head>
				<title>Log File</title>
				<style type="text/css">
					body
					{
					font-family: Arial,Helvetica;
					font-size: 13px;
					color: #6A6A6A;
					}

					table.metadata
					{
					border: none;
					font-size: 13px;
					}

					table.metadata td
					{
					padding: 3px;
					}

					table.metadata td.name
					{
					font-weight: bold;
					background-color: #ADD8E6;
					padding-left: 25px;
					padding-right: 5px;
					text-align: right;
					}

					table.metadata td.value
					{
					padding-left: 5px;
					}

					table.logFile
					{
					border: solid 1px #C0C0C0;
					border-bottom: none;
					border-right: none;
					width: 100%;
					font-family: Courier New;
					font-size: 13px;
					}

					table.logFile td
					{
					border-right: solid 1px #C0C0C0;
					border-bottom: solid 1px #C0C0C0;
					padding: 3px;
					}

					tr.information td.messageType
					{
					background-color: #FFFACD;
					width: 100px;
					}

					tr.warning td.messageType
					{
					background-color: #FFE4C4;
					width: 100px;
					}

					tr.error td.messageType
					{
					background-color: #F08080;
					width: 100px;
					}

					tr.debug td.messageType
					{
					background-color: #DEB887;
					width: 100px;
					}

					td.timestamp
					{
					width: 160px;
					}

					td.message
					{
					color: #404040;
					}

					label
					{
					font-size: 12px;
					margin-left: 10px;
					}

					label.right-aligned
					{
					font-size: 12px;
					margin-left: 0px;
					margin-right: 10px;
					}

					p.metadata
					{
					padding: 0px;
					margin: 0px;
					border: solid 1px #C0C0C0;
					}

					p.filters
					{
					background-color: #F0F0F0;
					padding: 4px;
					border: solid 1px #C0C0C0;
					font-size: 12px;
					margin-top: 7px;
					margin-bottom: 0px;
					border-bottom: none;
					}

					input.filter-text
					{
					margin-left: 10px;
					}

				</style>
			</head>
			<body>
				<form id="main" action="">

					<p class="metadata">
						<table class="metadata" cellpadding="0px" cellspacing="1px">
							<xsl:apply-templates select="//igr:Metadata"/>
						</table>
					</p>

					<p class="filters">
						<b>Level:</b>
						<label for="chkError">Error</label>
						<input type="checkbox" checked="checked" id="chkError" value="error" onclick="javascript:onCheckFilter(this);" />
						<label for="chkWarning">Warning</label>
						<input type="checkbox" checked="checked" id="chkWarning" value="warning" onclick="javascript:onCheckFilter(this);" />
						<label for="chkInformation">Information</label>
						<input type="checkbox" checked="checked" id="chkInformation" value="information" onclick="javascript:onCheckFilter(this);" />
						<label for="chkDebug">Debug</label>
						<input type="checkbox" checked="checked" id="chkDebug" value="debug" onclick="javascript:onCheckFilter(this);" />
						<b>Filter:</b>
						<input type="text" class="filter-text" size="60" id="txtFilter"/>
						<input type="checkbox" checked="checked" id="chkInsensitive" value="insensitive" onclick="javascript:onChangeTextFilter();" />
						<label for="chkInsensitive" class="right-aligned">Case insensitive</label>
						<input type="button" value="Apply" onmouseup="javascript:onChangeTextFilter();"/>
					</p>

					<table id="tblLogFile" class="logFile" cellpadding="0px" cellspacing="0px">
						<xsl:apply-templates select="(//igr:Error|//igr:Warning)[contains( ./text(), 'test message' )]" />
					</table>
				</form>
			</body>
		</html>

	</xsl:template>

	<xsl:template match="igr:Error|igr:Warning|igr:Information|igr:Debug">
		<tr class="{name()}">
			<td class="messageType">
				<xsl:value-of select="name()"/>
			</td>
			<td class="timestamp">
				<xsl:value-of select="@timestamp" />
			</td>
			<td class="message">
				<xsl:value-of select="text()" />
			</td>
		</tr>
	</xsl:template>

	<xsl:template match="igr:Metadata">
		<tr>
			<td class="name">
				<xsl:value-of select="./igr:Name/text()"/>:
			</td>
			<td class="value">
				<xsl:value-of select="./igr:Value/text()"/>
			</td>
		</tr>
	</xsl:template>

</xsl:stylesheet>
