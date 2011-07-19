<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	 xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl igr"
					 xmlns:igr="http://www.intergraph.com/asiapac/core/logger"
>
	<xsl:output method="html" indent="yes" doctype-public="-//W3C//DTD XHTML 1.0 Strict//EN" doctype-system="http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"/>

	<xsl:template match="/igr:LogFile">
		<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en" lang="en">
			<head>
				<title>Log File</title>
				<style type="text/css">
					<![CDATA[
		body
		{
			font-family: Arial,Helvetica;
			font-size: 13px;
			color: #6A6A6A;
		}

		table.metadata
		{
			border: solid 1px #C0C0C0;
			width: 100%;
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
			width: 150px;
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
					]]>
				</style>
			

			<script type="text/javascript">

				<xsl:text disable-output-escaping="yes">
					<![CDATA[
		var _xslt = "<xsl:stylesheet version=\"1.0\" " +
							"xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" " +
							"xmlns:msxsl=\"urn:schemas-microsoft-com:xslt\" exclude-result-prefixes=\"msxsl igr\" " +
							"xmlns:xhtml=\"http://www.w3.org/1999/xhtml\" " +
							"xmlns:igr=\"http://www.intergraph.com/asiapac/core/logger\">" +
									
							"<xsl:output method=\"html\" indent=\"yes\"/>" +

							"<xsl:variable name=\"lowercase\" select=\"'abcdefghijklmnopqrstuvwxyz'\" />" +
							"<xsl:variable name=\"uppercase\" select=\"'ABCDEFGHIJKLMNOPQRSTUVWXYZ'\" />" +

							"<xsl:template match=\"##ROOT_MATCH##\">" +
								"<table class=\"logFile\" cellpadding=\"0px\" cellspacing=\"0px\">" +
									"<xsl:apply-templates select=\"##NODE_FILTER##\"/>" +
								"</table>" +
							"</xsl:template>" +

							"<xsl:template match=\"##NODE_MATCH##\">" +
								"<tr class=\"{name()}\">" +
									"<td class=\"messageType\">" +
										"<xsl:value-of select=\"position()\"/> - <xsl:value-of select=\"name()\"/>" +
									"</td>" +
									"<td class=\"timestamp\">" +
										"<xsl:value-of select=\"@timestamp\"/>" +
									"</td>" +
									"<td class=\"message\">" +
										"<xsl:value-of select=\"text()\"/>" +
									"</td>" +
								"</tr>" +
							"</xsl:template>" +
						"</xsl:stylesheet>";

	var _levels = [ "error", "warning", "information", "debug" ];

	// browser detection
	var _userAgent = navigator.userAgent.toLowerCase();
	var _isGecko = /gecko/.test( _userAgent );
	var _isWebKit = /webkit/.test( _userAgent );
	var _isIE = /msie/.test( _userAgent );
	var _isPresto = /presto/.test( _userAgent );

	function refresh()
	{
		try
		{
			renderLog();
		}
		catch ( e )
		{
			alert( message in e ? e.message : e );
		}
	}

	function onChangeTextFilter()
	{
		document.getElementById( "btn_applyTextFilter" ).disabled = "";
	}

	function onApplyTextFilter()
	{
		refresh();
		document.getElementById( "btn_applyTextFilter" ).disabled = "disabled";
	}

	function renderLog()
	{
		var logTable = document.getElementById( "logTable" );

		while ( logTable.hasChildNodes() ) 
		{
			logTable.removeChild( logTable.lastChild );
		}

		// build the based on levels
				
		var filter = [];

		for ( var i = 0; i < _levels.length; ++i )
		{
			if ( document.getElementById( "chk_" + _levels[ i ] ).checked )
			{
				if ( _isIE || _isWebKit )
				{
					filter.push( ".//igr:" + _levels[ i ] );
				}
				else if ( _isGecko || _isPresto )
				{
					filter.push( ".//" + _levels[ i ] );
				}
				else
				{
					throw "Error: Unsupported browser";
				}
			}
		}

		if ( filter.length == 0 )
		{
			return;
		}

		filter = filter.join( "|" );

		var textFilter = document.getElementById( "txtFilter" ).value;

		if ( textFilter.length != 0 )
		{
			if ( document.getElementById( "chk_insensitive" ).checked )
			{
				filter = "(" + filter + ")[contains( translate( ./text(), $lowercase, $uppercase ), '" + textFilter.toUpperCase() + "' )]";
			}
			else
			{
				filter = "(" + filter + ")[contains( ./text(), '" + textFilter + "' )]";
			}
		}

				filter = "(" + filter + ")[position() &gt;= 1 and position() &lt;= 10 ]";


		var rootMatch, nodeMatch;

		if ( _isIE )
		{
			rootMatch = "igr:LogFile";
			nodeMatch = "igr:error|igr:information|igr:warning|igr:debug";
		}
		else if ( _isWebKit )
		{
			rootMatch = "igr:logfile";
			nodeMatch = "igr:error|igr:information|igr:warning|igr:debug";
		}
		else if ( _isGecko )
		{
			rootMatch = "xhtml:logfile";
			nodeMatch = "error|information|warning|debug";
		}
		else if ( _isPresto )
		{
			rootMatch = "LogFile";
			nodeMatch = "error|information|warning|debug";
		}
		else
		{
			throw "Error: Unsupported browser";
		}

		var xslt = _xslt.replace( "##ROOT_MATCH##", rootMatch ).replace( "##NODE_FILTER##", filter ).replace( "##NODE_MATCH##", nodeMatch );

		executeTransform( xslt, getFirstChildElement( "logData" ), logTable );
	}

	function getFirstChildElement( n )
	{
		if ( typeof n === "string" )
		{
			n = document.getElementById( n );
		}

		x = n.firstChild;

		while ( x.nodeType != 1 )
		{
			x = x.nextSibling;
		}

		return x;
	}

	function loadXml( text )
	{
		var xmlDoc;
		// code for IE
		if ( window.ActiveXObject )
		{
			xmlDoc = new ActiveXObject( "Microsoft.XMLDOM" );
			xmlDoc.async = "false";
			xmlDoc.validateOnParse = "false";

			if ( !xmlDoc.loadXML( text ) )
			{
				xmlDoc = null;
			}
		}
		// code for Mozilla, Firefox, Opera, etc.
		else if ( document.implementation && document.implementation.createDocument )
		{
			var parser = new DOMParser();
			xmlDoc = parser.parseFromString( text, "text/xml" );
		}
		else
		{
			throw "XML Document Objects not supported";
		}

		return xmlDoc;
	}

	function executeTransform( stylesheet, node, element )
	{
		if (typeof stylesheet === "string")
		{
			stylesheet = loadXml( stylesheet );
		}

		if ( typeof node === "string" )
		{
			node = loadXml( node );
		}

		if (typeof element === "string")
		{
			element = document.getElementById(element);
		}

		if ( typeof XSLTProcessor != "undefined" )
		{
			// In Mozilla-based browsers, create an XSLTProcessor object and
			// tell it about the stylesheet.
			processor = new XSLTProcessor();
			processor.importStylesheet(stylesheet);
			// Transform the node into a DOM DocumentFragment.
			var fragment = processor.transformToFragment(node, document);
			// Erase the existing content of element.
			element.innerHTML = "";
			// And insert the transformed nodes.
			element.appendChild(fragment);
		}
		else if ( "transformNode" in node )
		{
			// If the node has a transformNode() function (in IE), use that.
			// Note that transformNode() returns a string.
			element.innerHTML = node.transformNode(stylesheet);
		}
		else
		{
			// Otherwise, we're out of luck.
			throw "Error: XSLT is not supported in this browser";
		}
	}
				]]>
				</xsl:text>
			</script>
			</head>
			<body onload="javascript:refresh();">
				<xml id="logData" style="display:none;">
					<xsl:apply-templates select="/" mode="embedXml"/>
				</xml>
				<form id="main" action="">

					<table class="metadata" cellpadding="0px" cellspacing="1px">
						<xsl:apply-templates select="//igr:metadata"/>
					</table>

					<p class="filters">
						<b>Level:</b>
						<label for="chk_error">Error</label>
						<input type="checkbox" checked="checked" id="chk_error" value="error" onclick="javascript:refresh();" />
						<label for="chk_warning">Warning</label>
						<input type="checkbox" checked="checked" id="chk_warning" value="warning" onclick="javascript:refresh();" />
						<label for="chk_information">Information</label>
						<input type="checkbox" checked="checked" id="chk_information" value="information" onclick="javascript:refresh();" />
						<label for="chk_debug">Debug</label>
						<input type="checkbox" checked="checked" id="chk_debug" value="debug" onclick="javascript:refresh();" />
						<b style="margin-left: 20px;">Filter:</b>
						<input type="text" class="filter-text" size="60" id="txtFilter" onkeyup="javascript:onChangeTextFilter();"/>
						<input type="checkbox" checked="unchecked" id="chk_insensitive" value="insensitive" onclick="javascript:onApplyTextFilter();" />
						<label for="chk_insensitive" class="right-aligned">Case insensitive</label>
						<input type="button" value="Apply" id="btn_applyTextFilter" onmouseup="javascript:onApplyTextFilter();" disabled="disabled"/>
					</p>

					<div id="logTable"></div>
				</form>
			</body>
		</html>

	</xsl:template>

	<xsl:template match="@* | node()" mode="embedXml">
		  <xsl:copy>
				<xsl:apply-templates select="@* | node()" mode="embedXml"/>
		  </xsl:copy>
	 </xsl:template>

	<xsl:template match="igr:metadata">
		<tr>
			<td class="name">
				<xsl:value-of select="./igr:name/text()"/>:
			</td>
			<td class="value">
				<xsl:value-of select="./igr:value/text()"/>
			</td>
		</tr>
	</xsl:template>

</xsl:stylesheet>
