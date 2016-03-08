/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2015 Zongsoft Corporation <http://www.zongsoft.com>
 *
 * This file is part of Zongsoft.CoreLibrary.
 *
 * Zongsoft.CoreLibrary is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * Zongsoft.CoreLibrary is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * Lesser General Public License for more details.
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with Zongsoft.CoreLibrary; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA
 */

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using System.Threading;

namespace Zongsoft.Text
{
	public class TemplateEvaluatorManager
	{
		#region 静态字段
		private static readonly Regex _regex_ = new Regex(@"\$\{(?<scheme>\w+(\.\w+)*)(:(?<text>[^\{\}]+))?\}", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.ExplicitCapture);
		#endregion

		#region 成员字段
		private readonly ConcurrentDictionary<string, ITemplateEvaluator> _evaluators;
		#endregion

		#region 构造函数
		public TemplateEvaluatorManager()
		{
			_evaluators = new ConcurrentDictionary<string, ITemplateEvaluator>(StringComparer.OrdinalIgnoreCase);
		}
		#endregion

		#region 单例属性
		private static TemplateEvaluatorManager _default;
		private static int _initializationFlag;

		public static TemplateEvaluatorManager Default
		{
			get
			{
				if(_initializationFlag == 0)
				{
					var original = Interlocked.CompareExchange(ref _initializationFlag, 1, 0);

					if(original == 0)
					{
						_default = new TemplateEvaluatorManager();
						_default.Register(new Evaluation.DateTimeEvaluator());
						_default.Register(new Evaluation.BindingEvaluator());
						_default.Register(new Evaluation.RandomEvaluator());

						_initializationFlag = 2;
					}
				}

				if(_initializationFlag == 1)
					SpinWait.SpinUntil(() => _initializationFlag > 1);

				return _default;
			}
		}
		#endregion

		#region 公共方法
		public void Register(ITemplateEvaluator evaluator)
		{
			if(evaluator == null)
				throw new ArgumentNullException("evaluator");

			_evaluators[evaluator.Scheme] = evaluator;
		}

		public void Register(string scheme, Type type)
		{
			if(string.IsNullOrWhiteSpace(scheme))
				throw new ArgumentNullException("scheme");

			if(type == null)
				throw new ArgumentNullException("type");

			if(!typeof(ITemplateEvaluator).IsAssignableFrom(type))
				throw new ArgumentException();

			_evaluators[scheme] = (ITemplateEvaluator)Activator.CreateInstance(type);
		}

		public bool Unregister(string scheme)
		{
			if(scheme == null)
				return false;

			ITemplateEvaluator result;
			return _evaluators.TryRemove(scheme, out result);
		}

		public object Evaluate(string text, object data)
		{
			if(string.IsNullOrWhiteSpace(text))
				return text;

			var position = 0;
			var result = new List<TemplateEvaluatorResult>();
			var match = _regex_.Match(text);
			ITemplateEvaluator evaluator;

			while(match.Success)
			{
				if(position < match.Index)
					result.Add(new TemplateEvaluatorResult(text.Substring(position, match.Index - position), position, match.Index - position));

				if(_evaluators.TryGetValue(match.Groups["scheme"].Value, out evaluator))
				{
					object value = null;

					if(match.Groups["text"].Success)
						value = evaluator.Evaluate(new TemplateEvaluatorContext(match.Groups["text"].Value, data));
					else
						value = evaluator.Evaluate(new TemplateEvaluatorContext(null, data));

					result.Add(new TemplateEvaluatorResult(value, match.Index, match.Length));
				}

				position = match.Index + match.Length;

				match = match.NextMatch();
			}

			if(position < text.Length)
				result.Add(new TemplateEvaluatorResult(text.Substring(position), position, text.Length - position));

			if(result.Count < 1)
				return text;

			var builder = new System.Text.StringBuilder();

			foreach(var item in result)
			{
				builder.Append(item.Value);
			}

			return builder.ToString();
		}

		public T Evaluate<T>(string text, object data)
		{
			var result = this.Evaluate(text, data);
			return Zongsoft.Common.Convert.ConvertValue<T>(result);
		}
		#endregion

		#region 嵌套子类
		private class TemplateEvaluatorResult
		{
			public TemplateEvaluatorResult(object value, int index, int length)
			{
				this.Value = value;
				this.Index = index;
				this.Length = length;
			}

			public int Index;
			public int Length;
			public object Value;
		}
		#endregion
	}
}
