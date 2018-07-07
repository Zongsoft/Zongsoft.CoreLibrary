using System;
using System.Collections.Generic;

namespace Zongsoft.Samples.DataEntity.Models
{
	public interface IPerson : Zongsoft.Data.IDataEntity, System.ComponentModel.INotifyPropertyChanged
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
