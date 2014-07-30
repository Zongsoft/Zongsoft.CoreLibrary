
using System;
using System.Collections.Generic;

namespace Zongsoft.Services
{
	public interface IServiceLifetime
	{
		bool IsAlive(ServiceEntry entry);
	}
}
