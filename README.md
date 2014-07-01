Zongsoft.CoreLibrary
====================

The core library of [Zongsoft](http://www.zongsoft.com) corporation, with C#, cross-platform, support Windows/Windows Phone 8.x & Linux based [Mono](http://www.mono-project.com).

## 概述

Zongsoft.CoreLibrary 是 [Zongsoft](http://www.zongsoft.com) Corporation 的核心开发库，采用C#语言开发，支持跨平台可运行在 Windows、Windows Phone 8.x 以及 Linux([Mono](http://www.mono-project.com)) 等平台中。

-----

我们会优先解决在 Linux(CnetOS 6.x X64/Mono 3.4) 平台出现的问题，请在 [GitHub](https://githug.com/Zongsoft) 提交问题给我们，更期待各位大牛加入我们。


**如果你愿意帮助我们完善、翻译文档，或写范例代码都请致信给我：<zongsoft@gmail.com>、<sw515@21cn.com>**

## 项目结构

- Common
	> 该命名空间内包括一些常用的工具类。其中包括相对 .NET BCL 中进行了功能强化的 `Convert` 类，以及对枚举类型操作的 `EnumUtility` 类，以及一个支持线程安全的提供对象池管理的 `ObjectPool` 类。

- Collections
	> 该命名空间内包括有关集合的类。其中包括相对.NET BCL中同名集合类进行了功能强化的 `NamedCollectionBase`、`Collection<T>`、`Queue` 类，以及表示树型层次结构的 `HierarchicalNode`、`HierarchicalNodeCollection`、`Category`、`CategoryCollection` 这些类。

	> 当然，不排除在未来的某个时间我们会为它们添加并发处理、线程安全的支持，或者让 `Queue` 内置内存映射文件机制的持久化，当然所有这些功能特性(features)必须是易于扩展和可替换的。

- Communication
	> 该命名空间内包括进行通讯和自定义通讯协议包解析的基类、接口，设计通讯处理程序时应尽量使用这里定义的接口或基类。

	- Net
		> 该命名空间内包括进行TCP通讯处理的默认实现类，这些默认实现采用的是 .NET Socket 提供的异步事件处理模型机制，在 Windows 平台中由底层 WinSocket 提供IOCP支持，而在 Linux 平台中由于目前版本的 Mono 3.4 尚未完成对 epoll 模式的实现，因此其底层采用的是 select 模式的实现机制。

		> 如果你对该实现在 Linux 平台没采取 epoll 模式感到沮丧的话，那要么提请 Mono 团队改善 Socket 异步事件处理模型的实现；要么和我们一起来实现一个针对 Linux 平台采用 epoll 模式的 Socket 异步事件处理模型的特定实现。

		- Ftp
 			> 该命名空间内包括重新实现的一套 FTP 服务器类库，支持 FTP 数据传输的“主动”和“被动”两种模式的完整实现。为什么我们要重新实现一个 FTP 服务器？因为我们需要在FTP服务器收到任何命令时执行业务系统中的某些事务，而市面上的第三方FTP服务器似乎都没有提供这种灵活而高效的扩展机制。

			> 当然，如果您需要利用这些扩展机制的话，那么请先参考 `Services.Composition` 命令空间的那些类，我们称这套扩展机制为“执行管道(ExecutionPipelines)”模型。

- ComponentModel
	> 该命名空间内包括一些相对 .NET BCL 中进行了功能强化的 `TypeConverter`，譬如：`EnumConverter`、`CustomBooleanConverter`、`GuidConverter` 类；还有表示应用功能模块的 `Schema` 以及对应的 `SchemaCollection`、`SchemaCategory`、`SchemaCategoryCollection`、`SchemaProvider` 类和 `ISchemaProvider` 接口；表示应用模块的行为的 `Action` 和 `ActionCollection` 类；表示应用程序上下文的 `ApplicationContextBase` 类，该基类提供了一个应用程序可能会使用到的常用服务；其中的 `AliasAttribute` 类可用来定义枚举项的别称，`EnumConverter` 和 `Common.EnumUtility` 类将会应用枚举项的别称。

- Data
	> 该命名空间内包括进行数据访问相关类和接口，我们提供了一个支持多库同时访问、横向分表的分布式关系型数据库ORM引擎，有关这个引擎的详细信息请访问 **[Zongsoft.Data](https://github.com/Zongsoft/Zongsoft.Data)** 项目。

	- Entities
		> 该命名空间内包括 ORM 数据访问的应用层接口。

- Diagnostics
	> 该命名空间内包括诊断跟踪、异常处理的类和接口。

- IO
	> 该命名空间内包括一个IO路径处理的 `PathUtility` 工具类，以及抽象出来的文件和目录访问的接口：`IFile`、`IDirectory`。

- Options
	> 该命名空间内包含了一套选项配置处理的类和接口，这套选项配置以树型结构来组织应用内的所有选项配置数据，访问这些配置数据以通用的逻辑路径的方式来进行。

	- Configuration
		> 该命名空间内包括一套全新的配置文件的开发接口，该套接口完全兼容 .NET BCL 中的 `System.Configuration` 的编程模式。

		> 为什么我们要重新编写一套类似的配置开发接口？因为 .NET BCL 自带的配置的机制太过臃肿复杂、并且扩展性也不是太友好，我们希望应用模块的配置应该和该应用模块一样是可被插件化的，它们必须可随意插拔并且保证模块之间的隔离性，当不同模块被组装在一起的时候，这些分离的选项配置数据将自动组织成一个完整的逻辑树。

- Reporting
	> 该命名空间内包括关于对报表访问、操作的基础接口。

- Resources
	> 该命名空间内包括一个对资源处理的 `ResourceUtility` 工具类。

- Runtime
	- Caching
		> 该命名空间内包括一个 `IDictionaryCache` 字典缓存的接口和基于内存缓存的 `MemoryDictionaryCache` 实现类，以及一个 `IBufferManager` 缓存管理的接口和基于 MemoryMappedFile(内存映射文件)技术的 `BufferManager` 实现类。

		> `BufferManager` 提供了在频繁分配不确定大小的内存片段的场景下的一个很好的解决方案，譬如在 TCP 通讯中，接收端并发的收到各个发送端发送过来的数据片段，可以采用 `BufferManager` 来将这些临时数据片段保存起来待到整个数据包完全接收完成后再通知上层应用接收完成。

	- Serialization
		> 该命名空间内包括了一套序列化和反序列化的相关类，因为 .NET BCL 中并没有提供关于序列化器的统一接口，所以使用 `ISerializer` 这个接口可以隔离特定技术的实现。

- Security
	> 该命名空间内包括一个 `PasswordUtility` 密码操作的工具类，以及与安全、授权相关的基类和接口。

	- Membership
		> 该命名空间内包括一套完整的基于角色安全的授权管理接口，它还包含了一个最佳实践的方案。

- Services
	> 该命名空间内包括一套服务访问和管理的 `IServiceProvider`、`IServiceProviderFactory` 接口和实现 `ServiceProvider`、`ServiceProviderFactory`；以及一套有关命令装配模式的接口和实现；还有一个后台服务的工作者 `IWorker` 接口和 `WorkerBase` 基类。

	- Composition
		> 该命名空间内包括“执行管道(ExecutionPipelines)”模式的全部接口和实现，执行管道是一套强大的扩展接口方案，通讯层的 `Communication.Net.TcpServer` 和 `Communication.Net.FtpServer` 类均采用该机制来保证服务器端的扩展性。

- Terminals
	> 该命名空间内包括一套终端程序的接口和实现，使用该实现可以快速的完成一个强大的基于控制台的应用。

	- Commands
		> 该命名空间内包括关于终端程序的一些常用命令的实现类，譬如 `ExitCommand`、`ClearCommand`、`HelpCommand` 等类。

- Text
	> 该命名空间内包括一套基于正则表达式的文本表达式的解析、处理的类，我们在 [Zongsoft.Web](https://github.com/Zongsoft/Zongsoft.Web) 项目的视图引擎部分正是使用该文本表达式作为其数据绑定表达式的技术基础。

## 引用说明

本项目中的 `Common.UrlUtility` 和 `Collections.Collection<T>` 类的代码参考了[微软](http://www.microsoft.com).NET BCL中相应类的实现，如果遇有版权问题请通知我们，我们将再重新实现这部分代码。
除此之外本项目中的其他代码均未参考过任何特定实现，特此声明！

## 授权协议

- Zongsoft.CoreLibrary 是基于 [LGPL v2.1授权协议](http://www.gnu.org/licenses/lgpl-2.1.html)。
- Zongsoft.Data 是基于 [LGPL v2.1授权协议](http://www.gnu.org/licenses/lgpl-2.1.html)。
- Zongsoft.Plugins 是基于 [LGPL v2.1授权协议](http://www.gnu.org/licenses/lgpl-2.1.html)。
- Zongsoft.Terminals 是基于 [LGPL v2.1授权协议](http://www.gnu.org/licenses/lgpl-2.1.html)。
- Zongsoft.Web 是基于 [LGPL v2.1授权协议](http://www.gnu.org/licenses/lgpl-2.1.html)。

------------

**为什么要开源？**

**不求逆变但求滴水之源敢汇大海之心**。
