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
	SQL UPDATE columns
	============================================================================
	-->

  <!-- simple case for non-nullable fields -->
  <xsl:template match="cobos:Property[ @minOccurs = 1]" mode="sqlUpdateValue">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndentLevel3"/>
    </xsl:variable>
    <xsl:value-of select="concat($indent, 'buffer.Append(', $quot, @dbColumn, '=',  $quot, ');')"/>
    <xsl:apply-templates select="." mode="sqlPropertyRow"/>
    <xsl:if test="not(position() = last())">
      <xsl:value-of select="concat($indent, 'buffer.Append(', $quot, ', ', $quot, ');')"/>
    </xsl:if>
  </xsl:template>

  <!-- test for null for nullable fields before inserting -->
  <xsl:template match="cobos:Property[ @minOccurs = 0]" mode="sqlUpdateValue">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndentLevel3"/>
    </xsl:variable>
    <xsl:value-of select="concat($indent, 'buffer.Append(', $quot, @dbColumn, '=',  $quot, ');')"/>
    <xsl:apply-templates select="." mode="sqlPropertyRow"/>
    <xsl:if test="not(position() = last())">
      <xsl:value-of select="concat($indent, 'buffer.Append(', $quot, ', ', $quot, ');')"/>
    </xsl:if>
  </xsl:template>

  <!--
	============================================================================
	SQL UPDATE statement WHERE clause - list of PK fields
	============================================================================
	-->

  <xsl:template match="cobos:Object" mode="sqlUpdateWhere">
    <xsl:variable name="object" select="."/>
    <xsl:text>"(</xsl:text>
    <xsl:for-each select="$databaseConstraintsNodeSet//xsd:field[translate(../xsd:selector/@xpath, $lowercase, $uppercase) = concat('.//', translate($object/@dbTable, $lowercase, $uppercase))]">
      <xsl:apply-templates select="$object//cobos:Property[translate(@dbColumn, $lowercase, $uppercase) = translate(current()/@xpath, $lowercase, $uppercase)]"
										mode="sqlUpdateWhere"/>
      <xsl:if test="not(position() = last())">
        <xsl:text> AND </xsl:text>
      </xsl:if>
    </xsl:for-each>
    <xsl:text>)"</xsl:text>
  </xsl:template>

  <xsl:template match="cobos:Property[@dbType = 'xsd:string' or contains(@dbType, 'string_')]" mode="sqlUpdateWhere">
    <xsl:variable name="columnName">
      <xsl:apply-templates mode="fullName" select="."/>
    </xsl:variable>
    <xsl:value-of select="concat(@dbColumn, ' = ', $apos, $quot, ' + row.', $columnName, ' + ', $quot, $apos)"/>
  </xsl:template>

  <!-- -->
  <xsl:template match="cobos:Property[not(@dbType = 'xsd:string' or contains(@dbType, 'string_'))]" mode="sqlUpdateWhere">
    <xsl:variable name="columnName">
      <xsl:apply-templates mode="fullName" select="."/>
    </xsl:variable>
    <xsl:value-of select="concat(@dbColumn, ' = ', $quot, ' + row.', $columnName, ' + ', $quot)"/>
  </xsl:template>

</xsl:stylesheet>