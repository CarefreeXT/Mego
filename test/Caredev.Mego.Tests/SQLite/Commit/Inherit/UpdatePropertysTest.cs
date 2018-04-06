namespace Caredev.Mego.Tests.Core.Commit.Inherit
{
    public partial class UpdatePropertysTest
    {
        private const string UpdateSingleObjectTestSql =
@"UPDATE [CustomerBases]
SET [Name] = @p0
WHERE [Id] = @p1;
UPDATE [Customers]
SET [Address1] = @p2 || @p3 || @p4
WHERE [Id] = @p1;
SELECT
  c.[Address1]
FROM [customerbases] AS d
  INNER JOIN [customers] AS c
    ON d.[Id] = c.[Id]
WHERE d.[Id] = @p1;";

        private const string UpdateMultiObjectTestSql =
@"CREATE TEMPORARY TABLE [t$1] (
  [Name] longtext NULL,
  [Id] int NULL,
  [Address2] longtext NULL,
  [Address1] longtext NULL
);
INSERT INTO [t$1] ([Name], [Id], [Address2], [Address1])
  VALUES (@p0, @p1, @p2, @p3),
  (@p4, @p5, @p6, @p7);
UPDATE [customerbases] AS a
INNER JOIN [t$1] AS b
  ON a.[Id] = b.[Id]
SET a.[Name] = b.[Name];
UPDATE [customers] AS c
INNER JOIN [t$1] AS d
  ON c.[Id] = d.[Id]
SET c.[Address1] = CONCAT(CONCAT(d.[Address2], @p8), d.[Address1]);
SELECT
  e.[Address1]
FROM [customerbases] AS f
  INNER JOIN [customers] AS e
    ON f.[Id] = e.[Id]
  INNER JOIN [t$1] AS g
    ON f.[Id] = g.[Id];"; 

        private const string UpdateGenerateSingleObjectTestSql =
@"UPDATE [productbases] AS a
SET a.[Code] = @p0 || @p1,
    a.[Name] = @p0
WHERE a.[Id] = @p2;
UPDATE [products] AS b
SET b.[Category] = @p3,
    b.[UpdateDate] = datetime('now')
WHERE b.[Id] = @p2;
SELECT
  c.[Code],
  d.[UpdateDate]
FROM [productbases] AS c
  INNER JOIN [products] AS d
    ON c.[Id] = d.[Id]
WHERE c.[Id] = @p2;";

        private const string UpdateGenerateMultiObjectTestSql =
@"CREATE TEMPORARY TABLE [t$1] (
  [Name] longtext NULL,
  [Code] longtext NULL,
  [Id] int NULL,
  [Category] int NULL
);
INSERT INTO [t$1] ([Name], [Code], [Id], [Category])
  VALUES (@p0, @p1, @p2, @p2),
  (@p3, @p4, @p5, @p2);
UPDATE [productbases] AS a
INNER JOIN [t$1] AS b
  ON a.[Id] = b.[Id]
SET a.[Code] = CONCAT(b.[Name], b.[Code]),
    a.[Name] = b.[Name];
UPDATE [products] AS c
INNER JOIN [t$1] AS d
  ON c.[Id] = d.[Id]
SET c.[Category] = d.[Category],
    c.[UpdateDate] = datetime('now');
SELECT
  e.[Code],
  f.[UpdateDate]
FROM [productbases] AS e
  INNER JOIN [products] AS f
    ON e.[Id] = f.[Id]
  INNER JOIN [t$1] AS g
    ON e.[Id] = g.[Id];"; 
    }
}