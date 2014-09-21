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
	Filename: Filter.xslt
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
	GetChildFilter: Get a filter for selecting records in a child table.
	============================================================================
	============================================================================
	-->
  <xsl:template match="cobos:Object" mode="filterMethodBody">
        /// &lt;summary&gt;
        /// Gets a filter for the referencing parent and child tables by property values.
        /// &lt;/summary&gt;
        /// &lt;typeparam name="T"&gt;The property type.&lt;/typeparam&gt;
        /// &lt;param name="propertyName"&gt;The name of the parent property.&lt;/param&gt;
        /// &lt;param name="childPropertyName"&gt;The name of the child property.&lt;/param&gt;
        /// &lt;returns&gt;A filter containing the parent column values.&lt;/returns&gt;
        public Cobos.Data.Filter.Filter GetChildFilter&lt;T&gt;(string propertyName, string childPropertyName)
        {
            var propertyMap = Cobos.Data.Mapping.PropertyMapRegistry.Instance[typeof(<xsl:value-of select="@className"/>)];
            var columnInfo = propertyMap[propertyName];

            var values = Cobos.Data.Utilities.DataTableHelper.GetColumnValues&lt;T&gt;(this.Table, columnInfo.Column);

            if (values == null || values.Count == 0)
            {
                return null;
            }

            var filter = new Cobos.Data.Filter.Filter();
            filter.Predicate = Cobos.Data.Filter.BinaryLogicOp.FromList&lt;Cobos.Data.Filter.Or, Cobos.Data.Filter.PropertyIsEqualTo, T&gt;(childPropertyName, values);

            return filter;
        }
  </xsl:template>
</xsl:stylesheet>