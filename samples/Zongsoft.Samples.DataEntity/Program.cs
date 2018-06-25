using System;
using System.Diagnostics;

namespace Zongsoft.Samples.DataEntity
{
	class Program
	{
		static void Main(string[] args)
		{
			//TestChanges();
			//Performance(10000 * 100);

			var entity = DataEntity.Build<Models.IUserEntity>();
			entity.UserId = 100;
			entity.Name = "Popeye";

			Console.ReadLine();
		}

		private static void Performance(int count)
		{
			Console.WriteLine($"To run performance {count.ToString("#,##0")} times.");
			Console.WriteLine("**********************************");

			Func<Models.UserEntity> creator = new Func<Models.UserEntity>(() => new Models.UserEntity());

			var stopwatch = new Stopwatch();

			stopwatch.Start();

			for(var i = 0; i < count; i++)
			{
				var user = new Models.User()
				{
					UserId = (uint)i,
					Avatar = ":smile:",
					Name = "Name: " + i.ToString(),
					FullName = "FullName",
					Namespace = "Zongsoft",
					Status = (byte)(i % byte.MaxValue),
					StatusTimestamp = (i % 11 == 0) ? DateTime.Now : DateTime.MinValue,
					CreatedTime = DateTime.Now,
				};
			}

			Console.WriteLine($"Native Object: {stopwatch.ElapsedMilliseconds}");

			stopwatch.Restart();

			for(var i = 0; i < count; i++)
			{
				var user = new Models.UserEntity()
				{
					UserId = (uint)i,
					Avatar = ":smile:",
					Name = "Name: " + i.ToString(),
					FullName = "FullName",
					Namespace = "Zongsoft",
					Status = (byte)(i % byte.MaxValue),
					StatusTimestamp = (i % 11 == 0) ? DateTime.Now : DateTime.MinValue,
					CreatedTime = DateTime.Now,
				};
			}

			Console.WriteLine($"DataEntity: {stopwatch.ElapsedMilliseconds}");

			stopwatch.Restart();

			for(var i = 0; i < count; i++)
			{
				var user = creator();

				user.UserId = (uint)i;
				user.Avatar = ":smile:";
				user.Name = "Name: " + i.ToString();
				user.FullName = "FullName";
				user.Namespace = "Zongsoft";
				user.Status = (byte)(i % byte.MaxValue);
				user.StatusTimestamp = (i % 11 == 0) ? DateTime.Now : DateTime.MinValue;
				user.CreatedTime = DateTime.Now;
			}

			Console.WriteLine($"DataEntity(Active): {stopwatch.ElapsedMilliseconds}");

			stopwatch.Restart();

			for(var i = 0; i < count; i++)
			{
				Models.IUserEntity user = new Models.UserEntity();

				user.TrySet("UserId", (uint)i);
				user.TrySet("Avatar", ":smile:");
				user.TrySet("Name", "Name: " + i.ToString());
				user.TrySet("FullName", "FullName");
				user.TrySet("Namespace", "Zongsoft");
				user.TrySet("Status", (byte)(i % byte.MaxValue));
				user.TrySet("StatusTimestamp", (i % 11 == 0) ? DateTime.Now : DateTime.MinValue);
				user.TrySet("CreatedTime", DateTime.Now);
			}

			Console.WriteLine($"DataEntity(TrySet): {stopwatch.ElapsedMilliseconds}");
		}

		private static void TestChanges()
		{
			Models.IUserEntity entity = new Models.UserEntity();

			Console.WriteLine($"HasChanges: {entity.HasChanges()}");

			entity.UserId = 100;
			entity.UserId = 200;
			entity.Name = "Popeye";
			entity.Namespace = "Zongsoft";
			entity.Avatar = ":smile:";
			entity.CreatedTime = DateTime.Now;

			Console.WriteLine("HasChanges(Name, Email): {0}", entity.HasChanges("Email", "Name"));

			entity.TrySet("Email", "zongsoft@qq.com");
			entity.TrySet("PhoneNumber", "+86.13812345678");

			Console.WriteLine($"HasChanges: {entity.HasChanges()}");
			DisplayChanges(entity);
		}

		private static void DisplayChanges(Zongsoft.Data.IDataEntity entity)
		{
			int index = 0;

			foreach(var entry in entity.GetChanges())
			{
				Console.WriteLine($"\t[{index++}] {entry.Key} = {entry.Value}");
			}
		}
	}
}
