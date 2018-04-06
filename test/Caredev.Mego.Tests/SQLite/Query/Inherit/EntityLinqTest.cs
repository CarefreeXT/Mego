namespace Caredev.Mego.Tests.Core.Query.Inherit
{
    public partial class EntityLinqTest
    {
        private const string QueryListForSkipTestSql =
@"SELECT
  a.[Id],
  a.[Code],
  a.[Name],
  b.[Category],
  b.[IsValid],
  b.[UpdateDate]
FROM [productbases] AS a
  INNER JOIN [products] AS b
    ON a.[Id] = b.[Id]
ORDER BY a.[Id] ASC
LIMIT 10, 2147483647";

        private const string QueryListForTakeSkipTestSql =
@"SELECT
  a.[Id],
  a.[Code],
  a.[Name],
  b.[Category],
  b.[IsValid],
  b.[UpdateDate]
FROM [productbases] AS a
  INNER JOIN [products] AS b
    ON a.[Id] = b.[Id]
ORDER BY a.[Id] ASC
LIMIT 10, 10";

        private const string QueryListTestSql =
@"SELECT  a.[Id] ,
        a.[Code] ,
        a.[Name] ,
        b.[Category] ,
        b.[IsValid] ,
        b.[UpdateDate]
FROM    [ProductBases] AS a
        INNER JOIN [Products] AS b ON a.[Id] = b.[Id];";

        private const string QuerySinglePropertyTestSql =
        @"SELECT  a.[Code]
FROM    [ProductBases] AS a
        INNER JOIN [Products] AS b ON a.[Id] = b.[Id];";

        private const string QueryMultiPropertyTestSql =
        @"SELECT  a.[Id] ,
        a.[Name]
FROM    [ProductBases] AS a
        INNER JOIN [Products] AS b ON a.[Id] = b.[Id];";

        private const string QueryMultiPropertyDistinctTestSql =
        @"SELECT DISTINCT
        a.[State] ,
        a.[CustomerId]
FROM    [OrderBases] AS b
        INNER JOIN [Orders] AS a ON b.[Id] = a.[Id];";

        private const string QueryPropertyAndExpressionTestSql =
        @"SELECT  a.[Id] ,
        a.[Name] ,
        datetime('now') AS [Date]
FROM    [ProductBases] AS a
        INNER JOIN [Products] AS b ON a.[Id] = b.[Id];";

        private const string QueryFilterListForConstantTestSql =
        @"SELECT  a.[Id] ,
        a.[Code] ,
        a.[Name] ,
        b.[Category] ,
        b.[IsValid] ,
        b.[UpdateDate]
FROM    [ProductBases] AS a
        INNER JOIN [Products] AS b ON a.[Id] = b.[Id]
WHERE   a.[Code] = @p0;";

        private const string QueryFilterListForVariableTestSql =
        @"SELECT  a.[Id] ,
        a.[Code] ,
        a.[Name] ,
        b.[Category] ,
        b.[IsValid] ,
        b.[UpdateDate]
FROM    [ProductBases] AS a
        INNER JOIN [Products] AS b ON a.[Id] = b.[Id]
WHERE   a.[Code] = @p0;";

        private const string OrderQueryDataTestSql =
        @"SELECT  a.[Id] ,
        a.[Code] ,
        a.[Name] ,
        b.[Category] ,
        b.[IsValid] ,
        b.[UpdateDate]
FROM    [ProductBases] AS a
        INNER JOIN [Products] AS b ON a.[Id] = b.[Id]
ORDER BY b.[Category] DESC;";

        private const string CrossQueryDataTestSql =
        @"SELECT  a.[Id] ,
        a.[Name] ,
        b.[Id] AS [Id1] ,
        b.[Name] AS [Name1]
FROM    [ProductBases] AS a
        INNER JOIN [Products] AS c ON a.[Id] = c.[Id]
        CROSS JOIN [CustomerBases] AS b
        INNER JOIN [Customers] AS d ON b.[Id] = d.[Id];";

        private const string QueryListForTakeTestSql =
@"SELECT
  a.[Id],
  a.[Code],
  a.[Name],
  b.[Category],
  b.[IsValid],
  b.[UpdateDate]
FROM [productbases] AS a
  INNER JOIN [products] AS b
    ON a.[Id] = b.[Id]
LIMIT 10";

        private const string QuerySetOperateTestSql =
@"SELECT  a.[Id] ,
        a.[Code] ,
        a.[Name]
FROM    ( SELECT    b.[Id] ,
                    b.[Code] ,
                    b.[Name]
          FROM      [CustomerBases] AS b
                    INNER JOIN [Customers] AS c ON b.[Id] = c.[Id]
          UNION ALL
          SELECT    d.[Id] ,
                    d.[Code] ,
                    d.[Name]
          FROM      [ProductBases] AS d
                    INNER JOIN [Products] AS e ON d.[Id] = e.[Id]
        ) AS a;";

        private const string QueryObjectMemberTestSql =
@"SELECT
  a.[Id],
  a.[Code],
  a.[Name],
  b.[Address1],
  b.[Address2],
  b.[Zip]
FROM [orderbases] AS c
  INNER JOIN [orders] AS d
    ON c.[Id] = d.[Id]
  INNER JOIN [customerbases] AS a
  INNER JOIN [customers] AS b
    ON a.[Id] = b.[Id]
    AND d.[CustomerId] = a.[Id]
WHERE c.[Id] > @p0
AND c.[Id] < @p1";

    }
}