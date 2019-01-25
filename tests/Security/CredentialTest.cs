using System;
using System.Collections;
using System.Collections.Generic;

using Xunit;

namespace Zongsoft.Security
{
	public class CredentialTest
	{
		private Credential _credential;

		public CredentialTest()
		{
			_credential = new Credential("10012001", new Membership.User(101, "Popeye"), "Web", TimeSpan.FromHours(4), new DateTime(2015, 5, 15));
			_credential.Parameters.Add("User.Avatar", ":001:");
			_credential.Parameters.Add("User.Nickname", "钟少");
			_credential.Parameters.Add("User.Gender", Zongsoft.Tests.Gender.Male);
		}

		[Fact]
		public void ToDictionaryTest()
		{
			//var dictionary = _credential.ToDictionary();

			//Assert.Equal("10012001", dictionary["CertificationId"]);
			//Assert.Equal("Web", dictionary["Scene"]);
			//Assert.Equal(101, dictionary["User.UserId"]);
			//Assert.Equal("Popeye", dictionary["User.Name"]);
			//Assert.Equal(new DateTime(2015, 5, 15), dictionary["IssuedTime"]);
			//Assert.Equal(TimeSpan.FromHours(4), dictionary["Duration"]);

			//Assert.Equal(":001:", dictionary["ExtendedProperties.User.Avatar"]);
			//Assert.Equal("钟少", dictionary["ExtendedProperties.User.Nickname"]);
			//Assert.Equal(Zongsoft.Tests.Gender.Male, dictionary["ExtendedProperties.User.Gender"]);
		}

		[Fact]
		public void FromDictionaryTest()
		{
			//var dictionary = _credential.ToDictionary();
			//var credential = Credential.FromDictionary(dictionary);

			//Assert.Equal("10012001", credential.CredentialId);
			//Assert.Equal("Web", credential.Scene);
			//Assert.Equal(101, credential.User.UserId);
			//Assert.Equal(new DateTime(2015, 5, 15), credential.IssuedTime);
			//Assert.Equal(TimeSpan.FromHours(4), credential.Duration);

			//Assert.True(credential.HasExtendedProperties);
			//Assert.Equal(3, credential.ExtendedProperties.Count);

			//Assert.Equal(":001:", credential.ExtendedProperties["User.Avatar"]);
			//Assert.Equal("钟少", credential.ExtendedProperties["User.Nickname"]);
			//Assert.Equal(Zongsoft.Tests.Gender.Male, credential.ExtendedProperties["User.Gender"]);
		}
	}
}
