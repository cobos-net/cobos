﻿<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <!-- 
  =============================================================================
  Filename: Boilerplate.xslt
  Description: Auto-generation warning messages
  =============================================================================
  Created by: N.Davis                        Date: 2010-04-09
  Modified by:                               Date:
  =============================================================================
  

  =============================================================================
  -->
  <!-- 
  =============================================================================
  XML boilerplate.
  =============================================================================
  -->
  <xsl:template name="generatedXmlWarning">
    <xsl:comment>
===============================================================================
 &lt;auto-generated&gt;
    This code was generated by a tool.

    Changes to this file may cause incorrect behavior and will be lost if
    the code is regenerated.
 &lt;/auto-generated&gt;
===============================================================================

This file was auto-generated by Cobos SDK tools.
    </xsl:comment>
  </xsl:template>
  <!-- 
  =============================================================================
  C# boilerplate.
  =============================================================================
  -->
  <xsl:template name="generatedCSharpWarning">
    <xsl:text>// ----------------------------------------------------------------------------
// &lt;auto-generated&gt;
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// &lt;/auto-generated&gt;
// ----------------------------------------------------------------------------
//
// This source code was auto-generated by Cobos SDK tools.

</xsl:text>
  </xsl:template>
</xsl:stylesheet>
