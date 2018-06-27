using System;
using System.Diagnostics;

namespace Zongsoft.Samples.DataEntity
{
	class Program
	{
		private const int COUNT = 10000 * 100;

		static void Main(string[] args)
		{
			//TestChanges();
			//Performance(COUNT);
			//PerformanceDynamic(COUNT);

			var user = DataEntity.Build<Models.IUserEntity>();
			user.UserId = 100;
			user.Name = "Popeye";

			//var entities = DataEntity.Build<Models.IUserEntity>(COUNT);

			//foreach(var entity in entities)
			//{
			//	entity.UserId = 100;
			//}

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

		private static void PerformanceDynamic(int count)
		{
			var creator = DataEntity.GetCreator<Models.IUserEntity>(); //预先编译
			DataEntity.Build<Models.IUserEntity>(); //预热（预先编译）

			var stopwatch = new Stopwatch();

			stopwatch.Start();

			for(int i = 0; i < count; i++)
			{
				var user = (Models.IUserEntity)creator();

				user.UserId = (uint)i;
				user.Avatar = ":smile:";
				user.Name = "Name: " + i.ToString();
				user.FullName = "FullName";
				user.Namespace = "Zongsoft";
				user.Status = (byte)(i % byte.MaxValue);
				user.StatusTimestamp = (i % 11 == 0) ? DateTime.Now : DateTime.MinValue;
				user.CreatedTime = DateTime.Now;
			}

			//var entities = DataEntity.Build<Models.IUserEntity>(count, (entity, index) =>
			//{
			//	entity.UserId = (uint)index;
			//	entity.Avatar = ":smile:";
			//	entity.Name = "Name: " + index.ToString();
			//	entity.FullName = "FullName";
			//	entity.Namespace = "Zongsoft";
			//	entity.Status = (byte)(index % byte.MaxValue);
			//	entity.StatusTimestamp = (index % 11 == 0) ? DateTime.Now : DateTime.MinValue;
			//	entity.CreatedTime = DateTime.Now;
			//});

			//foreach(var entity in entities)
			//{
			//}

			//int index = 0;
			//var entities = DataEntity.Build<Models.IUserEntity>(count);

			//foreach(var user in entities)
			//{
			//	user.UserId = (uint)index;
			//	user.Avatar = ":smile:";
			//	user.Name = "Name: " + index.ToString();
			//	user.FullName = "FullName";
			//	user.Namespace = "Zongsoft";
			//	user.Status = (byte)(index % byte.MaxValue);
			//	user.StatusTimestamp = (index % 11 == 0) ? DateTime.Now : DateTime.MinValue;
			//	user.CreatedTime = DateTime.Now;
			//}

			//var iterator = entities.GetEnumerator();

			//while(iterator.MoveNext())
			//{
			//}

			//while(iterator.MoveNext())
			//{
			//	var user = iterator.Current;

			//	user.UserId = (uint)index;
			//	user.Avatar = ":smile:";
			//	user.Name = "Name: " + index.ToString();
			//	user.FullName = "FullName";
			//	user.Namespace = "Zongsoft";
			//	user.Status = (byte)(index % byte.MaxValue);
			//	user.StatusTimestamp = (index % 11 == 0) ? DateTime.Now : DateTime.MinValue;
			//	user.CreatedTime = DateTime.Now;

			//	//index++;
			//}

			Console.WriteLine($"Dynamic Object: {stopwatch.ElapsedMilliseconds}");

			stopwatch.Stop();
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
