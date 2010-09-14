<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
                xmlns:igr="http://www.intergraph.com/oz/core/logger"
>
    <xsl:output method="html" indent="yes"/>

  <xsl:template match="/">

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

        <script type="text/javascript">

          function onCheckFilter( obj )
          {
            changecss( "tr." + obj.value, "display", obj.checked ? "" : "none" );
          }

          function onChangeTextFilter( filter )
          {
            var isIE=/*@cc_on!@*/false; //IE detector

            var regEx;
            
            try
            {
              regEx = new RegExp( document.getElementById( "txtFilter" ).value, document.getElementById( "chkInsensitive" ).checked ? "i" : "" );
            }
            catch ( e )
            {
              if ( typeof e == "object" &amp;&amp; e.message )
              {
                alert( e.message );
              }
              else
              {
                alert( e );
              }
              return;
            }

            var log = document.getElementById( "tblLogFile" );

            var rows = log.rows;

            for ( var r = 0; r &lt; rows.length; ++r )
            {
              var display = regEx.exec( rows[ r ].cells[ 2 ].innerHTML ) ? "" : "display:none";
            
              if ( isIE )
              {
                rows[ r ].style.cssText = display;
              }
              else
              {
                rows[ r ].setAttribute( "style", display );
              }
            }
          }

          //changecss Copyright 2006-2008
          //http://www.shawnolson.net
          // Modified by N.Davis to provide case insensitive selector matching
          function changecss(theClass,element,value)
          {
            //Last Updated on June 23, 2009
            //documentation for this script at
            //http://www.shawnolson.net/a/503/altering-css-class-attributes-with-javascript.html
            var cssRules;

            theClass = theClass.toLowerCase();

            var added = false;
            for (var S = 0; S &lt; document.styleSheets.length; S++)
             {
                if (document.styleSheets[S]['rules']) 
                {
                  cssRules = 'rules';
                } 
                else if (document.styleSheets[S]['cssRules']) 
                {
                  cssRules = 'cssRules';
                } 
                else 
                {
                  //no rules found... browser unknown
                  return;
                }

                for (var R = 0; R &lt; document.styleSheets[S][cssRules].length; R++) 
                {
                  if (document.styleSheets[S][cssRules][R].selectorText.toLowerCase() == theClass) 
                  {
                    if (document.styleSheets[S][cssRules][R].style[element])
                    {
                      document.styleSheets[S][cssRules][R].style[element] = value;
                      added=true;
	                    break;
                    }
                  }
                }
      	      
                if (!added)
                {
                  if(document.styleSheets[S].insertRule)
                  {
		                document.styleSheets[S].insertRule(theClass+' { '+element+': '+value+'; }',document.styleSheets[S][cssRules].length);
		              } 
		              else if (document.styleSheets[S].addRule) 
		              {
			              document.styleSheets[S].addRule(theClass,element+': '+value+';');
		              }
                }
              }
            }    
            
        </script>

      </head>
      <body>
        <form id="main" action="">

          <p class="metadata">
            <table class="metadata" cellpadding="0px" cellspacing="1px">
              <xsl:apply-templates select="/igr:LogFile/igr:Metadata" mode="createMetadataTable" />
            </table>
          </p>

          <p class="filters">
            <b>Filters:</b>
            <label for="chkInformation">Information</label>
            <input type="checkbox" checked="checked" id="chkInformation" value="information" onclick="javascript:onCheckFilter(this);" />
            <label for="chkWarning">Warning</label>
            <input type="checkbox" checked="checked" id="chkWarning" value="warning" onclick="javascript:onCheckFilter(this);" />
            <label for="chkError">Error</label>
            <input type="checkbox" checked="checked" id="chkError" value="error" onclick="javascript:onCheckFilter(this);" />
            <label for="chkDebug">Debug</label>
            <input type="checkbox" checked="checked" id="chkDebug" value="debug" onclick="javascript:onCheckFilter(this);" />
            <input type="text" class="filter-text" size="60" id="txtFilter" onkeyup="javascript:onChangeTextFilter();" />
            <input type="checkbox" checked="checked" id="chkInsensitive" value="insensitive" onclick="javascript:onChangeTextFilter();" />
            <label for="chkInsensitive" class="right-aligned">Insensitive</label>
          </p>

          <table id="tblLogFile" class="logFile" cellpadding="0px" cellspacing="0px">
            <xsl:apply-templates />
          </table>
        </form>
      </body>
    </html>

  </xsl:template>

  <!-- match log information entry -->
  <xsl:template match="igr:Information">
    <tr class="information">
      <td class="messageType">
        Information
      </td>
      <td class="timestamp">
        <xsl:value-of select="@timestamp" />
      </td>
      <td class="message">
        <xsl:value-of select="text()" />
      </td>
    </tr>
  </xsl:template>

  <!-- match log warning entry -->
  <xsl:template match="igr:Warning">
    <tr class="warning">
      <td class="messageType">
        Warning
      </td>
      <td class="timestamp">
        <xsl:value-of select="@timestamp" />
      </td>
      <td class="message">
        <xsl:value-of select="text()" />
      </td>
    </tr>
  </xsl:template>

  <!-- match log error entry -->
  <xsl:template match="igr:Error">
    <tr class="error">
      <td class="messageType">
        Error
      </td>
      <td class="timestamp">
        <xsl:value-of select="@timestamp" />
      </td>
      <td class="message">
        <xsl:value-of select="text()" />
      </td>
    </tr>
  </xsl:template>

  <!-- match log error entry -->
  <xsl:template match="igr:Debug">
    <tr class="debug">
      <td class="messageType">
        Debug
      </td>
      <td class="timestamp">
        <xsl:value-of select="@timestamp" />
      </td>
      <td class="message">
        <xsl:value-of select="text()" />
      </td>
    </tr>
  </xsl:template>

  <!-- default match for metadata should ignore the elements -->
  <xsl:template match ="igr:Metadata">
  </xsl:template>

  <!-- create metadata table seperately from the logging table -->
  <xsl:template match ="igr:Metadata" mode="createMetadataTable">
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
