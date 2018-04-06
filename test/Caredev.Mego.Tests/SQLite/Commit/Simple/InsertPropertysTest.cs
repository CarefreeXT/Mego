namespace Caredev.Mego.Tests.Core.Commit.Simple
{
    public partial class InsertPropertysTest
    {

        private const string InsertSingleObjectTestSql =
@"INSERT INTO [Customers] ([Id], [Address1], [Name])
  VALUES (@p0, @p1 || @p2, @p3 || @p1);
SELECT
  a.[Address1],
  a.[Name]
FROM [customers] AS a
WHERE a.[Id] = @p0;";

        private const string InsertMultiObjectTestSql =
@"CREATE TEMPORARY TABLE [t$1] (
  [Id] INTEGER NULL,
  [Address1] TEXT NULL,
  [Address2] TEXT NULL,
  [Code] TEXT NULL,
  [Name] TEXT NULL,
  [Zip] TEXT NULL
);
INSERT INTO [t$1] ([Id], [Address1], [Name])
  VALUES (@p0, @p1 || @p2, @p3 || @p1),
  (@p4, @p1 || @p2, @p5 || @p1);
INSERT INTO [Customers] ([Id], [Address1], [Name])
  SELECT
    a.[Id],
    a.[Address1],
    a.[Name]
  FROM [t$1] AS a;
SELECT
  b.[Address1],
  b.[Name]
FROM [Customers] AS b
  INNER JOIN [t$1] AS c
    ON b.[Id] = c.[Id];"; 

        private const string InsertIdentitySingleObjectTestSql =
@"CREATE TEMPORARY TABLE [t$1] (
    [Id] INTEGER NULL, 
    [Category] INTEGER NULL, 
    [Code] TEXT NULL, 
    [IsValid] NUMERIC NULL, 
    [Name] TEXT NULL, 
    [UpdateDate] NUMERIC NULL, 
    [RowIndex] INTEGER NULL
);
INSERT INTO [t$1] ([Category], [Code], [IsValid], [Name], [UpdateDate], [Id], [RowIndex])
  VALUES (@p0, @p1, @p2, @p3 || @p4, datetime('now'), 0, 1);
INSERT INTO [Products] ([Category], [Code], [IsValid], [Name], [UpdateDate])
  SELECT
    a.[Category],
    a.[Code],
    a.[IsValid],
    a.[Name],
    a.[UpdateDate]
  FROM [t$1] AS a
  WHERE a.[RowIndex] = 1;
UPDATE [t$1]
SET [Id] = LAST_INSERT_ROWID()
WHERE [RowIndex] = 1;
SELECT
  b.[Id],
  b.[Name],
  b.[UpdateDate]
FROM [products] AS b
  INNER JOIN [t$1] AS c
    ON b.[Id] = c.[Id];";

        private const string InsertIdentityMultiObjectTestSql =
@"CREATE TEMPORARY TABLE [t$1] (
    [Id] INTEGER NULL, 
    [Category] INTEGER NULL, 
    [Code] TEXT NULL, 
    [IsValid] NUMERIC NULL, 
    [Name] TEXT NULL, 
    [UpdateDate] NUMERIC NULL, 
    [RowIndex] INTEGER NULL
);
INSERT INTO [t$1] ([Category], [Code], [IsValid], [Name], [UpdateDate], [Id], [RowIndex])
  VALUES (@p0, @p1, @p2, @p3 || @p4, datetime('now'), 0, 1),
  (@p5, @p6, @p7, @p3 || @p4, datetime('now'), 0, 2);
INSERT INTO [Products] ([Category], [Code], [IsValid], [Name], [UpdateDate])
  SELECT
    a.[Category],
    a.[Code],
    a.[IsValid],
    a.[Name],
    a.[UpdateDate]
  FROM [t$1] AS a
  WHERE a.[RowIndex] = 1;
UPDATE [t$1]
SET [Id] = LAST_INSERT_ROWID()
WHERE [RowIndex] = 1;
INSERT INTO [products] ([Category], [Code], [IsValid], [Name], [UpdateDate])
  SELECT
    a.[Category],
    a.[Code],
    a.[IsValid],
    a.[Name],
    a.[UpdateDate]
  FROM [t$1] AS a
  WHERE a.[RowIndex] = 2;
UPDATE [t$1]
SET [Id] = LAST_INSERT_ROWID()
WHERE [RowIndex] = 2;
SELECT
  b.[Id],
  b.[Name],
  b.[UpdateDate]
FROM [products] AS b
  INNER JOIN [t$1] AS c
    ON b.[Id] = c.[Id];"; 

        private const string InsertExpressionSingleObjectTestSql =
@"INSERT  INTO [Orders]
        ( [Id] ,
          [CreateDate] ,
          [CustomerId] ,
          [ModifyDate] ,
          [State]
        )
VALUES  ( @p0 ,
          datetime('now') ,
          @p1 ,
          datetime('now') ,
          @p2
        );
SELECT  a.[CreateDate] ,
        a.[CustomerId] ,
        a.[ModifyDate]
FROM    [Orders] AS a
WHERE   a.[Id] = @p0;";

        private const string InsertExpressionMultiObjectTestSql =
@"CREATE TEMPORARY TABLE [t$1] (
    [Id] INTEGER NULL, 
    [CreateDate] NUMERIC NULL, 
    [CustomerId] INTEGER NULL, 
    [ModifyDate] NUMERIC NULL, 
    [State] INTEGER NULL
);
INSERT INTO [t$1] ([Id], [CreateDate], [CustomerId], [ModifyDate], [State])
  VALUES (@p0, datetime('now'), @p1, datetime('now'), @p2),
  (@p3, datetime('now'), @p1, datetime('now'), @p4);
INSERT INTO [orders] ([Id], [CreateDate], [CustomerId], [ModifyDate], [State])
  SELECT
    a.[Id],
    a.[CreateDate],
    a.[CustomerId],
    a.[ModifyDate],
    a.[State]
  FROM [t$1] AS a;
SELECT
  b.[CreateDate],
  b.[CustomerId],
  b.[ModifyDate]
FROM [orders] AS b
  INNER JOIN [t$1] AS c
    ON b.[Id] = c.[Id];"; 
    }
}