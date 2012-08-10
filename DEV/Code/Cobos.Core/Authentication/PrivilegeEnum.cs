using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cobos.Core.Authentication
{
	public enum PrivilegeEnum
	{
		Guest = 1,
		User,
		PowerUser,
		Supervisor,
		Administrator
	}
}
