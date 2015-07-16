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
using System.ComponentModel;

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
		private Membership.User _user;
		private DateTime _timestamp;
		private DateTime _issuedTime;
		private TimeSpan _duration;
		private IDictionary<string, object> _extendedProperties;
		#endregion

		#region 构造函数
		public Certification(string certificationId, Membership.User user, string @namespace, string scene, TimeSpan duration)
			: this(certificationId, user, @namespace, scene, duration, DateTime.Now, null)
		{
		}

		public Certification(string certificationId, Membership.User user, string @namespace, string scene, TimeSpan duration, DateTime issuedTime, IDictionary<string, object> extendedProperties = null)
		{
			if(string.IsNullOrWhiteSpace(certificationId))
				throw new ArgumentNullException("certificationId");

			if(user == null)
				throw new ArgumentNullException("user");

			_user = user;
			_certificationId = certificationId.Trim();
			_namespace = @namespace == null ? null : @namespace.Trim();
			_scene = scene == null ? null : scene.Trim();
			_duration = duration;
			_issuedTime = issuedTime;
			_timestamp = issuedTime;

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
		/// 获取安全凭证对应的用户对象。
		/// </summary>
		public Membership.User User
		{
			get
			{
				return _user;
			}
		}

		/// <summary>
		/// 获取安全凭证对应的用户编号。
		/// </summary>
		public int UserId
		{
			get
			{
				var user = _user;
				return user == null ? 0 : user.UserId;
			}
		}

		/// <summary>
		/// 获取或设置安全凭证的最后活动时间。
		/// </summary>
		public DateTime Timestamp
		{
			get
			{
				return _timestamp;
			}
			set
			{
				_timestamp = value;
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
		/// 获取安全凭证的过期时间。
		/// </summary>
		/// <remarks>
		///		<para>该属性始终返回<see cref="Timestamp"/>属性加上<see cref="Duration"/>属性的值。</para>
		/// </remarks>
		public DateTime Expires
		{
			get
			{
				return _timestamp + _duration;
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
				{"Namespace", this.Namespace},
				{"Scene", this.Scene},
				{"Duration", this.Duration},
				{"IssuedTime", this.IssuedTime},
				{"Timestamp", this.Timestamp},
			};

			var properties = TypeDescriptor.GetProperties(typeof(Membership.User));

			foreach(PropertyDescriptor property in properties)
			{
				result.Add("User." + property.Name, property.GetValue(_user));
			}

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

			var user = new Membership.User(Zongsoft.Common.Convert.ConvertValue<int>(dictionary["User.UserId"]),
				Zongsoft.Common.Convert.ConvertValue<string>(dictionary["User.Name"]),
				Zongsoft.Common.Convert.ConvertValue<string>(dictionary["User.Namespace"]));

			var properties = TypeDescriptor.GetProperties(typeof(Membership.User));

			foreach(PropertyDescriptor property in properties)
			{
				if(property.IsReadOnly)
					continue;

				property.SetValue(user, Zongsoft.Common.Convert.ConvertValue(dictionary["User." + property.Name], property.PropertyType));
			}

			var result = new Certification((string)dictionary["CertificationId"], user,
				Zongsoft.Common.Convert.ConvertValue<string>(dictionary["Namespace"]),
				Zongsoft.Common.Convert.ConvertValue<string>(dictionary["Scene"]),
				Zongsoft.Common.Convert.ConvertValue<TimeSpan>(dictionary["Duration"], TimeSpan.Zero),
				Zongsoft.Common.Convert.ConvertValue<DateTime>(dictionary["IssuedTime"]))
				{
					Timestamp = Zongsoft.Common.Convert.ConvertValue<DateTime>(dictionary["Timestamp"]),
				};

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
			Membership.User user = null;
			TValue certificationId, userId, userName, @namespace, scene, timestamp, issuedTime, duration;

			if(dictionary.TryGetValue("User.UserId", out userId) && dictionary.TryGetValue("User.Name", out userName))
			{
				user = new Membership.User(Zongsoft.Common.Convert.ConvertValue<int>(userId),
				                           Zongsoft.Common.Convert.ConvertValue<string>(userName));

				var properties = TypeDescriptor.GetProperties(typeof(Membership.User));

				foreach(PropertyDescriptor property in properties)
				{
					if(property.IsReadOnly)
						continue;

					property.SetValue(user, Zongsoft.Common.Convert.ConvertValue(dictionary["User." + property.Name], property.PropertyType));
				}
			}

			if(dictionary.TryGetValue("CertificationId", out certificationId) && user != null)
			{
				dictionary.TryGetValue("Namespace", out @namespace);
				dictionary.TryGetValue("Scene", out scene);
				dictionary.TryGetValue("IssuedTime", out issuedTime);
				dictionary.TryGetValue("Duration", out duration);
				dictionary.TryGetValue("Timestamp", out timestamp);

				result = new Certification(Zongsoft.Common.Convert.ConvertValue<string>(certificationId), user,
											Zongsoft.Common.Convert.ConvertValue<string>(@namespace),
											Zongsoft.Common.Convert.ConvertValue<string>(scene),
											Zongsoft.Common.Convert.ConvertValue<TimeSpan>(duration),
											Zongsoft.Common.Convert.ConvertValue<DateTime>(issuedTime))
											{
												Timestamp = Zongsoft.Common.Convert.ConvertValue<DateTime>(timestamp),
											};
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

			return string.Equals(_certificationId, other.CertificationId, StringComparison.OrdinalIgnoreCase) &&
			       string.Equals(_namespace, other.Namespace, StringComparison.OrdinalIgnoreCase) &&
			       string.Equals(_scene, other.Scene, StringComparison.OrdinalIgnoreCase);
		}

		public override int GetHashCode()
		{
			return (_certificationId + ":" + _scene + "@" + _namespace).ToLowerInvariant().GetHashCode();
		}

		public override string ToString()
		{
			if(string.IsNullOrWhiteSpace(_namespace))
				return string.Format("{0} ({1})", _certificationId, _scene);
			else
				return string.Format("{0} ({1}@{2})", _certificationId, _scene, _namespace);
		}
		#endregion
	}
}
