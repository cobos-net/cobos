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
    <xsl:value-of select="concat($newline, '    using System.Linq;')"/>
    <xsl:apply-templates select="cobos:Object" mode="classDefinition"/>
    <xsl:value-of select="concat($newline, '}')"/>
  </xsl:template>
  <!-- 
  =============================================================================
  Class definition.
  =============================================================================
  -->
  <xsl:template match="cobos:Object" mode="classDefinition">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndent"/>
    </xsl:variable>
    <xsl:value-of select="$indent"/>
    <xsl:apply-templates select="." mode="commentClass"/>
    <xsl:value-of select="concat($indent, '[global::System.Runtime.Serialization.DataContract(Namespace = &quot;', $xmlNamespace, '&quot;)]')"/>
    <xsl:value-of select="concat($indent, '[global::Cobos.Data.Mapping.Table(Name = &quot;', @dbTable, '&quot;)]')"/>
    <xsl:value-of select="concat($indent, 'public partial class ', @className, ' : global::System.ComponentModel.INotifyPropertyChanged')"/>
    <xsl:value-of select="concat($indent, '{')"/>
    <xsl:apply-templates select="." mode="dataRowSource"/>
    <xsl:apply-templates select="cobos:Object" mode="members"/>
    <xsl:apply-templates select="." mode="constructor"/>
    <xsl:apply-templates select="." mode="events"/>
    <xsl:apply-templates select="cobos:Object|cobos:Reference|cobos:Property[not(@hidden)]" mode="propertyDefinition"/>
    <xsl:apply-templates select="/cobos:DataModel//cobos:Reference[@ref = current()/@name]" mode="parentReferencePropertyDefinition"/>
    <xsl:apply-templates select="cobos:Property[@hidden]" mode="propertyDefinition"/>
    <xsl:apply-templates select="." mode="notifyChanged"/>
    <xsl:apply-templates select="cobos:Object" mode="classDefinition"/>
    <xsl:value-of select="concat($indent, '}')"/>
  </xsl:template>
  <!-- 
  =============================================================================
  Constructor definition.
  =============================================================================
  -->
  <xsl:template match="cobos:Object" mode="constructor">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndentLevel1"/>
    </xsl:variable>
    <xsl:variable name="datasetRowType">
      <xsl:apply-templates select="." mode="datasetRowType"/>
    </xsl:variable>
    <xsl:apply-templates select="." mode="commentConstructor"/>
    <xsl:value-of select="concat($indent, 'public ', @className, '(', $datasetRowType, ' dataRow)')"/>
    <xsl:value-of select="concat($indent, '{')"/>
    <xsl:value-of select="concat($indent, '    this.DataRowSource = dataRow;')"/>
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
      <xsl:apply-templates select="." mode="newlineIndentLevel1"/>
    </xsl:variable>
    <xsl:value-of select="concat($indent, 'this.', @fieldName, ' = new ', @className, '(this.DataRowSource);')"/>
  </xsl:template>
  <!-- 
  =============================================================================
  Class member definitions.
  =============================================================================
  -->
  <xsl:template match="cobos:Object" mode="dataRowSource">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndentLevel1"/>
    </xsl:variable>
    <xsl:variable name="datasetRowType">
      <xsl:apply-templates select="." mode="datasetRowType"/>
    </xsl:variable>
    <xsl:apply-templates select="." mode="commentDataRowSource"/>
    <xsl:value-of select="concat($indent, 'public readonly ', $datasetRowType, ' DataRowSource;', $indent)" />
  </xsl:template>
  <!-- 
  =============================================================================
  =============================================================================
  -->
  <xsl:template match="cobos:Object" mode="members">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndent"/>
    </xsl:variable>
    <xsl:apply-templates select="." mode="commentMember"/>
    <xsl:value-of select="concat($indent, 'private ', @className, ' ', @fieldName, ';', $indent)" />
  </xsl:template>
  <!--
  =============================================================================
  Events.
  =============================================================================
  -->
  <xsl:template match="cobos:Object" mode="events">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndentLevel1"/>
    </xsl:variable>
    <xsl:value-of select="$indent"/>
    <xsl:apply-templates select="." mode="commentEvent"/>
    <xsl:value-of select="concat($indent, 'public event global::System.ComponentModel.PropertyChangedEventHandler PropertyChanged;')" />
  </xsl:template>
  <!-- 
  =============================================================================
  Property definitions.
  =============================================================================
  -->
  <xsl:template match="cobos:Object|cobos:Reference|cobos:Property" mode="propertyDefinition">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndent"/>
    </xsl:variable>
    <xsl:variable name="type">
      <xsl:apply-templates select="." mode="propertyType"/>
    </xsl:variable>
    <xsl:value-of select="$indent"/>
    <xsl:apply-templates select="." mode="commentProperty"/>
    <xsl:apply-templates select="." mode="propertySerializationAttribute">
      <xsl:with-param name="indent" select="$indent"/>
      <xsl:with-param name="order" select="position() - 1"/>
    </xsl:apply-templates>
    <xsl:apply-templates select="." mode="mappingProperty">
      <xsl:with-param name="indent" select="$indent"/>
    </xsl:apply-templates>
    <xsl:value-of select="concat($indent, 'public ', $type, ' ', @name)"/>
    <xsl:value-of select="concat($indent, '{')"/>
    <xsl:apply-templates select="." mode="propertyGet"/>
    <xsl:value-of select="$indent"/>
    <xsl:apply-templates select="." mode="propertySet"/>
    <xsl:value-of select="concat($indent, '}')"/>
  </xsl:template>
  <!-- 
  =============================================================================
  -->
  <xsl:template match="cobos:Object|cobos:Reference|cobos:Property" mode="propertySerializationAttribute">
    <xsl:param name="indent"/>
    <xsl:param name="order"/>
    <xsl:value-of select="concat($indent, '[global::System.Runtime.Serialization.DataMember(Order = ', $order, ')]')"/>
  </xsl:template>
  <!-- 
  =============================================================================
  -->
  <xsl:template match="cobos:Property[@hidden]" mode="propertySerializationAttribute">
    <xsl:param name="indent"/>
    <xsl:value-of select="concat($indent, '[global::System.Runtime.Serialization.IgnoreDataMember()]')"/>
  </xsl:template>
  <!-- 
  =============================================================================
  Property mapping attributes.
  =============================================================================
  -->
  <xsl:template match="cobos:Property" mode="mappingProperty">
    <xsl:param name="indent"/>
    <xsl:value-of select="concat($indent, '[global::Cobos.Data.Mapping.Table(Name = &quot;', @dbTable, '&quot;)]')"/>
    <xsl:value-of select="concat($indent, '[global::Cobos.Data.Mapping.Column(Name = &quot;', @dbColumn, '&quot;)]')"/>
  </xsl:template>
  <!-- 
  =============================================================================
  -->
  <xsl:template match="*" mode="mappingProperty"/>
  <!-- 
  =============================================================================
  Property definition for a parent record (foreign key parent)
  =============================================================================
  -->
  <xsl:template match="cobos:Reference" mode="parentReferencePropertyDefinition">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndent"/>
    </xsl:variable>
    <xsl:value-of select="$indent"/>
    <xsl:apply-templates select="." mode="commentProperty"/>
    <xsl:value-of select="concat($indent, '[global::System.Runtime.Serialization.IgnoreDataMember]')"/>
    <xsl:value-of select="concat($indent, 'public ', ../@name, ' ', ../@name)"/>
    <xsl:value-of select="concat($indent, '{')"/>
    <xsl:apply-templates select="." mode="parentReferencePropertyGet"/>
    <xsl:value-of select="$indent"/>
    <xsl:apply-templates select="." mode="propertySet"/>
    <xsl:value-of select="concat($indent, '}')"/>
  </xsl:template>
  <!-- 
  =============================================================================
  Property Get definition.
  =============================================================================
  -->
  <xsl:template match="*" mode="propertyGet">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndentLevel1"/>
    </xsl:variable>
    <xsl:value-of select="concat($indent, 'get')"/>
    <xsl:value-of select="concat($indent, '{')"/>
    <xsl:apply-templates select="." mode="propertyGetBody"/>
    <xsl:value-of select="concat($indent, '}')"/>
  </xsl:template>
  <!-- 
  =============================================================================
  =============================================================================
  -->
  <xsl:template match="cobos:Reference" mode="parentReferencePropertyGet">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndentLevel1"/>
    </xsl:variable>
    <xsl:value-of select="concat($indent, 'get')"/>
    <xsl:value-of select="concat($indent, '{')"/>
    <xsl:apply-templates select="." mode="parentReferencePropertyGetBody"/>
    <xsl:value-of select="concat($indent, '}')"/>
  </xsl:template>
  <!-- 
  =============================================================================
  Property Set definition.
  =============================================================================
  -->
  <xsl:template match="*" mode="propertySet">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndentLevel1"/>
    </xsl:variable>
    <xsl:value-of select="concat($indent, 'set')"/>
    <xsl:value-of select="concat($indent, '{')"/>
    <xsl:apply-templates select="." mode="propertySetBody"/>
    <xsl:value-of select="concat($indent, '}')"/>
  </xsl:template>
  <!-- 
  =============================================================================
  Property Get body definition.
  =============================================================================
  -->
  <xsl:template match="cobos:Property[@minOccurs = 1]" mode="propertyGetBody">
    <xsl:apply-templates select="." mode="propertyGetValue"/>
  </xsl:template>
  <!-- 
  =============================================================================
  =============================================================================
  -->
  <xsl:template match="cobos:Property[@minOccurs = 0]" mode="propertyGetBody">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndentLevel2"/>
    </xsl:variable>
    <xsl:variable name="columnName">
      <xsl:apply-templates select="." mode="fullName"/>
    </xsl:variable>
    <xsl:value-of select="concat($indent, 'if (this.DataRowSource.Is', $columnName, 'Null())')"/>
    <xsl:value-of select="concat($indent, '{')"/>
    <xsl:value-of select="concat($indent, '    return null;')"/>
    <xsl:value-of select="concat($indent, '}')"/>
    <xsl:value-of select="$indent"/>
    <xsl:apply-templates select="." mode="propertyGetValue"/>
  </xsl:template>
  <!-- 
  =============================================================================
  =============================================================================
  -->
  <xsl:template match="cobos:Object" mode="propertyGetBody">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndentLevel2"/>
    </xsl:variable>
    <xsl:value-of select="concat($indent, 'return this.', @fieldName, ';')"/>
  </xsl:template>
  <!-- 
  =============================================================================
  =============================================================================
  -->
  <xsl:template match="cobos:Reference" mode="propertyGetBody">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndentLevel2"/>
    </xsl:variable>
    <xsl:value-of select="concat($indent, 'return this.DataRowSource.Get', @name, '().Select(o => new ', @ref, '(o));')"/>
  </xsl:template>
  <!-- 
  =============================================================================
  =============================================================================
  -->
  <xsl:template match="cobos:Reference" mode="parentReferencePropertyGetBody">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndentLevel2"/>
    </xsl:variable>
    <xsl:value-of select="concat($indent, 'return new ', ../@name, '(this.DataRowSource.', ../@name, ');')"/>
  </xsl:template>
  <!-- 
  =============================================================================
  Property Set body definition.
  =============================================================================
  -->
  <xsl:template match="cobos:Object|cobos:Reference" mode="propertySetBody" />
  <!-- 
  =============================================================================
  =============================================================================
  -->
  <xsl:template match="cobos:Property[@minOccurs = 1]" mode="propertySetBody">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndentLevel2"/>
    </xsl:variable>
    <xsl:value-of select="$indent"/>
    <xsl:apply-templates select="." mode="propertySetValue"/>
    <xsl:value-of select="concat($indent, 'this.RaisePropertyChanged(&quot;', @name, '&quot;);')"/>
  </xsl:template>
  <!-- 
  =============================================================================
  =============================================================================
  -->
  <xsl:template match="cobos:Property[@minOccurs = 0]" mode="propertySetBody">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndentLevel2"/>
    </xsl:variable>
    <xsl:variable name="columnName">
      <xsl:apply-templates select="." mode="fullName"/>
    </xsl:variable>
    <xsl:value-of select="concat($indent, 'if (value == null)')"/>
    <xsl:value-of select="concat($indent, '{')"/>
    <xsl:value-of select="concat($indent, '    this.DataRowSource.Set', $columnName, 'Null();')"/>
    <xsl:value-of select="concat($indent, '}')"/>
    <xsl:value-of select="concat($indent, 'else')"/>
    <xsl:value-of select="concat($indent, '{')"/>
    <xsl:value-of select="concat($indent, '    ')"/>
    <xsl:apply-templates select="." mode="propertySetValue"/>
    <xsl:value-of select="concat($indent, '}')"/>
    <xsl:value-of select="$indent"/>
    <xsl:value-of select="concat($indent, 'this.RaisePropertyChanged(&quot;', @name, '&quot;);')"/>
  </xsl:template>
  <!-- 
  =============================================================================
  Property Get value.
  =============================================================================
  -->
  <xsl:template match="cobos:Property[not(@stringFormat)]" mode="propertyGetValue">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndentLevel2"/>
    </xsl:variable>
    <xsl:variable name="columnName">
      <xsl:apply-templates select="." mode="fullName"/>
    </xsl:variable>
    <xsl:value-of select="concat($indent, 'return this.DataRowSource.', $columnName, ';')"/>
  </xsl:template>
  <!-- 
  =============================================================================
  =============================================================================
  -->
  <xsl:template match="cobos:Property[@stringFormat]" mode="propertyGetValue">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndentLevel2"/>
    </xsl:variable>
    <xsl:variable name="columnName">
      <xsl:apply-templates select="." mode="fullName"/>
    </xsl:variable>
    <xsl:variable name="columnValue">
      <xsl:value-of select="concat('this.DataRowSource.', $columnName)"/>
    </xsl:variable>
    <xsl:variable name="codeTemplate">
      <xsl:value-of select="normalize-space(./cobos:StringFormat/cobos:PropertyGet)" disable-output-escaping="yes"/>
    </xsl:variable>
    <xsl:variable name="code">
      <xsl:call-template name="string-replace-all">
        <xsl:with-param name="text" select="$codeTemplate" />
        <xsl:with-param name="replace" select="string('$columnValue')" />
        <xsl:with-param name="by" select="$columnValue" />
      </xsl:call-template>
    </xsl:variable>
    <xsl:value-of select="concat($indent, $code)"/>
  </xsl:template>
  <!-- 
  =============================================================================
  Property Set value.
  =============================================================================
  -->
  <xsl:template match="cobos:Property[not(@stringFormat)]" mode="propertySetValue">
    <xsl:variable name="columnName">
      <xsl:apply-templates mode="fullName" select="."/>
    </xsl:variable>
    <xsl:variable name="value">
      <xsl:apply-templates mode="propertySetValueValue" select="."/>
    </xsl:variable>
    <xsl:value-of select="concat('this.DataRowSource.', $columnName, ' = ', $value, ';')"/>
  </xsl:template>
  <!-- 
  =============================================================================
  =============================================================================
  -->
  <xsl:template match="cobos:Property[@stringFormat]" mode="propertySetValue">
    <xsl:variable name="columnName">
      <xsl:apply-templates mode="fullName" select="."/>
    </xsl:variable>
    <xsl:variable name="columnValue">
      <xsl:value-of select="concat('this.DataRowSource.', $columnName)"/>
    </xsl:variable>
    <xsl:variable name="dataValue">
      <xsl:apply-templates mode="propertySetValueValue" select="."/>
    </xsl:variable>
    <xsl:variable name="codeTemplate">
      <xsl:value-of select="normalize-space(./cobos:StringFormat/cobos:PropertySet)" disable-output-escaping="yes"/>
    </xsl:variable>
    <xsl:variable name="code1">
      <xsl:call-template name="string-replace-all">
        <xsl:with-param name="text" select="$codeTemplate" />
        <xsl:with-param name="replace" select="string('$columnValue')" />
        <xsl:with-param name="by" select="$columnValue" />
      </xsl:call-template>
    </xsl:variable>
    <xsl:variable name="code2">
      <xsl:call-template name="string-replace-all">
        <xsl:with-param name="text" select="$code1" />
        <xsl:with-param name="replace" select="string('$dataValue')" />
        <xsl:with-param name="by" select="$dataValue" />
      </xsl:call-template>
    </xsl:variable>
    <xsl:value-of select="$code2"/>
  </xsl:template>
  <!-- 
  =============================================================================
  =============================================================================
  -->
  <!-- simple case for any non-nullable type -->
  <xsl:template match="cobos:Property[@minOccurs = 1]" mode="propertySetValueValue">
    <xsl:text>value</xsl:text>
  </xsl:template>
  <!-- 
  =============================================================================
  =============================================================================
  -->
  <!-- Strings can be set to null -->
  <xsl:template match="cobos:Property[@minOccurs = 0]" mode="propertySetValueValue">
    <xsl:text>value</xsl:text>
  </xsl:template>
  <!-- 
  =============================================================================
  =============================================================================
  -->
  <xsl:template match="cobos:Property[@minOccurs = 0 and @stringFormat]" mode="propertySetValueValue">
    <xsl:variable name="dataType" select="normalize-space(./cobos:StringFormat/cobos:CodeType)"/>
    <xsl:choose>
      <xsl:when test="$dataType = 'string'">
        <xsl:text>value</xsl:text>
      </xsl:when>
      <xsl:otherwise>
        <xsl:text>value.Value</xsl:text>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
  <!-- 
  =============================================================================
  =============================================================================
  -->
  <!-- Any other string length that is not a special formatted value -->
  <xsl:template match="cobos:Property[@minOccurs = 0 and (contains(@dbType, 'string') and not(@stringFormat))]" mode="propertySetValueValue">
    <xsl:text>value</xsl:text>
  </xsl:template>
  <!-- 
  =============================================================================
  =============================================================================
  -->
  <!-- Nullable value types -->
  <xsl:template match="cobos:Property[@minOccurs = 0 and not(contains(@dbType, 'string')) and not(contains(@dbType, 'hexBinary'))]" mode="propertySetValueValue">
    <xsl:text>value.Value</xsl:text>
  </xsl:template>
  <!-- 
  =============================================================================
  =============================================================================
  -->
  <!--
  =============================================================================
  Notify Changed
  =============================================================================
	-->
  <xsl:template match="cobos:Object" mode="notifyChanged">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndentLevel1"/>
    </xsl:variable>
    <xsl:variable name="type">
      <xsl:apply-templates select="." mode="propertyType"/>
    </xsl:variable>
    <xsl:value-of select="$indent"/>
    <xsl:value-of select="concat($indent, '/// &lt;summary&gt;')"/>
    <xsl:value-of select="concat($indent, '/// Raise a property changed event for a property.')"/>
    <xsl:value-of select="concat($indent, '/// &lt;/summary&gt;')"/>
    <xsl:value-of select="concat($indent, '/// &lt;param name=&quot;propertyName&quot;&gt;The name of the property that has changed.&lt;/param&gt;')"/>
    <xsl:value-of select="concat($indent, 'protected void RaisePropertyChanged(string propertyName)')"/>
    <xsl:value-of select="concat($indent, '{')"/>
    <xsl:value-of select="concat($indent, '    global::System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;', $indent)"/>
    <xsl:value-of select="concat($indent, '    if (propertyChanged != null)')"/>
    <xsl:value-of select="concat($indent, '    {')"/>
    <xsl:value-of select="concat($indent, '        propertyChanged(this, new global::System.ComponentModel.PropertyChangedEventArgs(propertyName));')"/>
    <xsl:value-of select="concat($indent, '    }')"/>
    <xsl:value-of select="concat($indent, '}')"/>
  </xsl:template>  
  <!-- 
  =============================================================================
  Comments.
  =============================================================================
  -->
  <xsl:template match="cobos:Object" mode="commentClass">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndent"/>
    </xsl:variable>
    <xsl:value-of select="$indent"/>
    <xsl:text>/// &lt;summary&gt;</xsl:text>
    <xsl:value-of select="concat($indent, '/// Class definition for the ', @name, ' row type.', $indent)"/>
    <xsl:text>/// &lt;/summary&gt;</xsl:text>
  </xsl:template>
  <!-- 
  =============================================================================
  =============================================================================
  -->
  <xsl:template match="cobos:Object" mode="commentConstructor">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndentLevel1"/>
    </xsl:variable>
    <xsl:value-of select="$indent"/>
    <xsl:text>/// &lt;summary&gt;</xsl:text>
    <xsl:value-of select="concat($indent, '/// Initializes a new instance of the &lt;see cref=&quot;', @className, '&quot;/&gt; class.', $indent)"/>
    <xsl:text>/// &lt;/summary&gt;</xsl:text>
    <xsl:value-of select="$indent"/>
    <xsl:text>/// &lt;param name="dataRow"&gt;The data row source.&lt;/param&gt;</xsl:text>
  </xsl:template>
  <!-- 
  =============================================================================
  =============================================================================
  -->
  <xsl:template match="cobos:Object" mode="commentDataRowSource">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndentLevel1"/>
    </xsl:variable>
    <xsl:value-of select="$indent"/>
    <xsl:text>/// &lt;summary&gt;</xsl:text>
    <xsl:value-of select="concat($indent, '/// The DataRow representing this object.', $indent)"/>
    <xsl:text>/// &lt;/summary&gt;</xsl:text>
  </xsl:template>
  <!-- 
  =============================================================================
  =============================================================================
  -->
  <xsl:template match="cobos:Object" mode="commentMember">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndent"/>
    </xsl:variable>
    <xsl:value-of select="$indent"/>
    <xsl:text>/// &lt;summary&gt;</xsl:text>
    <xsl:value-of select="concat($indent, '/// Represents the &lt;c&gt;', @name, '&lt;/c&gt; property.', $indent)"/>
    <xsl:text>/// &lt;/summary&gt;</xsl:text>
  </xsl:template>
  <!-- 
  =============================================================================
  =============================================================================
  -->
  <xsl:template match="cobos:Object" mode="commentEvent">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndentLevel1"/>
    </xsl:variable>
    <xsl:value-of select="$indent"/>
    <xsl:text>/// &lt;summary&gt;</xsl:text>
    <xsl:value-of select="concat($indent, '/// Event fired when a property changes.', $indent)"/>
    <xsl:text>/// &lt;/summary&gt;</xsl:text>
  </xsl:template>
  <!-- 
  =============================================================================
  Property comment.
  =============================================================================
  -->
  <xsl:template match="cobos:Object|cobos:Reference|cobos:Property" mode="commentProperty">
    <xsl:variable name="indent">
      <xsl:apply-templates select="." mode="newlineIndent"/>
    </xsl:variable>
    <xsl:value-of select="$indent"/>
    <xsl:text>/// &lt;summary&gt;</xsl:text>
    <xsl:value-of select="concat($indent, '/// Gets or sets the value of the &lt;c&gt;',  @name, '&lt;/c&gt; field.', $indent)"/>
    <xsl:text>/// &lt;/summary&gt;</xsl:text>
  </xsl:template>
</xsl:stylesheet>