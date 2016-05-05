Zongsoft.CoreLibrary
====================

## 概述

Zongsoft.CoreLibrary 类库提供了.NET开发的常用功能集以及一些相对于.NET BCL中进行了功能强化的实现。采用C#语言开发，支持跨平台可运行在 Windows、Linux、Mac OSX 等平台中。

-----

**欢迎大家在 [GitHub](https://githug.com/Zongsoft) 上提交反馈给我们，为我们点赞(Star)。**

**如果你愿意帮助我们完善、翻译文档，或写范例代码都请致信给我：<zongsoft@gmail.com>、<9555843@qq.com>**


## 项目结构

- Common
	> 该命名空间内包括一些常用的工具类。其中包括相对 .NET BCL 中进行了功能强化的 `Convert` 类，对枚举类型操作的 `EnumUtility` 类，`ISequence`序列器、`IAccumulator`累加器接口以及一些扩展类。

- Collections
	> 该命名空间内包括有关集合的类。其中包括相对.NET BCL中同名集合类进行了功能强化的 `NamedCollectionBase`、`Collection<T>`、`Queue` 类，以及表示树型层次结构的 `HierarchicalNode`、`HierarchicalNodeCollection`、`Category`、`CategoryCollection` 这些类，以及一个支持线程安全的提供对象池管理的 `ObjectPool` 类和支持指定容积的内存缓存`ObjectCache`。

- Communication
	> 该命名空间内包括进行通讯和自定义通讯协议包解析的基类、接口，设计通讯处理程序时应尽量使用这里定义的接口或基类。

	- Net
		> 该命名空间内包括进行TCP通讯处理的默认实现类，这些默认实现采用的是 .NET Socket 提供的异步事件处理模型机制，在 Windows 平台中由底层 WinSocket 提供IOCP支持。

		- Ftp
 			> 该命名空间内包括重新实现的一套 FTP 服务器类库，支持 FTP 数据传输的“主动”和“被动”两种模式的完整实现。为什么我们要重新实现一个 FTP 服务器？因为我们需要在FTP服务器收到任何命令时执行业务系统中的某些事务，而市面上的第三方FTP服务器似乎都没有提供这种灵活而高效的扩展机制。

			> 当然，如果您需要利用这些扩展机制的话，那么请先参考 `Services.Composition` 命令空间的那些类，我们称这套扩展机制为“执行管道(ExecutionPipelines)”模型。

- ComponentModel
	> 该命名空间内包括一些相对 .NET BCL 中进行了功能强化的 `TypeConverter`，譬如：`EnumConverter`、`CustomBooleanConverter`、`GuidConverter` 类；表示应用程序上下文的 `ApplicationContextBase` 类，该基类提供了一个应用程序可能会使用到的常用服务；其中的 `AliasAttribute` 类可用来定义枚举项或其他元素的别称。

- Data
	> 该命名空间内包括进行数据访问相关类和接口，我们提供了一个支持多库同时访问、横向分表的分布式关系型数据库ORM引擎，有关这个引擎的详细信息请访问 **[Zongsoft.Data](https://github.com/Zongsoft/Zongsoft.Data)** 项目。

- Diagnostics
	> 该命名空间内包括日志处理、诊断跟踪相关的类和接口。

- IO
	> 该命名空间内包括一个虚拟文件目录系统的功能集，使用该虚拟文件系统可隔离不同操作系统中IO处理的差异，并支持其他外部文件系统的扩展。具体实现可参考 [Zongsoft.Externals.Aliyun](https://github.com/Zongsoft/Zongsoft.Externals.Aliyun) 这个项目中的分布式文件存储部分。

- Messaging
	> 该命名空间内包含一个消息队列处理的抽象接口，具体实现可参考 [Zongsoft.Externals.Aliyun](https://github.com/Zongsoft/Zongsoft.Externals.Aliyun) 这个项目中的消息队列部分。

- Options
	> 该命名空间内包含了一套选项配置处理的类和接口，这套选项配置以树型结构来组织应用内的所有选项配置数据，访问这些配置数据以通用的逻辑路径的方式来进行。

	- Configuration
		> 该命名空间内包括一套全新的配置文件的开发接口，该套接口完全兼容 .NET BCL 中的 `System.Configuration` 的编程模式。

		> 为什么我们要重新编写一套类似的配置开发接口？因为 .NET BCL 自带的配置的机制太过臃肿复杂、并且扩展性也不是太友好，我们希望应用模块的配置应该和该应用模块一样是可被插件化的，它们必须可随意插拔并且保证模块之间的隔离性，当不同模块被组装在一起的时候，这些分离的选项配置数据将自动组织成一个完整的逻辑树。

	- Profiles
		> 该命名空间内包括一套对 Windows 中 INI 配置文件的开发接口，并且支持对 `Section` 以层次结构的访问方式。

- Reporting
	> 该命名空间内包括关于对报表访问、操作的基础接口。

- Resources
	> 该命名空间内包括一个对资源处理的 `ResourceUtility` 工具类。

- Runtime
	- Caching
		> 该命名空间内包含 `Buffer` 和 `Cache` 这两种缓存机制的功能集。

		> 1. `BufferManager` 提供了在频繁分配不确定大小的内存片段的场景下的一个很好的解决方案，譬如在 TCP 通讯中，接收端并发的收到各个发送端发送过来的数据片段，可以采用 `BufferManager` 来将这些临时数据片段保存起来待到整个数据包接收完成后再反馈给上层应用完整的数据包。

		> 2. `ICache` 表示操作缓存的接口，`MemoryCache` 是它的一个内存缓存的实现，远程缓存案例可参考 [Zongsoft.Externals.Redis](https://github.com/Zongsoft/Zongsoft.Externals.Redis) 项目。

	- Serialization
		> 该命名空间内包括了一套序列化和反序列化的相关类和接口，其中包括基于字典(`Dictionary`)的文本这两种常用序列化实现。由于 .NET BCL 中并没有提供关于序列化器的统一接口，所以使用 `ISerializer` 这个接口可以隔离特定技术的实现。通过 `Serializer` 类的 `Json` 属性可获得一个文本序列化器；通过 `DictionarySerializer` 类的 `Default` 属性可获得一个字典序列化器。

- Security
	> 该命名空间内包括一个 `PasswordUtility` 密码操作的工具类，以及与安全、授权相关的基类和接口。

	- Membership
		> 该命名空间内包括一套完整的基于角色安全的授权管理接口，它还包含了一个最佳实践的方案。具体实现请参考 [Zongsoft.Security](https://github.com/Zongsoft/Zongsoft.Security) 项目。

- Services
	> 该命名空间内包括一套服务访问和管理的 `IServiceProvider`、`IServiceProviderFactory` 接口和实现 `ServiceProvider`、`ServiceProviderFactory`；以及一套有关命令装配模式的接口和实现；还有一个后台服务的工作者 `IWorker` 接口和 `WorkerBase` 基类。

	- Composition
		> 该命名空间内包括“执行管道(ExecutionPipelines)”模式的全部接口和实现，执行管道是一套强大的扩展接口方案，通讯层的 `Communication.Net.TcpServer` 和 `Communication.Net.FtpServer` 类均采用该机制来保证服务器端的扩展性。

- Terminals
	> 该命名空间内包括一套终端程序的接口和实现，使用该实现可以快速的完成一个强大的基于控制台的应用。

	- Commands
		> 该命名空间内包括关于终端程序的一些常用命令的实现类，譬如 `ExitCommand`、`ClearCommand`、`HelpCommand` 等类。

- Text
	> 该命名空间内包括一套基于正则表达式的文本表达式的解析、处理的类。

- Transactions
	> 该命名空间内包括有关事务的类和接口，有关应用事务支持的实现可参考 [Zongsoft.Data 数据引擎](https://github.com/Zongsoft/Zongsoft.Data) 中的事务支持。


## 引用说明

本项目中的所有代码均未参考过任何特定实现，特此声明！

## 授权协议

- Zongsoft.CoreLibrary 是基于 [LGPL v2.1授权协议](http://www.gnu.org/licenses/lgpl-2.1.html)。
- 您可以将本项目应用于商业活动中，但是**必须确保对本项目的完整（含版权声明）引用，不要分割或部分引用该项目的源码**，我们保留追究违反授权协议的权利。

------------

## 为什么要开源？

**但求滴水之源敢汇大海之心**。
