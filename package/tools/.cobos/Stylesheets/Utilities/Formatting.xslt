<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <!-- 
  =============================================================================
  Filename: Formatting.xslt
  Description: Text Formatting constants
  =============================================================================
  Created by: N.Davis                        Date: 2010-04-09
  Modified by:                               Date:
  =============================================================================
  Common includes and definitions

  =============================================================================
  -->
  <!-- 
  =============================================================================
  Whitespace formatting.
  =============================================================================
  -->
  <xsl:variable name="newline" select="string('&#xD;&#xA;')"/>
  <xsl:variable name="tab" select="string('&#9;')"/>
  <xsl:variable name="space" select="string('    ')"/>
  <!-- 
  =============================================================================
	XML reserved characters for use in concat function
  =============================================================================
  -->
  <xsl:variable name="apos">'</xsl:variable>
  <xsl:variable name="quot">"</xsl:variable>
  <xsl:variable name="lt" select="string('&#x3C;')"/>
  <xsl:variable name="gt" select="string('&#x3E;')"/>
  <xsl:variable name="amp" select="string('&#x26;')"/>
</xsl:stylesheet>
