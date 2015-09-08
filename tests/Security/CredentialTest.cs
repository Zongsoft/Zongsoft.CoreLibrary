using System;
using System.Collections;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zongsoft.Security
{
	[TestClass]
	public class CredentialTest
	{
		private Credential _credential;

		public CredentialTest()
		{
			_credential = new Credential("10012001", new Membership.User(101, "Popeye"), "Web", TimeSpan.FromHours(4), new DateTime(2015, 5, 15));
			_credential.ExtendedProperties.Add("User.Avatar", ":001:");
			_credential.ExtendedProperties.Add("User.Nickname", "钟少");
			_credential.ExtendedProperties.Add("User.Gender", Zongsoft.Tests.Gender.Male);
		}

		[TestMethod]
		public void ToDictionaryTest()
		{
			//var dictionary = _credential.ToDictionary();

			//Assert.AreEqual("10012001", dictionary["CertificationId"]);
			//Assert.AreEqual("Web", dictionary["Scene"]);
			//Assert.AreEqual(101, dictionary["User.UserId"]);
			//Assert.AreEqual("Popeye", dictionary["User.Name"]);
			//Assert.AreEqual(new DateTime(2015, 5, 15), dictionary["IssuedTime"]);
			//Assert.AreEqual(TimeSpan.FromHours(4), dictionary["Duration"]);

			//Assert.AreEqual(":001:", dictionary["ExtendedProperties.User.Avatar"]);
			//Assert.AreEqual("钟少", dictionary["ExtendedProperties.User.Nickname"]);
			//Assert.AreEqual(Zongsoft.Tests.Gender.Male, dictionary["ExtendedProperties.User.Gender"]);
		}

		[TestMethod]
		public void FromDictionaryTest()
		{
			//var dictionary = _credential.ToDictionary();
			//var credential = Credential.FromDictionary(dictionary);

			//Assert.AreEqual("10012001", credential.CredentialId);
			//Assert.AreEqual("Web", credential.Scene);
			//Assert.AreEqual(101, credential.User.UserId);
			//Assert.AreEqual(new DateTime(2015, 5, 15), credential.IssuedTime);
			//Assert.AreEqual(TimeSpan.FromHours(4), credential.Duration);

			//Assert.IsTrue(credential.HasExtendedProperties);
			//Assert.AreEqual(3, credential.ExtendedProperties.Count);

			//Assert.AreEqual(":001:", credential.ExtendedProperties["User.Avatar"]);
			//Assert.AreEqual("钟少", credential.ExtendedProperties["User.Nickname"]);
			//Assert.AreEqual(Zongsoft.Tests.Gender.Male, credential.ExtendedProperties["User.Gender"]);
		}
	}
}
