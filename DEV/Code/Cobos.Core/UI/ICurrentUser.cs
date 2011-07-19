using System;

namespace Cobos.Core.UI
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
