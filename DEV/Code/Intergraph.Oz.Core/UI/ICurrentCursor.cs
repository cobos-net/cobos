using System;

namespace Intergraph.Oz.Core.UI
{
	public interface ICurrentCursor
	{
		CursorType Type
		{
			set;
		}

		void SetDefault();
	}
}
