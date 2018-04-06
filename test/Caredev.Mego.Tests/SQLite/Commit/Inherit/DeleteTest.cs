namespace Caredev.Mego.Tests.Core.Commit.Inherit
{
    public partial class DeleteTest
    {
        private const string DeleteSingleTestSql =
@"DELETE  a
FROM    [OrderDetails] AS a
WHERE   a.[Id] = @p0;
DELETE  b
FROM    [OrderDetailBases] AS b
WHERE   b.[Id] = @p0;";

        private const string DeleteMultiForKeyTestSql =
@"DELETE  a
FROM    [OrderDetails] AS a
WHERE   a.[Id] IN ( @p0, @p1, @p2 );
DELETE  b
FROM    [OrderDetailBases] AS b
WHERE   b.[Id] IN ( @p0, @p1, @p2 );";

        private const string DeleteMultiForKeysTestSql =
@"CREATE TEMPORARY TABLE [t$1] (
  [Id] int NULL,
  [Number] int NULL
);
INSERT INTO [t$1] ([Id], [Number])
  VALUES (@p0, @p1),
  (@p2, @p3);
DELETE a
  FROM [warehouses] AS a
    INNER JOIN [t$1] AS b
      ON a.[Id] = b.[Id]
      AND a.[Number] = b.[Number];
DELETE c
  FROM [warehousebases] AS c
    INNER JOIN [t$1] AS d
      ON c.[Id] = d.[Id]
      AND c.[Number] = d.[Number];"; 

        private const string DeleteStatementForExpressionTestSql =
@"CREATE TEMPORARY TABLE [t$1] (
  [Id] int NULL,
  [Number] int NULL
);
INSERT INTO [t$1] ([Id], [Number])
  SELECT
    a.[Id],
    a.[Number]
  FROM [warehousebases] AS a
    INNER JOIN [warehouses] AS b
      ON a.[Id] = b.[Id]
      AND a.[Number] = b.[Number]
  WHERE a.[Id] > @p0;
DELETE c
  FROM [warehouses] AS c
    INNER JOIN [t$1] AS d
      ON c.[Id] = d.[Id]
      AND c.[Number] = d.[Number];
DELETE e
  FROM [warehousebases] AS e
    INNER JOIN [t$1] AS f
      ON e.[Id] = f.[Id]
      AND e.[Number] = f.[Number];";

        private const string DeleteStatementForQueryTestSql =
@"CREATE TEMPORARY TABLE [t$1] (
  [Id] int NULL,
  [Number] int NULL
);
INSERT INTO [t$1] ([Id], [Number])
  SELECT
    a.[Id],
    a.[Number]
  FROM [warehousebases] AS a
    INNER JOIN [warehouses] AS b
      ON a.[Id] = b.[Id]
      AND a.[Number] = b.[Number]
    CROSS JOIN [customerbases] AS c
    INNER JOIN [customers] AS d
      ON c.[Id] = d.[Id]
  WHERE a.[Id] > c.[Id]
  AND a.[Number] > @p0;
DELETE e
  FROM [warehouses] AS e
    INNER JOIN [t$1] AS f
      ON e.[Id] = f.[Id]
      AND e.[Number] = f.[Number];
DELETE g
  FROM [warehousebases] AS g
    INNER JOIN [t$1] AS h
      ON g.[Id] = h.[Id]
      AND g.[Number] = h.[Number];";
    }
}