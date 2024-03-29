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
  <!-- 
  =============================================================================
  Code namespace definition.
  =============================================================================
  -->
  <xsl:template match="/cobos:DataModel">
    <xsl:call-template name="generatedCSharpWarning"/>
    <xsl:value-of select="concat('namespace ', $codeNamespace)"/>
    <xsl:value-of select="concat($newline, '{')"/>
    <xsl:apply-templates select="." mode="dataAdapter"/>
    <xsl:value-of select="concat($newline, '}')"/>
  </xsl:template>
  <!-- 
  =============================================================================
  Data Model adapter definition.
  =============================================================================
  -->
  <xsl:template match="cobos:DataModel" mode="dataAdapter">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndentLevel1"/>
    </xsl:variable>
    <xsl:apply-templates select="." mode="commentDataAdapter"/>
    <xsl:value-of select="concat($indent, 'public partial class ', @name, 'Adapter')"/>
    <xsl:value-of select="concat($indent, '{')"/>
    <xsl:apply-templates select="." mode="constructor"/>
    <xsl:apply-templates select="." mode="propertyDefinition"/>
    <xsl:apply-templates select="cobos:Object" mode="propertyDefinition"/>
    <xsl:apply-templates select="." mode="acceptChanges"/>
    <xsl:apply-templates select="." mode="fill"/>
    <xsl:apply-templates select="cobos:Object" mode="get"/>
    <xsl:apply-templates select="." mode="rejectChanges"/>
    <xsl:apply-templates select="cobos:Object" mode="fillTable"/>
    <xsl:value-of select="concat($indent, '}')"/>
  </xsl:template>
  <!-- 
  =============================================================================
  Constructor definition.
  =============================================================================
  -->
  <xsl:template match="cobos:DataModel" mode="constructor">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndentLevel2"/>
    </xsl:variable>
    <xsl:apply-templates select="." mode="commentConstructor"/>
    <xsl:value-of select="concat($indent, 'public ', @name, 'Adapter(global::Cobos.Data.IDatabaseAdapter database)')"/>
    <xsl:value-of select="concat($indent, '{')"/>
    <xsl:value-of select="concat($indent, '    this.Database = database;')"/>
    <xsl:value-of select="concat($indent, '    this.DataModel = new ', @name,'();')"/>
    <xsl:value-of select="$indent"/>
    <xsl:value-of select="concat($indent, '    var connectionString = database.ConnectionString;')"/>
    <xsl:value-of select="concat($indent, '    var providerFactory = database.ProviderFactory;')"/>
    <xsl:value-of select="$indent"/>
    <xsl:apply-templates select="cobos:Object" mode="constructorBody"/>
    <xsl:value-of select="concat($indent, '}')"/>
  </xsl:template>
  <!-- 
  =============================================================================
  Constructor body definition.
  =============================================================================
  -->
  <xsl:template match="cobos:Object" mode="constructorBody">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndentLevel2"/>
    </xsl:variable>
    <xsl:value-of select="concat($indent, 'this.', @className, ' = new ', @className, 'DataAdapter(connectionString, providerFactory, this.DataModel.', @className,');')"/>
  </xsl:template>
  <!-- 
  =============================================================================
  Property definitions.
  =============================================================================
  -->
  <xsl:template match="cobos:DataModel" mode="propertyDefinition">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndentLevel2"/>
    </xsl:variable>
    <xsl:value-of select="$indent"/>
    <xsl:value-of select="concat($indent, '/// &lt;summary&gt;')"/>
    <xsl:value-of select="concat($indent, '/// Gets the Database Adapter')"/>
    <xsl:value-of select="concat($indent, '/// &lt;/summary&gt;')"/>
    <xsl:value-of select="concat($indent, 'public global::Cobos.Data.IDatabaseAdapter Database')"/>
    <xsl:value-of select="concat($indent, '{')"/>
    <xsl:value-of select="concat($indent, '    get;')"/>
    <xsl:value-of select="concat($indent, '    private set;')"/>
    <xsl:value-of select="concat($indent, '}')"/>
    <xsl:value-of select="$indent"/>
    <xsl:value-of select="concat($indent, '/// &lt;summary&gt;')"/>
    <xsl:value-of select="concat($indent, '/// Gets the Data Model')"/>
    <xsl:value-of select="concat($indent, '/// &lt;/summary&gt;')"/>
    <xsl:value-of select="concat($indent, 'public ', @name, ' DataModel')"/>
    <xsl:value-of select="concat($indent, '{')"/>
    <xsl:value-of select="concat($indent, '    get;')"/>
    <xsl:value-of select="concat($indent, '    private set;')"/>
    <xsl:value-of select="concat($indent, '}')"/>
  </xsl:template>
  <!-- 
  =============================================================================
  -->
  <xsl:template match="cobos:Object" mode="propertyDefinition">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndentLevel1"/>
    </xsl:variable>
    <xsl:value-of select="$indent"/>
    <xsl:apply-templates select="." mode="commentProperty"/>
    <xsl:value-of select="concat($indent, 'public ', @className, 'DataAdapter ', @className)"/>
    <xsl:value-of select="concat($indent, '{')"/>
    <xsl:value-of select="concat($indent, '    get;')"/>
    <xsl:value-of select="concat($indent, '    private set;')"/>
    <xsl:value-of select="concat($indent, '}')"/>
  </xsl:template>
  <!-- 
  =============================================================================
  Accept Changes method.
  =============================================================================
  -->
  <xsl:template match="cobos:DataModel" mode="acceptChanges">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndentLevel2"/>
    </xsl:variable>
    <xsl:variable name="objectDependencies">
      <xsl:apply-templates select="." mode="objectDependencies"/>
    </xsl:variable>
    <xsl:value-of select="$indent"/>
    <xsl:value-of select="concat($indent, '/// &lt;summary&gt;')"/>
    <xsl:value-of select="concat($indent, '/// Accept all changes to the model.')"/>
    <xsl:value-of select="concat($indent, '/// &lt;/summary&gt;')"/>
    <xsl:value-of select="concat($indent, '/// &lt;remarks&gt;')"/>
    <xsl:value-of select="concat($indent, '/// Commit is in order of referential constraints.')"/>
    <xsl:value-of select="concat($indent, '/// &lt;/remarks&gt;')"/>
    <xsl:value-of select="concat($indent, 'public void AcceptChanges()')"/>
    <xsl:value-of select="concat($indent, '{')"/>
    <xsl:apply-templates select="msxsl:node-set($objectDependencies)/cobos:Object" mode="acceptChanges"/>
    <xsl:value-of select="concat($indent, '}')"/>
  </xsl:template>
  <!-- 
  =============================================================================
  -->
  <xsl:template match="cobos:Object" mode="acceptChanges">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndentLevel3"/>
    </xsl:variable>
    <xsl:if test="not(position() = 1)">
      <xsl:value-of select="$indent"/>
    </xsl:if>
    <xsl:value-of select="concat($indent, 'if (this.', @className, '.HasChanges())')"/>
    <xsl:value-of select="concat($indent, '{')"/>
    <xsl:value-of select="concat($indent, '    this.', @className, '.AcceptChanges();')"/>
    <xsl:value-of select="concat($indent, '}')"/>
  </xsl:template>
  <!-- 
  =============================================================================
  Reject Changes method.
  =============================================================================
  -->
  <xsl:template match="cobos:DataModel" mode="rejectChanges">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndentLevel2"/>
    </xsl:variable>
    <xsl:variable name="objectDependencies">
      <xsl:apply-templates select="." mode="objectDependencies"/>
    </xsl:variable>
    <xsl:value-of select="$indent"/>
    <xsl:value-of select="concat($indent, '/// &lt;summary&gt;')"/>
    <xsl:value-of select="concat($indent, '/// Reject all changes to the model.')"/>
    <xsl:value-of select="concat($indent, '/// &lt;/summary&gt;')"/>
    <xsl:value-of select="concat($indent, '/// &lt;remarks&gt;')"/>
    <xsl:value-of select="concat($indent, '/// Rollback is in reverse order of referential constraints.')"/>
    <xsl:value-of select="concat($indent, '/// &lt;/remarks&gt;')"/>
    <xsl:value-of select="concat($indent, 'public void RejectChanges()')"/>
    <xsl:value-of select="concat($indent, '{')"/>
    <xsl:apply-templates select="msxsl:node-set($objectDependencies)/cobos:Object" mode="rejectChanges">
      <xsl:sort select="position()" data-type="number" order="descending"/>
    </xsl:apply-templates> 
    <xsl:value-of select="concat($indent, '}')"/>
  </xsl:template>
  <!-- 
  =============================================================================
  -->
  <xsl:template match="cobos:Object" mode="rejectChanges">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndentLevel3"/>
    </xsl:variable>
    <xsl:if test="not(position() = 1)">
      <xsl:value-of select="$indent"/>
    </xsl:if>
    <xsl:value-of select="concat($indent, 'if (this.', @className, '.HasChanges())')"/>
    <xsl:value-of select="concat($indent, '{')"/>
    <xsl:value-of select="concat($indent, '    this.', @className, '.RejectChanges();')"/>
    <xsl:value-of select="concat($indent, '}')"/>
  </xsl:template>
  <!-- 
  =============================================================================
  Fill method.
  =============================================================================
  -->
  <xsl:template match="cobos:DataModel" mode="fill">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndentLevel2"/>
    </xsl:variable>
    <xsl:value-of select="$indent"/>
    <xsl:value-of select="concat($indent, '/// &lt;summary&gt;')"/>
    <xsl:value-of select="concat($indent, '/// Fill the data model with all data from the database.')"/>
    <xsl:value-of select="concat($indent, '/// &lt;/summary&gt;')"/>
    <xsl:value-of select="concat($indent, 'public void Fill()')"/>
    <xsl:value-of select="concat($indent, '{')"/>
    <xsl:value-of select="concat($indent, '    this.DataModel.EnforceConstraints = false;')"/>
    <xsl:value-of select="$indent"/>
    <xsl:apply-templates select="." mode="fillObject"/>
    <xsl:value-of select="$indent"/>
    <xsl:value-of select="concat($indent, '    this.DataModel.EnforceConstraints = true;')"/>
    <xsl:value-of select="concat($indent, '}')"/>
  </xsl:template>
  <!-- 
  =============================================================================
  -->
  <xsl:template match="cobos:DataModel[count(cobos:Object) = 1]" mode="fillObject">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndentLevel2"/>
    </xsl:variable>
    <xsl:value-of select="concat($indent, '    this.', cobos:Object[1]/@className, '.Fill(null, null);')"/>
  </xsl:template>
  <!-- 
  =============================================================================
  -->
  <xsl:template match="cobos:DataModel[count(cobos:Object) &gt; 1]" mode="fillObject">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndentLevel2"/>
    </xsl:variable>
    <xsl:variable name="objectDependencies">
      <xsl:apply-templates select="." mode="objectDependencies"/>
    </xsl:variable>
    <xsl:variable name="objects" select="msxsl:node-set($objectDependencies)"/>
    <xsl:variable name="count" select="count($objects/cobos:Object)"/>
    <xsl:value-of select="concat($indent, '    var action = new global::System.Action&lt;Cobos.Data.Filter.Filter, Cobos.Data.Filter.SortBy&gt;[', $count,']')"/>
    <xsl:value-of select="concat($indent, '    {')"/>
    <xsl:apply-templates select="$objects" mode="objectDelegates"/>
    <xsl:value-of select="concat($indent, '    };')"/>
    <xsl:value-of select="$indent"/>
    <xsl:value-of select="concat($indent, '    var result = new global::System.IAsyncResult[', $count,'];')"/>
    <xsl:value-of select="$indent"/>
    <xsl:apply-templates select="$objects/cobos:Object" mode="beginInvoke"/>
    <xsl:value-of select="$indent"/>
    <xsl:apply-templates select="$objects/cobos:Object" mode="endInvoke"/>
  </xsl:template>
  <!-- 
  =============================================================================
  Get method.
  =============================================================================
  -->
  <xsl:template match="cobos:Object" mode="get">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndentLevel1"/>
    </xsl:variable>
    <xsl:value-of select="$indent"/>
    <xsl:value-of select="concat($indent, '/// &lt;summary&gt;')"/>
    <xsl:value-of select="concat($indent, '/// Get the objects from the database.')"/>
    <xsl:value-of select="concat($indent, '/// &lt;/summary&gt;')"/>
    <xsl:value-of select="concat($indent, '/// &lt;param name=&quot;filter&quot;&gt;The filter specification. May be null&lt;/param&gt;')"/>
    <xsl:value-of select="concat($indent, '/// &lt;param name=&quot;sort&quot;&gt;The sort specification. May be null&lt;/param&gt;')"/>
    <xsl:value-of select="concat($indent, '/// &lt;returns&gt;A collection of objects from the database.&lt;/returns&gt;')"/>
    <xsl:value-of select="concat($indent, 'public global::System.Collections.Generic.IEnumerable&lt;',@className ,'&gt; Get', @className,'(Cobos.Data.Filter.Filter filter = null, Cobos.Data.Filter.SortBy sort = null)')"/>
    <xsl:value-of select="concat($indent, '{')"/>
    <xsl:value-of select="concat($indent, '    this.DataModel.EnforceConstraints = false;')"/>
    <xsl:value-of select="$indent"/>
    <xsl:apply-templates select="." mode="getObject"/>
    <xsl:value-of select="$indent"/>
    <xsl:value-of select="concat($indent, '    this.DataModel.EnforceConstraints = true;')"/>
    <xsl:value-of select="$indent"/>
    <xsl:value-of select="concat($indent, '    return this.', @className,'.GetEntities();')"/>
    <xsl:value-of select="concat($indent, '}')"/>
  </xsl:template>
  <!-- 
  =============================================================================
  -->
  <xsl:template match="cobos:Object[count(cobos:Reference) = 0]" mode="getObject">
    <xsl:apply-templates select="." mode="getObjectFill"/>
  </xsl:template>
  <!-- 
  =============================================================================
  -->
  <xsl:template match="cobos:Object[count(cobos:Reference) &gt; 0]" mode="getObject">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndentLevel1"/>
    </xsl:variable>
    <xsl:apply-templates select="." mode="getObjectFill"/>
  </xsl:template>
  <!-- 
  =============================================================================
  -->
  <xsl:template match="cobos:Object" mode="getObjectFill">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndentLevel1"/>
    </xsl:variable>
    <xsl:value-of select="concat($indent, '    this.Fill', @className,'(filter, sort);')"/>
  </xsl:template>
  <!-- 
  =============================================================================
  Fill Table method.
  =============================================================================
  -->
  <xsl:template match="cobos:Object" mode="fillTable">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndentLevel1"/>
    </xsl:variable>
    <xsl:value-of select="$indent"/>
    <xsl:value-of select="concat($indent, '/// &lt;summary&gt;')"/>
    <xsl:value-of select="concat($indent, '/// Fill the table from the database.  Also fills any child tables.')"/>
    <xsl:value-of select="concat($indent, '/// &lt;/summary&gt;')"/>
    <xsl:value-of select="concat($indent, '/// &lt;param name=&quot;filter&quot;&gt;The optional filter specification.&lt;/param&gt;')"/>
    <xsl:value-of select="concat($indent, '/// &lt;param name=&quot;sort&quot;&gt;The optional sort specification.&lt;/param&gt;')"/>
    <xsl:value-of select="concat($indent, 'private void Fill', @className,'(Cobos.Data.Filter.Filter filter = null, Cobos.Data.Filter.SortBy sort = null)')"/>
    <xsl:value-of select="concat($indent, '{')"/>
    <xsl:apply-templates select="." mode="fillTableObject"/>
    <xsl:value-of select="concat($indent, '}')"/>
  </xsl:template>
  <!-- 
  =============================================================================
  -->
  <xsl:template match="cobos:Object[count(cobos:Reference) = 0]" mode="fillTableObject">
    <xsl:apply-templates select="." mode="fillTableParent"/>
  </xsl:template>
  <!-- 
  =============================================================================
  -->
  <xsl:template match="cobos:Object[count(cobos:Reference) &gt; 0]" mode="fillTableObject">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndentLevel1"/>
    </xsl:variable>
    <xsl:apply-templates select="." mode="fillTableParent"/>
    <!--<xsl:apply-templates select="cobos:Reference" mode="fillTableChild"/>-->
    <xsl:value-of select="$indent"/>
    <xsl:variable name="count" select="count(cobos:Reference)"/>
    <xsl:value-of select="concat($indent, '    var action = new global::System.Action&lt;Cobos.Data.Filter.Filter, Cobos.Data.Filter.SortBy&gt;[', $count,']')"/>
    <xsl:value-of select="concat($indent, '    {')"/>
    <xsl:apply-templates select="cobos:Reference" mode="objectDelegates"/>
    <xsl:value-of select="concat($indent, '    };')"/>
    <xsl:value-of select="$indent"/>
    <xsl:value-of select="concat($indent, '    var result = new global::System.IAsyncResult[', $count,'];')"/>
    <xsl:value-of select="$indent"/>
    <xsl:apply-templates select="cobos:Reference" mode="beginInvoke"/>
    <xsl:value-of select="$indent"/>
    <xsl:apply-templates select="cobos:Reference" mode="endInvoke"/>
  </xsl:template>
  <!-- 
  =============================================================================
  -->
  <xsl:template match="cobos:Object" mode="fillTableParent">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndentLevel1"/>
    </xsl:variable>
    <xsl:value-of select="concat($indent, '    this.', @className,'.Fill(filter, sort);')"/>
  </xsl:template>
  <!-- 
  =============================================================================
  -->
  <xsl:template match="cobos:Reference" mode="fillTableChild">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndentLevel1"/>
    </xsl:variable>
    <xsl:value-of select="$indent"/>
    <xsl:apply-templates select="." mode="getChildFilter"/>
    <xsl:value-of select="$indent"/>
    <xsl:value-of select="concat($indent, 'if (filter != null)')"/>
    <xsl:value-of select="concat($indent, '{')"/>
    <xsl:value-of select="concat($indent, '    this.Fill', @ref, '(filter, null);')"/>
    <xsl:value-of select="concat($indent, '}')"/>
  </xsl:template>
  <!-- 
  =============================================================================
  Asynchronous processing.
  =============================================================================
  -->
  <xsl:template match="cobos:Object" mode="objectDelegates">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndentLevel3"/>
    </xsl:variable>
    <xsl:value-of select="concat($indent, '    this.', @className,'.Fill')"/>
    <xsl:if test="not(position() = last())">
      <xsl:text>,</xsl:text>
    </xsl:if>
  </xsl:template>
  <!-- 
  =============================================================================
  -->
  <xsl:template match="cobos:Reference" mode="objectDelegates">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndentLevel1"/>
    </xsl:variable>
    <xsl:value-of select="concat($indent, '    this.Fill', @ref)"/>
    <xsl:if test="not(position() = last())">
      <xsl:text>,</xsl:text>
    </xsl:if>
  </xsl:template>
  <!-- 
  =============================================================================
  -->
  <xsl:template match="cobos:Object" mode="beginInvoke">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndentLevel3"/>
    </xsl:variable>
    <xsl:value-of select="concat($indent, 'result[', position() - 1, '] = action[', position() - 1, '].BeginInvoke(null, null, null, null);')"/>
  </xsl:template>
  <!-- 
  =============================================================================
  -->
  <xsl:template match="cobos:Reference" mode="beginInvoke">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndentLevel1"/>
    </xsl:variable>
    <xsl:apply-templates select="." mode="getChildFilter"/>
    <xsl:value-of select="$indent"/>
    <xsl:value-of select="concat($indent, 'if (filter != null)')"/>
    <xsl:value-of select="concat($indent, '{')"/>
    <xsl:value-of select="concat($indent, '    result[', position() - 1, '] = action[', position() - 1, '].BeginInvoke(filter, null, null, null);')"/>
    <xsl:value-of select="concat($indent, '}')"/>
    <xsl:if test="not(position() = last())">
      <xsl:value-of select="$indent"/>
    </xsl:if>
  </xsl:template>
  <!-- 
  =============================================================================
  -->
  <xsl:template match="cobos:Object" mode="endInvoke">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndentLevel3"/>
    </xsl:variable>
    <xsl:value-of select="concat($indent, 'action[', position() - 1, '].EndInvoke(result[', position() - 1, ']);')"/>
  </xsl:template>
  <!-- 
  =============================================================================
  -->
  <xsl:template match="cobos:Reference" mode="endInvoke">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndentLevel1"/>
    </xsl:variable>
    <xsl:value-of select="concat($indent, 'if (result[', position() - 1, '] != null)')"/>
    <xsl:value-of select="concat($indent, '{')"/>
    <xsl:value-of select="concat($indent, '    action[', position() - 1, '].EndInvoke(result[', position() - 1, ']);')"/>
    <xsl:value-of select="concat($indent, '}')"/>
    <xsl:if test="not(position() = last())">
      <xsl:value-of select="$indent"/>
    </xsl:if>
  </xsl:template>
  <!-- 
  =============================================================================
  -->
  <xsl:template match="cobos:Reference" mode="getChildFilter">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndentLevel1"/>
    </xsl:variable>
    <xsl:variable name="type">
      <xsl:apply-templates select="..//cobos:Property[@name = current()/@key]" mode="propertyType"/>
    </xsl:variable>
    <xsl:value-of select="concat($indent, 'filter = this.', ../@name, '.GetChildFilter&lt;', $type, '&gt;(&quot;', @key, '&quot;, &quot;', @refer, '&quot;);')"/>
  </xsl:template>
  <!-- 
  =============================================================================
  Find references and order by dependency.
  =============================================================================
  -->
  <xsl:template match="cobos:DataModel" mode="objectDependencies">
    <xsl:variable name="objectDependencies">
      <xsl:apply-templates select="cobos:Object" mode="objectDependencies"/>
    </xsl:variable>
    <xsl:copy-of select="msxsl:node-set($objectDependencies)/cobos:Object[not(@className = preceding::cobos:Object/@className)]"/>
  </xsl:template>
  <!-- 
  =============================================================================
  -->
  <xsl:template match="cobos:Object" mode="objectDependencies">
    <xsl:apply-templates select="/cobos:DataModel/cobos:Object[cobos:Reference/@ref = current()/@name]" mode="objectDependencies"/>
    <xsl:apply-templates select="." mode="copyObjectDependency"/>
  </xsl:template>
  <!-- 
  =============================================================================
  -->
  <xsl:template match="cobos:Object" mode="copyObjectDependency">
    <xsl:copy-of select="."/>
  </xsl:template>
  <!-- 
  =============================================================================
  Comments.
  =============================================================================
  -->
  <xsl:template match="cobos:DataModel" mode="commentDataAdapter">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndentLevel1"/>
    </xsl:variable>
    <xsl:value-of select="$indent"/>
    <xsl:text>/// &lt;summary&gt;</xsl:text>
    <xsl:value-of select="concat($indent, '/// Class definition for the &lt;c&gt;', @name, '&lt;/c&gt; Adapter.', $indent)"/>
    <xsl:text>/// &lt;/summary&gt;</xsl:text>
  </xsl:template>
  <!-- 
  =============================================================================
  -->
  <xsl:template match="cobos:DataModel" mode="commentConstructor">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndentLevel2"/>
    </xsl:variable>
    <xsl:value-of select="$indent"/>
    <xsl:text>/// &lt;summary&gt;</xsl:text>
    <xsl:value-of select="concat($indent, '/// Initializes a new instance of the &lt;see cref=&quot;', @name, 'Adapter&quot;/&gt; class.', $indent)"/>
    <xsl:text>/// &lt;/summary&gt;</xsl:text>
    <xsl:value-of select="$indent"/>
    <xsl:text>/// &lt;param name="database"&gt;The database adapter.&lt;/param&gt;</xsl:text>
  </xsl:template>
  <!-- 
  =============================================================================
  Property comment.
  =============================================================================
  -->
  <xsl:template match="cobos:Object" mode="commentProperty">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndentLevel1"/>
    </xsl:variable>
    <xsl:value-of select="$indent"/>
    <xsl:text>/// &lt;summary&gt;</xsl:text>
    <xsl:value-of select="concat($indent, '/// Gets the &lt;c&gt;',  @name, '&lt;/c&gt; Data Adapter.', $indent)"/>
    <xsl:text>/// &lt;/summary&gt;</xsl:text>
  </xsl:template>
</xsl:stylesheet>