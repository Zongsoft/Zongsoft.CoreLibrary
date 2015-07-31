using System;
using System.Collections.Generic;

namespace Zongsoft.Text
{
	public interface ITemplateEvaluator
	{
		string Schema
		{
			get;
		}

		object Evaluate(TemplateEvaluatorContext context);
	}
}
