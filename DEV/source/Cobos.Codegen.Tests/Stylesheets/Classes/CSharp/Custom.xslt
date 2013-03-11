<xsl:stylesheet version="1.0"
						xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
						xmlns:msxsl="urn:schemas-microsoft-com:xslt"
						exclude-result-prefixes="msxsl"
						xmlns="http://schemas.cobos.co.uk/datamodel/1.0.0"
						xmlns:cobos="http://schemas.cobos.co.uk/datamodel/1.0.0"
						xmlns:xsd="http://www.w3.org/2001/XMLSchema"
>
  <!-- 
	=============================================================================
	Filename: custom.xslt
	Description: Custom extensions for schema and code generation
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Created by: N.Davis                        Date: 2010-04-09
	Modified by:                               Date:
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Usually for custom extensions to code generation

	============================================================================
	-->

  <!-- 
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Add your own namespace declarations here
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->
  <xsl:template name="customNamespaceDeclarations">
    using Cobos.Data;
    using Cobos.Data.Statements;
    using Cobos.Data.Utilities;
    using Cobos.Utilities.Extensions;
    using Cobos.Utilities.Text;
    //// using Intergraph.AsiaPac.Cad.Interop.Common;
  </xsl:template>

  <!-- 
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Define your own type of database base classes.
	In most cases the defaults are fine, but in some cases you may need
	to define the interfaces if a 3rd party doesn't implement properly.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

  <!--	
	<xsl:variable name="dbConnectionType">DbConnection</xsl:variable>
	<xsl:variable name="dbTransactionType">DbTransaction</xsl:variable>
	<xsl:variable name="dbCommandType">DbCommand</xsl:variable>
	<xsl:variable name="dataAdapterType">DbDataAdapter</xsl:variable>
	-->

  <xsl:variable name="dbConnectionType">IDbConnection</xsl:variable>
  <xsl:variable name="dbTransactionType">IDbTransaction</xsl:variable>
  <xsl:variable name="dbCommandType">IDbCommand</xsl:variable>
  <xsl:variable name="dataAdapterType">IDbDataAdapter</xsl:variable>

  <!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Add your own data object extensions here
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

  <xsl:template match="cobos:Object" mode="customDataObjectExtensions">
  </xsl:template>

  <!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Add your own TableAdapter extensions here
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

  <xsl:template match="cobos:Object" mode="customTableAdapterExtensions">
  </xsl:template>

  <!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Add your own custom code extensions here
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

  <xsl:template match="cobos:Object" mode="customCodeExtensions">
    public static class <xsl:value-of select="@className"/>TableAdapter
    {
        #region Public properties

        public static readonly SqlSelectTemplate SelectTemplate = new SqlSelectTemplate(Select, From, InnerJoin, Where, GroupBy, OrderBy, true);

        #endregion
    
        #region Static Data

        private const string Select = "<xsl:apply-templates select=".//cobos:Property" mode="sqlSelect"/>";

        private const string From = "<xsl:value-of select="@dbTable"/>";

        private const string GroupBy = <xsl:apply-templates mode="sqlGroupBy" select="."/>;

        private const string OrderBy = <xsl:apply-templates mode="sqlOrderBy" select="."/>;

        private static readonly string[] InnerJoin = <xsl:apply-templates mode="sqlJoin" select="."/>;

        private static readonly string[] Where = <xsl:apply-templates mode="sqlWhere" select="."/>;
    <xsl:apply-templates select="./cobos:Metadata/cobos:StringAggregate" mode="aggregatorDecl"/>
        #endregion

        #region Public methods
    <xsl:apply-templates select="." mode="getQueryMethodBody"/>

    <xsl:apply-templates select="." mode="getDataMethodBody"/>

    <xsl:apply-templates select="." mode="createDataSetMethodBody"/>

    <xsl:apply-templates select="./cobos:Metadata/cobos:StringAggregate" mode="getAggregatorMethodBody"/>
        #endregion
    }
  </xsl:template>

  <!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	match property types
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

  <xsl:template match="cobos:Property[ @stringFormat = 'Separator' ]" mode="propertyType">
    <xsl:text>string </xsl:text>
  </xsl:template>

  <xsl:template match="cobos:Property[ @stringFormat = 'CadDts' ]" mode="propertyType">
    <xsl:text>DateTime</xsl:text>
    <xsl:apply-templates select="@minOccurs" mode="propertyType"/>
  </xsl:template>

  <xsl:template match="cobos:Property[ @stringFormat = 'CadBoolean' ]" mode="propertyType">
    <xsl:text>bool</xsl:text>
    <xsl:apply-templates select="@minOccurs" mode="propertyType"/>
  </xsl:template>

  <!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	get function body implementation for special string formats
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

  <xsl:template match="cobos:Property[ @stringFormat = 'CadDts' ]" mode="propertyGet">
    <xsl:variable name="columnName">
      <xsl:apply-templates mode="qualifiedName" select="."/>
    </xsl:variable>
    <xsl:value-of select="concat( 'return ServiceConfiguration.DataFormat.ConvertFromDTS(this.ObjectDataRow.', $columnName, ');' )"/>
  </xsl:template>

  <xsl:template match="cobos:Property[ @stringFormat = 'CadBoolean' ]" mode="propertyGet">
    <xsl:variable name="columnName">
      <xsl:apply-templates mode="qualifiedName" select="."/>
    </xsl:variable>
    <xsl:value-of select="concat( 'return ServiceConfiguration.DataFormat.ParseCadBooleanString(this.ObjectDataRow.', $columnName, ');' )"/>
  </xsl:template>

  <xsl:template match="cobos:Property[ @stringFormat = 'Separator' ]" mode="propertyGet">
    <xsl:variable name="separator">
      <xsl:text>'</xsl:text>
      <xsl:value-of select="substring-before( @formatArgs, ' ' )"/>
      <xsl:text>'</xsl:text>
    </xsl:variable>
    <xsl:variable name="index">
      <xsl:value-of select="substring-after( @formatArgs, ' ' )"/>
    </xsl:variable>
    <xsl:variable name="columnName">
      <xsl:apply-templates mode="qualifiedName" select="."/>
    </xsl:variable>
    <xsl:value-of select="concat( 'return StringSeparator.GetTokenAt( this.ObjectDataRow.', $columnName, ', ', $separator, ', ', $index, ' );' )"/>
  </xsl:template>

  <!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	set function body implementation for special string formats
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

  <xsl:template match="cobos:Property[ @stringFormat = 'CadDts' ]" mode="propertySet">
    <xsl:variable name="columnName">
      <xsl:apply-templates mode="qualifiedName" select="."/>
    </xsl:variable>
    <xsl:variable name="value">
      <xsl:apply-templates mode="propertySetValue" select="."/>
    </xsl:variable>
    <xsl:value-of select="concat( 'this.ObjectDataRow.', $columnName, ' = ServiceConfiguration.DataFormat.ConvertToDTS(', $value, ');' )"/>
  </xsl:template>

  <xsl:template match="cobos:Property[ @stringFormat = 'CadBoolean' ]" mode="propertySet">
    <xsl:variable name="columnName">
      <xsl:apply-templates mode="qualifiedName" select="."/>
    </xsl:variable>
    <xsl:variable name="value">
      <xsl:apply-templates mode="propertySetValue" select="."/>
    </xsl:variable>
    <xsl:value-of select="concat( 'this.ObjectDataRow.', $columnName, ' = ServiceConfiguration.DataFormat.GetCadBooleanString(', $value, ');' )"/>
  </xsl:template>

  <xsl:template match="cobos:Property[ @stringFormat = 'Separator' ]" mode="propertySet">
    <xsl:variable name="separator">
      <xsl:text>'</xsl:text>
      <xsl:value-of select="substring-before( @formatArgs, ' ' )"/>
      <xsl:text>'</xsl:text>
    </xsl:variable>
    <xsl:variable name="index">
      <xsl:value-of select="substring-after( @formatArgs, ' ' )"/>
    </xsl:variable>
    <xsl:variable name="columnName">
      <xsl:apply-templates mode="qualifiedName" select="."/>
    </xsl:variable>
    <xsl:value-of select="concat( '/*this.ObjectDataRow.', $columnName, ' = StringSeparator.SetTokenAt( this.ObjectDataRow.', $columnName, ', ', $separator, ', ', $index, ', value );*/' )"/>
  </xsl:template>

  <!-- Nullable string formatted types -->
  <xsl:template match="cobos:Property[ @minOccurs = 0 and ( @stringFormat = 'CadDts' or @stringFormat = 'CadBoolean' ) ]" mode="propertySetValue">
    <xsl:text>value.Value</xsl:text>
  </xsl:template>

  <!-- string token separator -->
  <xsl:template match="cobos:Property[ @minOccurs = 0 and @stringFormat = 'Separator' ]" mode="propertySetValue">
    <xsl:text>value</xsl:text>
  </xsl:template>

  <!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	find by methods
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

  <xsl:template match="cobos:Property[ @stringFormat = 'CadDts' ]" mode="findByMethodParamValue">
    <xsl:value-of select="concat( 'ServiceConfiguration.DataFormat.ConvertToDTS(', @name, ')' )"/>
  </xsl:template>

  <xsl:template match="cobos:Property[ @stringFormat = 'Separator' ]" mode="findByMethodParamValue">
    <!-- can't find by seperator values... yet -->
  </xsl:template>

  <!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	TableAdapter extensions 
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

  <!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	GetQuery method body - return the query, aggregated types will return 
	a custom aggregator as well.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	
	

	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

  <xsl:template match="cobos:Object[ not( ./cobos:Metadata/cobos:StringAggregate ) ]" mode="getQueryMethodBody">
        public static DatabaseQuery GetQuery(string[] where, string orderBy)
        {
            return new DatabaseQuery(SelectTemplate.ToString(where, null, orderBy), new <xsl:value-of select="@datasetTableType"/>());
        }

        public static DatabaseQuery GetQuery(<xsl:value-of select="@datasetTableType"/> table, string[] where, string orderBy)
        {
            return new DatabaseQuery(SelectTemplate.ToString(where, null, orderBy), table);
        }
  </xsl:template>


  <xsl:template match="cobos:Object[ ./cobos:Metadata/cobos:StringAggregate ]" mode="getQueryMethodBody">
        public static DatabaseQuery GetQuery(string[] where, string orderBy)
        {
            return new DatabaseQuery(SelectTemplate.ToString(where, null, orderBy), new <xsl:value-of select="@datasetTableType"/>(), Aggregator);
        }

        public static DatabaseQuery GetQuery(<xsl:value-of select="@datasetTableType"/> table, string[] where, string orderBy)
        {
            return new DatabaseQuery(SelectTemplate.ToString(where, null, orderBy), table, Aggregator);
        }
  </xsl:template>

  <!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Aggregator member declaration.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->
  <xsl:template match="cobos:StringAggregate" mode="aggregatorDecl">
    <xsl:value-of select="concat($newlineIndent, 'private static readonly IDataRowAggregator Aggregator = GetAggregator();', $newline)" />
  </xsl:template>

  <!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	GetAggregator method body.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	
	
	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

  <xsl:template match="cobos:StringAggregate" mode="getAggregatorMethodBody">
        public static IDataRowAggregator GetAggregator()
        {
    <xsl:value-of select="concat($indent2, ../../@datasetTableType, ' table = new ', ../../@datasetTableType, '();')" />

            DataColumn aggregateOn = table.<xsl:apply-templates mode="qualifiedName" select="ancestor::cobos:Object[ position() = last() ]//cobos:Property[ @name = current()/cobos:On/text() ]"/>Column;

            DataColumn[] groupBy = new DataColumn[]
            {<xsl:value-of select="$newline" />
    <xsl:apply-templates select="./cobos:Group/cobos:By" mode="aggregateBy"/>
            };

            DataColumn[] sortBy = new DataColumn[]
            {<xsl:value-of select="$newline" />
    <xsl:apply-templates select="./cobos:Order/cobos:By" mode="aggregateBy"/>
            };

            DataRowColumnComparer comparer = new DataRowColumnComparer(sortBy);

            return new DataRowColumnAggregator(aggregateOn, groupBy, comparer);
        }
  </xsl:template>

  <!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Aggregation sort by/group by
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

  <xsl:template match="cobos:By" mode="aggregateBy">
    <xsl:value-of select="$indent4"/>
    <xsl:text>table.</xsl:text>
    <xsl:apply-templates mode="qualifiedName" select="ancestor::cobos:Object[position() = last()]//cobos:Property[@name = current()/text()]"/>
    <xsl:text>Column</xsl:text>
    <xsl:if test="not(position() = last())">
      <xsl:value-of select="concat(',', $newline)"/>
    </xsl:if>
  </xsl:template>

  <!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	GetData method body - for an Object NOT containing Reference objects:
	
	1)	Construct the query object and call the database.
	2) Create the return objects.
	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	
		public static List<AgencyEventSummary> GetData( IDatabaseAdapter database, string[] where, string orderBy )
		{
			DatabaseQuery query = GetQuery( where, orderBy );

			database.Fill( query );

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
  <xsl:template match="cobos:Object" mode="getDataMethodBody">
        public static <xsl:apply-templates select="." mode="listDecl"/> GetData(IDatabaseAdapter database, string[] where, string orderBy)
        {
            DatabaseQuery query = GetQuery(where, orderBy);

            database.Fill(query);

    <xsl:value-of select="concat($indent2, @datasetTableType, ' table = (', @datasetTableType, ')query.GetResult();')" />

            int numRows = table.Rows.Count;

    <xsl:variable name="listDecl">
      <xsl:apply-templates select="." mode="listDecl"/>
    </xsl:variable>
    <xsl:value-of select="concat($indent2, $listDecl, ' results = new ', $listDecl, '(numRows);')" />

    <xsl:value-of select="concat($newlineIndent3, 'for (int row = 0; row &lt; numRows; ++row)')" />
            {
                results.Add(new <xsl:value-of select="@className"/>(table[row]));
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
  <xsl:template match="cobos:Object[ cobos:Reference ]" mode="getDataMethodBody">
        public static <xsl:apply-templates select="." mode="listDecl"/> GetData(IDatabaseAdapter database, string[] where, string orderBy)
        {
    <xsl:value-of select="$indent2" />
    <xsl:apply-templates select=".|./cobos:Reference" mode="getDataMethodQueryDecl"/>

            DatabaseQuery[] queries = new DatabaseQuery[]
            {
    <xsl:value-of select="$indent3" />
    <xsl:apply-templates select=".|./cobos:Reference" mode="getDataMethodQueryInstance"/>
            };

            database.FillAsynch(queries);

    <xsl:value-of select="$indent2" />
    <xsl:apply-templates select=".|./cobos:Reference" mode="getDataMethodTableDecl"/>

            DataSet dataset = CreateDataSet(<xsl:apply-templates select=".|./cobos:Reference" mode="getDataMethodCreateDataSetArgs" />);
            queries = null;

            int numRows = table<xsl:value-of select="@name"/>.Rows.Count;

    <xsl:variable name="listDecl">
      <xsl:apply-templates select="." mode="listDecl"/>
    </xsl:variable>
    <xsl:value-of select="concat($indent2, $listDecl, ' results = new ', $listDecl, '(numRows);')" />

    <xsl:value-of select="concat($newlineIndent3, 'for (int row = 0; row &lt; numRows; ++row)')" />
            {
                results.Add(new <xsl:value-of select="@className"/>(table<xsl:value-of select="@name"/>[row]));
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

  <xsl:template match="cobos:Object" mode="getDataMethodQueryInstance">
    <xsl:value-of select="concat( 'query', @className )"/>
    <xsl:if test="not( position() = last() )">
      <xsl:value-of select="concat( ',', $newlineIndent4 )"/>
    </xsl:if>
  </xsl:template>

  <xsl:template match="cobos:Reference" mode="getDataMethodQueryInstance">
    <xsl:value-of select="concat( 'query', @ref )"/>
    <xsl:if test="not( position() = last() )">
      <xsl:value-of select="concat( ',', $newlineIndent4 )"/>
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

  <xsl:template match="cobos:Object" mode="getDataMethodTableDecl">
    <xsl:value-of select="concat( @datasetTableType, ' table', @className, ' = (', @datasetTableType, ')query', @className, '.GetResult();' )"/>
    <xsl:if test="not( position() = last() )">
      <xsl:value-of select="$newlineIndent3"/>
    </xsl:if>
  </xsl:template>

  <xsl:template match="cobos:Reference" mode="getDataMethodTableDecl">
    <xsl:value-of select="concat( @datasetTableType, ' table', @ref, ' = (', @datasetTableType, ')query', @ref, '.GetResult();' )"/>
    <xsl:if test="not( position() = last() )">
      <xsl:value-of select="$newlineIndent3"/>
    </xsl:if>
  </xsl:template>

  <!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	GetData method body - DatabaseQuery declaration for an Object
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	
		DatabaseQuery queryAgencyEvent = GetQuery(where, orderBy);
	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

  <xsl:template match="cobos:Object" mode="getDataMethodQueryDecl">
    <xsl:value-of select="concat( 'DatabaseQuery query', @name, ' = GetQuery(where, orderBy);' )"/>
    <xsl:if test="not( position() = last() )">
      <xsl:value-of select="$newlineIndent3"/>
    </xsl:if>
  </xsl:template>

  <!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	GetData method body - DatabaseQuery declaration for an Reference
	(no filter metadata for the reference)	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	
		DatabaseQuery queryEventComment = EventCommentTableAdapter<DbConnectionType, DbCommandType, DataAdapterType>.GetQuery( where, null );
	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

  <xsl:template match="cobos:Reference[ not( descendant::cobos:Filters ) ]" mode="getDataMethodQueryDecl">
    <xsl:value-of select="concat( 'DatabaseQuery query', @ref, ' = ', @ref, 'TableAdapter.GetQuery(where, null);' )"/>
    <xsl:if test="not( position() = last() )">
      <xsl:value-of select="$newlineIndent3"/>
    </xsl:if>
  </xsl:template>

  <!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	GetData method body - DatabaseQuery declaration for an Reference
	(with filter metadata for the reference)
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	
		DatabaseQuery queryEventComment = EventCommentTableAdapter<DbConnectionType, DbCommandType, DataAdapterType>.GetQuery( ArrayExtensions.ConcatenateAll<string>(where, new string[] {"EVCOM.comm_scope is null or EVCOM.comm_scope = AEVEN.ag_id"}), null );
	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

  <xsl:template match="cobos:Reference[ descendant::cobos:Filters ]" mode="getDataMethodQueryDecl">
    <xsl:variable name="filter">
      <xsl:text>new string[] { </xsl:text>
      <xsl:apply-templates select="descendant::cobos:Filters" mode="sqlWhere"/>
      <xsl:text> }</xsl:text>
    </xsl:variable>

    <xsl:value-of select="concat( 'DatabaseQuery query', @ref, ' = ', @ref, 'TableAdapter' )"/>
    <xsl:text disable-output-escaping="yes"><![CDATA[<DbConnectionType, DbCommandType, DataAdapterType>]]></xsl:text>
    <xsl:text>.GetQuery(</xsl:text>
    <xsl:text disable-output-escaping="yes"><![CDATA[ArrayExtensions.ConcatenateAll<string>]]></xsl:text>
    <xsl:value-of select="concat( '(new string[][] { where, ', $filter, ' }), null);' )"/>
    <xsl:if test="not( position() = last() )">
      <xsl:value-of select="$newlineIndent3"/>
    </xsl:if>
  </xsl:template>

  <!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	GetData/CreateDataSet arguments list
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

  <xsl:template match="cobos:Object" mode="getDataMethodCreateDataSetArgs">
    <xsl:value-of select="concat( 'table', @className )"/>
    <xsl:if test="not( position() = last() )">, </xsl:if>
  </xsl:template>

  <xsl:template match="cobos:Reference" mode="getDataMethodCreateDataSetArgs">
    <xsl:value-of select="concat( 'table', @ref )"/>
    <xsl:if test="not( position() = last() )">, </xsl:if>
  </xsl:template>

  <xsl:template match="cobos:Object" mode="getDataMethodCreateDataSetArgsDecl">
    <xsl:value-of select="concat( @datasetTableType, ' table', @className )"/>
    <xsl:if test="not( position() = last() )">, </xsl:if>
  </xsl:template>

  <xsl:template match="cobos:Reference" mode="getDataMethodCreateDataSetArgsDecl">
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

  <xsl:template match="cobos:Object" mode="createDataSetMethodBody"></xsl:template>

  <!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	CreateDataSet method body - for an Object containing Reference objects:
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

  <xsl:template match="cobos:Object[ cobos:Reference ]" mode="createDataSetMethodBody">
        public static DataSet CreateDataSet(<xsl:apply-templates select=".|./cobos:Reference" mode="getDataMethodCreateDataSetArgsDecl"/>)
        {
            DataSet dataset = new DataSet();
      <xsl:apply-templates select=".|./cobos:Reference" mode="createDataSetAddTable"/>
      <xsl:apply-templates select="./cobos:Reference" mode="createDataSetDataRelation"/>
            return dataset;
        }
  </xsl:template>

  <!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	CreateDataSet method body - add tables to the dataset
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		
		dataset.Tables.Add(tableAgencyEvent);
		dataset.Tables.Add(tableEventComment);
		dataset.Tables.Add(tableDisposition);
	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

  <xsl:template match="cobos:Object" mode="createDataSetAddTable">
    <xsl:value-of select="concat( 'dataset.Tables.Add(table', @className, ');' )"/>
    <xsl:if test="not( position() = last() )">
      <xsl:value-of select="$newlineIndent3"/>
    </xsl:if>
  </xsl:template>

  <xsl:template match="cobos:Reference" mode="createDataSetAddTable">
    <xsl:value-of select="concat( 'dataset.Tables.Add(table', @ref, ');' )"/>
    <xsl:if test="not( position() = last() )">
      <xsl:value-of select="$newlineIndent3"/>
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

  <xsl:template match="cobos:Reference" mode="createDataSetDataRelation">
    <xsl:variable name="relationName" select="concat( 'relation', ../@className, @ref )"/>

    <xsl:value-of select="$newlineIndent3"/>
    <xsl:value-of select="concat( 'DataRelation ', $relationName, ' = new DataRelation(&quot;', ../@className, @ref, '&quot;, ', 'table', ../@className, '.', @key, 'Column, ', 'table', @ref, '.', @refer, 'Column, false);' )" />
    <xsl:value-of select="$newlineIndent3"/>

    <xsl:value-of select="concat( $relationName, '.ExtendedProperties.Add(&quot;typedChildren&quot;, &quot;Get', @name, '&quot;);' )"/>
    <xsl:value-of select="$newlineIndent3"/>

    <xsl:value-of select="concat( $relationName, '.ExtendedProperties.Add(&quot;typedParent&quot;, &quot;', ../@name, '&quot;);' )"/>
    <xsl:value-of select="$newlineIndent3"/>

    <xsl:value-of select="concat( 'dataset.Relations.Add(', $relationName, ');' )"/>
    <xsl:value-of select="$newlineIndent3"/>

  </xsl:template>

</xsl:stylesheet>
