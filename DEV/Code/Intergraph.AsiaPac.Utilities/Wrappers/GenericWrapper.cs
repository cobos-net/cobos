using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intergraph.AsiaPac.Utilities.Wrappers
{
	/// <summary>
	/// Can be used to implement generic classes that are designed to be used with
	/// unrelated reference types that cannot satisfy a 'where' constaint with
	/// multiple definitions.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class GenericWrapper<T> : IDisposable
	{
		/// <summary>
		/// 
		/// </summary>
		T _object = default( T );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		GenericWrapper( T obj )
		{
			_object = obj;
		}

		/// <summary>
		/// 
		/// </summary>
		public void Dispose()
		{
			IDisposable idisp = _object as IDisposable;
			
			if ( idisp != null )
			{
				idisp.Dispose();
			}
			
			_object = default( T );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="C"></typeparam>
		/// <returns></returns>
		public C Cast<C>() where C : class
		{
			return _object as C;
		}
	}
}
