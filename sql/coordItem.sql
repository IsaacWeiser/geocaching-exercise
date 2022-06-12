USE [master]
GO

IF db_id('coordItem') IS NULL
  CREATE DATABASE coordItem
GO

USE [coordItem]
GO

DROP TABLE IF EXISTS [Item];
DROP TABLE IF EXISTS [Cache];
GO

CREATE TABLE [Cache] (
  [Id] int PRIMARY KEY IDENTITY(1, 1),
  [Name] nvarchar(255) NOT NULL,
  [Coordinate] nvarchar(255) NOT NULL
)
GO

CREATE TABLE [Item] (
  [Id] int PRIMARY KEY IDENTITY(1, 1),
  [Name] nvarchar(255) UNIQUE NOT NULL,
  [CacheId] int,
  [ActiveStartDate] date NOT NULL,
  [ActiveEndDate] date NOT NULL
)
GO

ALTER TABLE [Item] ADD FOREIGN KEY ([CacheId]) REFERENCES [Cache] ([Id])
GO
