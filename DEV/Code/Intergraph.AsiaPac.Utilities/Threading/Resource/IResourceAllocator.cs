namespace Intergraph.AsiaPac.Utilities.Threading.Resource
{
	/// <summary>
	/// User defined resource allocator to enable the resource pool
	/// to dynamically add resources on demand.
	/// </summary>
	public interface IResourceAllocator<T>
	{
		T Create();
	}

	/// <summary>
	/// Convenience default allocator for objects with default or parameterless constructors.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ResourceDefaultAllocator<T> : IResourceAllocator<T>
		where T : new()
	{
		public T Create()
		{
			return new T();
		}
	}
}
