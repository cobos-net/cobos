using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intergraph.AsiaPac.Data.Statements
{
	/// <summary>
	/// Simple select constructor, allows basic definition of
	/// a select statement with the option of adding additional
	/// clauses when constructing the string representation.
	/// </summary>
	public class SqlSelect
	{
		readonly string _select;
		readonly string _from;
		readonly string[] _innerJoin;
		readonly string[] _where;
		readonly string _groupBy;
		readonly string _orderBy;
		readonly string _statement;	// cache the basic ToString result

		public SqlSelect()
		{
		}

		public SqlSelect( string select, string from, string[] innerJoin, 
								string[] where, string groupBy, string orderBy )
		{
			_select = select;
			_from = from;
			_innerJoin = innerJoin;
			_where = where;
			_groupBy = groupBy;
			_orderBy = orderBy;
			_statement = ToStringInternal();
		}

		public override string ToString()
		{
			return _statement;
		}

		string ToStringInternal()
		{
			StringBuilder buffer = new StringBuilder( 512 );

			// detect the most obvious error case
			if ( string.IsNullOrEmpty( _select ) )
			{
				throw new InvalidOperationException( "SqlSelect.ToString: The select clause is empty" ); 
			}

			buffer.Append( "SELECT " + _select );

			if ( !string.IsNullOrEmpty( _from ) )
			{
				buffer.Append( " FROM " + _from );
			}

			if ( _innerJoin != null && _innerJoin.Length > 0 )
			{
				for ( int i = 0; i < _innerJoin.Length; ++i )
				{
					buffer.Append( " INNER JOIN " + _innerJoin[ i ] );
				}
			}

			if ( _where != null && _where.Length > 0 )
			{
				buffer.Append( " WHERE (" + _where[ 0 ] + ")" );

				for ( int i = 1; i < _where.Length; ++i )
				{
					buffer.Append( " AND (" + _where[ i ] + ")" );
				}
			}

			if ( !string.IsNullOrEmpty( _groupBy ) )
			{
				buffer.Append( " GROUP BY " + _groupBy );
			}

			if ( !string.IsNullOrEmpty( _orderBy ) )
			{
				buffer.Append( " ORDER BY " + _orderBy );
			}

			return buffer.ToString();
		}

		public string ToString( string select, string from, string[] innerJoin, 
										string[] where, string groupBy, string orderBy )
		{
			StringBuilder buffer = new StringBuilder( 512 );

			// detect the most obvious error case
			if ( string.IsNullOrEmpty( _select ) && string.IsNullOrEmpty( select ) )
			{
				throw new InvalidOperationException( "SqlSelect.ToString: The select clause is empty" );
			}

			bool existsSelect = !string.IsNullOrEmpty( _select );

			if ( existsSelect )
			{
				buffer.Append( "SELECT " + _select );
			}

			if ( !string.IsNullOrEmpty( select ) )
			{
				if ( !existsSelect )
				{
					buffer.Append( "SELECT " + select );
				}
				else
				{
					buffer.Append( ", " + select );
				}
			}

			bool existsFrom = !string.IsNullOrEmpty( _from ); 

			if ( existsFrom )
			{
				buffer.Append( " FROM " + _from );
			}

			if ( !string.IsNullOrEmpty( from ) )
			{
				if ( !existsFrom )
				{
					buffer.Append( " FROM " + from );
				}
				else
				{
					buffer.Append( ", " + from );
				}
			}

			if ( _innerJoin != null && _innerJoin.Length > 0 )
			{
				for ( int i = 0; i < _innerJoin.Length; ++i )
				{
					buffer.Append( " INNER JOIN " + _innerJoin[ i ] );
				}
			}

			if ( innerJoin != null && innerJoin.Length > 0 )
			{
				for ( int i = 0; i < innerJoin.Length; ++i )
				{
					buffer.Append( " INNER JOIN " + innerJoin[ i ] );
				}
			}

			bool existsWhere = _where != null && _where.Length > 0;

			if ( existsWhere )
			{
				buffer.Append( " WHERE (" + _where[ 0 ] + ")" );

				for ( int i = 1; i < _where.Length; ++i )
				{
					buffer.Append( " AND (" + _where[ i ] + ")" );
				}
			}

			if ( where != null && where.Length > 0 )
			{
				int i = 0;
				if ( !existsWhere )
				{
					buffer.Append( " WHERE (" + where[ 0 ] + ")" );
					i = 1;
				}

				for ( ; i < where.Length; ++i )
				{
					buffer.Append( " AND (" + where[ i ] + ")" );
				}
			}

			bool existsGroupBy = !string.IsNullOrEmpty( _groupBy );

			if ( existsGroupBy )
			{
				buffer.Append( " GROUP BY " + _groupBy );
			}

			if ( !string.IsNullOrEmpty( groupBy ) )
			{
				if ( existsGroupBy )
				{
					buffer.Append( ", " + groupBy );
				}
				else
				{
					buffer.Append( " GROUP BY " + groupBy );
				}
			}

			bool existsOrderBy = !string.IsNullOrEmpty( _orderBy );

			if ( existsOrderBy )
			{
				buffer.Append( " ORDER BY " + _orderBy );
			}

			if ( !string.IsNullOrEmpty( orderBy ) )
			{
				if ( existsOrderBy )
				{
					buffer.Append( ", " + orderBy );
				}
				else
				{
					buffer.Append( " ORDER BY " + orderBy );
				}
			}

			return buffer.ToString();
		}
		
	}
}
