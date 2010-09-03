using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intergraph.Oz.Build.Configuration
{
	/// <summary>
	/// Partial declaration of Generated class to add useful functionality
	/// </summary>
	public partial class VersionNumber : BuildEntity 
	{
		/// <summary>
		/// 
		/// </summary>
		public VersionNumber()
		{
			// parameterless constructor for Xml serialization
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="M"></param>
		/// <param name="N"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public VersionNumber( uint M, uint N, uint x, uint y )
		{
			majorField.Value = M;
			minorField.Value = N;
			buildField.Value = x;
			revisionField.Value = y;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return String.Format( "{0}.{1}.{2}.{3}", majorField.Value, minorField.Value, buildField.Value, revisionField.Value );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="v"></param>
		/// <returns></returns>
		public static implicit operator string( VersionNumber v )
		{
			return String.Format( "{0}.{1}.{2}.{3}", v.majorField.Value, v.minorField.Value, v.buildField.Value, v.revisionField.Value );
		}

		/// <summary>
		/// 
		/// </summary>
		public VersionNumber Increment()
		{
			if ( majorField.incrementOnBuild )
			{
				++majorField.Value;
			}
			if ( minorField.incrementOnBuild )
			{
				++minorField.Value;
			}
			if ( buildField.incrementOnBuild )
			{
				++buildField.Value;
			}
			if ( revisionField.incrementOnBuild )
			{
				++revisionField.Value;
			}
			return this;
		}
	}

	/// <summary>
	/// Partial declaration of Generated class to add useful functionality
	/// </summary>
	public partial class VersionNumberOctet
	{
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Convert.ToString( valueField );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="v"></param>
		/// <returns></returns>
		public static implicit operator string( VersionNumberOctet v )
		{
			return Convert.ToString( v.valueField );
		}
	}

}
