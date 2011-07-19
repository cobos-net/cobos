using System;

namespace Intergraph.AsiaPac.Core.UI
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
