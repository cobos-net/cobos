using System;

namespace Cobos.Core.UI
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
