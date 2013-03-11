<xsl:stylesheet version="1.0"
						xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
						xmlns:msxsl="urn:schemas-microsoft-com:xslt"
						exclude-result-prefixes="msxsl"
						xmlns="http://schemas.cobos.co.uk/datamodel/1.0.0"
						xmlns:cobos="http://schemas.cobos.co.uk/datamodel/1.0.0"
						xmlns:xsd="http://www.w3.org/2001/XMLSchema"
>
	<!-- 
	=============================================================================
	Filename: Custom.xslt
	Description: Custom extensions for schema and code generation
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Created by: N.Davis                        Date: 2010-04-09
	Modified by:                               Date:
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Usually for custom 

	============================================================================
	-->

	<!-- 
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Add your own namespace declarations here
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->
	<xsl:template name="customNamespaceDeclarations">
	</xsl:template>

	<!-- 
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Define how your data provider implements the System.Data accessors.
	In most cases the System.Data.Common implementations are fine, but some 
	third party data providers don't derive from the common implementations,
	so you may need to derive from the System.Data interfaces.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:variable name="dbConnectionType">DbConnection</xsl:variable>
	<xsl:variable name="dbTransactionType">DbTransaction</xsl:variable>
	<xsl:variable name="dbCommandType">DbCommand</xsl:variable>
	<xsl:variable name="dataAdapterType">DbDataAdapter</xsl:variable>
	
	<!-- 
	<xsl:variable name="dbConnectionType">IDbConnection</xsl:variable>
	<xsl:variable name="dbTransactionType">IDbTransaction</xsl:variable>
	<xsl:variable name="dbCommandType">IDbCommand</xsl:variable>
	<xsl:variable name="dataAdapterType">IDbDataAdapter</xsl:variable>
	-->
	
	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Add your own data object extensions here
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Object" mode="customDataObjectExtensions">
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Add your own TableAdapter extensions here
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Object" mode="customTableAdapterExtensions">
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Add your own custom code extensions here
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Object" mode="customCustomCodeExtensions">
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	match property types
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<!--
	<xsl:template match="cobos:Property[ @stringFormat = 'MyCustomDateFormat' ]" mode="propertyType">
		<xsl:text>DateTime</xsl:text>
	</xsl:template>
	-->

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	get function body implementation for special string formats
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<!--
	<xsl:template match="cobos:Property[ @stringFormat = 'MyCustomDateFormat' ]" mode="propertyGet">
		<xsl:variable name="columnName">
			<xsl:apply-templates mode="qualifiedName" select="."/>
		</xsl:variable>
		<xsl:value-of select="concat( 'return MyDateFormatter.ConvertFromTimeStamp(this.ObjectDataRow.', $columnName, ');' )"/>
	</xsl:template>
	-->

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	set function body implementation for special string formats
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<!--
	<xsl:template match="cobos:Property[ @stringFormat = 'MyCustomDateFormat' ]" mode="propertySet">
		<xsl:variable name="columnName">
			<xsl:apply-templates mode="qualifiedName" select="."/>
		</xsl:variable>
		<xsl:variable name="value">
			<xsl:apply-templates mode="propertySetValue" select="."/>
		</xsl:variable>
		<xsl:value-of select="concat( 'this.ObjectDataRow.', $columnName, ' = MyDateFormatter.ConvertToTimeStamp(', $value, ');' )"/>
	</xsl:template>
	-->

	<!-- Nullable string formatted types -->
	<!-- 
	<xsl:template match="cobos:Property[ @minOccurs = 0 and ( @stringFormat = 'MyCustomDateFormat' ) ]" mode="propertySetValue">
		<xsl:text>value.Value</xsl:text>
	</xsl:template>
	-->

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	find by methods
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<!--
	<xsl:template match="cobos:Property[ @stringFormat = 'MyCustomDateFormat' ]" mode="findByMethodParamValue">
		<xsl:value-of select="concat( 'MyDateFormatter.ConvertToTimeStamp(', @name, ')' )"/>
	</xsl:template>
	-->
	
</xsl:stylesheet>
