using System;
using System.Collections.Generic;

namespace Zongsoft.Collections
{
	public static class HashSetExtension
	{
		public static T[] ToArray<T>(this HashSet<T> hashset)
		{
			if(hashset == null)
				return null;

			T[] result = new T[hashset.Count];
			hashset.CopyTo(result);
			return result;
		}
	}
}
