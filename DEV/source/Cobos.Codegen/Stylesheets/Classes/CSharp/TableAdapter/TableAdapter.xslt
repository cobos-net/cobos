<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
					 xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
					 xmlns:msxsl="urn:schemas-microsoft-com:xslt"
					 xmlns:cobos="http://schemas.cobos.co.uk/datamodel/1.0.0"
					 xmlns="http://schemas.cobos.co.uk/datamodel/1.0.0"
					 xmlns:xsd="http://www.w3.org/2001/XMLSchema"
					 exclude-result-prefixes="msxsl">
  <xsl:include href="./Delegates.xslt"/>
  <xsl:include href="./New.xslt"/>
  <xsl:include href="./GetEntities.xslt"/>
  <xsl:include href="./Changes.xslt"/>
  <xsl:include href="./Fill.xslt"/>
  <xsl:include href="./Insert.xslt"/>
  <xsl:include href="./Update.xslt"/>
  <xsl:include href="./Delete.xslt"/>
  <xsl:include href="./Sql/Sql.xslt"/>
  <!-- 
	=============================================================================
	Filename: .xslt
	Description: 
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Created by: N.Davis                        Date: 2010-04-09
	Modified by:                               Date:
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Notes: 
	
	
	============================================================================
	-->
  <!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Create data adapter
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->
  <xsl:template match="cobos:Object" mode="tableAdapter">
    <![CDATA[/// <summary>
    /// Provides an adapter for the strongly typed DataTable.
    /// </summary>]]>
    public partial class <xsl:value-of select="@className"/>TableAdapter
    {
        <![CDATA[/// <summary>
        /// Gets the select template.
        /// </summary>]]>
        public static readonly SqlSelectTemplate SelectTemplate = new SqlSelectTemplate(Select, From, InnerJoin, Where, GroupBy, OrderBy, true);

        <![CDATA[/// <summary>
        /// Represents the columns to select.
        /// </summary>]]>
        private const string Select = "<xsl:apply-templates select=".//cobos:Property" mode="sqlSelect"/>";

        <![CDATA[/// <summary>
        /// Represents the table to select.
        /// </summary>]]>
        private const string From = "<xsl:value-of select="@dbTable"/>";
        
        <![CDATA[/// <summary>
        /// Represents the group by clause.
        /// </summary>]]>
        private const string GroupBy = <xsl:apply-templates mode="sqlGroupBy" select="."/>;

        <![CDATA[/// <summary>
        /// Represents the order by clause.
        /// </summary>]]>
        private const string OrderBy = <xsl:apply-templates mode="sqlOrderBy" select="."/>;

        <![CDATA[/// <summary>
        /// Represents the tables to join on.
        /// </summary>]]>
        private static readonly string[] InnerJoin = <xsl:apply-templates mode="sqlJoin" select="."/>;

        <![CDATA[/// <summary>
        /// Represents the sub-clauses in the where clause.
        /// </summary>]]>
        private static readonly string[] Where = <xsl:apply-templates mode="sqlWhere" select="."/>;

        <![CDATA[/// <summary>
        /// Initializes a new instance of the <see cref="]]><xsl:value-of select="@className"/><![CDATA[TableAdapter"/> class.
        /// </summary>
        /// <param name="table">The source table.</param>
        /// <param name="database">The database connection.</param>]]>
        public <xsl:value-of select="@className"/>TableAdapter(<xsl:value-of select="@datasetTableType"/> table, Cobos.Data.IDatabaseAdapter database)
        {
            this.Database = database;
            this.Table = table;
            this.Table.RowChanging += this.Table_RowChanging;
            this.Table.RowChanged += this.Table_RowChanged;
            this.Table.RowDeleting += this.Table_RowDeleting;
            this.Table.RowDeleted += this.Table_RowDeleted;
            this.Table.TableNewRow += this.Table_TableNewRow;
        }

        <![CDATA[/// <summary>
        /// Initializes a new instance of the <see cref="]]><xsl:value-of select="@className"/><![CDATA[TableAdapter"/> class.
        /// </summary>
        /// <param name="database">The database connection.</param>]]>
        public <xsl:value-of select="@className"/>TableAdapter(Cobos.Data.IDatabaseAdapter database)
            : this(new <xsl:value-of select="@datasetTableType"/>(), database)
        {
        }

        <![CDATA[/// <summary>
        /// Event fired before an entity is changed.
        /// </summary>]]>
        public event Action<![CDATA[<]]><xsl:value-of select="@className"/><![CDATA[>]]> OnChanging<xsl:value-of select="@className"/>;

        <![CDATA[/// <summary>
        /// Event fired after an entity is changed.
        /// </summary>]]>
        public event Action<![CDATA[<]]><xsl:value-of select="@className"/><![CDATA[>]]> OnChanged<xsl:value-of select="@className"/>;

        <![CDATA[/// <summary>
        /// Event fired when an entity is being deleted.
        /// </summary>]]>
        public event Action<![CDATA[<]]><xsl:value-of select="@className"/><![CDATA[>]]> OnDeleting<xsl:value-of select="@className"/>;

        <![CDATA[/// <summary>
        /// Event fired after an entity is deleted.
        /// </summary>]]>
        public event Action<![CDATA[<]]><xsl:value-of select="@className"/><![CDATA[>]]> OnDeleted<xsl:value-of select="@className"/>;

        <![CDATA[/// <summary>
        /// Event fired after an entity is added.
        /// </summary>]]>
        public event Action<![CDATA[<]]><xsl:value-of select="@className"/><![CDATA[>]]> OnAdded<xsl:value-of select="@className"/>;

        <![CDATA[/// <summary>
        /// Gets the object representing the database connection.
        /// </summary>]]>
        public Cobos.Data.IDatabaseAdapter Database
        {
            get;
            private set;
        }
        
        <![CDATA[/// <summary>
        /// Gets the object representing the data table.
        /// </summary>]]>
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

  <!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Common templates for generating generic List<> declarations.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

  <xsl:template match="cobos:Object" mode="listDecl">
    <xsl:value-of select="concat('List', $lt, @className, $gt)"/>
  </xsl:template>

  <xsl:template match="cobos:Object" mode="listDeclDataRow">
    <xsl:value-of select="concat('List', $lt, @datasetRowType, $gt)"/>
  </xsl:template>

  <xsl:template match="cobos:Reference" mode="listDecl">
    <xsl:value-of select="concat('List', $lt, @ref, $gt)"/>
  </xsl:template>

</xsl:stylesheet>