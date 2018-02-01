using System;
using System.Collections.Generic;

using Xunit;

namespace Zongsoft.Data
{
	public class ScopingTest
	{
		[Fact]
		public void Test()
		{
			var scoping = Scoping.Parse(null);

			Assert.Equal(0, scoping.Count);
			Assert.True(string.IsNullOrEmpty(scoping.ToString()));

			scoping.Include("UserId");
			Assert.Equal(1, scoping.Count);

			scoping.Add("*, !CreatedTime");
			Assert.Equal(3, scoping.Count);

			scoping.Add("CreatedTime");
			Assert.Equal(3, scoping.Count);

			scoping.Add("!");
			Assert.Equal(1, scoping.Count);

			var members = scoping.ToArray();

			Assert.Equal(1, members.Length);
			Assert.Equal("!", members[0]);

			members = scoping.ToArray(wildcard => this.GetEntityProperties());
			Assert.Equal(0, members.Length);

			scoping.Add("*, !CreatorId, ! createdTime");
			members = scoping.ToArray(wildcard => this.GetEntityProperties());
			Assert.True(members.Length > 1);
			Assert.True(Array.Exists(members, item => string.Equals(item, "AssetId", StringComparison.OrdinalIgnoreCase)));
			Assert.False(Array.Exists(members, item => string.Equals(item, "CreatorId", StringComparison.OrdinalIgnoreCase)));
			Assert.False(Array.Exists(members, item => string.Equals(item, "CreatedTime", StringComparison.OrdinalIgnoreCase)));
		}

		private string[] GetEntityProperties()
		{
			return new string[]
			{
				"AssetId",
				"AssetNo",
				"PlateNo",
				"Barcode",
				"Name",
				"PinYin",
				"Spec",
				"Size",
				"Kind",
				"Tags",
				"Source",
				"PointId",
				"ManufacturerId",
				"ManufacturedDate",
				"CorporationId",
				"BranchId",
				"Schema",
				"DepartmentId",
				"AssetTypeId",
				"AssetClassId",
				"Grade",
				"GradeDescription",
				"Score",
				"ScoreTimestamp",
				"Flags",
				"FlagsTimestamp",
				"Status",
				"StatusTimestamp",
				"StatusDescription",
				"PlaceId",
				"AddressId",
				"AddressDetail",
				"Village",
				"Altitude",
				"Longitude",
				"Latitude",
				"Visible",
				"ActiveTime",
				"PlatedTime",
				"MeasuredValue",
				"MeasuredDescription",
				"SummaryPath",
				"AttachmentMark",
				"ResponsibleId",
				"ResponsibleName",
				"ResponsiblePhoneNumber",
				"OperatingVendorId",
				"OperatingVendorName",
				"OperatingVendorPhoneNumber",
				"ReservedData",
				"ReservedText1",
				"ReservedText2",
				"ReservedText3",
				"ReservedText4",
				"ReservedCount1",
				"ReservedCount2",
				"ReservedCount3",
				"ReservedCount4",
				"ReservedAmount1",
				"ReservedAmount2",
				"ReservedAmount3",
				"ReservedAmount4",
				"CreatorId",
				"CreatedTime",
				"Remark",
			};
		}
	}
}
