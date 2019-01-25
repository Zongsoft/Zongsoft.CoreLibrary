/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2003-2019 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Security
{
	/// <summary>
	/// 表示安全凭证的实体类。
	/// </summary>
	[Serializable]
	public class Credential
	{
		#region 成员字段
		private string _credentialId;
		private string _scene;
		private DateTime _timestamp;
		private DateTime _issuedTime;
		private TimeSpan _duration;
		private Membership.IUser _user;
		private IDictionary<string, object> _parameters;
		private Credential _innerCredential;
		#endregion

		#region 构造函数
		public Credential(string credentialId, Membership.IUser user, string scene, TimeSpan duration) : this(credentialId, user, scene, duration, DateTime.Now, null)
		{
		}

		public Credential(string credentialId, Membership.IUser user, string scene, TimeSpan duration, DateTime issuedTime, IDictionary<string, object> parameters = null)
		{
			if(string.IsNullOrWhiteSpace(credentialId))
				throw new ArgumentNullException(nameof(credentialId));

			_credentialId = credentialId.Trim();
			_user = user ?? throw new ArgumentNullException(nameof(user));
			_scene = scene == null ? null : scene.Trim();
			_duration = duration;
			_issuedTime = issuedTime;
			_timestamp = issuedTime;

			if(parameters != null && parameters.Count > 0)
				_parameters = new Dictionary<string, object>(parameters, StringComparer.OrdinalIgnoreCase);
		}

		protected Credential(Credential innerCredential)
		{
			_innerCredential = innerCredential ?? throw new ArgumentNullException(nameof(innerCredential));
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取安全凭证编号。
		/// </summary>
		public string CredentialId
		{
			get
			{
				if(_innerCredential != null)
					return _innerCredential.CredentialId;

				return _credentialId;
			}
		}

		/// <summary>
		/// 获取安全凭证所属的命令空间。
		/// </summary>
		[Zongsoft.Runtime.Serialization.SerializationMember(Runtime.Serialization.SerializationMemberBehavior.Ignored)]
		public string Namespace
		{
			get
			{
				if(_innerCredential != null)
					return _innerCredential.Namespace;

				return _user == null ? null : _user.Namespace;
			}
		}

		/// <summary>
		/// 获取安全凭证的应用场景，譬如：Web、Mobile 等。
		/// </summary>
		public string Scene
		{
			get
			{
				if(_innerCredential != null)
					return _innerCredential.Scene;

				return _scene;
			}
		}

		/// <summary>
		/// 获取安全凭证对应的用户对象。
		/// </summary>
		public Membership.IUser User
		{
			get
			{
				if(_innerCredential != null)
					return _innerCredential.User;

				return _user;
			}
		}

		/// <summary>
		/// 获取安全凭证对应的用户编号。
		/// </summary>
		[Zongsoft.Runtime.Serialization.SerializationMember(Runtime.Serialization.SerializationMemberBehavior.Ignored)]
		public uint UserId
		{
			get
			{
				if(_innerCredential != null)
					return _innerCredential.UserId;

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
				if(_innerCredential != null)
					return _innerCredential.Timestamp;

				return _timestamp;
			}
			set
			{
				if(_innerCredential != null)
					_innerCredential.Timestamp = value;
				else
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
				if(_innerCredential != null)
					return _innerCredential.IssuedTime;

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
				if(_innerCredential != null)
					return _innerCredential.Duration;

				return _duration;
			}
		}

		/// <summary>
		/// 获取安全凭证的过期时间。
		/// </summary>
		/// <remarks>
		///		<para>该属性始终返回<see cref="Timestamp"/>属性加上<see cref="Duration"/>属性的值。</para>
		/// </remarks>
		[Zongsoft.Runtime.Serialization.SerializationMember(Runtime.Serialization.SerializationMemberBehavior.Ignored)]
		public DateTime Expires
		{
			get
			{
				if(_innerCredential != null)
					return _innerCredential.Expires;

				return _timestamp + _duration;
			}
		}

		/// <summary>
		/// 获取一个值，指示当前凭证是否为一个空凭证。
		/// </summary>
		[Zongsoft.Runtime.Serialization.SerializationMember(Behavior = Runtime.Serialization.SerializationMemberBehavior.Ignored)]
		public bool IsEmpty
		{
			get
			{
				return string.IsNullOrEmpty(this.CredentialId);
			}
		}

		/// <summary>
		/// 获取一个值，指示参数集是否存在并且有值。
		/// </summary>
		[Zongsoft.Runtime.Serialization.SerializationMember(Runtime.Serialization.SerializationMemberBehavior.Ignored)]
		public bool HasParameters
		{
			get
			{
				if(_innerCredential != null)
					return _innerCredential.HasParameters;

				return _parameters != null && _parameters.Count > 0;
			}
		}

		/// <summary>
		/// 获取安全凭证的参数集。
		/// </summary>
		public IDictionary<string, object> Parameters
		{
			get
			{
				if(_innerCredential != null)
					return _innerCredential.Parameters;

				if(_parameters == null)
					System.Threading.Interlocked.CompareExchange(ref _parameters, new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase), null);

				return _parameters;
			}
		}
		#endregion

		#region 重写方法
		public override bool Equals(object obj)
		{
			if(obj == null || obj.GetType() != this.GetType())
				return false;

			var other = (Credential)obj;

			return string.Equals(this.CredentialId, other.CredentialId, StringComparison.OrdinalIgnoreCase) &&
				   string.Equals(this.Scene, other.Scene, StringComparison.OrdinalIgnoreCase);
		}

		public override int GetHashCode()
		{
			return (this.CredentialId + ":" + this.Scene).ToLowerInvariant().GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("{0}:{1} {2}", this.CredentialId, this.Scene, this.User);
		}
		#endregion
	}
}
