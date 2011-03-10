<xsl:stylesheet version="1.0"
						xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
						xmlns:msxsl="urn:schemas-microsoft-com:xslt"
						exclude-result-prefixes="msxsl"
						xmlns="http://schemas.intergraph.com/asiapac/cad/datamodel"
						xmlns:cad="http://schemas.intergraph.com/asiapac/cad/datamodel"
						xmlns:xsd="http://www.w3.org/2001/XMLSchema"
>
	<xsl:output method="text" indent="yes" omit-xml-declaration="yes"/>
	<xsl:strip-space elements="*"/>

	<!-- 
	=============================================================================
	Filename: datamodelclasses.xslt
	Description: XSLT for creation of C# code from data model
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Created by: N.Davis                        Date: 2010-04-09
	Modified by:                               Date:
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Notes: In the datamodel, each top level object refers to a single 
	table in the stronly typed dataset.  Nested object hierarchies are 
	flattened for more efficient database access and post-processing.

	============================================================================
	-->

	<xsl:include href="DataModelCommon.xslt"/>

	<!-- C# namespace -->
	<xsl:param name="codeNS"/>
	<!-- System.Runtime.Serialization.DataContract namespace -->
	<xsl:param name="contractNS"/>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Process the data model into CSharp classes
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="/cad:DataModel">

		<xsl:call-template name="generatedCSharpWarning"/>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Intergraph.AsiaPac.Data;
using Intergraph.AsiaPac.Data.Statements;

namespace <xsl:value-of select="$codeNS"/>
{
	<xsl:apply-templates select="cad:Object"/>
}
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Create a class declaration for each object
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->
	
	<xsl:template match="cad:Object">

		<xsl:variable name="classHierarchy">
			<xsl:apply-templates select="." mode="classHierarchy"/>
			<xsl:copy-of select="//cad:TableMetadata"/>
		</xsl:variable>
		
		<xsl:variable name="classHierarchyNodeset" select="msxsl:node-set( $classHierarchy )"/>
		<!--xsl:copy-of select="$classHierarchyNodeset"/-->
		
		<xsl:apply-templates select="$classHierarchyNodeset/cad:Object" mode="classDefinition"/>

		<xsl:apply-templates select="$classHierarchyNodeset/cad:Object" mode="dataAdapter"/>

	</xsl:template>
	
	<!-- output a nested class declaration -->
	<xsl:template match="cad:Object" mode="classDefinition">
		
	[DataContract(Namespace="<xsl:value-of select="$contractNS"/>")]
	public class <xsl:value-of select="@className"/>
	{
		<xsl:apply-templates select="." mode="classBody"/>
	}
	
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Class body definition
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<!-- output class body for a concrete object or a type -->
	<xsl:template match="cad:Object" mode="classBody">

		<xsl:variable name="datasetRowType">
			<xsl:apply-templates mode="datasetRowType" select="."/>
		</xsl:variable>
		
		readonly <xsl:value-of select="$datasetRowType"/> _rowData;

		public <xsl:value-of select="@className"/>( <xsl:value-of select="$datasetRowType"/> dataRow )
		{
			_rowData = dataRow;
		}

		<!-- add property declarations -->
		<xsl:apply-templates select="child::cad:Property|child::cad:Object" mode="propertyDefinition"/>

		<!-- add nested classes -->
		<xsl:apply-templates select="child::cad:Object" mode="classDefinition"/>

	</xsl:template>
	
	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Create property declarations
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<!-- output an embedded object -->
	<xsl:template match="cad:Object" mode="propertyDefinition">
		[DataMember(Order=<xsl:value-of select="position() - 1"/>)]
		public <xsl:value-of select="@className"/><xsl:text> </xsl:text><xsl:value-of select="@name"/>
		{
			get { return new <xsl:value-of select="@className"/>( _rowData ); }
			set { }
		}
	</xsl:template>

	<!-- output a simple property type -->
	<xsl:template match="cad:Property" mode="propertyDefinition">
		[DataMember(Order=<xsl:value-of select="position() - 1"/>)]
		public <xsl:apply-templates select="." mode="propertyType"/> <xsl:value-of select="@name"/>
		{
			<xsl:apply-templates select="." mode="propertyBody"/>
		}
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Match property types
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cad:Property[ @dbType = 'xsd:string' or contains( @dbType, 'string_' ) ]" mode="propertyType">
		<xsl:text>string </xsl:text>
	</xsl:template>

	<xsl:template match="cad:Property[ @dbType = 'xsd:integer' ]" mode="propertyType">
		<xsl:text>long</xsl:text>
		<xsl:apply-templates select="@minOccurs" mode="propertyType"/>
	</xsl:template>

	<xsl:template match="cad:Property[ @dbType = 'xsd:float' ]" mode="propertyType">
		<xsl:text>float</xsl:text>
		<xsl:apply-templates select="@minOccurs" mode="propertyType"/>
	</xsl:template>

	<xsl:template match="cad:Property[ @dbType = 'xsd:dateTime' ]" mode="propertyType">
		<xsl:text>DateTime</xsl:text>
		<xsl:apply-templates select="@minOccurs" mode="propertyType"/>
	</xsl:template>

	<xsl:template match="@minOccurs[ . = 0 ]" mode="propertyType">
		<xsl:text>? </xsl:text>
	</xsl:template>

	<xsl:template match="@minOccurs[ . = 1 ]" mode="propertyType">
		<xsl:text> </xsl:text>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Create the property get function body
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cad:Property[ @minOccurs = 1 ]" mode="propertyBody">
			get { return _rowData.<xsl:value-of select="@dbColumn"/>; }
			set { }
	</xsl:template>

	<xsl:template match="cad:Property[ @minOccurs = 0 ]" mode="propertyBody">
			get
			{
				if ( _rowData.Is<xsl:value-of select="@dbColumn"/>Null() )
				{
					return null;
				}
				return _rowData.<xsl:value-of select="@dbColumn"/>;
			}
			set { }
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Create data adapter
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cad:Object" mode="dataAdapter">
	public class <xsl:value-of select="@className"/>DataAdapter
	{
		readonly DatabaseConnection _dbConnection;

		public <xsl:value-of select="@className"/>DataAdapter( DatabaseConnection dbConnection )
		{
			_dbConnection = dbConnection;
		}
			
		const string _select = "<xsl:apply-templates select=".//cad:Property" mode="sqlSelect"/>";
		
		const string _from = "<xsl:value-of select="@dbTable"/>";
		
		static readonly string[] _innerJoin = <xsl:apply-templates mode="sqlInnerJoin" select="."/>;

		static readonly string[] _where = <xsl:apply-templates mode="sqlWhere" select="."/>;
		
		const string _groupBy = null;
		
		const string _orderBy = null;
			
		<xsl:apply-templates select="." mode="getDataMethodDecl"/>
		{
			<xsl:apply-templates select="." mode="getDataMethodBody"/>
		}
	}
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Create data adapter
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<!-- Add the getData method signature -->
	<xsl:template match="cad:Object" mode="getDataMethodDecl">
		public <xsl:call-template name="listDecl"/>
		<xsl:text> GetData( string[] where, string orderBy, string groupBy )</xsl:text>
	</xsl:template>

	<!-- Add the getData method body -->
	<xsl:template match="cad:Object" mode="getDataMethodBody">
			SqlSelect selectObj = new SqlSelect( _select, _from, _innerJoin, _where, _groupBy, _orderBy );
		
			string select = selectObj.ToString( null, null, null, where, groupBy, orderBy );
		
			<xsl:value-of select="@datasetTableType"/> table = <xsl:call-template name="executeStatement"/>( select );

			int numRows = table.Rows.Count;

			<xsl:call-template name="listDecl"/> results = new <xsl:call-template name="listDecl"/>( numRows );

			<xsl:text  disable-output-escaping="yes"><![CDATA[for ( int row = 0; row < numRows; ++row )]]></xsl:text>
			{
				results.Add( new <xsl:value-of select="@className"/>( table[ row ] ) );			
			}
		
			return results;
	</xsl:template>
	
	<!-- encapsulate details for statements using generics -->
	<xsl:template name="listDecl">
		<xsl:text disable-output-escaping="yes"><![CDATA[List<]]></xsl:text>
		<xsl:value-of select="@className"/>
		<xsl:text disable-output-escaping="yes"><![CDATA[>]]></xsl:text>
	</xsl:template>

	<xsl:template name="executeStatement">
		<xsl:text disable-output-escaping="yes"><![CDATA[_dbConnection.Execute<]]></xsl:text>
		<xsl:value-of select="@datasetTableType"/>
		<xsl:text disable-output-escaping="yes"><![CDATA[>]]></xsl:text>
	</xsl:template>

	<!-- Append the column names to form the select string-->
	<xsl:template match="cad:Property" mode="sqlSelect">
		<xsl:value-of select="../@dbTable"/>.<xsl:value-of select="@dbColumn"/>
		<xsl:if test="not( position() = last() )">, </xsl:if>
	</xsl:template>

	<!-- Get all of the required joins or where clauses -->
	<xsl:key name="byDbTable" match="cad:Object" use="@dbTable"/>

	<!-- Find all inner join tables -->
	<xsl:template match="cad:Object" mode="sqlInnerJoin">
		<xsl:variable name="masterTable">
			<xsl:value-of select="@dbTable"/>
		</xsl:variable>
		<xsl:text>new string[]{ </xsl:text>
		<xsl:for-each select="descendant::cad:Object[ not( @dbTable = current()/@dbTable ) ][ generate-id() = generate-id( key( 'byDbTable', @dbTable )[ 1 ] ) ]">
			<xsl:text>"</xsl:text>
			<xsl:apply-templates mode="sqlInnerJoin" select="//cad:TableMetadata[ @name = $masterTable ]//cad:Key[ @references = current()/@dbTable ]"/>
			<xsl:text>"</xsl:text>
			<xsl:if test="not( position() = last() )">, </xsl:if>
		</xsl:for-each>
		<xsl:text> }</xsl:text>
	</xsl:template>

	<!-- INNER JOIN clause -->
	<xsl:template match="cad:Key" mode="sqlInnerJoin">
		<xsl:value-of select="@references"/>
		<xsl:text> ON </xsl:text>
		<xsl:value-of select="ancestor::cad:TableMetadata[ 1 ]/@name"/>
		<xsl:text>.</xsl:text>
		<xsl:value-of select="@foreignKey"/>
		<xsl:text> = </xsl:text>
		<xsl:value-of select="@references"/>
		<xsl:text>.</xsl:text>
		<xsl:value-of select="@referenceKey"/>
	</xsl:template>
	
	<!-- Get all of the required where clauses -->
	<xsl:template match="cad:Object" mode="sqlWhere">
		<xsl:text>new string[]{ </xsl:text>
		<xsl:for-each select="descendant-or-self::cad:Object[ generate-id() = generate-id( key( 'byDbTable', @dbTable )[ 1 ] ) ]">
			<xsl:text>"</xsl:text>
			<xsl:value-of select="//cad:TableMetadata[ @name = current()/@dbTable ]//cad:Filter[ @name = 'default' ]"/>
			<xsl:text>"</xsl:text>
			<xsl:if test="not( position() = last() )">, </xsl:if>
		</xsl:for-each>
		<xsl:text> }</xsl:text>
	</xsl:template>

</xsl:stylesheet>