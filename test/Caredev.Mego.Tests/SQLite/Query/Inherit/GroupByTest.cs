namespace Caredev.Mego.Tests.Core.Query.Inherit
{
    public partial class GroupByTest
    {
        private const string QuerySimpleKeyAndListPageTestSql =
@"SELECT
  a.[Category],
  b.[Id],
  b.[Code],
  b.[Name],
  b.[Category] AS [Category1],
  b.[IsValid],
  b.[UpdateDate]
FROM (SELECT
    c.[Category]
  FROM (SELECT
      d.[Category]
    FROM [productbases] AS e
      INNER JOIN [products] AS d
        ON e.[Id] = d.[Id]
    GROUP BY d.[Category]) AS c
  ORDER BY c.[Category] ASC
  LIMIT 2, 2) AS a
  INNER JOIN (SELECT
      e.[Id],
      e.[Code],
      e.[Name],
      d.[Category],
      d.[IsValid],
      d.[UpdateDate]
    FROM [productbases] AS e
      INNER JOIN [products] AS d
        ON e.[Id] = d.[Id]) AS b
    ON b.[Category] = a.[Category]";

        private const string QuerySimpleKeyTestSql =
        @"SELECT  a.[Category]
FROM    ( SELECT    b.[Category]
          FROM      [ProductBases] AS c
                    INNER JOIN [Products] AS b ON c.[Id] = b.[Id]
          GROUP BY  b.[Category]
        ) AS a;";

        private const string QueryComplexKeysTestSql =
        @"SELECT  a.[Category] ,
        a.[IsValid]
FROM    ( SELECT    b.[Category] ,
                    b.[IsValid]
          FROM      [ProductBases] AS c
                    INNER JOIN [Products] AS b ON c.[Id] = b.[Id]
          GROUP BY  b.[Category] ,
                    b.[IsValid]
        ) AS a;";

        private const string QueryKeyMembersTestSql =
        @"SELECT  a.[Category] ,
        a.[IsValid]
FROM    ( SELECT    b.[Category] ,
                    b.[IsValid]
          FROM      [ProductBases] AS c
                    INNER JOIN [Products] AS b ON c.[Id] = b.[Id]
          GROUP BY  b.[Category] ,
                    b.[IsValid]
        ) AS a;";

        private const string QueryKeyAndAggregateCountTestSql =
        @"SELECT  a.[Category] ,
        a.[Count]
FROM    ( SELECT    b.[Category] ,
                    COUNT(1) AS [Count]
          FROM      [ProductBases] AS c
                    INNER JOIN [Products] AS b ON c.[Id] = b.[Id]
          GROUP BY  b.[Category]
        ) AS a;";

        private const string QueryKeyAndAggregateMaxTestSql =
        @"SELECT  a.[Category] ,
        a.[MaxId]
FROM    ( SELECT    b.[Category] ,
                    MAX(c.[Id]) AS [MaxId]
          FROM      [ProductBases] AS c
                    INNER JOIN [Products] AS b ON c.[Id] = b.[Id]
          GROUP BY  b.[Category]
        ) AS a;";

        private const string QueryKeyAndAggregateCountMaxTestSql =
        @"SELECT  a.[Category] ,
        a.[Count] ,
        a.[MaxId]
FROM    ( SELECT    b.[Category] ,
                    COUNT(1) AS [Count] ,
                    MAX(c.[Id]) AS [MaxId]
          FROM      [ProductBases] AS c
                    INNER JOIN [Products] AS b ON c.[Id] = b.[Id]
          GROUP BY  b.[Category]
        ) AS a;";

        private const string QuerySimpleKeyAndListTestSql =
        @"SELECT  a.[Category] ,
        b.[Id] ,
        b.[Code] ,
        b.[Name] ,
        b.[Category] AS [Category1] ,
        b.[IsValid] ,
        b.[UpdateDate]
FROM    ( SELECT    c.[Category]
          FROM      [ProductBases] AS d
                    INNER JOIN [Products] AS c ON d.[Id] = c.[Id]
          GROUP BY  c.[Category]
        ) AS a
        INNER JOIN ( SELECT d.[Id] ,
                            d.[Code] ,
                            d.[Name] ,
                            c.[Category] ,
                            c.[IsValid] ,
                            c.[UpdateDate]
                     FROM   [ProductBases] AS d
                            INNER JOIN [Products] AS c ON d.[Id] = c.[Id]
                   ) AS b ON b.[Category] = a.[Category];";

    }
}