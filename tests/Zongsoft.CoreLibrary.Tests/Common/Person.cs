using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zongsoft.Common.Tests
{
	public class Person
	{
		public string Name
		{
			get;
			set;
		}

		public Gender? Gender
		{
			get;
			set;
		}

		public Address HomeAddress
		{
			get;
			set;
		}

		public Address OfficeAddress
		{
			get;
			set;
		}
	}
}
