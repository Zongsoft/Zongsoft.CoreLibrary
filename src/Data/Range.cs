using System;

namespace Zongsoft.Data
{
	public static class Range
	{
		#region 公共方法
		public static Range<T> Create<T>(T minimum, T maximum) where T : struct, IComparable<T>
		{
			return new Range<T>(minimum, maximum);
		}

		public static bool IsRange(object target)
		{
			if(target == null)
				return false;

			return IsRange(target.GetType());
		}

		public static bool IsRange(Type type)
		{
			if(type == null)
				throw new ArgumentNullException(nameof(type));

			if(type.IsArray)
			{
				var elementType = type.GetElementType();

				if(typeof(IComparable).IsAssignableFrom(elementType) ||
				   typeof(IComparable<>).MakeGenericType(elementType).IsAssignableFrom(elementType))
					return true;

				return false;
			}

			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Range<>);
		}

		public static void Resolve(object target, out object minimum, out object maximum)
		{
			if(target == null)
				throw new ArgumentNullException(nameof(target));

			if(!TryResolve(target, out minimum, out maximum))
				throw new ArgumentException("The specified target is not a range.");
		}

		public static bool TryResolve(object target, out object minimum, out object maximum)
		{
			minimum = null;
			maximum = null;

			if(target == null)
				return false;

			var type = target.GetType();

			if(!IsRange(type))
				return false;

			if(type.IsArray)
			{
				Array array = (Array)target;

				if(array.Length >= 2)
				{
					minimum = array.GetValue(0);
					maximum = array.GetValue(1);

					return true;
				}

				return false;
			}

			minimum = Reflection.MemberTokenProvider.Default.GetMember(target.GetType(), "Minimum")?.GetValue(target);
			maximum = Reflection.MemberTokenProvider.Default.GetMember(target.GetType(), "Maximum")?.GetValue(target);

			return minimum != null && maximum != null;
		}
		#endregion
	}
}
