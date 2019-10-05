using System;
using System.Collections.Generic;

namespace Zongsoft.Samples.Models
{
	public interface IPerson : Zongsoft.Data.IModel //, System.ComponentModel.INotifyPropertyChanged
	{
		string Name
		{
			get; set;
		}

		string FullName
		{
			get; set;
		}
	}
}
