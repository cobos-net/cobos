using System;

namespace Intergraph.IPS.Utility
{
	/// <summary>
	/// Defines a standard string used by CAD to write boolean values to the database
	/// </summary>
	public sealed class CADBoolean
	{
		/// <summary>
		/// CAD string representing boolean "true"
		/// </summary>
		public const string TrueString = "T";
		/// <summary>
		/// CAD string representing boolean "false"
		/// </summary>
		public const string FalseString = "F";
		/// <summary>
		/// Maximimum number of characters that will appear in the
		/// TrueString or FalseString.
		/// <seealso cref="TrueString"/>
		/// <seealso cref="FalseString"/>
		/// </summary>
		public const int MaxStringLength = 1;

		private CADBoolean()
		{
		}

		/// <summary>
		/// Returns the string representing the given boolean state
		/// </summary>
		/// <param name="state">Boolean state</param>
		/// <returns>String for the given boolean state</returns>
		public static string GetString( bool state )
		{
			if ( state )
				return TrueString;
			else
				return FalseString;
		}

		/// <summary>
		/// Interprets a string from the database as boolean based on the typical boolean strings used by CAD
		/// </summary>
		/// <param name="interpret">String to interpret</param>
		/// <returns>True if string is a single T, Y, or 1 character, false otherwise</returns>
		public static bool ParseString( string interpret )
		{
			bool
				state = false;

			if ( interpret != null && interpret.Length == 1 )
			{
				if ( interpret[ 0 ] == 'Y' || interpret[ 0 ] == 'y' ||
					 interpret[ 0 ] == 'T' || interpret[ 0 ] == 't' ||
					 interpret[ 0 ] == '1' )
					state = true;
			}

			return state;
		}
	}
}
