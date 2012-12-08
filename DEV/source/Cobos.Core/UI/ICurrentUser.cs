using System;

namespace Cobos.Core.UI
{
	public interface ICurrentUser
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="prompt"></param>
		/// <param name="username"></param>
		/// <returns></returns>
		UserLoginDetails ShowLogin( string prompt, string username );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="prompt"></param>
		/// <param name="username"></param>
		/// <param name="hostname"></param>
		/// <param name="port"></param>
		/// <returns></returns>
		DatabaseLoginDetails ShowDatabaseLogin( string prompt, string username, string hostname, int? port );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="prompt"></param>
		/// <param name="hostname"></param>
		/// <param name="port"></param>
		/// <returns></returns>
		DatabaseSettingsDetails ShowDatabaseSettings( string prompt, string hostname, int? port );
	}
}
