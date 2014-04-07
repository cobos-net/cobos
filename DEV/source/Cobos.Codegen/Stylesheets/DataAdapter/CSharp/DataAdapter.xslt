<xsl:stylesheet version="1.0"
						xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
						xmlns:msxsl="urn:schemas-microsoft-com:xslt"
						exclude-result-prefixes="msxsl"
						xmlns="http://schemas.cobos.co.uk/datamodel/1.0.0"
						xmlns:cobos="http://schemas.cobos.co.uk/datamodel/1.0.0"
						xmlns:doc="http://schemas.cobos.co.uk/documentation/1.0.0"
						xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <xsl:output method="text" omit-xml-declaration="yes"/>
  <xsl:strip-space elements="*"/>
  <!-- 
  =============================================================================
  Stylesheet parameters.
  =============================================================================
  -->
  <xsl:param name="codeNamespace"/>
  <xsl:param name="xmlNamespace"/>
  <!-- 
  =============================================================================
  Stylesheet includes.
  =============================================================================
  -->
  <xsl:include href="../../Utilities/Utilities.inc"/>
  <xsl:include href="./Methods/Methods.inc"/>
  <xsl:include href="./Sql/Sql.inc"/>
  <!-- 
  =============================================================================
  Code namespace definition.
  =============================================================================
  -->
  <xsl:template match="/cobos:DataModel">
    <xsl:call-template name="generatedCSharpWarning"/>
    <xsl:value-of select="concat('namespace ', $codeNamespace)"/>
    <xsl:value-of select="concat($newline, '{')"/>
    <xsl:value-of select="concat($newline, '    using System.Linq;', $newline)"/>
    <xsl:apply-templates select="cobos:Object" mode="classDefinition"/>
    <xsl:value-of select="concat($newline, '}')"/>
  </xsl:template>
  <!-- 
  =============================================================================
  Class definition.
  =============================================================================
  -->
  <xsl:template match="cobos:Object" mode="classDefinition">
    /// &lt;summary&gt;
    /// Class definition for the <xsl:value-of select="@name"/> row data adapter.
    /// &lt;/summary&gt;
    public partial class <xsl:value-of select="@className"/>TableAdapter
    {
        /// &lt;summary&gt;
        /// Gets the select template.
        /// &lt;/summary&gt;
        public static readonly Cobos.Data.Statements.SqlSelectTemplate SelectTemplate = new Cobos.Data.Statements.SqlSelectTemplate(Select, From, InnerJoin, OuterJoin, Where, GroupBy, OrderBy, true);

        /// &lt;summary&gt;
        /// Represents the columns to select.
        /// &lt;/summary&gt;
        private const string Select = "<xsl:apply-templates select=".//cobos:Property" mode="sqlSelect"/>";

        /// &lt;summary&gt;
        /// Represents the table to select.
        /// &lt;/summary&gt;
        private const string From = "<xsl:value-of select="@dbTable"/>";
        
        /// &lt;summary&gt;
        /// Represents the group by clause.
        /// &lt;/summary&gt;
        private const string GroupBy = <xsl:apply-templates mode="sqlGroupBy" select="."/>;

        /// &lt;summary&gt;
        /// Represents the order by clause.
        /// &lt;/summary&gt;
        private const string OrderBy = <xsl:apply-templates mode="sqlOrderBy" select="."/>;

        /// &lt;summary&gt;
        /// Represents the tables to join on.
        /// &lt;/summary&gt;
        private static readonly string[] InnerJoin = <xsl:apply-templates mode="sqlInnerJoin" select="."/>;

        /// &lt;summary&gt;
        /// Represents the tables to join on.
        /// &lt;/summary&gt;
        private static readonly string[] OuterJoin = <xsl:apply-templates mode="sqlOuterJoin" select="."/>;

        /// &lt;summary&gt;
        /// Represents the sub-clauses in the where clause.
        /// &lt;/summary&gt;
        private static readonly string[] Where = <xsl:apply-templates mode="sqlWhere" select="."/>;

        /// &lt;summary&gt;
        /// Delegate method providing a Database connection object.
        /// &lt;/summary&gt;
        private global::System.Func&lt;global::System.Data.IDbConnection&gt; databaseConnection;

        /// &lt;summary&gt;
        /// Initializes a new instance of the &lt;see cref="<xsl:value-of select="@className"/>TableAdapter"/&gt; class.
        /// &lt;/summary&gt;
        /// &lt;param name="table"&gt;The source table.&lt;/param&gt;
        /// &lt;param name="database"&gt;The database connection.&lt;/param&gt;
        public <xsl:value-of select="@className"/>TableAdapter(<xsl:value-of select="@datasetTableType"/> table, global::System.Func&lt;global::System.Data.IDbConnection&gt; databaseConnection)
        {
            this.databaseConnection = databaseConnection;
            this.Table = table;
            this.Table.RowChanging += this.Table_RowChanging;
            this.Table.RowChanged += this.Table_RowChanged;
            this.Table.RowDeleting += this.Table_RowDeleting;
            this.Table.RowDeleted += this.Table_RowDeleted;
            this.Table.TableNewRow += this.Table_TableNewRow;
        }

        /// &lt;summary&gt;
        /// Initializes a new instance of the &lt;see cref="<xsl:value-of select="@className"/>TableAdapter"/&gt; class.
        /// &lt;/summary&gt;
        /// &lt;param name="database"&gt;The database connection.&lt;/param&gt;
        public <xsl:value-of select="@className"/>TableAdapter(global::System.Func&lt;global::System.Data.IDbConnection&gt; databaseConnection)
            : this(new <xsl:value-of select="@datasetTableType"/>(), databaseConnection)
        {
        }

        /// &lt;summary&gt;
        /// Event fired before an entity is changed.
        /// &lt;/summary&gt;
        public event global::System.Action&lt;<xsl:value-of select="@className"/>&gt; OnChanging<xsl:value-of select="@className"/>;

        /// &lt;summary&gt;
        /// Event fired after an entity is changed.
        /// &lt;/summary&gt;
        public event global::System.Action&lt;<xsl:value-of select="@className"/>&gt; OnChanged<xsl:value-of select="@className"/>;

        /// &lt;summary&gt;
        /// Event fired when an entity is being deleted.
        /// &lt;/summary&gt;
        public event global::System.Action&lt;<xsl:value-of select="@className"/>&gt; OnDeleting<xsl:value-of select="@className"/>;

        /// &lt;summary&gt;
        /// Event fired after an entity is deleted.
        /// &lt;/summary&gt;
        public event global::System.Action&lt;<xsl:value-of select="@className"/>&gt; OnDeleted<xsl:value-of select="@className"/>;

        /// &lt;summary&gt;
        /// Event fired after an entity is added.
        /// &lt;/summary&gt;
        public event global::System.Action&lt;<xsl:value-of select="@className"/>&gt; OnAdded<xsl:value-of select="@className"/>;
        
        /// &lt;summary&gt;
        /// Gets the object representing the data table.
        /// &lt;/summary&gt;
        public <xsl:value-of select="@datasetTableType"/> Table
        {
            get;
            private set;
        }
        <xsl:apply-templates select="." mode="acceptChangesMethodBody"/>
        <xsl:apply-templates select="." mode="addNewMethodBody"/>
        <xsl:apply-templates select="." mode="createNewMethodBody"/>
        <xsl:apply-templates select="." mode="getEntityByMethodBody"/>
        <xsl:apply-templates select="." mode="getEntitiesMethodBody"/>
        <xsl:apply-templates select="." mode="hasChangesMethodBody"/>
        <xsl:apply-templates select="." mode="rejectChangesMethodBody"/>
        <xsl:apply-templates select="." mode="fillMethodBody"/>
        <xsl:apply-templates select="." mode="insertRowsMethodBody"/>
        <xsl:apply-templates select="." mode="updateRowsMethodBody"/>
        <xsl:apply-templates select="." mode="deleteRowsMethodBody"/>
        <xsl:apply-templates select="." mode="delegatesDeclarations"/>
    }
  </xsl:template> 
</xsl:stylesheet>