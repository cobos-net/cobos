<xsl:stylesheet version="1.0"
						xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
						xmlns:msxsl="urn:schemas-microsoft-com:xslt"
						exclude-result-prefixes="msxsl"
						xmlns="http://schemas.cobos.co.uk/datamodel/1.0.0"
						xmlns:cobos="http://schemas.cobos.co.uk/datamodel/1.0.0"
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

	<xsl:include href="custom.xslt"/>

	<!-- C# namespace -->
	<xsl:param name="codeNamespace"/>
	<!-- System.Runtime.Serialization.DataContract namespace -->
	<xsl:param name="xmlNamespace"/>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Process the data model into CSharp classes
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="/cobos:DataModel">

		<xsl:call-template name="generatedCSharpWarning"/>

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Runtime.Serialization;
using System.Text;
<xsl:call-template name="userNamespaceDeclarations"/>

namespace <xsl:value-of select="$codeNamespace"/>
{
	<xsl:apply-templates select="cobos:Object|cobos:Interface" mode="classDefinition"/>
	<xsl:apply-templates select="cobos:Object" mode="tableAdapter"/>
}
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	class	qualifiers for nested classes within abstract interfaces
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Object[ ancestor::cobos:Interface[ not( @isAbstractClass = 'true' ) ] ]" mode="classQualifiers">
		<xsl:text>interface </xsl:text>
	</xsl:template>

	<xsl:template match="cobos:Object[ ancestor::cobos:Interface[ @isAbstractClass = 'true' ] ]" mode="classQualifiers">
		<xsl:text>abstract partial class </xsl:text>
	</xsl:template>

	<xsl:template match="cobos:Object[ not( ancestor::cobos:Interface ) and not( ancestor::cobos:Object/@implements ) ]" mode="classQualifiers">
		<xsl:text>class </xsl:text>
	</xsl:template>

	<xsl:template match="cobos:Object[ not( ancestor::cobos:Interface ) and ancestor::cobos:Object/@implements ]" mode="classQualifiers">
		<!-- get the interface object the parent class implements -->
		<xsl:variable name="interface" select="/cobos:DataModel/cobos:Interface[ @name = current()/ancestor-or-self::cobos:Object[ @implements ]/@implements ]"/>
		<!-- only apply if the new keyword if this class is defined in thes base class -->
		<xsl:variable name="property" select="$interface//cobos:Object[ substring-after( @qualifiedName, '.' ) = substring-after( current()/@qualifiedName, '.' ) ]"/>
		<xsl:choose>
			<xsl:when test="$property">
				<xsl:text>new class </xsl:text>
			</xsl:when>
			<xsl:otherwise>
				<xsl:text>class </xsl:text>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>



	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Create a class declaration for each object
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->
	
	<!-- top level class -->
	<xsl:template match="cobos:Object[ parent::cobos:DataModel ]" mode="classDefinition">
		
	[DataContract(Namespace="<xsl:value-of select="$xmlNamespace"/>")]
	public partial class <xsl:value-of select="@name"/> <xsl:apply-templates select="." mode="classInheritance"/>
	{
		<xsl:apply-templates select="." mode="classBody"/>

		#region IDisposable

		~<xsl:value-of select="@name"/>()
		{
			Dispose( false );
		}
		
		public void Dispose()
		{
			Dispose( true );
		}
		
		public void Dispose( bool disposing )
		{
			if ( _disposed )
			{
				return;
			}
			
			if ( disposing )
			{
				ObjectDataRow.Delete();
			}
			
			_disposed = true;
		}
		
		bool _disposed = false;
		
		public bool IsDisposed
		{
			get { return _disposed; }
		}
		
		#endregion
	}
	
	</xsl:template>

	<!-- interface - declared as an abstract class -->
	<xsl:template match="cobos:Interface[ @isAbstractClass = 'true' ]" mode="classDefinition">
	[DataContract(Namespace="<xsl:value-of select="$xmlNamespace"/>")]
	public abstract partial class <xsl:value-of select="@name"/>
	{
		<xsl:apply-templates select="." mode="classBody"/>
	}
	</xsl:template>
	
	<!-- interface - declared as an interface -->
	<xsl:template match="cobos:Interface[ not( @isAbstractClass = 'true' ) ]" mode="classDefinition">
	[DataContract(Namespace="<xsl:value-of select="$xmlNamespace"/>")]
	public interface <xsl:value-of select="@name"/>
	{
		<xsl:apply-templates select="." mode="classBody"/>
	}
	</xsl:template>
	
	<!-- nested class -->
	<xsl:template match="cobos:Object[ parent::cobos:Object | parent::cobos:Interface ]" mode="classDefinition">
	[DataContract(Namespace="<xsl:value-of select="$xmlNamespace"/>")]
	public <xsl:apply-templates select="." mode="classQualifiers"/> <xsl:value-of select="@typeName"/> <xsl:apply-templates select="." mode="classInheritance"/>
	{
		<xsl:apply-templates select="." mode="classBody"/>
	}
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Class Inheritance
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->
	

	<!-- top level objects implements IDisposable and abstract class or interface -->
	<xsl:template match="cobos:Object[ parent::cobos:DataModel and @implements ]" mode="classInheritance">
		<xsl:text> : </xsl:text>
		<xsl:value-of select="@implements"/>
		<xsl:text>, IDisposable </xsl:text>
	</xsl:template>

	<!-- top level object that does not inherit or implement an interface -->
	<xsl:template match="cobos:Object[ parent::cobos:DataModel and not( @implements ) ]" mode="classInheritance">
		<xsl:text> : IDisposable</xsl:text>
	</xsl:template>

	<!-- nested object within a class that implements an interface or inherits an abstract class -->
	<xsl:template match="cobos:Object[ not( parent::cobos:DataModel ) and ancestor-or-self::cobos:Object[ @implements ] ]" mode="classInheritance">
		<!-- get the interface object the parent class implements -->
		<xsl:variable name="interface" select="/cobos:DataModel/cobos:Interface[ @name = current()/ancestor-or-self::cobos:Object[ @implements ]/@implements ]"/>
		<!-- only apply if this property is defined in the abstract class -->
		<xsl:apply-templates select="$interface//cobos:Object[ substring-after( @qualifiedName, '.' ) = substring-after( current()/@qualifiedName, '.' ) ]" mode="classInheritanceDeclaration"/>
	</xsl:template>

	<xsl:template match="cobos:Object" mode="classInheritanceDeclaration">
		<xsl:text> : </xsl:text>
		<xsl:value-of select="@qualifiedTypeName"/>
	</xsl:template>
	
	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Class body definition
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Interface|cobos:Object" mode="classBody">
		
		<!-- add data object declarations -->
		<xsl:apply-templates select="." mode="constructorAndMemberDefinition"/>

		<!-- add property declarations -->
		<xsl:apply-templates select="child::cobos:Property[ not( @hidden ) ]|child::cobos:Object|child::cobos:Reference" mode="propertyDefinition"/>

		<!-- add nested classes -->
		<xsl:apply-templates select="child::cobos:Object" mode="classDefinition"/>

	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Data object member variables and constructor body
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Object[ not( ancestor::cobos:Interface ) ]" mode="constructorAndMemberDefinition">
		<xsl:variable name="datasetRowType">
			<xsl:apply-templates select="." mode="datasetRowType"/>
		</xsl:variable>
		<xsl:apply-templates select="cobos:Object" mode="classMemberDecl"/>
		<xsl:apply-templates select="cobos:Reference" mode="classMemberDecl"/>
		
		public readonly <xsl:value-of select="$datasetRowType"/> ObjectDataRow;
		<xsl:apply-templates select="." mode="classConstructorDeclaration"/>
	</xsl:template>

	<!-- Top level objects -->
	<xsl:template match="cobos:Object[ parent::cobos:DataModel ]" mode="classConstructorDeclaration">

		public <xsl:value-of select="@typeName"/>( <xsl:value-of select="@datasetRowType"/> dataRow )
		{
			ObjectDataRow = dataRow;
			<xsl:apply-templates select="cobos:Object" mode="classConstructorBody"/>
			<xsl:apply-templates select="cobos:Reference" mode="classConstructorBody"/>
		}
	</xsl:template>

	<!-- Nested objects -->
	<xsl:template match="cobos:Object[ parent::cobos:Object ]" mode="classConstructorDeclaration">
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
	Member variables and constructor body for references
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<!-- class member declaration for a nested object -->
	<xsl:template match="cobos:Object" mode="classMemberDecl">
		<xsl:value-of select="@typeName"/>
		<xsl:text> _</xsl:text>
		<xsl:value-of select="@name"/>
		<xsl:text>;</xsl:text>
		<xsl:if test="not( position() = last() )">
			<xsl:value-of select="$newlineTab2"/>
		</xsl:if>
	</xsl:template>

	<!-- class member declaration for a reference type -->
	<xsl:template match="cobos:Reference[ not( @isCollection ) ]" mode="classMemberDecl">
		<xsl:value-of select="@ref"/>
		<xsl:text> _</xsl:text>
		<xsl:value-of select="@name"/>
		<xsl:text>;</xsl:text>
		<xsl:if test="not( position() = last() )">
			<xsl:value-of select="$newlineTab2"/>
		</xsl:if>
	</xsl:template>

	<!-- class member declaration for reference type that is a collection-->
	<xsl:template match="cobos:Reference[ @isCollection ]" mode="classMemberDecl">
		<xsl:apply-templates select="." mode="listDecl"/>
		<xsl:text> _</xsl:text>
		<xsl:value-of select="@name"/>
		<xsl:text>;</xsl:text>
		<xsl:if test="not( position() = last() )">
			<xsl:value-of select="$newlineTab2"/>
		</xsl:if>
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

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Create property declarations
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<!-- output a simple property type, nested object or reference -->
	<xsl:template match="cobos:Property|cobos:Object|cobos:Reference" mode="propertyDefinition">
		[DataMember(Order=<xsl:value-of select="position() - 1"/>)]
		<xsl:apply-templates select="." mode="propertyQualifiers"/> <xsl:apply-templates select="." mode="propertyType"/> <xsl:value-of select="@name"/>
		{
			<xsl:apply-templates select="." mode="propertyBody"/>
		}
	</xsl:template>

	<!-- Property qualifiers in abstract classes -->
	<xsl:template match="*[ self::cobos:Reference|self::cobos:Property|self::cobos:Object ][ ancestor::cobos:Interface[ @isAbstractClass = 'true' ] ]" mode="propertyQualifiers">
		<xsl:text>public abstract </xsl:text>
	</xsl:template>

	<!-- Property qualifiers in interfaces -->
	<xsl:template match="*[ self::cobos:Reference|self::cobos:Property|self::cobos:Object ][ ancestor::cobos:Interface[ not( @isAbstractClass = 'true' ) ] ]" mode="propertyQualifiers"/>

	<!-- Property qualifiers in concrete classes -->
	<xsl:template match="*[ self::cobos:Reference|self::cobos:Property|self::cobos:Object ][ not( ancestor::cobos:Interface ) ]" mode="propertyQualifiers">
		<xsl:text>public </xsl:text>
		<xsl:apply-templates select="." mode="propertyQualifiersForInheritance"/>
	</xsl:template>

	<!-- Property qualifiers for inherited properties -->
	<xsl:template match="*[ self::cobos:Reference|self::cobos:Property|self::cobos:Object ][ ancestor::cobos:Object[ @implements ] ]" mode="propertyQualifiersForInheritance">
		<!-- get the interface object the parent class implements -->
		<xsl:variable name="interface" select="/cobos:DataModel/cobos:Interface[ @name = current()/ancestor-or-self::cobos:Object[ @implements ]/@implements ]"/>
		<!-- only apply if this property is defined in the abstract class -->
		<xsl:apply-templates select="$interface//*[ self::cobos:Object | self::cobos:Property ][ substring-after( @qualifiedName, '.' ) = substring-after( current()/@qualifiedName, '.' ) ]" mode="propertyQualifiersForInheritanceOverride"/>
	</xsl:template>

	<xsl:template match="cobos:Reference|cobos:Property|cobos:Object" mode="propertyQualifiersForInheritanceOverride">
		<xsl:text>override </xsl:text>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Match property types
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Property[ @dbType = 'xsd:string' or (contains( @dbType, 'string_' ) and not( @stringFormat )) ]" mode="propertyType">
		<xsl:text>string </xsl:text>
	</xsl:template>

	<xsl:template match="cobos:Property[ @dbType = 'xsd:integer' ]" mode="propertyType">
		<xsl:text>long</xsl:text>
		<xsl:apply-templates select="@minOccurs" mode="propertyType"/>
	</xsl:template>

	<xsl:template match="cobos:Property[ @dbType = 'xsd:float' ]" mode="propertyType">
		<xsl:text>float</xsl:text>
		<xsl:apply-templates select="@minOccurs" mode="propertyType"/>
	</xsl:template>

	<xsl:template match="cobos:Property[ @dbType = 'xsd:dateTime' ]" mode="propertyType">
		<xsl:text>DateTime</xsl:text>
		<xsl:apply-templates select="@minOccurs" mode="propertyType"/>
	</xsl:template>

	<xsl:template match="cobos:Property[ contains( @dbType, 'hexBinary_' ) ]" mode="propertyType">
		<xsl:text>byte[] </xsl:text>
	</xsl:template>

	<xsl:template match="@minOccurs[ . = 0 ]" mode="propertyType">
		<xsl:text>? </xsl:text>
	</xsl:template>

	<xsl:template match="@minOccurs[ . = 1 ]" mode="propertyType">
		<xsl:text> </xsl:text>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Match object types - may be overriding abstract class
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Object[ ancestor-or-self::cobos:Object[ @implements ] ]" mode="propertyType">
		<!-- get the interface object the parent class implements -->
		<xsl:variable name="interface" select="/cobos:DataModel/cobos:Interface[ @name = current()/ancestor-or-self::cobos:Object[ @implements ]/@implements ]"/>
		<!-- only apply if this property is defined in the abstract class -->
		<xsl:variable name="property" select="$interface//cobos:Object[ substring-after( @qualifiedName, '.' ) = substring-after( current()/@qualifiedName, '.' ) ]"/>
		<xsl:choose>
			<xsl:when test="$property">
				<xsl:value-of select="$property/@qualifiedTypeName"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="@typeName"/>
			</xsl:otherwise>
		</xsl:choose>
		<xsl:text> </xsl:text>
	</xsl:template>

	<xsl:template match="cobos:Object[ not( ancestor-or-self::cobos:Object[ @implements ] ) ]" mode="propertyType">
		<xsl:value-of select="@typeName"/>
		<xsl:text> </xsl:text>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Match reference types
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Reference[ not( @isCollection ) ]" mode="propertyType">
		<xsl:value-of select="@ref"/>
		<xsl:text> </xsl:text>
	</xsl:template>

	<xsl:template match="cobos:Reference[ @isCollection ]" mode="propertyType">
		<xsl:apply-templates select="." mode="listDecl"/>
		<xsl:text> </xsl:text>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Create the property get/set function declarations
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Object|cobos:Reference|cobos:Property[ ancestor::cobos:Interface ]" mode="propertyBody">
		<xsl:text>get;
			set;</xsl:text>
	</xsl:template>

	<xsl:template match="cobos:Object[ not( ancestor::cobos:Interface ) ]" mode="propertyBody">
		<xsl:text>get { return _</xsl:text><xsl:value-of select="@name"/><xsl:text>; }
			set { }</xsl:text>
	</xsl:template>
	
	<xsl:template match="cobos:Reference[ not( ancestor::cobos:Interface ) ]" mode="propertyBody">
		<xsl:text>get { </xsl:text>
		<xsl:apply-templates select="." mode="propertyGet"/>
		<xsl:text> }
			set { }</xsl:text>
	</xsl:template>

	<xsl:template match="cobos:Property[ not( ancestor::cobos:Interface ) ][ @minOccurs = 1 ]" mode="propertyBody">
		<xsl:text>get { </xsl:text>
		<xsl:apply-templates select="." mode="propertyGet"/>
		<xsl:text> }
			set { </xsl:text>
		<xsl:apply-templates select="." mode="propertySet"/>
		<xsl:text> } </xsl:text>
	</xsl:template>

	<xsl:template match="cobos:Property[ not( ancestor::cobos:Interface ) ][ @minOccurs = 0 ]" mode="propertyBody">
		<xsl:text>get
			{ </xsl:text>
				if ( ObjectDataRow.Is<xsl:apply-templates mode="qualifiedName" select="."/>Null() )
				{
					return null;
				}
				<xsl:apply-templates select="." mode="propertyGet"/>
			}
			set 
			{
				if ( value == null )
				{
					ObjectDataRow.Set<xsl:apply-templates mode="qualifiedName" select="."/>Null();
				}
				else
				{
					<xsl:apply-templates select="." mode="propertySet"/>
				} <xsl:text>
			} </xsl:text>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Create the property get function body implementation
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<!-- basic return type -->
	<xsl:template match="cobos:Property[ not( @stringFormat ) ]" mode="propertyGet">
		<xsl:variable name="columnName">
			<xsl:apply-templates mode="qualifiedName" select="."/>
		</xsl:variable>
		<xsl:value-of select="concat( 'return ObjectDataRow.', $columnName, ';' )"/>
	</xsl:template>

	<xsl:template match="cobos:Reference" mode="propertyGet">
		<xsl:value-of select="concat( 'return _', @name, ';' )"/>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Create the property set function body implementation
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<!-- basic type -->
	<xsl:template match="cobos:Property[ not( @stringFormat ) ]" mode="propertySet">
		<xsl:variable name="columnName">
			<xsl:apply-templates mode="qualifiedName" select="."/>
		</xsl:variable>
		<xsl:variable name="value">
			<xsl:apply-templates mode="propertySetValue" select="."/>
		</xsl:variable>
		<xsl:value-of select="concat( 'ObjectDataRow.', $columnName, ' = ', $value, ';' )"/>
	</xsl:template>

	<!-- simple case for any non-nullable type -->
	<xsl:template match="cobos:Property[ @minOccurs = 1 ]" mode="propertySetValue">
		<xsl:text>value</xsl:text>
	</xsl:template>

	<!-- Strings can be set to null -->
	<xsl:template match="cobos:Property[ @minOccurs = 0 and @dbType = 'xsd:string' ]" mode="propertySetValue">
		<xsl:text>value</xsl:text>
	</xsl:template>

	<!-- any other string length that is not a special formatted value -->
	<xsl:template match="cobos:Property[ @minOccurs = 0 and (contains( @dbType, 'string_' ) and not( @stringFormat )) ]" mode="propertySetValue">
		<xsl:text>value</xsl:text>
	</xsl:template>

	<!-- Nullable value types -->
	<xsl:template match="cobos:Property[ @minOccurs = 0 and ( @dbType = 'xsd:integer' or @dbType = 'xsd:float' or @dbType = 'xsd:dateTime' ) ]" mode="propertySetValue">
		<xsl:text>value.Value</xsl:text>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Create data adapter
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Object" mode="tableAdapter">
	public class <xsl:value-of select="@className"/>TableAdapter<xsl:text disable-output-escaping="yes"><![CDATA[<DbConnectionType,DbCommandType,DataAdapterType>]]></xsl:text>
		where DbConnectionType : <xsl:value-of select="$dbConnectionType"/>, IDisposable, new()
		where DbCommandType : <xsl:value-of select="$dbCommandType"/>, IDisposable, new()
		where DataAdapterType : <xsl:value-of select="$dataAdapterType"/>, IDisposable, new()
	{
		#region Instance Data

		DbConnectionType _connection;

		<xsl:value-of select="@datasetTableType"/> _table = new <xsl:value-of select="@datasetTableType"/>();

		const string _select = "<xsl:apply-templates select=".//cobos:Property" mode="sqlSelect"/>";
		
		const string _from = "<xsl:value-of select="@dbTable"/>";
		
		static readonly string[] _innerJoin = <xsl:apply-templates mode="sqlJoin" select="."/>;

		static readonly string[] _where = <xsl:apply-templates mode="sqlWhere" select="."/>;

		const string _groupBy = <xsl:apply-templates mode="sqlGroupBy" select="."/>;

		const string _orderBy = <xsl:apply-templates mode="sqlOrderBy" select="."/>;

		#endregion

		#region Construction

		public <xsl:value-of select="@className"/>TableAdapter( DbConnectionType connection )
		{
			_connection = connection;
			_table.RowChanging += new DataRowChangeEventHandler( _table_RowChanging );
			_table.RowChanged += new DataRowChangeEventHandler( _table_RowChanged );
			_table.RowDeleting += new DataRowChangeEventHandler( _table_RowDeleting );
			_table.RowDeleted += new DataRowChangeEventHandler( _table_RowDeleted );
			_table.TableNewRow += new DataTableNewRowEventHandler( _table_TableNewRow );
		}

		#endregion

		#region Public properties

		public static readonly SqlSelectTemplate SelectTemplate = new SqlSelectTemplate( _select, _from, _innerJoin, _where, _groupBy, _orderBy, true );

		#endregion

		#region Public methods

		<xsl:apply-templates select="." mode="acceptChangesMethodBody"/>

		<xsl:apply-templates select="." mode="addNewMethodBody"/>

		<xsl:apply-templates select="." mode="createNewMethodBody"/>

		<xsl:apply-templates select="." mode="findByMethodBody"/>

		<xsl:apply-templates select="." mode="findAllMethodBody"/>

		<xsl:apply-templates select="." mode="hasChangesMethodBody"/>

		<xsl:apply-templates select="." mode="rejectChangesMethodBody"/>

		<xsl:apply-templates select="." mode="selectMethodBody"/>

		#endregion

		#region Events

		<xsl:apply-templates select="." mode="delegatesDeclarations"/>

		#endregion

		#region Private methods

		<xsl:apply-templates select="." mode="insertRowsMethodBody"/>

		<xsl:apply-templates select="." mode="updateRowsMethodBody"/>

		<xsl:apply-templates select="." mode="deleteRowsMethodBody"/>

		#endregion

		#region User extensions

		<xsl:apply-templates select="." mode="userTableAdapterExtensions"/>

		#endregion
	}
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	AcceptChanges method body: Commit all changes to the database
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	
	

	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Object" mode="acceptChangesMethodBody">
		/// <xsl:text disable-output-escaping="yes"><![CDATA[<summary>]]></xsl:text>
		/// Commit all changes to the database.
		/// <xsl:text disable-output-escaping="yes"><![CDATA[</summary>]]></xsl:text>
		public void AcceptChanges()
		{
			<xsl:apply-templates select="." mode="listDeclDataRow"/> inserted = new <xsl:apply-templates select="." mode="listDeclDataRow"/>();
			<xsl:apply-templates select="." mode="listDeclDataRow"/> updated = new <xsl:apply-templates select="." mode="listDeclDataRow"/>();
			<xsl:apply-templates select="." mode="listDeclDataRow"/> deleted = new <xsl:apply-templates select="." mode="listDeclDataRow"/>();

			foreach ( <xsl:value-of select="@datasetRowType"/> row in _table.Rows )
			{
				if ( row.RowState == DataRowState.Added )
				{
					inserted.Add( row );
				}
				else if ( row.RowState == DataRowState.Modified )
				{
					updated.Add( row );
				}
				else if ( row.RowState == DataRowState.Deleted )
				{
					deleted.Add( row );
				}
			}

			if ( <xsl:text disable-output-escaping="yes"><![CDATA[inserted.Count == 0 && updated.Count == 0 && deleted.Count == 0]]></xsl:text> )
			{
				return;
			}

			using ( IDbTransaction transaction = _connection.BeginTransaction() )
			{
				try
				{
					InsertRows( inserted );
					UpdateRows( updated );
					DeleteRows( deleted );

					transaction.Commit();

					_table.AcceptChanges();
				}
				catch ( Exception )
				{
					transaction.Rollback();
					throw;
				}
			}
		}
	</xsl:template>


	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	CreateNew method body: Creates a new data object.  
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	
	

	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Object" mode="createNewMethodBody">
		/// <xsl:text disable-output-escaping="yes"><![CDATA[<summary>]]></xsl:text>
		/// Creates a new data object.
		/// <xsl:text disable-output-escaping="yes"><![CDATA[</summary>]]></xsl:text>
		public <xsl:value-of select="@className"/> CreateNew<xsl:value-of select="@className"/>()
		{
			<xsl:value-of select="@datasetRowType"/> row = _table.New<xsl:value-of select="@className"/>Row();

			return new <xsl:value-of select="@className"/>( row );
		}
	</xsl:template>


	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	AddNew method body: Adds the newly created object to the model.  
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	
	

	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Object" mode="addNewMethodBody">
		/// <xsl:text disable-output-escaping="yes"><![CDATA[<summary>]]></xsl:text>
		/// If the object needs to be initialised with primary or unique key contstraints 
		/// then make sure that it's done before adding to the model.
		/// <xsl:text disable-output-escaping="yes"><![CDATA[</summary>]]></xsl:text>
		public <xsl:value-of select="@className"/> AddNew<xsl:value-of select="@className"/>( <xsl:value-of select="@className"/> @object)
		{
			_table.Rows.Add( @object.ObjectDataRow );
			
			return @object;
		}
	</xsl:template>


	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	FindBy method body: Find the object using the primary key fields.
	Only add this method if the table has primary key or unique key constraints.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	
		public LineupUnit FindByLineupNameUnitId( string LineupName, string UnitId )
		{
			UnitDataModel.LineupUnitRow found = _table.FindByLineupNameUnitId( LineupName, UnitId );

			if ( found == null || found.RowState == DataRowState.Deleted )
			{
				return null;
			}

			return new LineupUnit( found );
		}
	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Object" mode="findByMethodBody">
		<xsl:if test="$databaseConstraintsNodeSet/*[ self::xsd:key | self::xsd:unique ][ xsd:selector/@xpath = concat( './/', current()/@dbTable ) ]">
		/// <xsl:text disable-output-escaping="yes"><![CDATA[<summary>]]></xsl:text>
		/// Find the object using the primary key fields.
		/// <xsl:text disable-output-escaping="yes"><![CDATA[</summary>]]></xsl:text>
		public <xsl:value-of select="@className"/> FindBy<xsl:apply-templates select="." mode="findByMethodName"/>( <xsl:apply-templates select="." mode="findByMethodArgumentList"/> )
		{
			<xsl:apply-templates select="." mode="findByMethodImpl"/>
		}
		</xsl:if>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Argument list: string LineupName, string UnitId.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->
	
	<xsl:template match="cobos:Object" mode="findByMethodArgumentList">
		<xsl:variable name="object" select="."/>
		<xsl:for-each select="$databaseConstraintsNodeSet//xsd:key//xsd:field[ ../xsd:selector/@xpath = concat( './/', $object/@dbTable ) ]">
			<!-- Get the [ 1 ]st node since we will get multiple node results for db fields 
					that are decomposed into more than property, e.g. stringFormat="Seperator" -->
			<xsl:variable name="property" select="$object//cobos:Property[ translate( @dbColumn, $lowercase, $uppercase ) = current()/@xpath ][ 1 ]"/>
			<xsl:variable name="propertyType">
				<xsl:apply-templates select="$property" mode="propertyType"/>
			</xsl:variable>
			<xsl:value-of select="$propertyType"/>
			<xsl:value-of select="$property/@name"/>
			<xsl:if test="not( position() = last() )">
				<xsl:text>, </xsl:text>
			</xsl:if>
		</xsl:for-each>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Method body: 
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Object" mode="findByMethodImpl">
		<xsl:value-of select="@datasetRowType"/> found = _table.FindBy<xsl:apply-templates select="." mode="findByMethodName"/>( <xsl:apply-templates select="." mode="findByMethodParams"/> );

			if ( found == null || found.RowState == DataRowState.Deleted )
			{
				return null;
			}

			return new <xsl:value-of select="@className"/>( found );
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Method name: FindByLineupNameUnitId.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Object" mode="findByMethodName">
		<xsl:variable name="object" select="."/>
		<xsl:for-each select="$databaseConstraintsNodeSet//xsd:key//xsd:field[ ../xsd:selector/@xpath = concat( './/', $object/@dbTable ) ]">
			<xsl:variable name="property" select="$object//cobos:Property[ translate( @dbColumn, $lowercase, $uppercase ) = current()/@xpath ]"/>
			<xsl:value-of select="$property/@name"/>
		</xsl:for-each>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Method params: LineupName, UnitId.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Object" mode="findByMethodParams">
		<xsl:variable name="object" select="."/>
		<xsl:for-each select="$databaseConstraintsNodeSet//xsd:key//xsd:field[ ../xsd:selector/@xpath = concat( './/', $object/@dbTable ) ]">
			<xsl:apply-templates select="$object//cobos:Property[ translate( @dbColumn, $lowercase, $uppercase ) = current()/@xpath ]" mode="findByMethodParamValue"/>
			<xsl:if test="not( position() = last() )">
				<xsl:text>, </xsl:text>
			</xsl:if>
		</xsl:for-each>
	</xsl:template>

	<xsl:template match="cobos:Property" mode="findByMethodParamValue">
		<xsl:value-of select="@name"/>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	FindAll method body: Get all objects stored in memory.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	
	

	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Object" mode="findAllMethodBody">
		/// <xsl:text disable-output-escaping="yes"><![CDATA[<summary>]]></xsl:text>
		/// Get all objects stored in memory.
		/// <xsl:text disable-output-escaping="yes"><![CDATA[</summary>]]></xsl:text>
		public <xsl:apply-templates select="." mode="listDecl"/> FindAll()
		{
			int numRows = _table.Rows.Count;

			<xsl:apply-templates select="." mode="listDecl"/> results = new <xsl:apply-templates select="." mode="listDecl"/>( numRows );

			<xsl:text disable-output-escaping="yes"><![CDATA[for ( int row = 0; row < numRows; ++row )]]></xsl:text>
			{
				if ( _table[ row ].RowState != DataRowState.Deleted )
				{
					results.Add( new <xsl:value-of select="@className"/>( _table[ row ] ) );
				}
			}

			return results;
		}
	</xsl:template>


	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Select method body: Refresh the data table from the database.  Loses
	all in-memory changes that aren't committed.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	
	

	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Object" mode="selectMethodBody">
		/// <xsl:text disable-output-escaping="yes"><![CDATA[<summary>]]></xsl:text>
		/// Refresh the data table from the database.
		/// Loses all in-memory changes that aren't committed.
		/// <xsl:text disable-output-escaping="yes"><![CDATA[</summary>]]></xsl:text>
		public void Select( string[] where, string orderBy )
		{
			_table.Clear();

			using ( DbCommandType command = new DbCommandType() )
			{
				command.Connection = _connection;
				command.CommandText = SelectTemplate.ToString( where, null, orderBy );
			
				using ( DataAdapterType adapter = new DataAdapterType() )
				{
					<xsl:choose>
						<xsl:when test="$dataAdapterType = 'IDbDataAdapter' and $dbCommandType = 'IDbCommand'">
					// special case for data adapters and commands that implement the System.Data interfaces
					// but not the System.Data.Common common implementations.  The IDataAdapter/IDbDataAdapter
					// interfaces do not provide a Fill( DataTable table ) method, only a Fill( DataSet dataset)
					// method.  Rather than implementing the method here, temporarily add the table to a new
					// DataSet and then use the existing implementation.
					using ( DataSet dataset = new DataSet() )
					{
						string tableName = _table.TableName;
						_table.TableName = "Table";
						dataset.Tables.Add( _table );

						adapter.SelectCommand = command;
						adapter.Fill( dataset );

						dataset.Tables.Remove( _table );
						_table.TableName = tableName;
					}
						</xsl:when>
						<xsl:otherwise>
					adapter.SelectCommand = command;
					adapter.Fill( dataset );
						</xsl:otherwise>
					</xsl:choose>
				}
			}
		}

		public void Select()
		{
			Select( null, null );
		}
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	HasChanges method body: Check whether this has been modified.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	
	

	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Object" mode="hasChangesMethodBody">
		/// <xsl:text disable-output-escaping="yes"><![CDATA[<summary>]]></xsl:text>
		/// Check whether this has changes.
		/// <xsl:text disable-output-escaping="yes"><![CDATA[</summary>]]></xsl:text>
		public bool HasChanges()
		{
			return (_table.GetChanges() != null);
		}
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	RejectChanges method body: Undo all in-memory changes.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	
	

	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Object" mode="rejectChangesMethodBody">
		/// <xsl:text disable-output-escaping="yes"><![CDATA[<summary>]]></xsl:text>
		/// Undo all in-memory changes.
		/// <xsl:text disable-output-escaping="yes"><![CDATA[</summary>]]></xsl:text>
		public void RejectChanges()
		{
			_table.RejectChanges();
		}
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	InsertRows method body: Insert all new in-memory rows into the database.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	
		private void InsertRows( List<UnitTestDataModel.NessUnitTestRow> changed )
		{
			if ( changed.Count == 0 )
			{
				return;
			}

			StringBuilder buffer = new StringBuilder( 1024 );

			buffer.Append( "INSERT INTO NESS_UNIT_TESTS " );
			buffer.Append( "( column1, column2, column3, column4, column5, column6 ) " );

			if ( changed.Count == 1 )
			{
				// single row insert - do this the simple way
				buffer.Append( "VALUES (" );
				AppendInsertValues( buffer, changed[ 0 ] );
				buffer.Append( ")" );
			}
			else
			{
				// multi-row insert - do a batch insert for efficiency (non-standard Sql, Oracle only).
				bool first = true;

				foreach ( UnitTestDataModel.NessUnitTestRow row in changed )
				{
					if ( !first )
					{
						buffer.Append( " UNION ALL " );
					}
					else
					{
						first = false;
					}

					buffer.Append( "SELECT " );
					AppendInsertValues( buffer, row );
					buffer.Append( " FROM DUAL " );
				}
			}

			using ( DbCommandType command = new DbCommandType( buffer.ToString(), _connection ) )
			{
				command.ExecuteNonQuery();
			}
		}

		/// <summary>
		/// Append the SQL insert fragment for this row.
		/// </summary>
		private void AppendInsertValues( StringBuilder buffer, UnitTestDataModel.NessUnitTestRow row )
		{
			
			buffer.Append( "'" + row.Col1 + "'" );
			buffer.Append( ", ");
			buffer.Append( row.IsCol2Null() ? "NULL" : row.Col2.ToString() );
			buffer.Append( ", ");
			buffer.Append( row.IsCol3Null() ? "NULL" : "'" + row.Col3 + "'" );
			buffer.Append( ", ");
			buffer.Append( row.IsCol4Null() ? "NULL" : "'" + row.Col4 + "'" );
			buffer.Append( ", ");
			buffer.Append( row.IsCol5Null() ? "NULL" : "'" + row.Col5 + "'" );
			buffer.Append( ", ");
			buffer.Append( row.Col6.ToString() );
		}
	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Object" mode="insertRowsMethodBody">
		/// <xsl:text disable-output-escaping="yes"><![CDATA[<summary>]]></xsl:text>
		/// Insert all new in-memory rows into the database.
		/// <xsl:text disable-output-escaping="yes"><![CDATA[</summary>]]></xsl:text>
		private void InsertRows( <xsl:apply-templates select="." mode="listDeclDataRow"/> changed )
		{
			if ( changed.Count == 0 )
			{
				return;
			}

			StringBuilder buffer = new StringBuilder( 1024 );

			buffer.Append( "INSERT INTO <xsl:value-of select="@dbTable"/> " );
			buffer.Append( "( <xsl:apply-templates select=".//cobos:Property" mode="sqlInsert"/> ) " );

			if ( changed.Count == 1 )
			{
				// single row insert - do this the simple way
				buffer.Append( "VALUES (" );
				AppendInsertValues( buffer, changed[ 0 ] );
				buffer.Append( ")" );
			}
			else
			{
				// multi-row insert - do a batch insert for efficiency (non-standard Sql, Oracle only).
				bool first = true;

				foreach ( <xsl:value-of select="@datasetRowType"/> row in changed )
				{
					if ( !first )
					{
						buffer.Append( " UNION ALL " );
					}
					else
					{
						first = false;
					}

					buffer.Append( "SELECT " );
					AppendInsertValues( buffer, row );
					buffer.Append( " FROM DUAL " );
				}
			}

			using ( DbCommandType command = new DbCommandType() )
			{
				command.Connection = _connection;
				command.CommandText = buffer.ToString();
				command.ExecuteNonQuery();
			}
		}

		/// <xsl:text disable-output-escaping="yes"><![CDATA[<summary>]]></xsl:text>
		/// Append the SQL insert fragment for this row.
		/// <xsl:text disable-output-escaping="yes"><![CDATA[</summary>]]></xsl:text>
		private void AppendInsertValues( StringBuilder buffer, <xsl:value-of select="@datasetRowType"/> row )
		{
			<xsl:apply-templates select=".//cobos:Property" mode="sqlInsertValue"/>
		}
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	UpdateRows method body: Update all in-memory modified rows into the database.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	

	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Object" mode="updateRowsMethodBody">
		/// <xsl:text disable-output-escaping="yes"><![CDATA[<summary>]]></xsl:text>
		/// Update all in-memory modified rows into the database.
		/// <xsl:text disable-output-escaping="yes"><![CDATA[</summary>]]></xsl:text>
		private void UpdateRows( <xsl:apply-templates select="." mode="listDeclDataRow"/> changed )
		{
			if ( changed.Count == 0 )
			{
				return;
			}

			using ( DbCommandType command = new DbCommandType() )
			{
				command.Connection = _connection;

				foreach ( <xsl:value-of select="@datasetRowType"/> row in changed )
				{
					StringBuilder buffer = new StringBuilder( 1024 );
				
					buffer.Append( "UPDATE <xsl:value-of select="@dbTable"/> SET " );

					AppendUpdateValues( buffer, row );

					buffer.Append( " WHERE " + <xsl:apply-templates select="." mode="sqlUpdateWhere"/> );

					command.CommandText = buffer.ToString();
					
					command.ExecuteNonQuery();
				}
			}
		}

		/// <xsl:text disable-output-escaping="yes"><![CDATA[<summary>]]></xsl:text>
		/// Append the SQL update fragment for this row.
		/// <xsl:text disable-output-escaping="yes"><![CDATA[</summary>]]></xsl:text>
		private void AppendUpdateValues( StringBuilder buffer, <xsl:value-of select="@datasetRowType"/> row )
		{
			<xsl:apply-templates select=".//cobos:Property" mode="sqlUpdateValue"/>
		}

	</xsl:template>


	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	DeleteRows method body: Delete all in-memory deleted rows from the database.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	
		private void DeleteRows( List<UnitTestDataModel.NessUnitTestRow> changed )
		{
			if ( changed.Count == 0 )
			{
				return;
			}
			
			StringBuilder buffer = new StringBuilder( 1024 );
			buffer.Append( "DELETE FROM NESS_UNIT_TESTS WHERE " );
			
			bool first = true;
			
			foreach ( UnitTestDataModel.NessUnitTestRow row in changed )
			{
				if ( first )
				{
					first = false;
				}
				else
				{
					buffer.Append( " OR " );
				}
				
				buffer.Append( "(column1 = '" + row[ "column1", DataRowVersion.Original ].ToString() + "')" );
			}
			
			using ( DbCommandType command = new DbCommandType( buffer.ToString(), _connection ) )
			{
				command.ExecuteNonQuery();
			}
		}
	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Object" mode="deleteRowsMethodBody">
		/// <xsl:text disable-output-escaping="yes"><![CDATA[<summary>]]></xsl:text>
		/// Delete all in-memory deleted rows from the database.
		/// <xsl:text disable-output-escaping="yes"><![CDATA[</summary>]]></xsl:text>
		private void DeleteRows( <xsl:apply-templates select="." mode="listDeclDataRow"/> changed )
		{
			if ( changed.Count == 0 )
			{
				return;
			}
			
			StringBuilder buffer = new StringBuilder( 1024 );
			buffer.Append( "DELETE FROM <xsl:value-of select="@dbTable"/> WHERE " );
			
			bool first = true;
			
			foreach ( <xsl:value-of select="@datasetRowType"/> row in changed )
			{
				if ( first )
				{
					first = false;
				}
				else
				{
					buffer.Append( " OR " );
				}
				
				buffer.Append( <xsl:apply-templates select="." mode="sqlDeleteWhere"/> );
			}
			
			using ( DbCommandType command = new DbCommandType() )
			{
				command.Connection = _connection;
				command.CommandText = buffer.ToString();
				command.ExecuteNonQuery();
			}
		}
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	DataTable events: pass on any row changing events to our delegates.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	
	
	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Object" mode="delegatesDeclarations">

		<xsl:call-template name="delegateMethodBody">
			<xsl:with-param name="methodName">_table_RowChanging</xsl:with-param>
			<xsl:with-param name="rowEventType">DataRowChangeEventArgs</xsl:with-param>
			<xsl:with-param name="delegateType">OnModifying</xsl:with-param>
		</xsl:call-template>

		<xsl:call-template name="delegateMethodBody">
			<xsl:with-param name="methodName">_table_RowChanged</xsl:with-param>
			<xsl:with-param name="rowEventType">DataRowChangeEventArgs</xsl:with-param>
			<xsl:with-param name="delegateType">OnModified</xsl:with-param>
		</xsl:call-template>

		<xsl:call-template name="delegateMethodBody">
			<xsl:with-param name="methodName">_table_RowDeleting</xsl:with-param>
			<xsl:with-param name="rowEventType">DataRowChangeEventArgs</xsl:with-param>
			<xsl:with-param name="delegateType">OnDeleting</xsl:with-param>
		</xsl:call-template>

		<xsl:call-template name="delegateMethodBody">
			<xsl:with-param name="methodName">_table_RowDeleted</xsl:with-param>
			<xsl:with-param name="rowEventType">DataRowChangeEventArgs</xsl:with-param>
			<xsl:with-param name="delegateType">OnDeleted</xsl:with-param>
		</xsl:call-template>

		<xsl:call-template name="delegateMethodBody">
			<xsl:with-param name="methodName">_table_TableNewRow</xsl:with-param>
			<xsl:with-param name="rowEventType">DataTableNewRowEventArgs</xsl:with-param>
			<xsl:with-param name="delegateType">OnAdded</xsl:with-param>
		</xsl:call-template>

	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Delegate generator: pass on any row changing events to our delegates.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		
		public event OnAddedDelegate OnAddedInstanceLineup;

		public delegate void OnAddedDelegate( InstanceLineup @object );
	
		void _table_TableNewRow( object sender, DataTableNewRowEventArgs e )
		{
			if ( OnAddedInstanceLineup != null )
			{
				LineupDataModel.InstanceLineupRow row = e.Row as LineupDataModel.InstanceLineupRow;

				if ( row != null )
				{
					OnAddedInstanceLineup( new InstanceLineup( row ) );
				}
			}
		}
	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template name="delegateMethodBody">
		<xsl:param name="methodName"/>
		<xsl:param name="rowEventType"/>
		<xsl:param name="delegateType"/>
		<xsl:variable name="delegateName" select="concat( $delegateType, @className )"/>
		
		public event <xsl:value-of select="$delegateType"/>Delegate <xsl:value-of select="$delegateName"/>;

		public delegate void <xsl:value-of select="$delegateType"/>Delegate( <xsl:value-of select="@className"/> @object );

		void <xsl:value-of select="$methodName"/>( object sender, <xsl:value-of select="$rowEventType"/> e )
		{
			if ( <xsl:value-of select="$delegateName"/> != null )
			{
				<xsl:value-of select="@datasetRowType"/> row = e.Row as <xsl:value-of select="@datasetRowType"/>;

				if ( row != null )
				{
					<xsl:value-of select="$delegateName"/>( new <xsl:value-of select="@className"/>( row ) );
				}
			}
		}
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Object" mode="listDecl">
		<xsl:text disable-output-escaping="yes"><![CDATA[List<]]></xsl:text>
		<xsl:value-of select="@className"/>
		<xsl:text disable-output-escaping="yes"><![CDATA[>]]></xsl:text>
	</xsl:template>

	<xsl:template match="cobos:Object" mode="listDeclDataRow">
		<xsl:text disable-output-escaping="yes"><![CDATA[List<]]></xsl:text>
		<xsl:value-of select="@datasetRowType"/>
		<xsl:text disable-output-escaping="yes"><![CDATA[>]]></xsl:text>
	</xsl:template>

	<xsl:template match="cobos:Reference" mode="listDecl">
		<xsl:text disable-output-escaping="yes"><![CDATA[List<]]></xsl:text>
		<xsl:value-of select="@ref"/>
		<xsl:text disable-output-escaping="yes"><![CDATA[>]]></xsl:text>
	</xsl:template>

	<xsl:template name="executeStatement">
		<xsl:text disable-output-escaping="yes"><![CDATA[database.Execute<]]></xsl:text>
		<xsl:value-of select="@datasetTableType"/>
		<xsl:text disable-output-escaping="yes"><![CDATA[>]]></xsl:text>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	SQL SELECT columns
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Property" mode="sqlSelect">
		<xsl:value-of select="@dbTable"/>.<xsl:apply-templates select="." mode="sqlSelectColumn"/>
		<xsl:if test="not( position() = last() )">, </xsl:if>
	</xsl:template>

	<xsl:template match="cobos:Property[ not( @dbAlias ) ]" mode="sqlSelectColumn">
		<xsl:value-of select="@dbColumn"/>
	</xsl:template>

	<xsl:template match="cobos:Property[ @dbAlias ]" mode="sqlSelectColumn">
		<xsl:value-of select="@dbColumn"/>
		<xsl:text > AS </xsl:text>
		<xsl:value-of select="@dbAlias"/>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	SQL INSERT columns
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Property" mode="sqlInsert">
		<xsl:value-of select="@dbColumn"/>
		<xsl:if test="not( position() = last() )">, </xsl:if>
	</xsl:template>

	<!-- simple case for non-nullable fields -->
	<xsl:template match="cobos:Property[ @minOccurs = 1 ]" mode="sqlInsertValue">
		<xsl:apply-templates select="." mode="sqlPropertyRow"/>
		<xsl:if test="not( position() = last() )">
			<xsl:value-of select="concat( $newlineTab3, 'buffer.Append( ', $quot, ', ', $quot, ');' )"/>
		</xsl:if>
	</xsl:template>

	<!-- test for null for nullable fields before inserting -->
	<xsl:template match="cobos:Property[ @minOccurs = 0 ]" mode="sqlInsertValue">
		<xsl:apply-templates select="." mode="sqlPropertyRow"/>
		<xsl:if test="not( position() = last() )">
			<xsl:value-of select="concat( $newlineTab3, 'buffer.Append( ', $quot, ', ', $quot, ');' )"/>
		</xsl:if>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	SQL UPDATE columns
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<!-- simple case for non-nullable fields -->
	<xsl:template match="cobos:Property[ @minOccurs = 1 ]" mode="sqlUpdateValue">
		<xsl:value-of select="concat( $newlineTab3, 'buffer.Append( ', $quot, @dbColumn, '=',  $quot, ' );' )"/>
		<xsl:apply-templates select="." mode="sqlPropertyRow"/>
		<xsl:if test="not( position() = last() )">
			<xsl:value-of select="concat( $newlineTab3, 'buffer.Append( ', $quot, ', ', $quot, ');' )"/>
		</xsl:if>
	</xsl:template>

	<!-- test for null for nullable fields before inserting -->
	<xsl:template match="cobos:Property[ @minOccurs = 0 ]" mode="sqlUpdateValue">
		<xsl:value-of select="concat( $newlineTab3, 'buffer.Append( ', $quot, @dbColumn, '=',  $quot, ' );' )"/>
		<xsl:apply-templates select="." mode="sqlPropertyRow"/>
		<xsl:if test="not( position() = last() )">
			<xsl:value-of select="concat( $newlineTab3, 'buffer.Append( ', $quot, ', ', $quot, ');' )"/>
		</xsl:if>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	SQL column value fragments for inserting and updating rows
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<!-- simple case for non-nullable fields -->
	<xsl:template match="cobos:Property[ @minOccurs = 1 ]" mode="sqlPropertyRow">
		<xsl:variable name="columnValue">
			<xsl:apply-templates mode="sqlPropertyValue" select="."/>
		</xsl:variable>
		<xsl:value-of select="concat( $newlineTab3, 'buffer.Append( ', $columnValue, ' );' )"/>
		<xsl:if test="not( position() = last() )">
			<xsl:value-of select="concat( $newlineTab3, 'buffer.Append( ', $quot, ', ', $quot, ');' )"/>
		</xsl:if>
	</xsl:template>

	<!-- test for null for nullable fields before inserting -->
	<xsl:template match="cobos:Property[ @minOccurs = 0 ]" mode="sqlPropertyRow">
		<xsl:variable name="columnName">
			<xsl:apply-templates mode="qualifiedName" select="."/>
		</xsl:variable>
		<xsl:variable name="columnValue">
			<xsl:apply-templates mode="sqlPropertyValue" select="."/>
		</xsl:variable>
		<xsl:value-of select="concat( $newlineTab3, 'buffer.Append( row.Is', $columnName, 'Null() ? ', $quot, 'NULL', $quot, ' : ', $columnValue, ' );' )"/>
		<xsl:if test="not( position() = last() )">
			<xsl:value-of select="concat( $newlineTab3, 'buffer.Append( ', $quot, ', ', $quot, ');' )"/>
		</xsl:if>
	</xsl:template>


	<!-- Simple property value for non-string types -->
	<xsl:template match="cobos:Property[ not( @dbType = 'xsd:string' or contains( @dbType, 'string_' ) ) ]" mode="sqlPropertyValue">
		<xsl:variable name="columnName">
			<xsl:apply-templates mode="qualifiedName" select="."/>
		</xsl:variable>
		<xsl:value-of select="concat( 'row.', $columnName, '.ToString()' )"/>
	</xsl:template>

	<!-- Get a quoted value for the string property -->
	<xsl:template match="cobos:Property[ @dbType = 'xsd:string' or contains( @dbType, 'string_' ) ]" mode="sqlPropertyValue">
		<xsl:variable name="columnName">
			<xsl:apply-templates mode="qualifiedName" select="."/>
		</xsl:variable>
		<xsl:value-of select="concat( $quot, $apos, $quot, ' + row.', $columnName, ' + ', $quot, $apos, $quot )"/>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	SQL UPDATE statement WHERE clause - list of PK fields
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Object" mode="sqlUpdateWhere">
		<xsl:variable name="object" select="."/>
		<xsl:text>"(</xsl:text>
		<xsl:for-each select="$databaseConstraintsNodeSet//xsd:field[ ../xsd:selector/@xpath = concat( './/', $object/@dbTable ) ]">
			<xsl:apply-templates select="$object//cobos:Property[ translate( @dbColumn, $lowercase, $uppercase ) = current()/@xpath ]"
										mode="sqlUpdateWhere"/>
			<xsl:if test="not( position() = last() )">
				<xsl:text> AND </xsl:text>
			</xsl:if>
		</xsl:for-each>
		<xsl:text>)"</xsl:text>
	</xsl:template>

	<xsl:template match="cobos:Property[ @dbType = 'xsd:string' or contains( @dbType, 'string_' ) ]" mode="sqlUpdateWhere">
		<xsl:variable name="columnName">
			<xsl:apply-templates mode="qualifiedName" select="."/>
		</xsl:variable>
		<xsl:value-of select="concat( @dbColumn, ' = ', $apos, $quot, ' + row.', $columnName, ' + ', $quot, $apos )"/>
	</xsl:template>

	<!-- -->
	<xsl:template match="cobos:Property[ not( @dbType = 'xsd:string' or contains( @dbType, 'string_' ) ) ]" mode="sqlUpdateWhere">
		<xsl:variable name="columnName">
			<xsl:apply-templates mode="qualifiedName" select="."/>
		</xsl:variable>
		<xsl:value-of select="concat( @dbColumn, ' = ', $quot, ' + row.', $columnName, ' + ', $quot )"/>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	SQL DELETE statement WHERE clause - list of PK fields, handling deleted rows
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:Object" mode="sqlDeleteWhere">
		<xsl:variable name="object" select="."/>
		<xsl:text>"(</xsl:text>
		<xsl:for-each select="$databaseConstraintsNodeSet//xsd:field[ ../xsd:selector/@xpath = concat( './/', $object/@dbTable ) ]">
			<xsl:apply-templates select="$object//cobos:Property[ translate( @dbColumn, $lowercase, $uppercase ) = current()/@xpath ]"
										mode="sqlDeleteWhere"/>
			<xsl:if test="not( position() = last() )">
				<xsl:text> AND </xsl:text>
			</xsl:if>
		</xsl:for-each>
		<xsl:text>)"</xsl:text>
	</xsl:template>

	<xsl:template match="cobos:Property[ @dbType = 'xsd:string' or contains( @dbType, 'string_' ) ]" mode="sqlDeleteWhere">
		<xsl:variable name="columnName">
			<xsl:apply-templates mode="qualifiedName" select="."/>
		</xsl:variable>
		<xsl:value-of select="concat( @dbColumn, ' = ', $apos, $quot, ' + row[ ', $quot, @dbColumn, $quot, ', DataRowVersion.Original ].ToString() + ', $quot, $apos )"/>
	</xsl:template>

	<!-- -->
	<xsl:template match="cobos:Property[ not( @dbType = 'xsd:string' or contains( @dbType, 'string_' ) ) ]" mode="sqlDeleteWhere">
		<xsl:variable name="columnName">
			<xsl:apply-templates mode="qualifiedName" select="."/>
		</xsl:variable>
		<xsl:value-of select="concat( @dbColumn, ' = ', $quot, ' + row[ ', $quot, @dbColumn, $quot, ', DataRowVersion.Original ].ToString() + ', $quot )"/>
	</xsl:template>


	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	INNER/OUTER JOIN
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	
		static readonly string[] _innerJoin = new string[]{ "EVENT ON AEVEN.eid = EVENT.eid" };
	
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<!-- Find all inner join tables -->
	<xsl:template match="cobos:Object[ ./cobos:Metadata/cobos:Joins/* ]" mode="sqlJoin">
		<xsl:text>new string[]{ </xsl:text>
		<xsl:apply-templates mode="sqlJoin" select="./cobos:Metadata/cobos:Joins/*[ self::cobos:InnerJoin | self::cobos:OuterJoin ]"/>
		<xsl:text> }</xsl:text>
	</xsl:template>

	<xsl:template match="cobos:Object[ not( ./cobos:Metadata/cobos:Joins/* ) ]" mode="sqlJoin">
		<xsl:text>null</xsl:text>
	</xsl:template>
	
	<!-- INNER JOIN and OUTER JOIN clauses -->
	<xsl:template match="cobos:InnerJoin | cobos:OuterJoin" mode="sqlJoin">
		<xsl:text>"</xsl:text>
		<xsl:value-of select="@references"/>
		<xsl:text> ON </xsl:text>
		<xsl:value-of select="ancestor::cobos:Object[ 1 ]/@dbTable"/>
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

	<xsl:template match="cobos:Object[ ./cobos:Metadata//cobos:Filter ]" mode="sqlWhere">
		<xsl:text>new string[]{ </xsl:text>
		<xsl:apply-templates select="./cobos:Metadata//cobos:Filter" mode="sqlWhere"/>
		<xsl:text> }</xsl:text>
	</xsl:template>

	<xsl:template match="cobos:Object[ not( ./cobos:Metadata//cobos:Filter ) ]" mode="sqlWhere">
		<xsl:text>null</xsl:text>
	</xsl:template>

	<xsl:template match="cobos:Filter" mode="sqlWhere">
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

	<xsl:template match="cobos:Object[ ./cobos:Metadata/cobos:Order/cobos:By ]" mode="sqlOrderBy">
		<xsl:text>"</xsl:text>
		<xsl:apply-templates select="./cobos:Metadata/cobos:Order/cobos:By"/>
		<xsl:text>"</xsl:text>
	</xsl:template>


	<xsl:template match="cobos:Object[ not( ./cobos:Metadata/cobos:Order/cobos:By ) ]" mode="sqlOrderBy">
		<xsl:text>null</xsl:text>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	GROUP BY clause
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->
	
	<xsl:template match="cobos:Object[ ./cobos:Metadata/cobos:Group/cobos:By ]" mode="sqlGroupBy">
		<xsl:text>"</xsl:text>
		<xsl:apply-templates select="./cobos:Metadata/cobos:Group/cobos:By"/>
		<xsl:text>"</xsl:text>
	</xsl:template>

	<xsl:template match="cobos:Object[ not( ./cobos:Metadata/cobos:Group/cobos:By ) ]" mode="sqlGroupBy">
		<xsl:text>null</xsl:text>
	</xsl:template>

	<!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	ORDER/GROUP BY value
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->

	<xsl:template match="cobos:By">
		<xsl:variable name="property" select="ancestor::cobos:Object[ 1 ]//cobos:Property[ @name = current()/text() ]"/>
		<xsl:value-of select="$property/@dbTable"/>
		<xsl:text>.</xsl:text>
		<xsl:value-of select="$property/@dbColumn"/>
		<xsl:if test="not( position() = last() )">, </xsl:if>
	</xsl:template>


</xsl:stylesheet>