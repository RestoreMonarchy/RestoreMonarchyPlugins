﻿CREATE TABLE dbo.Messages
(
	Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Messages PRIMARY KEY,
	FromUserId INT NOT NULL CONSTRAINT FK_Messages_FromUserId FOREIGN KEY REFERENCES dbo.Users(Id),
	ToUserId INT NOT NULL CONSTRAINT FK_Messages_ToUserId FOREIGN KEY REFERENCES dbo.Users(Id),
	Title NVARCHAR(255) NOT NULL,
	IsClosed BIT NOT NULL CONSTRAINT DF_Messages_IsClosed DEFAULT 0,
	ClosingUserId INT NULL CONSTRAINT FK_Messages_ClosingUserId FOREIGN KEY REFERENCES dbo.Users(Id),
	CreateDate DATETIME2(0) NOT NULL CONSTRAINT DF_Messages_CreateDate DEFAULT SYSDATETIME()
)
