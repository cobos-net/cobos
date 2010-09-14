using System;

namespace Intergraph.Oz.Core.UI
{
	public interface ICurrentUser
	{
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		bool? ShowLogin( string prompt );

		/// <summary>
		/// 
		/// </summary>
		string Username
		{
			get;
		}

		/// <summary>
		/// 
		/// </summary>
		string Password
		{
			get;
		}
	}
}
