using System;
using System.Collections.Generic;

namespace Zongsoft.Text
{
	public class TemplateEvaluatorContext
	{
		#region 成员字段
		private string _text;
		private object _data;
		private IDictionary<string, object> _parameters;
		#endregion

		#region 构造函数
		public TemplateEvaluatorContext(string text, object data)
		{
			_text = text;
			_data = data;
		}
		#endregion

		#region 公共属性
		public string Text
		{
			get
			{
				return _text;
			}
		}

		public object Data
		{
			get
			{
				return _data;
			}
		}

		public bool HasParameters
		{
			get
			{
				return _parameters != null && _parameters.Count > 0;
			}
		}

		public IDictionary<string, object> Parameters
		{
			get
			{
				if(_parameters == null)
					System.Threading.Interlocked.CompareExchange(ref _parameters, new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase), null);

				return _parameters;
			}
		}
		#endregion
	}
}
