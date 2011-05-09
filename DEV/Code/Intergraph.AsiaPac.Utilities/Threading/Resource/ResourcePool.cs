using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Intergraph.AsiaPac.Utilities.Threading.Resource
{
	/// <summary>
	/// Class factory for resource pools
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public static class ResourcePool<T>
	{
		public static IResourcePool<T> Create( ResourcePoolSettings<T> settings )
		{
			return new ResourcePoolImpl<T>( settings );
		}
	}
}
