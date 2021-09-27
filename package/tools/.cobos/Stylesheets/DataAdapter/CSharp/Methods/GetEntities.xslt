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
  Filename: GetEntities.xslt
  Description: Methods to get entities from the data table.
  ============================================================================
  Created by: N.Davis                        Date: 2010-04-09
  Modified by:                               Date:
  ============================================================================
  Notes: 
	
	
  ============================================================================
  -->
  <!--
  ============================================================================
  getEntityBy method body: Find the object using the primary key fields.
  Only add this method if the table has primary key or unique key constraints.
  ============================================================================
	
  ============================================================================
  -->
  <xsl:template match="cobos:Object" mode="getEntityByMethodBody">
    <xsl:if test="$databaseConstraintsNodeSet/*[self::xsd:key|self::xsd:unique][translate(xsd:selector/@xpath, $lowercase, $uppercase) = concat('.//', translate(current()/@dbTable, $lowercase, $uppercase))]">
        /// &lt;summary&gt;
        /// Get the entity using the primary key fields.
        /// &lt;/summary&gt;
        public <xsl:value-of select="@className"/> GetEntityBy<xsl:apply-templates select="." mode="getEntityByMethodName"/>(<xsl:apply-templates select="." mode="getEntityByMethodArgumentList"/>)
        {
            <xsl:apply-templates select="." mode="getEntityByMethodImpl"/>
        }
    </xsl:if>
  </xsl:template>
	<!--
	============================================================================
	Argument list: string LineupName, string UnitId.
	============================================================================
	-->
  <xsl:template match="cobos:Object" mode="getEntityByMethodArgumentList">
    <xsl:variable name="object" select="."/>
    <xsl:for-each select="$databaseConstraintsNodeSet//xsd:key//xsd:field[translate(../xsd:selector/@xpath, $lowercase, $uppercase) = concat('.//', translate($object/@dbTable, $lowercase, $uppercase))]">
      <!-- Get the [1]st node since we will get multiple node results for db fields 
					that are decomposed into more than property, e.g. converter="StringSeperator" -->
      <xsl:variable name="property" select="$object//cobos:Property[translate(@dbColumn, $lowercase, $uppercase) = translate(current()/@xpath, $lowercase, $uppercase)][1]"/>
      <xsl:variable name="propertyType">
        <xsl:apply-templates select="$property" mode="propertyType"/>
      </xsl:variable>
	  <xsl:value-of select="concat($propertyType, ' ', $property/@name)"/>
      <xsl:if test="not(position() = last())">
	    <xsl:value-of select="string(', ')"/>
      </xsl:if>
    </xsl:for-each>
  </xsl:template>
  <!--
  ============================================================================
  Method body: 
  ============================================================================
  -->
  <xsl:template match="cobos:Object" mode="getEntityByMethodImpl">
            var found = this.Table.FindBy<xsl:apply-templates select="." mode="getEntityByMethodName"/>(<xsl:apply-templates select="." mode="getEntityByMethodParams"/>);

            if (found == null || found.RowState == global::System.Data.DataRowState.Deleted)
            {
                return null;
            }

            return new <xsl:value-of select="@className"/>(found);
  </xsl:template>
  <!--
	============================================================================
	Method name: getEntityByLineupNameUnitId.
	============================================================================
	-->
  <xsl:template match="cobos:Object" mode="getEntityByMethodName">
    <xsl:variable name="object" select="."/>
    <xsl:for-each select="$databaseConstraintsNodeSet//xsd:key//xsd:field[translate(../xsd:selector/@xpath, $lowercase, $uppercase) = concat('.//', translate($object/@dbTable, $lowercase, $uppercase))]">
      <xsl:variable name="property" select="$object//cobos:Property[not(@dbAlias) and (translate(@dbColumn, $lowercase, $uppercase) = translate(current()/@xpath, $lowercase, $uppercase))]"/>
      <xsl:apply-templates select="$property" mode="getEntityByMethodNamePath"/>
    </xsl:for-each>
  </xsl:template>
  <!--
	============================================================================
	-->
  <xsl:template match="cobos:Property|cobos:Object" mode="getEntityByMethodNamePath">
    <xsl:apply-templates select="parent::cobos:Object[parent::cobos:Object]" mode="getEntityByMethodNamePath"/>
    <xsl:value-of select="@name"/>
  </xsl:template>
  <!--
	============================================================================
	Method parameters
	============================================================================
	-->
  <xsl:template match="cobos:Object" mode="getEntityByMethodParams">
    <xsl:variable name="object" select="."/>
    <xsl:for-each select="$databaseConstraintsNodeSet//xsd:key//xsd:field[translate(../xsd:selector/@xpath, $lowercase, $uppercase) = concat('.//', translate($object/@dbTable, $lowercase, $uppercase))]">
      <xsl:apply-templates select="$object//cobos:Property[not(@dbAlias) and (translate(@dbColumn, $lowercase, $uppercase) = translate(current()/@xpath, $lowercase, $uppercase))]" mode="getEntityByMethodParamValue"/>
      <xsl:if test="not(position() = last())">
        <xsl:text>, </xsl:text>
      </xsl:if>
    </xsl:for-each>
  </xsl:template>

  <xsl:template match="cobos:Property" mode="getEntityByMethodParamValue">
    <xsl:value-of select="@name"/>
  </xsl:template>
  
  <xsl:template match="cobos:Property[@converter]" mode="getEntityByMethodParamValue">
    <xsl:variable name="converterTargetType">
      <xsl:apply-templates select="@converterTargetType" mode="propertyType"/>
    </xsl:variable>
    <xsl:variable name="converterSourceType">
      <xsl:apply-templates select="@dbType" mode="propertyType"/>
    </xsl:variable>
    <xsl:variable name="converterParameter">
      <xsl:apply-templates select="." mode="converterParameter"/>
    </xsl:variable>
    <xsl:value-of select="concat('(', $converterSourceType ,')(new ', @converter, '().ConvertBack(', @name, ', typeof(', $converterTargetType, '), ', $converterParameter, '))')"/>
  </xsl:template>
  <!--
	============================================================================
	GetEntities: Gets all entities stored in the data table.
	============================================================================
	
	

	
	============================================================================
	-->

  <xsl:template match="cobos:Object" mode="getEntitiesMethodBody">
        /// &lt;summary&gt;
        /// Get all entities stored in the data table.
        /// &lt;/summary&gt;
        /// &lt;returns&gt;A collection containing the entities stored in the table.&lt;/returns&gt;
        public <xsl:apply-templates select="." mode="listDecl"/> GetEntities()
        {
            int numRows = this.Table.Rows.Count;

            for (int row = 0; row &lt; numRows; ++row)
            {
                if (this.Table[row].RowState != global::System.Data.DataRowState.Deleted)
                {
                    yield return new <xsl:value-of select="@className"/>(this.Table[row]);
                }
            }
        }
  </xsl:template>
</xsl:stylesheet>