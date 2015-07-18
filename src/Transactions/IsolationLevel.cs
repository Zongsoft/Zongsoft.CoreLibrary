using System;

namespace Zongsoft.Transactions
{
	/// <summary>
	/// 表示事务的隔离级别。
	/// </summary>
	public enum IsolationLevel
	{
		/// <summary>可以在事务期间读取和修改可变数据，可以进行脏读。</summary>
		ReadUncommitted = 0,

		/// <summary>可以在事务期间读取可变数据，但是不可以修改它。在正在读取数据时保持共享锁，以避免脏读，但是在事务结束之前可以更改数据，从而导致不可重复的读取或幻像数据。</summary>
		ReadCommitted,

		/// <summary>可以在事务期间读取可变数据，但是不可以修改。可以在事务期间添加新数据。防止不可重复的读取，但是仍可以有幻像数据。</summary>
		RepeatableRead,

		/// <summary>可以在事务期间读取可变数据，但是不可以修改，也不可以添加任何新数据。</summary>
		Serializable,
	}
}
