namespace Caredev.Mego.Tests.Core.Commit.Simple
{
    public partial class UpdatePropertysTest : ISimpleTest
    {
        private const string UpdateSingleObjectTestSql =
@"UPDATE [Customers]
SET [Address1] = @p0 || @p1 || @p2,
    [Name] = @p3
WHERE [Id] = @p4;
SELECT
  a.[Address1]
FROM [customers] AS a
WHERE a.[Id] = @p4;";
        
        private const string UpdateMultiObjectTestSql =
@"CREATE TEMPORARY TABLE [t$1] (
  [Address2] longtext NULL,
  [Address1] longtext NULL,
  [Name] longtext NULL,
  [Id] int NULL
);
INSERT INTO [t$1] ([Address2], [Address1], [Name], [Id])
  VALUES (@p0, @p1, @p2, @p3),
  (@p4, @p5, @p6, @p7);
UPDATE [customers] AS a
INNER JOIN [t$1] AS b
  ON a.[Id] = b.[Id]
SET a.[Address1] = CONCAT(CONCAT(b.[Address2], @p8), b.[Address1]),
    a.[Name] = b.[Name];
SELECT
  a.[Address1]
FROM [customers] AS a
  INNER JOIN [t$1] AS c
    ON a.[Id] = c.[Id];"; 

        private const string UpdateGenerateSingleObjectTestSql =
@"UPDATE [products]
SET [Category] = @p0,
    [Code] = @p1 || @p2,
    [Name] = @p1,
    [UpdateDate] = datetime('now')
WHERE [Id] = @p3;
SELECT
  a.[Code],
  a.[UpdateDate]
FROM [products] AS a
WHERE a.[Id] = @p3;";
        
        private const string UpdateGenerateMultiObjectTestSql =
@"CREATE TEMPORARY TABLE [t$1] (
  [Category] int NULL,
  [Name] longtext NULL,
  [Code] longtext NULL,
  [Id] int NULL
);
INSERT INTO [t$1] ([Category], [Name], [Code], [Id])
  VALUES (@p0, @p1, @p2, @p0),
  (@p0, @p3, @p4, @p5);
UPDATE [products] AS a
INNER JOIN [t$1] AS b
  ON a.[Id] = b.[Id]
SET a.[Category] = b.[Category],
    a.[Code] = CONCAT(b.[Name], b.[Code]),
    a.[Name] = b.[Name],
    a.[UpdateDate] = datetime('now');
SELECT
  a.[Code],
  a.[UpdateDate]
FROM [products] AS a
  INNER JOIN [t$1] AS c
    ON a.[Id] = c.[Id];"; 
    }
}