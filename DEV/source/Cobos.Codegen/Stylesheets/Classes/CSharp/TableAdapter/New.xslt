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
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Created by: N.Davis                        Date: 2010-04-09
	Modified by:                               Date:
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Notes: 
	
	
	============================================================================
	-->
					 
	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	CreateNew method body: Creates a new data object.  
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	
	

	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Object" mode="createNewMethodBody">
		/// <xsl:text disable-output-escaping="yes"><![CDATA[<summary>]]></xsl:text>
		/// Creates a new data object.
		/// <xsl:text disable-output-escaping="yes"><![CDATA[</summary>]]></xsl:text>
		public <xsl:value-of select="@className"/> CreateNew<xsl:value-of select="@className"/>()
		{
			<xsl:value-of select="@datasetRowType"/> row = _table.New<xsl:value-of select="@className"/>Row();

			return new <xsl:value-of select="@className"/>( row );
		}
	</xsl:template>


	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	AddNew method body: Adds the newly created object to the model.  
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	
	

	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Object" mode="addNewMethodBody">
		/// <xsl:text disable-output-escaping="yes"><![CDATA[<summary>]]></xsl:text>
		/// If the object needs to be initialised with primary or unique key constraints 
		/// then make sure that it's done before adding to the model.
		/// <xsl:text disable-output-escaping="yes"><![CDATA[</summary>]]></xsl:text>
		public <xsl:value-of select="@className"/> AddNew<xsl:value-of select="@className"/>( <xsl:value-of select="@className"/> @object)
		{
			_table.Rows.Add( @object.ObjectDataRow );
			
			return @object;
		}
	</xsl:template>
					 
</xsl:stylesheet>