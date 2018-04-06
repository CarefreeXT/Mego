namespace Caredev.Mego.Tests.Core.Commit.Inherit
{
    public partial class InsertTest 
    {
        private const string InsertSingleObjectTestSql =
@"INSERT  INTO [CustomerBases]
        ( [Id], [Code], [Name] )
VALUES  ( @p0, @p1, @p2 );
INSERT  INTO [Customers]
        ( [Id], [Address1], [Address2], [Zip] )
VALUES  ( @p0, @p3, @p4, @p5 );";

        private const string InsertMultiObjectTestSql =
@"INSERT  INTO [CustomerBases]
        ( [Id], [Code], [Name] )
VALUES  ( @p0, @p1, @p2 ),
        ( @p3, @p1, @p4 );
INSERT  INTO [Customers]
        ( [Id], [Address1], [Address2], [Zip] )
VALUES  ( @p0, @p5, @p6, @p7 ),
        ( @p3, @p5, @p6, @p7 );"; 

        private const string InsertExpressionSingleObjectTestSql =
@"INSERT  INTO [OrderBases]
        ( [Id], [CreateDate], [ModifyDate] )
VALUES  ( @p0, datetime('now'), @p1 );
INSERT  INTO [Orders]
        ( [Id], [CustomerId], [State] )
VALUES  ( @p0, @p2, @p3 );
SELECT  a.[CreateDate]
FROM    [OrderBases] AS a
        INNER JOIN [Orders] AS b ON a.[Id] = b.[Id]
WHERE   a.[Id] = @p0;";

        private const string InsertExpressionMultiObjectTestSql =
@"CREATE TEMPORARY TABLE [t$1] (
  [Id] int NULL,
  [CreateDate] datetime NULL,
  [ModifyDate] datetime NULL,
  [CustomerId] int NULL,
  [State] int NULL
);
INSERT INTO [t$1] ([Id], [CreateDate], [ModifyDate], [CustomerId], [State])
  VALUES (@p0, datetime('now'), @p1, @p2, @p3),
  (@p4, datetime('now'), @p1, @p2, @p5);
INSERT INTO [orderbases] ([Id], [CreateDate], [ModifyDate])
  SELECT
    a.[Id],
    a.[CreateDate],
    a.[ModifyDate]
  FROM [t$1] AS a;
INSERT INTO [orders] ([Id], [CustomerId], [State])
  SELECT
    a.[Id],
    a.[CustomerId],
    a.[State]
  FROM [t$1] AS a;
SELECT
  b.[CreateDate]
FROM [orderbases] AS b
  INNER JOIN [orders] AS c
    ON b.[Id] = c.[Id]
  INNER JOIN [t$1] AS d
    ON b.[Id] = d.[Id];"; 

        private const string InsertIdentitySingleObjectTestSql =
@"INSERT INTO [ProductBases] ([Code], [Name])
  VALUES (@p0, @p1);
SET @v0 = LAST_INSERT_ROWID();
INSERT INTO [products] ([Id], [Category], [IsValid], [UpdateDate])
  VALUES (@v0, @p2, @p3, datetime('now'));
SELECT
  a.[Id],
  b.[UpdateDate]
FROM [ProductBases] AS a
  INNER JOIN [products] AS b
    ON a.[Id] = b.[Id]
WHERE a.[Id] = @v0;";

        private const string InsertIdentityMultiObjectTestSql =
@"CREATE TEMPORARY TABLE [t$1] (
  [Id] int NULL,
  [Code] longtext NULL,
  [Name] longtext NULL,
  [Category] int NULL,
  [IsValid] bit(1) NULL,
  [UpdateDate] datetime NULL,
  [RowIndex] int NULL
);
INSERT INTO [t$1] ([Code], [Name], [Category], [IsValid], [UpdateDate], [Id], [RowIndex])
  VALUES (@p0, @p1, @p2, @p3, datetime('now'), 0, 1),
  (@p4, @p1, @p5, @p6, datetime('now'), 0, 2);
INSERT INTO [ProductBases] ([Code], [Name])
  SELECT
    a.[Code],
    a.[Name]
  FROM [t$1] AS a
  WHERE a.[RowIndex] = 1;
UPDATE [t$1]
SET [Id] = LAST_INSERT_ROWID()
WHERE [RowIndex] = 1;
INSERT INTO [ProductBases] ([Code], [Name])
  SELECT
    a.[Code],
    a.[Name]
  FROM [t$1] AS a
  WHERE a.[RowIndex] = 2;
UPDATE [t$1]
SET [Id] = LAST_INSERT_ROWID()
WHERE [RowIndex] = 2;
INSERT INTO [products] ([Id], [Category], [IsValid], [UpdateDate])
  SELECT
    a.[Id],
    a.[Category],
    a.[IsValid],
    a.[UpdateDate]
  FROM [t$1] AS a;
SELECT
  b.[Id],
  c.[UpdateDate]
FROM [ProductBases] AS b
  INNER JOIN [products] AS c
    ON b.[Id] = c.[Id]
  INNER JOIN [t$1] AS d
    ON b.[Id] = d.[Id];"; 

        private const string InsertQueryTestSql =
@"CREATE TEMPORARY TABLE [t$1] (
  [Id] int NULL,
  [Name] longtext NULL,
  [Code] longtext NULL,
  [Address1] longtext NULL
);
INSERT INTO [t$1] ([Id], [Name], [Code], [Address1])
  SELECT
    a.[Id] + @p0 AS [Id],
    a.[Name],
    a.[Name],
    a.[Code]
  FROM [productbases] AS a
    INNER JOIN [products] AS b
      ON a.[Id] = b.[Id]
  WHERE a.[Id] > @p1;
INSERT INTO [customerbases] ([Id], [Name], [Code])
  SELECT
    c.[Id],
    c.[Name],
    c.[Code]
  FROM [t$1] AS c;
INSERT INTO [customers] ([Id], [Address1])
  SELECT
    d.[Id],
    d.[Address1]
  FROM [t$1] AS d;";
    }
}