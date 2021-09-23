<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
					 xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
					 xmlns:msxsl="urn:schemas-microsoft-com:xslt"
					 xmlns:cobos="http://schemas.cobos.co.uk/datamodel/1.0.0"
					 xmlns="http://schemas.cobos.co.uk/datamodel/1.0.0"
					 xmlns:xsd="http://www.w3.org/2001/XMLSchema"
					 exclude-result-prefixes="msxsl">

  <!-- 
	=============================================================================
	Filename: .xslt
	Description: 
	============================================================================
	Created by: N.Davis                        Date: 2010-04-09
	Modified by:                               Date:
	============================================================================
	Notes: 
	
	
	============================================================================
	-->

  <!--
	============================================================================
	SQL DELETE statement WHERE clause - list of PK fields, handling deleted rows
	============================================================================
	-->

  <xsl:template match="cobos:Object" mode="sqlDeleteWhere">
    <xsl:variable name="object" select="."/>
    <xsl:text>"(</xsl:text>
    <xsl:for-each select="$databaseConstraintsNodeSet//xsd:field[translate(../xsd:selector/@xpath, $lowercase, $uppercase) = concat('.//', translate($object/@dbTable, $lowercase, $uppercase))]">
      <xsl:apply-templates select="$object//cobos:Property[translate(@dbColumn, $lowercase, $uppercase) = translate(current()/@xpath, $lowercase, $uppercase)]" mode="sqlDeleteWhere"/>
      <xsl:if test="not(position() = last())">
        <xsl:text> AND </xsl:text>
      </xsl:if>
    </xsl:for-each>
    <xsl:text>)"</xsl:text>
  </xsl:template>

  <xsl:template match="cobos:Property[@dbType = 'xsd:string' or contains(@dbType, 'string_')]" mode="sqlDeleteWhere">
    <xsl:variable name="columnName">
      <xsl:apply-templates mode="fullName" select="."/>
    </xsl:variable>
    <xsl:value-of select="concat(@dbColumn, ' = ', $apos, $quot, ' + row[', $quot, @dbColumn, $quot, ', global::System.Data.DataRowVersion.Original].ToString() + ', $quot, $apos)"/>
  </xsl:template>

  <!-- -->
  <xsl:template match="cobos:Property[not(@dbType = 'xsd:string' or contains(@dbType, 'string_'))]" mode="sqlDeleteWhere">
    <xsl:variable name="columnName">
      <xsl:apply-templates mode="fullName" select="."/>
    </xsl:variable>
    <xsl:value-of select="concat(@dbColumn, ' = ', $quot, ' + row[', $quot, @dbColumn, $quot, ', global::System.Data.DataRowVersion.Original].ToString() + ', $quot)"/>
  </xsl:template>

</xsl:stylesheet>