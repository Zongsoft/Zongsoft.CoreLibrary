SET NAMES utf8;
SET TIME_ZONE='+08:00';

CREATE TABLE IF NOT EXISTS `Security_Role` (

  `RoleId` int NOT NULL COMMENT '主键，角色编号',
  `Namespace` VARCHAR(50) NULL COMMENT '角色所属的命名空间，该字段表示应用或组织机构的标识',
  `Name` VARCHAR(50) NOT NULL COMMENT '角色名称，该名称在所属命名空间内具有唯一性',
  `FullName` VARCHAR(50) NULL COMMENT '角色全称',
  `CreatorId` int NULL COMMENT '创建者编号',
  `CreatedTime` DATETIME NOT NULL COMMENT '创建时间',
  `ModifierId` VARCHAR(50) NULL COMMENT '最后修改者编号',
  `ModifiedTime` DATETIME NULL COMMENT '最后修改时间',
  `Description` VARCHAR(500) NULL COMMENT '描述信息',
  PRIMARY KEY (`RoleId`))
ENGINE = InnoDB DEFAULT CHARSET=utf8 COMMENT='角色表';

CREATE TABLE IF NOT EXISTS `Security_User` (
  `UserId` int NOT NULL COMMENT '主键，用户编号',
  `Namespace` VARCHAR(50) NULL COMMENT '用户所属的命名空间，该字段表示应用或组织机构的标识',
  `Name` VARCHAR(50) NOT NULL COMMENT '用户名称，该名称在所属命名空间内具有唯一性',
  `Password` BINARY(64) NULL COMMENT '用户的登录口令',
  `PasswordSalt` BINARY(8) NULL COMMENT '口令加密向量(随机数)',
  `FullName` VARCHAR(50) NULL COMMENT '用户全称',
  `Principal` VARCHAR(100) NULL COMMENT '用户对应到业务系统中的负责人',
  `Email` VARCHAR(50) NULL COMMENT '用户的电子邮箱，该邮箱地址在所属命名空间内具有唯一性',
  `PhoneNumber` VARCHAR(50) NULL COMMENT '用户的手机号码，该手机号码在所属命名空间内具有唯一性',
  `Approved` TINYINT(1) NOT NULL COMMENT '帐户是否已审核通过',
  `Suspended` TINYINT(1) NOT NULL COMMENT '帐户是否暂停使用',
  `ChangePasswordOnFirstTime` TINYINT(1) NOT NULL COMMENT '用户第一次登入是否必须变更密码',
  `MaxInvalidPasswordAttempts` TINYINT NULL DEFAULT 3 COMMENT '锁定用户前允许的无效密码或无效密码提示问题答案尝试次数',
  `MinRequiredPasswordLength` TINYINT NULL DEFAULT 6 COMMENT '密码所要求的最小长度',
  `PasswordAttemptWindow` INT NULL DEFAULT 30 COMMENT '无效密码被锁定后到下次解锁的间隔分钟数',
  `PasswordExpires` DATETIME NULL COMMENT '密码的过期时间',
  `PasswordQuestion1` VARCHAR(50) NULL COMMENT '用户的密码问答的题面(1)',
  `PasswordAnswer1` VARBINARY(64) NULL COMMENT '用户的密码问答的答案(1)',
  `PasswordQuestion2` VARCHAR(50) NULL COMMENT '用户的密码问答的题面(2)',
  `PasswordAnswer2` VARBINARY(64) NULL COMMENT '用户的密码问答的答案(2)',
  `PasswordQuestion3` VARCHAR(50) NULL COMMENT '用户的密码问答的题面(3)',
  `PasswordAnswer3` VARBINARY(64) NULL COMMENT '用户的密码问答的答案(3)',
  `CreatorId` int NULL COMMENT '创建人编号',
  `CreatedTime` DATETIME NOT NULL COMMENT '创建时间',
  `ModifierId` int NULL COMMENT '最后修改人编号',
  `ModifiedTime` DATETIME NULL COMMENT '最后修改时间',
  `ApprovedTime` DATETIME NULL COMMENT '审核通过时间',
  `SuspendedTime` DATETIME NULL COMMENT '禁用设置时间',
  `Description` VARCHAR(500) NULL COMMENT '描述信息',
  PRIMARY KEY (`UserId`))
ENGINE = InnoDB DEFAULT CHARSET=utf8 COMMENT='用户表';

CREATE TABLE IF NOT EXISTS `Security_Member` (

  `RoleId` int NOT NULL COMMENT '主键，角色编号',
  `MemberId` int NOT NULL COMMENT '主键，成员编号',
  `MemberType` TINYINT(1) NOT NULL COMMENT '主键，成员类型',
  PRIMARY KEY (`RoleId`, `MemberId`, `MemberType`))
ENGINE = InnoDB DEFAULT CHARSET=utf8 COMMENT='角色成员表';

CREATE TABLE IF NOT EXISTS `Security_Permission` (

  `MemberId` int NOT NULL COMMENT '主键，成员编号',
  `MemberType` TINYINT(1) NOT NULL COMMENT '主键，成员类型',
  `SchemaId` VARCHAR(50) NOT NULL COMMENT '授权目标的标识',
  `ActionId` VARCHAR(50) NOT NULL COMMENT '授权行为的标识',
  `Granted` TINYINT(1) NOT NULL COMMENT '是否授权(0: 表示拒绝; 1: 表示授予)',
  PRIMARY KEY (`MemberId`, `MemberType`, `SchemaId`, `ActionId`))
ENGINE = InnoDB DEFAULT CHARSET=utf8 COMMENT='权限表';

CREATE TABLE IF NOT EXISTS `Security_PermissionFilter` (

  `MemberId` int NOT NULL COMMENT '主键，成员编号',
  `MemberType` TINYINT(1) NOT NULL COMMENT '主键，成员类型',
  `SchemaId` VARCHAR(50) NOT NULL COMMENT '授权目标的标识',
  `ActionId` VARCHAR(50) NOT NULL COMMENT '授权行为的标识',
  `Filter` VARCHAR(4000) NOT NULL COMMENT '拒绝授权的过滤表达式',
  PRIMARY KEY (`MemberId`, `MemberType`, `SchemaId`, `ActionId`))
ENGINE = InnoDB DEFAULT CHARSET=utf8 COMMENT='权限表';

# COMMENT '角色名在命名空间范围内的唯一索引'
ALTER TABLE `Automao`.`Security_Role` 
ADD UNIQUE INDEX `IX_Unique_Name` (`Namespace` ASC, `Name` ASC);

# COMMENT '用户名在命名空间范围内的唯一索引'
ALTER TABLE `Automao`.`Security_User` 
ADD UNIQUE INDEX `IX_Unique_Name` (`Namespace` ASC, `Name` ASC);

# COMMENT '邮箱地址在命名空间范围内的唯一索引'
ALTER TABLE `Automao`.`Security_User` 
ADD UNIQUE INDEX `IX_Unique_Email` (`Namespace` ASC, `Email` ASC);

# COMMENT '手机号码在命名空间范围内的唯一索引'
ALTER TABLE `Automao`.`Security_User` 
ADD UNIQUE INDEX `IX_Unique_PhoneNumber` (`Namespace` ASC, `PhoneNumber` ASC);
