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

		public <xsl:value-of select="@typeName"/>( <xsl:value-of select="@datasetRowType"/> dataRow )
		{
			ObjectDataRow = dataRow;
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
		public <xsl:value-of select="@typeName"/>( <xsl:value-of select="$datasetRowType"/> dataRow )
		{
			ObjectDataRow = dataRow;
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
		<xsl:value-of select="concat( $newlineTab3, '_', @name, ' = new ', @typeName, '( ObjectDataRow );' )"/>
	</xsl:template>

	<!-- 
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Get the single child row for this reference.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

		EventDataModel.DispositionRow[] childDisposition = ObjectDataRow.GetDisposition();

		if ( childDisposition != null && childDisposition.Length > 0 )
		{
			_Disposition = new Disposition( childDisposition[ 0 ] );
		}
	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->
	<xsl:template match="cobos:Reference[ not( @isCollection ) ]" mode="classConstructorBody">
			<xsl:value-of select="$newlineTab3"/>
			<xsl:value-of select="@datasetRowType"/>[] child<xsl:value-of select="@name"/> = ObjectDataRow.Get<xsl:value-of select="@name"/>();

			if ( <xsl:apply-templates select="." mode="classConstructorBodyChildRowCheck"/> )
			{
				_<xsl:value-of select="@name"/> = new <xsl:value-of select="@ref"/>( child<xsl:value-of select="@name"/>[ 0 ] );
			}
	</xsl:template>

	<!-- 
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Get all of the child rows for this reference collection.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

		EventDataModel.EventCommentRow[] childComments = ObjectDataRow.GetComments();

		if ( childComments != null && childComments.Length > 0 )
		{
			_Comments = new List<EventComment>( childComments.Length );

			for ( int i = 0; i < childComments.Length; ++i  )
			{
				_Comments.Add( new EventComment( childComments[ i ] ) );
			}
		}
	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->
	<xsl:template match="cobos:Reference[ @isCollection ]" mode="classConstructorBody">
			<xsl:value-of select="@datasetRowType"/>[] child<xsl:value-of select="@name"/> = ObjectDataRow.Get<xsl:value-of select="@name"/>();

			if ( <xsl:apply-templates select="." mode="classConstructorBodyChildRowCheck"/> )
			{
				_<xsl:value-of select="@name"/> = new <xsl:apply-templates select="." mode="listDecl"/>( child<xsl:value-of select="@name"/>.Length );

				for ( <xsl:apply-templates select="." mode="classConstructorBodyChildRowForStatement"/>  )
				{
					_<xsl:value-of select="@name"/>.Add( new <xsl:value-of select="@ref"/>( child<xsl:value-of select="@name"/>[ i ] ) );
				}
			}
			<xsl:if test="not( position() = last() )">
				<xsl:value-of select="$newlineTab3"/>
			</xsl:if>
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