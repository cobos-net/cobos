<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
					 xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
					 xmlns:msxsl="urn:schemas-microsoft-com:xslt"
					 xmlns:cobos="http://schemas.cobos.co.uk/datamodel/1.0.0"
					 xmlns="http://schemas.cobos.co.uk/datamodel/1.0.0"
					 xmlns:xsd="http://www.w3.org/2001/XMLSchema"
					 exclude-result-prefixes="msxsl">


  <!-- 
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Top level classes.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

  <xsl:template match="cobos:DataModel/cobos:Object" mode="classConstructorDeclaration">

        public <xsl:value-of select="@typeName"/>(<xsl:value-of select="@datasetRowType"/> dataRow)
        {
    <xsl:text>        this.ObjectDataRow = dataRow;</xsl:text>
    <xsl:apply-templates select="cobos:Object" mode="classConstructorBody"/>
    <xsl:apply-templates select="cobos:Reference" mode="classConstructorBody"/>
        }
  </xsl:template>

  <!-- 
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Nested classes.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

  <xsl:template match="cobos:Object/cobos:Object[ not( ancestor::cobos:Interface ) ]" mode="classConstructorDeclaration">
    <xsl:variable name="datasetRowType">
      <xsl:apply-templates select="." mode="datasetRowType"/>
    </xsl:variable>
        
        public <xsl:value-of select="@typeName"/>(<xsl:value-of select="$datasetRowType"/> dataRow)
        {
    <xsl:text>        this.ObjectDataRow = dataRow;</xsl:text>
    <xsl:apply-templates select="cobos:Object" mode="classConstructorBody"/>
    <xsl:apply-templates select="cobos:Reference" mode="classConstructorBody"/>
        }
  </xsl:template>


  <!-- 
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Construct the nested objects.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

  <xsl:template match="cobos:Object" mode="classConstructorBody">
    <xsl:value-of select="concat( $newlineIndent3, 'this.', @memberName, ' = new ', @typeName, '(this.ObjectDataRow);' )"/>
  </xsl:template>

  <!-- 
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Get the single child row for this reference.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

		EventDataModel.DispositionRow[] childDisposition = this.ObjectDataRow.GetDisposition();

		if (childDisposition != null && childDisposition.Length > 0)
		{
			this.dispositionField = new Disposition(childDisposition[0]);
		}
	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->
  <xsl:template match="cobos:Reference[ not( @isCollection ) ]" mode="classConstructorBody">
    <xsl:value-of select="concat($newline, $newlineIndent3)"/>
    <xsl:value-of select="@datasetRowType"/>[] child<xsl:value-of select="@name"/> = this.ObjectDataRow.Get<xsl:value-of select="@name"/>();

            if (<xsl:apply-templates select="." mode="classConstructorBodyChildRowCheck"/>)
            {
                this.<xsl:value-of select="@memberName"/> = new <xsl:value-of select="@ref"/>(child<xsl:value-of select="@name"/>[0]);
            }<xsl:text />
  </xsl:template>

  <!-- 
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Get all of the child rows for this reference collection.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

		EventDataModel.EventCommentRow[] childComments = this.ObjectDataRow.GetComments();

		if (childComments != null && childComments.Length > 0)
		{
			this.commentsField = new List<EventComment>(childComments.Length);

			for (int i = 0; i < childComments.Length; ++i)
			{
				this.commentsField.Add(new EventComment(childComments[i]));
			}
		}
	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->
  <xsl:template match="cobos:Reference[ @isCollection ]" mode="classConstructorBody">
    <xsl:value-of select="concat($newline, $newlineIndent3)"/>
    <xsl:value-of select="@datasetRowType"/>[] child<xsl:value-of select="@name"/> = this.ObjectDataRow.Get<xsl:value-of select="@name"/>();

            if (<xsl:apply-templates select="." mode="classConstructorBodyChildRowCheck"/>)
            {
                this.<xsl:value-of select="@memberName"/> = new <xsl:apply-templates select="." mode="listDecl"/>(child<xsl:value-of select="@name"/>.Length);

                for (<xsl:apply-templates select="." mode="classConstructorBodyChildRowForStatement"/>)
                {
                    this.<xsl:value-of select="@memberName"/>.Add(new <xsl:value-of select="@ref"/>(child<xsl:value-of select="@name"/>[i]));
                }
            }<xsl:text />
  </xsl:template>

  <!-- helper to construct the if statement to check we got some child rows -->
  <xsl:template match="cobos:Reference" mode="classConstructorBodyChildRowCheck">
    <xsl:text>child</xsl:text>
    <xsl:value-of select="@name"/>
    <xsl:text disable-output-escaping="yes"><![CDATA[ != null && child]]></xsl:text>
    <xsl:value-of select="@name"/>
    <xsl:text disable-output-escaping="yes"><![CDATA[.Length > 0]]></xsl:text>
  </xsl:template>

  <!-- helper to construct the for statement to populate -->
  <xsl:template match="cobos:Reference" mode="classConstructorBodyChildRowForStatement">
    <xsl:text disable-output-escaping="yes"><![CDATA[int i = 0; i < child]]></xsl:text>
    <xsl:value-of select="@name"/>
    <xsl:text disable-output-escaping="yes"><![CDATA[.Length; ++i]]></xsl:text>
  </xsl:template>

</xsl:stylesheet>