using System;
using System.Collections;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zongsoft.Security
{
	[TestClass]
	public class CertificationTest
	{
		private Certification _certification;

		public CertificationTest()
		{
			_certification = new Certification("10012001", new Membership.User(101, "Popeye"), "Namespace", "Web", TimeSpan.FromHours(4), new DateTime(2015, 5, 15));
			_certification.ExtendedProperties.Add("User.Avatar", ":001:");
			_certification.ExtendedProperties.Add("User.Nickname", "钟少");
			_certification.ExtendedProperties.Add("User.Gender", Zongsoft.Tests.Gender.Male);
		}

		[TestMethod]
		public void ToDictionaryTest()
		{
			var dictionary = _certification.ToDictionary();

			Assert.AreEqual("10012001", dictionary["CertificationId"]);
			Assert.AreEqual("Namespace", dictionary["Namespace"]);
			Assert.AreEqual("Web", dictionary["Scene"]);
			Assert.AreEqual(101, dictionary["User.UserId"]);
			Assert.AreEqual("Popeye", dictionary["User.Name"]);
			Assert.AreEqual(new DateTime(2015, 5, 15), dictionary["IssuedTime"]);
			Assert.AreEqual(TimeSpan.FromHours(4), dictionary["Duration"]);

			Assert.AreEqual(":001:", dictionary["ExtendedProperties.User.Avatar"]);
			Assert.AreEqual("钟少", dictionary["ExtendedProperties.User.Nickname"]);
			Assert.AreEqual(Zongsoft.Tests.Gender.Male, dictionary["ExtendedProperties.User.Gender"]);
		}

		[TestMethod]
		public void FromDictionaryTest()
		{
			var dictionary = _certification.ToDictionary();
			var certification = Certification.FromDictionary(dictionary);

			Assert.AreEqual("10012001", certification.CertificationId);
			Assert.AreEqual("Namespace", certification.Namespace);
			Assert.AreEqual("Web", certification.Scene);
			Assert.AreEqual(101, certification.User.UserId);
			Assert.AreEqual(new DateTime(2015, 5, 15), certification.IssuedTime);
			Assert.AreEqual(TimeSpan.FromHours(4), certification.Duration);

			Assert.IsTrue(certification.HasExtendedProperties);
			Assert.AreEqual(3, certification.ExtendedProperties.Count);

			Assert.AreEqual(":001:", certification.ExtendedProperties["User.Avatar"]);
			Assert.AreEqual("钟少", certification.ExtendedProperties["User.Nickname"]);
			Assert.AreEqual(Zongsoft.Tests.Gender.Male, certification.ExtendedProperties["User.Gender"]);
		}
	}
}
