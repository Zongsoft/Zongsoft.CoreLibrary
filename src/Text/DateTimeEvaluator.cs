using System;
using System.Collections.Generic;

namespace Zongsoft.Text
{
	public class DateTimeEvaluator : TemplateEvaluatorBase
	{
		#region 构造函数
		public DateTimeEvaluator() : base("datetime")
		{
		}

		public DateTimeEvaluator(string schema) : base(schema)
		{
		}
		#endregion

		#region 重写方法
		public override object Evaluate(TemplateEvaluatorContext context)
		{
			if(string.IsNullOrWhiteSpace(context.Text))
				return DateTime.Now.ToString();

			return DateTime.Now.ToString(context.Text);
		}
		#endregion
	}
}
