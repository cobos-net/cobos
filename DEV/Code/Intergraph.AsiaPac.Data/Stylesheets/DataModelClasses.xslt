<xsl:stylesheet version="1.0"
						xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
						xmlns:msxsl="urn:schemas-microsoft-com:xslt"
						exclude-result-prefixes="msxsl"
						xmlns="http://schemas.intergraph.com/asiapac/cad/datamodel/1.0.0"
						xmlns:cad="http://schemas.intergraph.com/asiapac/cad/datamodel/1.0.0"
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
using Intergraph.AsiaPac.Data.Utilities;
using Intergraph.IPS.Utility;

namespace <xsl:value-of select="$codeNS"/>
{
	<xsl:apply-templates select="cad:Object|cad:TableObject"/>
}
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Create a class declaration for each object
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->
	
	<xsl:template match="cad:Object|cad:TableObject">

		<xsl:variable name="classHierarchy">
			<xsl:apply-templates select="." mode="classHierarchy"/>
		</xsl:variable>
		
		<xsl:variable name="classHierarchyNodeset" select="msxsl:node-set( $classHierarchy )"/>
		
		<!-- Useful for debugging -->
		<!-- xsl:copy-of select="$classHierarchyNodeset"/ -->
		
		<xsl:apply-templates select="$classHierarchyNodeset/node()[ self::cad:Object | self::cad:TableObject ]" mode="classDefinition"/>

		<xsl:apply-templates select="$classHierarchyNodeset/node()[ self::cad:Object | self::cad:TableObject ]" mode="dataAdapter"/>

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
		<xsl:apply-templates select="child::cad:Property[ not( @hidden ) ]|child::cad:Object|child::cad:Reference" mode="propertyDefinition"/>

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
	<xsl:template match="cad:Property|cad:Reference" mode="propertyDefinition">
		[DataMember(Order=<xsl:value-of select="position() - 1"/>)]
		public <xsl:apply-templates select="." mode="propertyType"/> <xsl:apply-templates select="." mode="propertyDefinitionName"/>
		{
		<xsl:apply-templates select="." mode="propertyBody"/>
		}
	</xsl:template>

	<xsl:template match="cad:Property" mode="propertyDefinitionName">
		<xsl:value-of select="@name"/>
	</xsl:template>

	<xsl:template match="cad:Reference[ not( @isCollection ) ]" mode="propertyDefinitionName">
		<xsl:value-of select="@ref"/>
	</xsl:template>

	<xsl:template match="cad:Reference[ @isCollection ]" mode="propertyDefinitionName">
		<xsl:value-of select="@name"/>
	</xsl:template>
	
	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Match property types
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cad:Property[ @dbType = 'xsd:string' ]" mode="propertyType">
		<xsl:text>string </xsl:text>
	</xsl:template>

	<xsl:template match="cad:Property[ contains( @dbType, 'string_' ) and not( @stringFormat ) ]" mode="propertyType">
		<xsl:text>string </xsl:text>
	</xsl:template>

	<xsl:template match="cad:Property[ contains( @dbType, 'string_' ) and @stringFormat = 'Separator' ]" mode="propertyType">
		<xsl:text>string </xsl:text>
	</xsl:template>

	<xsl:template match="cad:Property[ contains( @dbType, 'string_' ) and @stringFormat = 'CadDts' ]" mode="propertyType">
		<xsl:text>DateTime</xsl:text>
		<xsl:apply-templates select="@minOccurs" mode="propertyType"/>
	</xsl:template>

	<xsl:template match="cad:Property[ contains( @dbType, 'string_' ) and @stringFormat = 'CadBoolean' ]" mode="propertyType">
		<xsl:text>bool</xsl:text>
		<xsl:apply-templates select="@minOccurs" mode="propertyType"/>
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
	Match reference types
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cad:Reference[ not( @isCollection ) ]" mode="propertyType">
		<xsl:value-of select="@ref"/>
		<xsl:text> </xsl:text>
	</xsl:template>

	<xsl:template match="cad:Reference[ @isCollection ]" mode="propertyType">
		<xsl:apply-templates select="." mode="listDecl"/>
		<xsl:text> </xsl:text>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Create the property get function body
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cad:Property[ @minOccurs = 1 ]|cad:Reference" mode="propertyBody">
			get 
			{ 
				<xsl:apply-templates select="." mode="propertyReturn"/> 
			}
			set { }
	</xsl:template>

	<xsl:template match="cad:Property[ @minOccurs = 0 ]" mode="propertyBody">
			get
			{
				if ( _rowData.Is<xsl:apply-templates mode="qualifiedName" select="."/>Null() )
				{
					return null;
				}
				<xsl:apply-templates select="." mode="propertyReturn"/>
			}
			set { }
	</xsl:template>

	<!-- basic return type -->
	<xsl:template match="cad:Property[ not( @stringFormat ) ]" mode="propertyReturn">
				return _rowData.<xsl:apply-templates mode="qualifiedName" select="."/>;
	</xsl:template>

	<xsl:template match="cad:Property[ @stringFormat = 'CadDts' ]" mode="propertyReturn">
				return DateFormatter.ConvertFromDTS( _rowData.<xsl:apply-templates mode="qualifiedName" select="."/> );
	</xsl:template>

	<xsl:template match="cad:Property[ @stringFormat = 'CadBoolean' ]" mode="propertyReturn">
				return CADBoolean.ParseString( _rowData.<xsl:apply-templates mode="qualifiedName" select="."/> );
	</xsl:template>

	<xsl:template match="cad:Property[ @stringFormat = 'Separator' ]" mode="propertyReturn">
		<xsl:variable name="separator">
			<xsl:value-of select="substring-before( @formatArgs, ' ' )"/>
		</xsl:variable>
		<xsl:variable name="index">
			<xsl:value-of select="substring-after( @formatArgs, ' ' )"/>
		</xsl:variable>
				return StringSeparator.GetTokenAt( _rowData.<xsl:apply-templates mode="qualifiedName" select="."/>, '<xsl:value-of select="$separator"/>', <xsl:value-of select="$index"/> );
	</xsl:template>

	<xsl:template match="cad:Reference[ not( @isCollection ) ]" mode="propertyReturn">
				return null; // is not a collection
	</xsl:template>

	<xsl:template match="cad:Reference[ @isCollection ]" mode="propertyReturn">
				return null; // is a collection
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Create data adapter
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cad:Object" mode="dataAdapter">
	public class <xsl:value-of select="@className"/>DataAdapter
	{
		readonly DatabaseAdapter _database;

		public <xsl:value-of select="@className"/>DataAdapter( DatabaseAdapter database )
		{
			_database = database;
		}

		const string _select = "<xsl:apply-templates select=".//cad:Property" mode="sqlSelect"/>";
		
		const string _from = "<xsl:value-of select="@dbTable"/>";
		
		static readonly string[] _innerJoin = <xsl:apply-templates mode="sqlJoin" select="."/>;

		static readonly string[] _where = <xsl:apply-templates mode="sqlWhere" select="."/>;

		const string _groupBy = null;

		const string _orderBy = null;

		public static readonly SqlSelect Select = new SqlSelect( _select, _from, _innerJoin, _where, _groupBy, _orderBy );

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
		public <xsl:apply-templates select="." mode="listDecl"/>
		<xsl:text> GetData( string[] where, string orderBy, string groupBy )</xsl:text>
	</xsl:template>

	<!-- Add the getData method body -->
	<xsl:template match="cad:Object" mode="getDataMethodBody">
			string select = Select.ToString( null, null, null, where, groupBy, orderBy );
		
			<xsl:value-of select="@datasetTableType"/> table = <xsl:call-template name="executeStatement"/>( select );

			int numRows = table.Rows.Count;

			<xsl:apply-templates select="." mode="listDecl"/> results = new <xsl:apply-templates select="." mode="listDecl"/>( numRows );

			<xsl:text  disable-output-escaping="yes"><![CDATA[for ( int row = 0; row < numRows; ++row )]]></xsl:text>
			{
				results.Add( new <xsl:value-of select="@className"/>( table[ row ] ) );			
			}
		
			return results;
	</xsl:template>
	
	<!-- encapsulate details for statements using generics -->
	<xsl:template match="cad:Object" mode="listDecl">
		<xsl:text disable-output-escaping="yes"><![CDATA[List<]]></xsl:text>
		<xsl:value-of select="@className"/>
		<xsl:text disable-output-escaping="yes"><![CDATA[>]]></xsl:text>
	</xsl:template>

	<xsl:template match="cad:Reference" mode="listDecl">
		<xsl:text disable-output-escaping="yes"><![CDATA[List<]]></xsl:text>
		<xsl:value-of select="@ref"/>
		<xsl:text disable-output-escaping="yes"><![CDATA[>]]></xsl:text>
	</xsl:template>

	<xsl:template name="executeStatement">
		<xsl:text disable-output-escaping="yes"><![CDATA[_database.Execute<]]></xsl:text>
		<xsl:value-of select="@datasetTableType"/>
		<xsl:text disable-output-escaping="yes"><![CDATA[>]]></xsl:text>
	</xsl:template>

	<!-- Append the column names to form the select string-->
	<xsl:template match="cad:Property" mode="sqlSelect">
		<xsl:value-of select="@dbTable"/>.<xsl:apply-templates select="." mode="sqlSelectColumn"/>
		<xsl:if test="not( position() = last() )">, </xsl:if>
	</xsl:template>

	<xsl:template match="cad:Property[ not( @dbAlias ) ]" mode="sqlSelectColumn">
		<xsl:value-of select="@dbColumn"/>
	</xsl:template>

	<xsl:template match="cad:Property[ @dbAlias ]" mode="sqlSelectColumn">
		<xsl:value-of select="@dbColumn"/>
		<xsl:text > AS </xsl:text>
		<xsl:value-of select="@dbAlias"/>
	</xsl:template>
	
	<!-- Find all inner join tables -->
	<xsl:template match="cad:Object[ ./cad:Metadata/cad:Joins/* ]" mode="sqlJoin">
		<xsl:text>new string[]{ </xsl:text>
		<xsl:apply-templates mode="sqlJoin" select="./cad:Metadata/cad:Joins/*[ self::cad:InnerJoin | self::cad:OuterJoin ]"/>
		<xsl:text> }</xsl:text>
	</xsl:template>

	<xsl:template match="cad:Object[ not( ./cad:Metadata/cad:Joins/* ) ]" mode="sqlJoin">
		<xsl:text>null</xsl:text>
	</xsl:template>
	
	<!-- INNER JOIN and OUTER JOIN clauses -->
	<xsl:template match="cad:InnerJoin | cad:OuterJoin" mode="sqlJoin">
		<xsl:text>"</xsl:text>
		<xsl:value-of select="@references"/>
		<xsl:text> ON </xsl:text>
		<xsl:value-of select="ancestor::cad:Object[ 1 ]/@dbTable"/>
		<xsl:text>.</xsl:text>
		<xsl:value-of select="@foreignKey"/>
		<xsl:text> = </xsl:text>
		<xsl:value-of select="@references"/>
		<xsl:text>.</xsl:text>
		<xsl:value-of select="@referenceKey"/>
		<xsl:text>"</xsl:text>
		<xsl:if test="not( position() = last() )">, </xsl:if>
	</xsl:template>
	
	<!-- Get all of the required where clauses -->
	<xsl:template match="cad:Object[ ./cad:Metadata//cad:Filter ]" mode="sqlWhere">
		<xsl:text>new string[]{ </xsl:text>
		<xsl:apply-templates select="./cad:Metadata//cad:Filter" mode="sqlWhere"/>
		<xsl:text> }</xsl:text>
	</xsl:template>

	<!-- WHERE clause -->
	<xsl:template match="cad:Filter" mode="sqlWhere">
		<xsl:text>"</xsl:text>
		<xsl:value-of select="."/>
		<xsl:text>"</xsl:text>
		<xsl:if test="not( position() = last() )">, </xsl:if>
	</xsl:template>

	<xsl:template match="cad:Object[ not( ./cad:Metadata//cad:Filter ) ]" mode="sqlWhere">
		<xsl:text>null</xsl:text>
	</xsl:template>
					  
	
	
</xsl:stylesheet>