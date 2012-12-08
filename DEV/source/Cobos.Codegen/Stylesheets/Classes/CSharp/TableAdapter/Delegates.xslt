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
	DataTable events: pass on any row changing events to our delegates.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	
	
	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Object" mode="delegatesDeclarations">

		<xsl:call-template name="delegateMethodBody">
			<xsl:with-param name="methodName">_table_RowChanging</xsl:with-param>
			<xsl:with-param name="rowEventType">DataRowChangeEventArgs</xsl:with-param>
			<xsl:with-param name="delegateType">OnModifying</xsl:with-param>
		</xsl:call-template>

		<xsl:call-template name="delegateMethodBody">
			<xsl:with-param name="methodName">_table_RowChanged</xsl:with-param>
			<xsl:with-param name="rowEventType">DataRowChangeEventArgs</xsl:with-param>
			<xsl:with-param name="delegateType">OnModified</xsl:with-param>
		</xsl:call-template>

		<xsl:call-template name="delegateMethodBody">
			<xsl:with-param name="methodName">_table_RowDeleting</xsl:with-param>
			<xsl:with-param name="rowEventType">DataRowChangeEventArgs</xsl:with-param>
			<xsl:with-param name="delegateType">OnDeleting</xsl:with-param>
		</xsl:call-template>

		<xsl:call-template name="delegateMethodBody">
			<xsl:with-param name="methodName">_table_RowDeleted</xsl:with-param>
			<xsl:with-param name="rowEventType">DataRowChangeEventArgs</xsl:with-param>
			<xsl:with-param name="delegateType">OnDeleted</xsl:with-param>
		</xsl:call-template>

		<xsl:call-template name="delegateMethodBody">
			<xsl:with-param name="methodName">_table_TableNewRow</xsl:with-param>
			<xsl:with-param name="rowEventType">DataTableNewRowEventArgs</xsl:with-param>
			<xsl:with-param name="delegateType">OnAdded</xsl:with-param>
		</xsl:call-template>

	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Delegate generator: pass on any row changing events to our delegates.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		
		public event OnAddedDelegate OnAddedInstanceLineup;

		public delegate void OnAddedDelegate( InstanceLineup @object );
	
		void _table_TableNewRow( object sender, DataTableNewRowEventArgs e )
		{
			if ( OnAddedInstanceLineup != null )
			{
				LineupDataModel.InstanceLineupRow row = e.Row as LineupDataModel.InstanceLineupRow;

				if ( row != null )
				{
					OnAddedInstanceLineup( new InstanceLineup( row ) );
				}
			}
		}
	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template name="delegateMethodBody">
		<xsl:param name="methodName"/>
		<xsl:param name="rowEventType"/>
		<xsl:param name="delegateType"/>
		<xsl:variable name="delegateName" select="concat( $delegateType, @className )"/>
		
		public event <xsl:value-of select="$delegateType"/>Delegate <xsl:value-of select="$delegateName"/>;

		public delegate void <xsl:value-of select="$delegateType"/>Delegate( <xsl:value-of select="@className"/> @object );

		void <xsl:value-of select="$methodName"/>( object sender, <xsl:value-of select="$rowEventType"/> e )
		{
			if ( <xsl:value-of select="$delegateName"/> != null )
			{
				<xsl:value-of select="@datasetRowType"/> row = e.Row as <xsl:value-of select="@datasetRowType"/>;

				if ( row != null )
				{
					<xsl:value-of select="$delegateName"/>( new <xsl:value-of select="@className"/>( row ) );
				}
			}
		}
	</xsl:template>
					 
</xsl:stylesheet>