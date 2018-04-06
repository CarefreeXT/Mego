namespace Caredev.Mego.Tests.Core.Commit.Simple
{
    public partial class UpdateTest
    {
        private const string UpdateSingleObjectTestSql =
@"UPDATE [Customers]
SET [Address1] = @p0, [Address2] = @p1, [Code] = @p2, [Name] = @p3, [Zip] = @p4
WHERE [Id] = @p5;";

        private const string UpdateMultiObjectTestSql =
@"CREATE TEMPORARY TABLE [t$1] (
  [Address1] longtext NULL,
  [Address2] longtext NULL,
  [Code] longtext NULL,
  [Name] longtext NULL,
  [Zip] longtext NULL,
  [Id] int NULL
);
INSERT INTO [t$1] ([Address1], [Address2], [Code], [Name], [Zip], [Id])
  VALUES (@p0, @p1, @p2, @p3, @p4, @p5),
  (@p6, @p1, @p7, @p8, @p4, @p9);
UPDATE [customers] AS a
INNER JOIN [t$1] AS b
  ON a.[Id] = b.[Id]
SET a.[Address1] = b.[Address1],
    a.[Address2] = b.[Address2],
    a.[Code] = b.[Code],
    a.[Name] = b.[Name],
    a.[Zip] = b.[Zip];";

        private const string UpdateGenerateSingleObjectTestSql =
@"UPDATE [Products]
SET [Category] = @p0,
    [Code] = @p1,
    [IsValid] = @p2,
    [Name] = @p3,
    [UpdateDate] = datetime('now')
WHERE [Id] = @p4;
SELECT
  a.[UpdateDate]
FROM [products] AS a
WHERE a.[Id] = @p4;";
        
        private const string UpdateGenerateMultiObjectTestSql =
@"CREATE TEMPORARY TABLE [t$1] (
  [Category] int NULL,
  [Code] longtext NULL,
  [IsValid] bit(1) NULL,
  [Name] longtext NULL,
  [Id] int NULL
);
INSERT INTO [t$1] ([Category], [Code], [IsValid], [Name], [Id])
  VALUES (@p0, @p1, @p2, @p3, @p4),
  (@p0, @p5, @p6, @p7, @p8);
UPDATE [products] AS a
INNER JOIN [t$1] AS b
  ON a.[Id] = b.[Id]
SET a.[Category] = b.[Category],
    a.[Code] = b.[Code],
    a.[IsValid] = b.[IsValid],
    a.[Name] = b.[Name],
    a.[UpdateDate] = datetime('now');
SELECT
  a.[UpdateDate]
FROM [products] AS a
  INNER JOIN [t$1] AS c
    ON a.[Id] = c.[Id];"; 

        private const string UpdateQueryTestSql =
@"UPDATE [customers] AS a
CROSS JOIN [products] AS b
SET a.[Name] = b.[Name],
    a.[Code] = b.[Name],
    a.[Address1] = b.[Code]
WHERE b.[Id] > @p0";
    }
}