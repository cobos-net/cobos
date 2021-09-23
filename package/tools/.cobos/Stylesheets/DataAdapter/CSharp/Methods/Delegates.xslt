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
	DataTable Delegates: pass on any row changing events to our delegates.
	============================================================================
	
	
	
	============================================================================
	-->
  <xsl:template match="cobos:Object" mode="delegatesDeclarations">
		<xsl:call-template name="delegateMethodBody">
			<xsl:with-param name="methodName">Table_RowChanging</xsl:with-param>
			<xsl:with-param name="rowEventType">global::System.Data.DataRowChangeEventArgs</xsl:with-param>
			<xsl:with-param name="delegateType">OnChanging</xsl:with-param>
		</xsl:call-template>

		<xsl:call-template name="delegateMethodBody">
			<xsl:with-param name="methodName">Table_RowChanged</xsl:with-param>
			<xsl:with-param name="rowEventType">global::System.Data.DataRowChangeEventArgs</xsl:with-param>
			<xsl:with-param name="delegateType">OnChanged</xsl:with-param>
		</xsl:call-template>

		<xsl:call-template name="delegateMethodBody">
			<xsl:with-param name="methodName">Table_RowDeleting</xsl:with-param>
			<xsl:with-param name="rowEventType">global::System.Data.DataRowChangeEventArgs</xsl:with-param>
			<xsl:with-param name="delegateType">OnDeleting</xsl:with-param>
		</xsl:call-template>

		<xsl:call-template name="delegateMethodBody">
			<xsl:with-param name="methodName">Table_RowDeleted</xsl:with-param>
			<xsl:with-param name="rowEventType">global::System.Data.DataRowChangeEventArgs</xsl:with-param>
			<xsl:with-param name="delegateType">OnDeleted</xsl:with-param>
		</xsl:call-template>

		<xsl:call-template name="delegateMethodBody">
			<xsl:with-param name="methodName">Table_TableNewRow</xsl:with-param>
			<xsl:with-param name="rowEventType">global::System.Data.DataTableNewRowEventArgs</xsl:with-param>
			<xsl:with-param name="delegateType">OnAdded</xsl:with-param>
		</xsl:call-template>
	</xsl:template>
	<!--
	============================================================================
	Delegate generator: pass on any row changing events to our delegates.
	============================================================================
			
	============================================================================
	-->
  <xsl:template name="delegateMethodBody">
    <xsl:param name="methodName"/>
    <xsl:param name="rowEventType"/>
    <xsl:param name="delegateType"/>
    <xsl:variable name="delegateName" select="concat($delegateType, @className)"/>
        /// &lt;summary&gt;
        /// Delegate called when table data has changed.
        /// &lt;/summary&gt;
        /// &lt;param name="sender"&gt;The source of the event.&lt;/param&gt;
        /// &lt;param name="e"&gt;The event arguments.&lt;/param&gt;     
        private void <xsl:value-of select="$methodName"/>(object sender, <xsl:value-of select="$rowEventType"/> e)
        {
            var handler = this.<xsl:value-of select="$delegateName"/>;
        
            if (handler != null)
            {
                var row = e.Row as <xsl:value-of select="@datasetRowType"/>;

                if (row != null)
                {
                    handler(new <xsl:value-of select="@className"/>(row));
                }
            }
        }
  </xsl:template>
</xsl:stylesheet>