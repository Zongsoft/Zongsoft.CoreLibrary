/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2003-2015 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Collections;
using System.Collections.Generic;

namespace Zongsoft.Security
{
	/// <summary>
	/// 表示安全凭证的实体类。
	/// </summary>
	[Serializable]
	public class Certification
	{
		#region 常量定义
		private const string EXTENDEDPROPERTIESPREFIX = "ExtendedProperties.";
		#endregion

		#region 成员字段
		private string _certificationId;
		private string _namespace;
		private string _scene;
		private int _userId;
		private DateTime _issuedTime;
		private TimeSpan _duration;
		private IDictionary<string, object> _extendedProperties;
		#endregion

		#region 构造函数
		public Certification(string certificationId, string @namespace, string scene, int userId, TimeSpan duration)
			: this(certificationId, @namespace, scene, userId, duration, DateTime.Now, null)
		{
		}

		public Certification(string certificationId, string @namespace, string scene, int userId, TimeSpan duration, DateTime issuedTime, IDictionary<string, object> extendedProperties = null)
		{
			if(string.IsNullOrWhiteSpace(certificationId))
				throw new ArgumentNullException("certificationId");

			_certificationId = certificationId.Trim();
			_namespace = @namespace == null ? null : @namespace.Trim();
			_scene = scene == null ? null : scene.Trim();
			_userId = userId;
			_duration = duration;
			_issuedTime = issuedTime;

			if(extendedProperties != null && extendedProperties.Count > 0)
				_extendedProperties = new Dictionary<string, object>(extendedProperties, StringComparer.OrdinalIgnoreCase);
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取安全凭证编号。
		/// </summary>
		public string CertificationId
		{
			get
			{
				return _certificationId;
			}
		}

		/// <summary>
		/// 获取安全凭证所属的命令空间。
		/// </summary>
		public string Namespace
		{
			get
			{
				return _namespace;
			}
		}

		/// <summary>
		/// 获取安全凭证的应用场景，譬如：Web、Mobile 等。
		/// </summary>
		public string Scene
		{
			get
			{
				return _scene;
			}
		}

		/// <summary>
		/// 获取安全凭证对应的用户编号。
		/// </summary>
		public int UserId
		{
			get
			{
				return _userId;
			}
		}

		/// <summary>
		/// 获取安全凭证的签发时间。
		/// </summary>
		public DateTime IssuedTime
		{
			get
			{
				return _issuedTime;
			}
		}

		/// <summary>
		/// 获取安全凭证的有效期限。
		/// </summary>
		public TimeSpan Duration
		{
			get
			{
				return _duration;
			}
		}

		/// <summary>
		/// 获取一个值，指示扩展属性集是否存在并且有值。
		/// </summary>
		public bool HasExtendedProperties
		{
			get
			{
				return _extendedProperties != null && _extendedProperties.Count > 0;
			}
		}

		/// <summary>
		/// 获取安全凭证的扩展属性集。
		/// </summary>
		public IDictionary<string, object> ExtendedProperties
		{
			get
			{
				if(_extendedProperties == null)
					System.Threading.Interlocked.CompareExchange(ref _extendedProperties, new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase), null);

				return _extendedProperties;
			}
		}
		#endregion

		#region 公共方法
		public IDictionary<string, object> ToDictionary()
		{
			var result = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase )
			{
				{"CertificationId", this.CertificationId},
				{"UserId", this.UserId},
				{"Namespace", this.Namespace},
				{"Scene", this.Scene},
				{"Duration", this.Duration},
				{"IssuedTime", this.IssuedTime},
			};

			var extendedProperties = _extendedProperties;

			if(extendedProperties != null && extendedProperties.Count > 0)
			{
				foreach(var extendedProperty in extendedProperties)
				{
					result.Add(EXTENDEDPROPERTIESPREFIX + extendedProperty.Key, extendedProperty.Value);
				}
			}

			return result;
		}

		public static Certification FromDictionary(IDictionary dictionary)
		{
			if(dictionary == null || dictionary.Count < 1)
				return null;

			var result = new Certification((string)dictionary["CertificationId"],
				(string)dictionary["Namespace"],
				(string)dictionary["Scene"],
				Zongsoft.Common.Convert.ConvertValue<int>(dictionary["UserId"]),
				Zongsoft.Common.Convert.ConvertValue<TimeSpan>(dictionary["Duration"], TimeSpan.Zero),
				Zongsoft.Common.Convert.ConvertValue<DateTime>(dictionary["IssuedTime"]));

			foreach(var key in dictionary.Keys)
			{
				if(key == null)
					continue;

				if(key.ToString().StartsWith(EXTENDEDPROPERTIESPREFIX))
					result.ExtendedProperties[key.ToString().Substring(EXTENDEDPROPERTIESPREFIX.Length)] = dictionary[key];
			}

			return result;
		}

		public static Certification FromDictionary<TValue>(IDictionary<string, TValue> dictionary)
		{
			if(dictionary == null || dictionary.Count < 1)
				return null;

			Certification result;
			TValue certificationId, @namespace, scene, userId, issuedTime, duration;

			if(dictionary.TryGetValue("CertificationId", out certificationId) &&
			   dictionary.TryGetValue("UserId", out userId))
			{
				dictionary.TryGetValue("Namespace", out @namespace);
				dictionary.TryGetValue("Scene", out scene);
				dictionary.TryGetValue("IssuedTime", out issuedTime);
				dictionary.TryGetValue("Duration", out duration);

				result = new Certification(Zongsoft.Common.Convert.ConvertValue<string>(certificationId),
											Zongsoft.Common.Convert.ConvertValue<string>(@namespace),
											Zongsoft.Common.Convert.ConvertValue<string>(scene),
											Zongsoft.Common.Convert.ConvertValue<int>(userId),
											Zongsoft.Common.Convert.ConvertValue<TimeSpan>(duration),
											Zongsoft.Common.Convert.ConvertValue<DateTime>(issuedTime));
			}
			else
				return null;

			foreach(var key in dictionary.Keys)
			{
				if(key == null)
					continue;

				if(key.ToString().StartsWith(EXTENDEDPROPERTIESPREFIX))
					result.ExtendedProperties[key.ToString().Substring(EXTENDEDPROPERTIESPREFIX.Length)] = dictionary[key];
			}

			return result;
		}
		#endregion

		#region 重写方法
		public override bool Equals(object obj)
		{
			if(obj == null || obj.GetType() != this.GetType())
				return false;

			var other = (Certification)obj;

			return string.Equals(_certificationId, other._certificationId, StringComparison.OrdinalIgnoreCase) &&
				   string.Equals(_namespace, other._namespace, StringComparison.OrdinalIgnoreCase) &&
				   _userId == other._userId;
		}

		public override int GetHashCode()
		{
			return (_certificationId + ":" + _userId.ToString()).ToLowerInvariant().GetHashCode();
		}

		public override string ToString()
		{
			if(string.IsNullOrWhiteSpace(_namespace))
				return string.Format("[{0}] {1}", _certificationId, _userId);
			else
				return string.Format("[{0}] {1}@{2}", _certificationId, _userId, _namespace);
		}
		#endregion
	}
}
