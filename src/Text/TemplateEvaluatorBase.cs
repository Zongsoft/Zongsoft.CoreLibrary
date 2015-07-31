using System;
using System.Collections.Generic;

namespace Zongsoft.Text
{
	public abstract class TemplateEvaluatorBase : ITemplateEvaluator
	{
		#region 成员字段
		private string _schema;
		#endregion

		#region 构造函数
		protected TemplateEvaluatorBase(string schema)
		{
			if(string.IsNullOrWhiteSpace(schema))
				throw new ArgumentNullException("schema");

			_schema = schema.Trim();
		}
		#endregion

		#region 公共属性
		public string Schema
		{
			get
			{
				return _schema;
			}
		}
		#endregion

		#region 抽象方法
		public abstract object Evaluate(TemplateEvaluatorContext context);
		#endregion
	}
}
