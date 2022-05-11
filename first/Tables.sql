﻿CREATE TABLE [dbo].[Roles]
(
	[Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    [Title] NVARCHAR(50) NOT NULL
)
CREATE TABLE [dbo].[Users]
(
	[Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	[RoleId] INT NOT NULL,
	[Login] NVARCHAR(20) NOT NULL,
	[Password] NVARCHAR(20) NOT NULL,
	[RegisterDate] DATE NOT NULL DEFAULT GETDATE(),
	CONSTRAINT FK_RoleID FOREIGN KEY(RoleID) REFERENCES Roles(Id)
)