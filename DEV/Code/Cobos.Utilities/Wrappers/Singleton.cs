// ============================================================================
// Filename: Singleton.cs
// Description: 
// ----------------------------------------------------------------------------
// Created by: N.Davis                          Date: 27-Nov-09
// Modified by:                                 Date:
// ============================================================================
// Copyright (c) 2009-2011 Nicholas Davis		nick@cobos.co.uk
//
// Cobos Software Development Kit v0.1
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ============================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Cobos.Utilities.Wrappers
{
	/// <summary>
	/// Provides Singleton&lt;T&gt.Instance
	/// This class is thread-safe
	/// </summary>
	/// <remarks>
	/// A private or protected constructor must be implemented in the T class
	/// </remarks>
	public static class Singleton<T>
			 where T : class
	{
		static volatile T _instance;
		static object _lock = new object();

		static Singleton()
		{
		}

		/// <summary>
		/// Use as Singleton&lt;MyClass&gt;.Instance
		/// </summary>
		/// <exception cref="SingletonException">If the T classes constructor is not private or protected</exception>
		public static T Instance
		{
			get
			{
				if ( _instance == null )
				{
					lock ( _lock )
					{
						if ( _instance == null )
						{
							ConstructorInfo constructor = null;

							try
							{
								// Binding flags exclude public constructors.
								constructor = typeof( T ).GetConstructor( BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[ 0 ], null );
							}
							catch ( Exception exception )
							{
								throw new SingletonException( exception );
							}

							if ( constructor == null || constructor.IsAssembly )
							{
								// Also exclude internal constructors.
								throw new SingletonException( string.Format( "A private or protected constructor is missing for '{0}'.", typeof( T ).Name ) );
							}

							_instance = (T)constructor.Invoke( null );
						}
					}
				}

				return _instance;
			}
		}
	}
}
