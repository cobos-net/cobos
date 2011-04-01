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
using System.Data;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Intergraph.AsiaPac.Data;
using Intergraph.AsiaPac.Data.Statements;
using Intergraph.AsiaPac.Data.Utilities;
using Intergraph.AsiaPac.Utilities.Extensions;
using Intergraph.AsiaPac.Utilities.Text;
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
		<!-- xsl:copy-of select="$classHierarchyNodeset"/-->
		
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
		
		readonly <xsl:value-of select="$datasetRowType"/> _dataRow;

		<xsl:apply-templates select="cad:Reference" mode="classMemberDecl"/>

		public <xsl:value-of select="@className"/>( <xsl:value-of select="$datasetRowType"/> dataRow )
		{
			_dataRow = dataRow;
			
			<xsl:apply-templates select="cad:Reference" mode="classConstructorBody"/>
		}

		<!-- add property declarations -->
		<xsl:apply-templates select="child::cad:Property[ not( @hidden ) ]|child::cad:Object|child::cad:Reference" mode="propertyDefinition"/>

		<!-- add nested classes -->
		<xsl:apply-templates select="child::cad:Object" mode="classDefinition"/>

	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Member variables and constructor body for references
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<!-- class member declaration for a reference type -->
	<xsl:template match="cad:Reference[ not( @isCollection ) ]" mode="classMemberDecl">
		<xsl:value-of select="@ref"/>
		<xsl:text> _</xsl:text>
		<xsl:value-of select="@name"/>
		<xsl:text>;</xsl:text>
		<xsl:if test="not( position() = last() )">
			<xsl:text>
			</xsl:text>
		</xsl:if>
	</xsl:template>

	<!-- class member declaration for reference type that is a collection-->
	<xsl:template match="cad:Reference[ @isCollection ]" mode="classMemberDecl">
		<xsl:apply-templates select="." mode="listDecl"/>
		<xsl:text> _</xsl:text>
		<xsl:value-of select="@name"/>
		<xsl:text>;</xsl:text>
		<xsl:if test="not( position() = last() )">
			<xsl:text>
		</xsl:text>
		</xsl:if>
	</xsl:template>

	<!-- 
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Get the single child row for this reference.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

		EventDataModel.DispositionRow[] childDisposition = _dataRow.GetDisposition();

		if ( childDisposition != null && childDisposition.Length > 0 )
		{
			_Disposition = new Disposition( childDisposition[ 0 ] );
		}
	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->
	<xsl:template match="cad:Reference[ not( @isCollection ) ]" mode="classConstructorBody">
			<xsl:value-of select="@datasetRowType"/>[] child<xsl:value-of select="@name"/> = _dataRow.Get<xsl:value-of select="@name"/>();

			if ( <xsl:apply-templates select="." mode="classConstructorBodyChildRowCheck"/> )
			{
				_<xsl:value-of select="@name"/> = new <xsl:value-of select="@ref"/>( child<xsl:value-of select="@name"/>[ 0 ] );
			}
			<xsl:if test="not( position() = last() )">
				<xsl:text>
			</xsl:text>
			</xsl:if>
	</xsl:template>

	<!-- 
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Get all of the child rows for this reference collection.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

		EventDataModel.EventCommentRow[] childComments = _dataRow.GetComments();

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
	<xsl:template match="cad:Reference[ @isCollection ]" mode="classConstructorBody">
			<xsl:value-of select="@datasetRowType"/>[] child<xsl:value-of select="@name"/> = _dataRow.Get<xsl:value-of select="@name"/>();

			if ( <xsl:apply-templates select="." mode="classConstructorBodyChildRowCheck"/> )
			{
				_<xsl:value-of select="@name"/> = new <xsl:apply-templates select="." mode="listDecl"/>( child<xsl:value-of select="@name"/>.Length );

				for ( <xsl:apply-templates select="." mode="classConstructorBodyChildRowForStatement"/>  )
				{
					_<xsl:value-of select="@name"/>.Add( new <xsl:value-of select="@ref"/>( child<xsl:value-of select="@name"/>[ i ] ) );
				}
			}
			<xsl:if test="not( position() = last() )">
				<xsl:text>
			</xsl:text>
			</xsl:if>
	</xsl:template>

	<!-- helper to construct the if statement to check we got some child rows -->
	<xsl:template match="cad:Reference" mode="classConstructorBodyChildRowCheck">
		<xsl:text>child</xsl:text>
		<xsl:value-of select="@name"/>
		<xsl:text disable-output-escaping="yes"><![CDATA[ != null && child]]></xsl:text>
		<xsl:value-of select="@name"/>
		<xsl:text disable-output-escaping="yes"><![CDATA[.Length > 0]]></xsl:text>
	</xsl:template>
	
	<!-- helper to construct the for statement to populate -->
	<xsl:template match="cad:Reference" mode="classConstructorBodyChildRowForStatement">
		<xsl:text disable-output-escaping="yes"><![CDATA[int i = 0; i < child]]></xsl:text>
		<xsl:value-of select="@name"/>
		<xsl:text disable-output-escaping="yes"><![CDATA[.Length; ++i]]></xsl:text>
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
			get { return new <xsl:value-of select="@className"/>( _dataRow ); }
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

	<xsl:template match="cad:Reference" mode="propertyDefinitionName">
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
				if ( _dataRow.Is<xsl:apply-templates mode="qualifiedName" select="."/>Null() )
				{
					return null;
				}
				<xsl:apply-templates select="." mode="propertyReturn"/>
			}
			set { }
	</xsl:template>

	<!-- basic return type -->
	<xsl:template match="cad:Property[ not( @stringFormat ) ]" mode="propertyReturn">
				return _dataRow.<xsl:apply-templates mode="qualifiedName" select="."/>;
	</xsl:template>

	<xsl:template match="cad:Property[ @stringFormat = 'CadDts' ]" mode="propertyReturn">
				return DateFormatter.ConvertFromDTS( _dataRow.<xsl:apply-templates mode="qualifiedName" select="."/> );
	</xsl:template>

	<xsl:template match="cad:Property[ @stringFormat = 'CadBoolean' ]" mode="propertyReturn">
				return CADBoolean.ParseString( _dataRow.<xsl:apply-templates mode="qualifiedName" select="."/> );
	</xsl:template>

	<xsl:template match="cad:Property[ @stringFormat = 'Separator' ]" mode="propertyReturn">
		<xsl:variable name="separator">
			<xsl:value-of select="substring-before( @formatArgs, ' ' )"/>
		</xsl:variable>
		<xsl:variable name="index">
			<xsl:value-of select="substring-after( @formatArgs, ' ' )"/>
		</xsl:variable>
				return StringSeparator.GetTokenAt( _dataRow.<xsl:apply-templates mode="qualifiedName" select="."/>, '<xsl:value-of select="$separator"/>', <xsl:value-of select="$index"/> );
	</xsl:template>

	<xsl:template match="cad:Reference" mode="propertyReturn">
				return _<xsl:value-of select="@name"/>;
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Create data adapter
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cad:Object" mode="dataAdapter">
	public static class <xsl:value-of select="@className"/>DataAdapter
	{
		const string _select = "<xsl:apply-templates select=".//cad:Property" mode="sqlSelect"/>";
		
		const string _from = "<xsl:value-of select="@dbTable"/>";
		
		static readonly string[] _innerJoin = <xsl:apply-templates mode="sqlJoin" select="."/>;

		static readonly string[] _where = <xsl:apply-templates mode="sqlWhere" select="."/>;

		const string _groupBy = <xsl:apply-templates mode="sqlGroupBy" select="."/>;

		const string _orderBy = <xsl:apply-templates mode="sqlOrderBy" select="."/>;

		public static readonly SqlSelectTemplate Select = new SqlSelectTemplate( _select, _from, _innerJoin, _where, _groupBy, _orderBy );

		<xsl:apply-templates select="./cad:Metadata/cad:StringAggregate" mode="aggregatorDecl"/>

		<xsl:apply-templates select="." mode="getQueryMethodBody"/>
	
		<xsl:apply-templates select="." mode="getDataMethodBody"/>

		<xsl:apply-templates select="." mode="createDataSetMethodBody"/>

		<xsl:apply-templates select="./cad:Metadata/cad:StringAggregate" mode="getAggregatorMethodBody"/>
	}
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	GetQuery method body - return the query, aggregated types will return 
	a custom aggregator as well.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	
	

	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cad:Object[ not( ./cad:Metadata/cad:StringAggregate ) ]" mode="getQueryMethodBody">
		public static DatabaseQuery GetQuery( string[] where, string orderBy )
		{
			return new DatabaseQuery( Select.ToString( where, null, orderBy ), new <xsl:value-of select="@datasetTableType"/>() );
		}

		public static DatabaseQuery GetQuery( <xsl:value-of select="@datasetTableType"/> table, string[] where, string orderBy )
		{
			return new DatabaseQuery( Select.ToString( where, null, orderBy ), table );
		}
	</xsl:template>

	
	<xsl:template match="cad:Object[ ./cad:Metadata/cad:StringAggregate ]" mode="getQueryMethodBody">
		public static DatabaseQuery GetQuery( string[] where, string orderBy )
		{
			return new DatabaseQuery( Select.ToString( where, null, orderBy ), new <xsl:value-of select="@datasetTableType"/>(), Aggregator );
		}

		public static DatabaseQuery GetQuery( <xsl:value-of select="@datasetTableType"/> table, string[] where, string orderBy )
		{
			return new DatabaseQuery( Select.ToString( where, null, orderBy ), table, Aggregator );
		}
	</xsl:template>
	
	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	GetData method body - for an Object NOT containing Reference objects:
	
	1)	Construct the query object and call the database.
	2) Create the return objects.
	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	
		public static List<AgencyEventSummary> GetData( DatabaseAdapter database, string[] where, string orderBy )
		{
			DatabaseQuery query = GetQuery( where, orderBy );

			database.Execute( query );

			EventDataModel.AgencyEventSummaryDataTable table = (EventDataModel.AgencyEventSummaryDataTable)query.GetResult();

			int numRows = table.Rows.Count;

			List<AgencyEventSummary> results = new List<AgencyEventSummary>( numRows );

			for ( int row = 0; row < numRows; ++row )
			{
				results.Add( new AgencyEventSummary( table[ row ] ) );			
			}
		
			return results;
		}
	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->
	<xsl:template match="cad:Object[ not( cad:Reference ) ]" mode="getDataMethodBody">
		public static <xsl:apply-templates select="." mode="listDecl"/> GetData( DatabaseAdapter database, string[] where, string orderBy )
		{
			DatabaseQuery query = GetQuery( where, orderBy );

			database.Execute( query );

			<xsl:value-of select="@datasetTableType"/> table = (<xsl:value-of select="@datasetTableType"/>)query.GetResult();

			int numRows = table.Rows.Count;

			<xsl:apply-templates select="." mode="listDecl"/> results = new <xsl:apply-templates select="." mode="listDecl"/>( numRows );

			<xsl:text  disable-output-escaping="yes"><![CDATA[for ( int row = 0; row < numRows; ++row )]]></xsl:text>
			{
				results.Add( new <xsl:value-of select="@className"/>( table[ row ] ) );			
			}
		
			return results;
		}
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	GetData method body - for an Object containing Reference objects:
	
	1)	Query mutliple tables for object and all references.
	2) Get the table results (aggregated tables returned automatically).
	3) Create a DataSet and add the tables.
	4) Create DataRelation objects to join the tables together.
	5) Create the return objects.
	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	
	
	
	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->
	<xsl:template match="cad:Object[ cad:Reference ]" mode="getDataMethodBody">
		public static <xsl:apply-templates select="." mode="listDecl"/> GetData( DatabaseAdapter database, string[] where, string orderBy )
		{
			<xsl:apply-templates select=".|./cad:Reference" mode="getDataMethodQueryDecl"/>

			DatabaseQuery[] queries = new DatabaseQuery[]
			{
				<xsl:apply-templates select=".|./cad:Reference" mode="getDataMethodQueryInstance"/>
			};

			database.ExecuteAsynch( queries );

			<xsl:apply-templates select=".|./cad:Reference" mode="getDataMethodTableDecl"/>
		
			DataSet dataset = CreateDataSet( <xsl:apply-templates select=".|./cad:Reference" mode="getDataMethodCreateDataSetArgs" /> );
			queries = null;

			int numRows = table<xsl:value-of select="@name"/>.Rows.Count; 

			<xsl:apply-templates select="." mode="listDecl"/> results = new <xsl:apply-templates select="." mode="listDecl"/>( numRows );

			<xsl:text  disable-output-escaping="yes"><![CDATA[for ( int row = 0; row < numRows; ++row )]]></xsl:text>
			{
				results.Add( new <xsl:value-of select="@className"/>( table<xsl:value-of select="@name"/>[ row ] ) );
			}

			return results;
		}
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	GetData method body - Output the query instances for Object or Reference
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

		queryAgencyEvent,
		queryEventComment,
		queryDisposition

	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cad:Object" mode="getDataMethodQueryInstance">
		<xsl:value-of select="concat( 'query', @className )"/>
		<xsl:if test="not( position() = last() )">
			<xsl:value-of select="concat( ',', $newlineTab4 )"/>
		</xsl:if>
	</xsl:template>

	<xsl:template match="cad:Reference" mode="getDataMethodQueryInstance">
		<xsl:value-of select="concat( 'query', @ref )"/>
		<xsl:if test="not( position() = last() )">
			<xsl:value-of select="concat( ',', $newlineTab4 )"/>
		</xsl:if>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	GetData method body - Cast the table instances for Object or Reference
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

		EventDataModel.AgencyEventDataTable tableAgencyEvent = (EventDataModel.AgencyEventDataTable)queryAgencyEvent.GetResult();
		EventDataModel.EventCommentDataTable tableEventComment = (EventDataModel.EventCommentDataTable)queryEventComment.GetResult();
		EventDataModel.DispositionDataTable tableDisposition = (EventDataModel.DispositionDataTable)queryDisposition.GetResult();

	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cad:Object" mode="getDataMethodTableDecl">
		<xsl:value-of select="concat( @datasetTableType, ' table', @className, ' = (', @datasetTableType, ')query', @className, '.GetResult();' )"/>
		<xsl:if test="not( position() = last() )">
			<xsl:value-of select="$newlineTab3"/>
		</xsl:if>
	</xsl:template>

	<xsl:template match="cad:Reference" mode="getDataMethodTableDecl">
		<xsl:value-of select="concat( @datasetTableType, ' table', @ref, ' = (', @datasetTableType, ')query', @ref, '.GetResult();' )"/>
		<xsl:if test="not( position() = last() )">
			<xsl:value-of select="$newlineTab3"/>
		</xsl:if>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	GetData method body - DatabaseQuery declaration for an Object
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	
		DatabaseQuery queryAgencyEvent = GetQuery( where, orderBy );
	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cad:Object" mode="getDataMethodQueryDecl">
		<xsl:value-of select="concat( 'DatabaseQuery query', @name, '= GetQuery( where, orderBy );' )"/>
		<xsl:if test="not( position() = last() )">
			<xsl:value-of select="$newlineTab3"/>
		</xsl:if>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	GetData method body - DatabaseQuery declaration for an Reference
	(no filter metadata for the reference)	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	
		DatabaseQuery queryEventComment = EventCommentDataAdapter.GetQuery( where, null );
	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cad:Reference[ not( descendant::cad:Filters ) ]" mode="getDataMethodQueryDecl">
		<xsl:value-of select="concat( 'DatabaseQuery query', @ref, ' = ', @ref, 'DataAdapter.GetQuery( where, null );' )"/>
		<xsl:if test="not( position() = last() )">
			<xsl:value-of select="$newlineTab3"/>
		</xsl:if>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	GetData method body - DatabaseQuery declaration for an Reference
	(with filter metadata for the reference)
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	
		DatabaseQuery queryEventComment = EventCommentDataAdapter.GetQuery( ArrayExtensions.ConcatenateAll<string>( where, new string[]{ "EVCOM.comm_scope is null or EVCOM.comm_scope = AEVEN.ag_id" } ), null );
	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cad:Reference[ descendant::cad:Filters ]" mode="getDataMethodQueryDecl">
		<xsl:variable name="filter">
			<xsl:text>new string[]{ </xsl:text>
			<xsl:apply-templates select="descendant::cad:Filters" mode="sqlWhere"/>
			<xsl:text> }</xsl:text>
		</xsl:variable>

		<xsl:value-of select="concat( 'DatabaseQuery query', @ref, ' = ', @ref, 'DataAdapter.GetQuery( ' )"/>
		<xsl:text disable-output-escaping="yes"><![CDATA[ArrayExtensions.ConcatenateAll<string>]]></xsl:text>
		<xsl:value-of select="concat( '( new string[][]{ where, ', $filter, ' } ), null );' )"/>
		<xsl:if test="not( position() = last() )">
			<xsl:value-of select="$newlineTab3"/>
		</xsl:if>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	GetData/CreateDataSet arguments list
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cad:Object" mode="getDataMethodCreateDataSetArgs">
		<xsl:value-of select="concat( 'table', @className )"/>
		<xsl:if test="not( position() = last() )">, </xsl:if>
	</xsl:template>

	<xsl:template match="cad:Reference" mode="getDataMethodCreateDataSetArgs">
		<xsl:value-of select="concat( 'table', @ref )"/>
		<xsl:if test="not( position() = last() )">, </xsl:if>
	</xsl:template>

	<xsl:template match="cad:Object" mode="getDataMethodCreateDataSetArgsDecl">
		<xsl:value-of select="concat( @datasetTableType, ' table', @className )"/>
		<xsl:if test="not( position() = last() )">, </xsl:if>
	</xsl:template>

	<xsl:template match="cad:Reference" mode="getDataMethodCreateDataSetArgsDecl">
		<xsl:value-of select="concat( @datasetTableType, ' table', @ref )"/>
		<xsl:if test="not( position() = last() )">, </xsl:if>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	CreateDataSet method body - for an Object NOT containing Reference objects:
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	No method required.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cad:Object[ not( cad:Reference ) ]" mode="createDataSetMethodBody"></xsl:template>
	
	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	CreateDataSet method body - for an Object containing Reference objects:
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cad:Object[ cad:Reference ]" mode="createDataSetMethodBody">
		public static DataSet CreateDataSet( <xsl:apply-templates select=".|./cad:Reference" mode="getDataMethodCreateDataSetArgsDecl"/> )
		{
			DataSet dataset = new DataSet();
			<xsl:apply-templates select=".|./cad:Reference" mode="createDataSetAddTable"/>
			<xsl:text>
			</xsl:text>
			<xsl:apply-templates select="./cad:Reference" mode="createDataSetDataRelation"/>
			return dataset;
		}
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	CreateDataSet method body - add tables to the dataset
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		
		dataset.Tables.Add( tableAgencyEvent );
		dataset.Tables.Add( tableEventComment );
		dataset.Tables.Add( tableDisposition );
	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cad:Object" mode="createDataSetAddTable">
		<xsl:value-of select="concat( 'dataset.Tables.Add( table', @className, ' );' )"/>
		<xsl:if test="not( position() = last() )">
			<xsl:value-of select="$newlineTab3"/>
		</xsl:if>
	</xsl:template>

	<xsl:template match="cad:Reference" mode="createDataSetAddTable">
		<xsl:value-of select="concat( 'dataset.Tables.Add( table', @ref, ' );' )"/>
		<xsl:if test="not( position() = last() )">
			<xsl:value-of select="$newlineTab3"/>
		</xsl:if>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	CreateDataSet method body - create the data relation object
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		
		DataRelation relationAgencyEventEventComment = new ( "AgencyEventEventComment", 
																				tableAgencyEvent.AgencyEventIdColumn,
																				tableEventComment.AgencyEventIdColumn, 
																				false );

		relationAgencyEventEventComment.ExtendedProperties.Add( "typedChildren", "GetComments" );
		relationAgencyEventEventComment.ExtendedProperties.Add( "typedParent", "AgencyEvent" );

		dataset.Relations.Add( relationAgencyEventEventComment );
	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cad:Reference" mode="createDataSetDataRelation">
		<xsl:variable name="relationName" select="concat( 'relation', ../@className, @ref )"/>

		<xsl:value-of select="$newlineTab3"/>
		<xsl:value-of select="concat( 'DataRelation ', $relationName, ' = new DataRelation( &quot;', $relationName, '&quot;, ',  
																														'table', ../@className, '.', @key, 'Column, ',
																														'table', @ref, '.', @refer, 'Column, false );' )" />
		<xsl:value-of select="$newlineTab3"/>

		<xsl:value-of select="concat( $relationName, '.ExtendedProperties.Add( &quot;typedChildren&quot;, &quot;Get', @name, '&quot; );' )"/>
		<xsl:value-of select="$newlineTab3"/>
		
		<xsl:value-of select="concat( $relationName, '.ExtendedProperties.Add( &quot;typedParent&quot;, &quot;', ../@name, '&quot; );' )"/>
		<xsl:value-of select="$newlineTab3"/>

		<xsl:value-of select="concat( 'dataset.Relations.Add( ', $relationName, '  );' )"/>
		<xsl:value-of select="$newlineTab3"/>

	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Aggregator member declaration.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->
	<xsl:template match="cad:StringAggregate" mode="aggregatorDecl">
		static readonly IDataRowAggregator Aggregator = GetAggregator();
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	GetAggregator method body.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	
	
	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cad:StringAggregate" mode="getAggregatorMethodBody">
		public static IDataRowAggregator GetAggregator()
		{
			<xsl:value-of select="../../@datasetTableType"/> table = new <xsl:value-of select="../../@datasetTableType"/>();

			DataColumn aggregateOn = table.<xsl:apply-templates mode="qualifiedName" select="ancestor::cad:Object[ position() = last() ]//cad:Property[ @name = current()/cad:On/text() ]"/>Column;

			DataColumn[] groupBy = new DataColumn[]
			{
				<xsl:apply-templates select="./cad:Group/cad:By" mode="aggregateBy"/>
			};

			DataColumn[] sortBy = new DataColumn[]
			{
				<xsl:apply-templates select="./cad:Order/cad:By" mode="aggregateBy"/>
			};

			DataRowColumnComparer comparer = new DataRowColumnComparer( sortBy );

			return new DataRowColumnAggregator( aggregateOn, groupBy, comparer );
		}
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Aggregation sort by/group by
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cad:By" mode="aggregateBy">
		<xsl:text>table.</xsl:text>
		<xsl:apply-templates mode="qualifiedName" select="ancestor::cad:Object[ position() = last() ]//cad:Property[ @name = current()/text() ]"/>
		<xsl:text>Column</xsl:text>
		<xsl:if test="not( position() = last() )">
			<xsl:value-of select="concat( ',', $newlineTab4 )"/>
		</xsl:if>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->
	
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
		<xsl:text disable-output-escaping="yes"><![CDATA[database.Execute<]]></xsl:text>
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

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	INNER/OUTER JOIN
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	
		static readonly string[] _innerJoin = new string[]{ "EVENT ON AEVEN.eid = EVENT.eid" };
	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

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

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	WHERE clause
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

		static readonly string[] _where = new string[]{ "AEVEN.curent in ('T', 'O')", "EVENT.curent in ('T', 'O')" };
	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cad:Object[ ./cad:Metadata//cad:Filter ]" mode="sqlWhere">
		<xsl:text>new string[]{ </xsl:text>
		<xsl:apply-templates select="./cad:Metadata//cad:Filter" mode="sqlWhere"/>
		<xsl:text> }</xsl:text>
	</xsl:template>

	<xsl:template match="cad:Object[ not( ./cad:Metadata//cad:Filter ) ]" mode="sqlWhere">
		<xsl:text>null</xsl:text>
	</xsl:template>

	<xsl:template match="cad:Filter" mode="sqlWhere">
		<xsl:text>"</xsl:text>
		<xsl:value-of select="."/>
		<xsl:text>"</xsl:text>
		<xsl:if test="not( position() = last() )">, </xsl:if>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	ORDER BY clause
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cad:Object[ ./cad:Metadata/cad:Order/cad:By ]" mode="sqlOrderBy">
		<xsl:text>"</xsl:text>
		<xsl:apply-templates select="./cad:Metadata/cad:Order/cad:By"/>
		<xsl:text>"</xsl:text>
	</xsl:template>


	<xsl:template match="cad:Object[ not( ./cad:Metadata/cad:Order/cad:By ) ]" mode="sqlOrderBy">
		<xsl:text>null</xsl:text>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	GROUP BY clause
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->
	
	<xsl:template match="cad:Object[ ./cad:Metadata/cad:Group/cad:By ]" mode="sqlGroupBy">
		<xsl:text>"</xsl:text>
		<xsl:apply-templates select="./cad:Metadata/cad:Group/cad:By"/>
		<xsl:text>"</xsl:text>
	</xsl:template>

	<xsl:template match="cad:Object[ not( ./cad:Metadata/cad:Group/cad:By ) ]" mode="sqlGroupBy">
		<xsl:text>null</xsl:text>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	ORDER/GROUP BY value
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cad:By">
		<xsl:variable name="property" select="ancestor::cad:Object[ 1 ]//cad:Property[ @name = current()/text() ]"/>
		<xsl:value-of select="$property/@dbTable"/>
		<xsl:text>.</xsl:text>
		<xsl:value-of select="$property/@dbColumn"/>
		<xsl:if test="not( position() = last() )">, </xsl:if>
	</xsl:template>


</xsl:stylesheet>