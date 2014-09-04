CREATE TABLE IF NOT EXISTS `Security.Role` (
  `ApplicationId` VARCHAR(50) NOT NULL,
  `Name` VARCHAR(50) NOT NULL,
  `FullName` VARCHAR(100) NULL,
  `Creator` VARCHAR(50) NULL,
  `CreatedTime` DATETIME NOT NULL,
  `Modifier` VARCHAR(50) NULL,
  `ModifiedTime` DATETIME NULL,
  `Remark` VARCHAR(500) NULL,
  PRIMARY KEY (`ApplicationId`, `Name`))
ENGINE = InnoDB DEFAULT CHARSET=utf8

CREATE TABLE IF NOT EXISTS `Security.User` (
  `ApplicationId` VARCHAR(50) NOT NULL,
  `Name` VARCHAR(50) NOT NULL,
  `Password` BINARY(64) NULL,
  `PasswordSalt` BINARY(8) NULL,
  `FullName` VARCHAR(100) NULL,
  `Principal` VARCHAR(100) NULL,
  `Email` VARCHAR(100) NULL,
  `Approved` TINYINT(1) NOT NULL,
  `Suspended` TINYINT(1) NOT NULL,
  `ChangePasswordOnFirstTime` TINYINT(1) NOT NULL,
  `MaxInvalidPasswordAttempts` TINYINT NULL DEFAULT 3,
  `MinRequiredPasswordLength` TINYINT NULL DEFAULT 6,
  `PasswordAttemptWindow` INT NULL DEFAULT 30,
  `PasswordExpires` DATETIME NULL,
  `PasswordQuestion` VARCHAR(50) NULL,
  `PasswordAnswer` BINARY(64) NULL,
  `Creator` VARCHAR(50) NULL,
  `CreatedTime` DATETIME NOT NULL,
  `Modifier` VARCHAR(50) NULL,
  `ModifiedTime` DATETIME NULL,
  `ApprovedTime` DATETIME NULL,
  `SuspendedTime` DATETIME NULL,
  `Remark` VARCHAR(500) NULL,
  PRIMARY KEY (`ApplicationId`, `Name`))
ENGINE = InnoDB DEFAULT CHARSET=utf8
