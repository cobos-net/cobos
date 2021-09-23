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
  CreateNew method body: Creates a new data object.  
  ============================================================================
	
	

	
  ============================================================================
  -->
  <xsl:template match="cobos:Object" mode="newMethodBody">
        /// &lt;summary&gt;
        /// Creates a new data object.
        /// &lt;/summary&gt;
        public <xsl:value-of select="@className"/> New<xsl:value-of select="@className"/>()
        {
            return new <xsl:value-of select="@className"/>(this.Table.New<xsl:value-of select="@className"/>Row());
        }
  </xsl:template>

  <!--
  ============================================================================
  Add method body: Adds the newly created object to the model.  
  ============================================================================
	
	

	
  ============================================================================
  -->

  <xsl:template match="cobos:Object" mode="addMethodBody">
        /// &lt;summary&gt;
        /// Adds a new object to the table.
        /// &lt;/summary&gt;
        /// &lt;param name="value"&gt;The new object to add.&lt;/param&gt;
        /// &lt;remarks&gt;
        /// If the object needs to be initialised with primary or unique key constraints 
        /// then make sure that it's done before adding to the model.
        /// &lt;/remarks&gt;
        public <xsl:value-of select="@className"/> Add<xsl:value-of select="@className"/>(<xsl:value-of select="@className"/> value)
        {
            this.Table.Add<xsl:value-of select="@className"/>Row(value.DataRowSource);
            return value;
        }
  </xsl:template>
</xsl:stylesheet>