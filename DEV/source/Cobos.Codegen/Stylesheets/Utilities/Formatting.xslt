<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" 
				xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
				xmlns:msxsl="urn:schemas-microsoft-com:xslt" 
				exclude-result-prefixes="msxsl">

	<!-- 
	=============================================================================
	Filename: Formatting.xslt
	Description: Text Formatting constants
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Created by: N.Davis                        Date: 2010-04-09
	Modified by:                               Date:
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Common includes and definitions

	============================================================================
	-->

	<!-- formatting text output -->
	<xsl:variable name="newline" select="string('&#xD;&#xA;')"/>
  
  <!-- tab based formatting -->
  <xsl:variable name="tab" select="string('&#9;')"/>
  <xsl:variable name="tab2" select="concat($tab, $tab)"/>
  <xsl:variable name="tab3" select="concat($tab, $tab, $tab)"/>
  <xsl:variable name="tab4" select="concat($tab, $tab, $tab, $tab)"/>
  <xsl:variable name="tab5" select="concat($tab, $tab, $tab, $tab, $tab)"/>
  <xsl:variable name="tab6" select="concat($tab, $tab, $tab, $tab, $tab, $tab)"/>
  <xsl:variable name="tab7" select="concat($tab, $tab, $tab, $tab, $tab, $tab, $tab)"/>
  <xsl:variable name="tab8" select="concat($tab, $tab, $tab, $tab, $tab, $tab, $tab, $tab)"/>

  <xsl:variable name="newlineTab" select="concat($newline, $tab)"/>
	<xsl:variable name="newlineTab2" select="concat($newline, $tab2)"/>
	<xsl:variable name="newlineTab3" select="concat($newline, $tab3)"/>
	<xsl:variable name="newlineTab4" select="concat($newline, $tab4)"/>
	<xsl:variable name="newlineTab5" select="concat($newline, $tab5)"/>
	<xsl:variable name="newlineTab6" select="concat($newline, $tab6)"/>
	<xsl:variable name="newlineTab7" select="concat($newline, $tab7)"/>
	<xsl:variable name="newlineTab8" select="concat($newline, $tab8)"/>
	
<!-- indent based formatting -->
<xsl:variable name="indent" select="string('    ')"/>
<xsl:variable name="indent2" select="concat($indent, $indent)"/>
<xsl:variable name="indent3" select="concat($indent, $indent, $indent)"/>
<xsl:variable name="indent4" select="concat($indent, $indent, $indent, $indent)"/>
<xsl:variable name="indent5" select="concat($indent, $indent, $indent, $indent, $indent)"/>
<xsl:variable name="indent6" select="concat($indent, $indent, $indent, $indent, $indent, $indent)"/>
<xsl:variable name="indent7" select="concat($indent, $indent, $indent, $indent, $indent, $indent, $indent)"/>
<xsl:variable name="indent8" select="concat($indent, $indent, $indent, $indent, $indent, $indent, $indent, $indent)"/>

<xsl:variable name="newlineIndent" select="concat($newline, $indent)"/>
<xsl:variable name="newlineIndent2" select="concat($newline, $indent2)"/>
<xsl:variable name="newlineIndent3" select="concat($newline, $indent3)"/>
<xsl:variable name="newlineIndent4" select="concat($newline, $indent4)"/>
<xsl:variable name="newlineIndent5" select="concat($newline, $indent5)"/>
<xsl:variable name="newlineIndent6" select="concat($newline, $indent6)"/>
<xsl:variable name="newlineIndent7" select="concat($newline, $indent7)"/>
<xsl:variable name="newlineIndent8" select="concat($newline, $indent8)"/>

  <!-- XML reserved characters for use in concat function -->
	<xsl:variable name="apos">'</xsl:variable>
	<xsl:variable name="quot">"</xsl:variable>
  <xsl:variable name="lt" select="string('&#x3C;')"/>
  <xsl:variable name="gt" select="string('&#x3E;')"/>
  <xsl:variable name="amp" select="string('&#x26;')"/>

</xsl:stylesheet>
