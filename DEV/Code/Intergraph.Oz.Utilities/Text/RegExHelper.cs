using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;


namespace Intergraph.Oz.Utilities.Text
{
	public static class RegExHelper
	{
		/// <summary>
		/// Guid pattern matching.
		/// </summary>
		public static readonly string GuidPattern = @"\{[A-F0-9]{8}-[A-F0-9]{4}-[A-F0-9]{4}-[A-F0-9]{4}-[A-F0-9]{12}\}";

		public static readonly Regex GuidExpression = new Regex( GuidPattern );

		/// <summary>
		/// Phone number pattern matching.  Generic phone, not country specific.
		/// </summary>
		public static readonly string PhonePattern = @"\b(\(?\+?[0-9]*\)?)?[0-9 ()-]{3,}\b";

		public static readonly Regex PhoneExpression = new Regex( PhonePattern );

		/// <summary>
		/// Time pattern matching.
		/// </summary>
		public static readonly string TimePattern = @"(([0-1]?[0-9])|([2][0-3])):([0-5]?[0-9])(:([0-5]?[0-9]))?";

		public static readonly Regex TimeExpression = new Regex( TimePattern );

		/// <summary>
		/// Numeric pattern matching.
		/// </summary>
		public static readonly string NumericPattern = @"^[-+]?([0-9]{1,3}[,]?)?([0-9]{3}[,]?)*[.]?[0-9]*$";

		public static readonly Regex NumericExpression = new Regex( NumericPattern );
	}
}
