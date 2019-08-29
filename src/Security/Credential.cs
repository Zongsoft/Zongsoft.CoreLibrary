/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@qq.com>
 *
 * Copyright (C) 2015-2019 Zongsoft Corporation <http://www.zongsoft.com>
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
	public class Credential : IEquatable<Credential>
	{
		#region 常量定义
		private static readonly DateTime EPOCH = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		#endregion

		#region 成员字段
		private string _credentialId;
		private string _scene;
		private DateTime _creation;
		private TimeSpan _duration;
		private Membership.IUserIdentity _user;
		private IDictionary<string, object> _parameters;
		private Credential _innerCredential;
		#endregion

		#region 构造函数
		public Credential(Membership.IUserIdentity user, string scene, TimeSpan duration, IDictionary<string, object> parameters = null) : this(null, user, scene, duration, DateTime.UtcNow, parameters)
		{
		}

		public Credential(Membership.IUserIdentity user, string scene, TimeSpan duration, DateTime creation, IDictionary<string, object> parameters = null) : this(null, user, scene, duration, creation, parameters)
		{
		}

		public Credential(string credentialId, Membership.IUserIdentity user, string scene, TimeSpan duration, IDictionary<string, object> parameters = null) : this(credentialId, user, scene, duration, DateTime.UtcNow, parameters)
		{
		}

		public Credential(string credentialId, Membership.IUserIdentity user, string scene, TimeSpan duration, DateTime creation, IDictionary<string, object> parameters = null)
		{
			_credentialId = string.IsNullOrWhiteSpace(credentialId) ? this.GenerateId() : credentialId.Trim();
			_user = user ?? throw new ArgumentNullException(nameof(user));
			_scene = string.IsNullOrWhiteSpace(scene) ? null : scene.Trim().ToLowerInvariant();
			_duration = duration.TotalSeconds < 60 ? TimeSpan.FromSeconds(60) : duration;
			_creation = creation.ToUniversalTime();

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
		/// 获取安全凭证的应用场景，譬如：Web、Mobile、Wechat、Facebook 等。
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
		public Membership.IUserIdentity User
		{
			get
			{
				if(_innerCredential != null)
					return _innerCredential.User;

				return _user;
			}
		}

		/// <summary>
		/// 获取安全凭证的签发时间，该值始终为世界时(UTC)。
		/// </summary>
		public DateTime Creation
		{
			get
			{
				if(_innerCredential != null)
					return _innerCredential.Creation;

				return _creation;
			}
		}

		/// <summary>
		/// 获取安全凭证的有效时长，建议不能低于60秒。
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

		#region 公共方法
		/// <summary>
		/// 续约凭证，以当前凭证构建一个新的凭证对象，新凭证除凭证编号和创建时间外其他属性值均相同。
		/// </summary>
		/// <returns>返回续约后的新凭证对象。</returns>
		public virtual Credential Renew()
		{
			if(_innerCredential != null)
				_innerCredential.Renew();

			return new Credential(_user, _scene, _duration, DateTime.UtcNow, _parameters);
		}
		#endregion

		#region 私有方法
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		private string GenerateId()
		{
			return ((ulong)(DateTime.UtcNow - EPOCH).TotalSeconds).ToString() + Zongsoft.Common.Randomizer.GenerateString(8);
		}
		#endregion

		#region 重写方法
		public bool Equals(Credential other)
		{
			if(other == null)
				return false;

			return string.Equals(other.CredentialId, this.CredentialId);
		}

		public override bool Equals(object obj)
		{
			if(obj == null || obj.GetType() != this.GetType())
				return false;

			return string.Equals(this.CredentialId, ((Credential)obj).CredentialId);
		}

		public override int GetHashCode()
		{
			return this.CredentialId.GetHashCode();
		}

		public override string ToString()
		{
			var text = string.IsNullOrEmpty(this.Scene) ?
				this.CredentialId :
				this.CredentialId + "!" + this.Scene;

			if(this.User == null)
				return text;

			return text + " {" + this.User.ToString() + "}";
		}
		#endregion
	}
}
