# Zongsoft 开发框架核心类库

![license](https://img.shields.io/github/license/Zongsoft/Zongsoft.CoreLibrary) ![download](https://img.shields.io/nuget/dt/Zongsoft.CoreLibrary) ![version](https://img.shields.io/github/v/release/Zongsoft/Zongsoft.CoreLibrary?include_prereleases) ![github stars](https://img.shields.io/github/stars/Zongsoft/Zongsoft.CoreLibrary?style=social)

README: [English](https://github.com/Zongsoft/Zongsoft.CoreLibrary/blob/master/README.md) | [简体中文](https://github.com/Zongsoft/Zongsoft.CoreLibrary/blob/master/README-zh_CN.md)

-----

[Zongsoft.CoreLibrary](https://github.com/Zongsoft/Zongsoft.CoreLibrary) 是 [**Zongsoft**](https://github.com/Zongsoft) 开源项目集的核心类库，除 .NET 核心库外不依赖任何其他类库。


<a name="structure"></a>
## 目录结构

```
src
├── Collection
├── Common
├── Communication
│   └── Composition
├── ComponentModel
├── Data
│   └── Metadata
├── Diagnostics
│   └── Configuration
├── Expressions
│   └── Tokenization
├── IO
├── Messaging
├── Options
│   ├── Configuration
│   └── Profiles
├── Reflection
│   └── Expressions
├── Runtime
│   ├── Caching
│   └── Serialization
├── Security
│   └── Membership
├── Services
│   └── Commands
├── Terminals
│   └── Commands
├── Text
│   └── Evaluation
├── Transactions
└── Transitions
```

<a name="function"></a>
## 功能说明

下面将按命名空间(目录结构)进行概要说明。

### Collections
该命名空间内包括有关集合的类。

### Common
该命名空间内包括一些常用的工具类。其中包括相对 .NET BCL 中进行了功能强化的 `Convert`, `Randomizer` 类，对枚举类型操作的 `EnumUtility` 类，`ISequence`接口，以及 `StringExtension`, `TeypExtension`, `UriExtension` 等扩展类。

### Communication
该命名空间内包括进行通讯和自定义通讯协议包解析的基类、接口，设计通讯处理程序时应尽量使用这里定义的接口或基类。具体实现请参考 [Zongsoft.Net](https://github.com/Zongsoft/Zongsoft.Net) 项目。

#### Communication.Composition
该命名空间内包括“执行管道(ExecutionPipelines)”模式的全部接口和实现，执行管道是一套强大的扩展接口方案，通讯层的 `Communication.Net.TcpServer` 和 `Communication.Net.FtpServer` 类均采用该机制来保证服务器端的扩展性。

### ComponentModel
该命名空间内包括一些相对 .NET BCL 中进行了功能强化的 `TypeConverter`，譬如：`EnumConverter`、`CustomBooleanConverter`、`GuidConverter` 类等。

### Data
该命名空间内包括进行数据访问相关类和接口，我们提供了一个 **ORM** 数据引擎，有关这个它的更多内容请访问 [**Zongsoft.Data**](https://github.com/Zongsoft/Zongsoft.Data)。

### Diagnostics
该命名空间内包括日志处理、诊断跟踪相关的类和接口。

### Expressions
该命名空间内包括一个表达式解析以及一个类C#脚本的词法解析等功能实现（语法解析待实现）。

### IO
该命名空间内包括一个虚拟文件目录系统的功能集，使用该虚拟文件系统可隔离不同操作系统中 **IO** 处理的差异，并支持其他外部文件系统的扩展。具体实现可参考 [Zongsoft.Externals.Aliyun](https://github.com/Zongsoft/Zongsoft.Externals.Aliyun) 这个项目中的分布式文件存储部分。

### Messaging
该命名空间内包含一个消息队列处理的抽象接口，具体实现可参考 [Zongsoft.Externals.Aliyun](https://github.com/Zongsoft/Zongsoft.Externals.Aliyun) 这个项目中的消息队列部分。

### Options
该命名空间内包含了一套选项配置处理的类和接口，这套选项配置以树型结构来组织应用内的所有选项配置数据，访问这些配置数据以通用的逻辑路径的方式来进行。

#### Options.Configuration
该命名空间内包括一套全新的配置文件的开发接口，该套接口完全兼容 .NET BCL 中的 `System.Configuration` 的编程模式。

> 为什么我们要重新编写一套类似的配置开发接口？因为 .NET BCL 自带的配置的机制太过臃肿复杂、并且扩展性也不是太友好，我们希望应用模块的配置应该和该应用模块一样是可被插件化的，它们必须可随意插拔并且保证模块之间的隔离性，当不同模块被组装在一起的时候，这些分离的选项配置数据将自动组织成一个完整的逻辑树。

#### Options.Profiles
该命名空间内包括一套对 Windows 中 INI 配置文件的开发接口，并且支持对 `Section` 以层次结构的访问方式。

### Reflection
该命名空间内包括一个高性能的反射器以及访问路径表达式的解析套件。

### Runtime
#### Runtime.Caching
该命名空间内包含 `Buffer` 和 `Cache` 这两种缓存机制的功能集。

> 1. `BufferManager` 提供了在频繁分配不确定大小的内存片段的场景下的一个很好的解决方案，譬如在 TCP 通讯中，接收端并发的收到各个发送端发送过来的数据片段，可以采用 `BufferManager` 来将这些临时数据片段保存起来待到整个数据包接收完成后再反馈给上层应用完整的数据包。
> 
> 2. `ICache` 表示操作缓存的接口，`MemoryCache` 是它的一个内存缓存的实现，远程缓存案例可参考 [Zongsoft.Externals.Redis](https://github.com/Zongsoft/Zongsoft.Externals.Redis) 项目。

#### Runtime.Serialization
该命名空间内包括了一套序列化和反序列化的相关类和接口，由于 .NET BCL 中并没有提供关于序列化器的统一接口，所以使用 `ISerializer` 这个接口可以隔离特定技术的实现，譬如 `Serializer` 类的 `Json` 属性可获得一个文本序列化器，有关 JSON 序列化的实现可参考 [Zongsoft.Externals.Json](https://github.com/Zongsoft/Zongsoft.Externals.Json) 项目。。

### Security
该命名空间包括凭证、验证码以及安全、授权相关的基类和接口。

#### Security.Membership
该命名空间内包括一套完整的基于角色安全的授权管理接口，它还包含了一个最佳实践的方案。具体实现请参考 [Zongsoft.Security](https://github.com/Zongsoft/Zongsoft.Security) 项目。

### Services
该命名空间内包括一套服务访问和管理的 `IServiceProvider`、`IServiceProviderFactory` 接口和实现 `ServiceProvider`、`ServiceProviderFactory`；以及一套有关 `ICommand` 命令模式的接口和实现；还有一个后台服务的工作者 `IWorker` 接口和 `WorkerBase` 基类。

#### Services.Commands
该命名空间内包括后台服务命令的实现类，譬如 `WorkerCommandBase`、`WorkerStartCommand`、`WorkerStopCommand` 等类。

### Terminals
该命名空间内包括一套终端程序的接口和实现，使用该实现可以快速的完成一个强大的基于控制台的应用。

#### Commands
该命名空间内包括关于终端程序的一些常用命令的实现类，譬如 `ExitCommand`、`ClearCommand`、`HelpCommand` 等类。

### Text
该命名空间内包括一套基于正则表达式的文本表达式的解析、处理的类。

### Transactions
该命名空间内包括有关事务的类和接口，有关应用事务支持的实现可参考 [Zongsoft.Data 数据引擎](https://github.com/Zongsoft/Zongsoft.Data) 中的事务支持。

### Transitions
该命名空间内包括一个轻量级的通用状态机的接口和实现类。


<a name="contribution"></a>
## 贡献

请不要在项目的 **I**ssues 中提交询问(**Q**uestion)以及咨询讨论，**I**ssue 是用来报告问题(**B**ug)和功能特性(**F**eature)。如果你希望参与贡献，欢迎提交 代码合并请求(_[**P**ull**R**equest](https://github.com/Zongsoft/Zongsoft.CoreLibrary/pulls)_) 或问题反馈(_[**I**ssue](https://github.com/Zongsoft/Zongsoft.CoreLibrary/issues)_)。

对于新功能，请务必创建一个功能反馈(_[**I**ssue](https://github.com/Zongsoft/Zongsoft.CoreLibrary/issues)_)来详细描述你的建议，以便我们进行充分讨论，这也将使我们更好的协调工作防止重复开发，并帮助你调整建议或需求，使之成功地被接受到项目中。

欢迎你为我们的开源项目撰写文章进行推广，如果需要我们在官网(_[http://zongsoft.com/blog](http://zongsoft.com/blog)_) 中转发你的文章、博客、视频等可通过 [**电子邮件**](mailto:zongsoft@qq.com) 联系我们。

> 强烈推荐阅读 [《提问的智慧》](https://github.com/ryanhanwu/How-To-Ask-Questions-The-Smart-Way/blob/master/README-zh_CN.md)、[《如何向开源社区提问题》](https://github.com/seajs/seajs/issues/545) 和 [《如何有效地报告 Bug》](http://www.chiark.greenend.org.uk/~sgtatham/bugs-cn.html)、[《如何向开源项目提交无法解答的问题》](https://zhuanlan.zhihu.com/p/25795393)，更好的问题更容易获得帮助。


<a name="sponsor"></a>
## 支持赞助

非常期待您的支持与赞助，可以通过下面几种方式为我们提供必要的资金支持：

1. 关注 **Zongsoft 微信公众号**，对我们的文章进行打赏；
2. 加入 [**Zongsoft 知识星球号**](https://t.zsxq.com/2nyjqrr)，可以获得在线问答和技术支持；
3. 如果您的企业需要现场技术支持与辅导，又或者需要特定新功能、即刻的错误修复等请[发邮件](mailto:zongsoft@qq.com)给我。

[![微信公号](https://raw.githubusercontent.com/Zongsoft/Guidelines/master/zongsoft-qrcode%28wechat%29.png)](http://weixin.qq.com/r/zy-g_GnEWTQmrS2b93rd)

[![知识星球](https://raw.githubusercontent.com/Zongsoft/Guidelines/master/zongsoft-qrcode%28zsxq%29.png)](https://t.zsxq.com/2nyjqrr)


<a name="license"></a>
## 授权协议

本项目采用 [LGPL](https://opensource.org/licenses/LGPL-2.1) 授权协议。