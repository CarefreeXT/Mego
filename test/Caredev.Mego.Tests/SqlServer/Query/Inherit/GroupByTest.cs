namespace Caredev.Mego.Tests.Core.Query.Inherit
{
    public partial class GroupByTest
    {
#if SQLSERVER2012
        private const string QuerySimpleKeyAndListPageTestSql =
@"SELECT  a.[Category] ,
        b.[Id] ,
        b.[Code] ,
        b.[Name] ,
        b.[Category] AS [Category1] ,
        b.[IsValid] ,
        b.[UpdateDate]
FROM    ( SELECT    c.[Category]
          FROM      ( SELECT    d.[Category]
                      FROM      [dbo].[ProductBases] AS e
                                INNER JOIN [dbo].[Products] AS d ON e.[Id] = d.[Id]
                      GROUP BY  d.[Category]
                    ) AS c
          ORDER BY  c.[Category] ASC
                    OFFSET 2 ROWS
FETCH NEXT 2 ROWS ONLY
        ) AS a
        INNER JOIN ( SELECT e.[Id] ,
                            e.[Code] ,
                            e.[Name] ,
                            d.[Category] ,
                            d.[IsValid] ,
                            d.[UpdateDate]
                     FROM   [dbo].[ProductBases] AS e
                            INNER JOIN [dbo].[Products] AS d ON e.[Id] = d.[Id]
                   ) AS b ON b.[Category] = a.[Category];";
#else
        private const string QuerySimpleKeyAndListPageTestSql =
@"SELECT  a.[Category] ,
        b.[Id] ,
        b.[Code] ,
        b.[Name] ,
        b.[Category] AS [Category1] ,
        b.[IsValid] ,
        b.[UpdateDate]
FROM    ( SELECT    c.[Category]
          FROM      ( SELECT    d.[Category] ,
                                ROW_NUMBER() OVER ( ORDER BY d.[Category] ASC ) AS RowIndex
                      FROM      ( SELECT    e.[Category]
                                  FROM      [dbo].[ProductBases] AS f
                                            INNER JOIN [dbo].[Products] AS e ON f.[Id] = e.[Id]
                                  GROUP BY  e.[Category]
                                ) AS d
                    ) c
          WHERE     c.RowIndex > 2
                    AND c.RowIndex <= 4
        ) AS a
        INNER JOIN ( SELECT f.[Id] ,
                            f.[Code] ,
                            f.[Name] ,
                            e.[Category] ,
                            e.[IsValid] ,
                            e.[UpdateDate]
                     FROM   [dbo].[ProductBases] AS f
                            INNER JOIN [dbo].[Products] AS e ON f.[Id] = e.[Id]
                   ) AS b ON b.[Category] = a.[Category];";
#endif

        private const string QuerySimpleKeyTestSql =
        @"SELECT  a.[Category]
FROM    ( SELECT    b.[Category]
          FROM      [dbo].[ProductBases] AS c
                    INNER JOIN [dbo].[Products] AS b ON c.[Id] = b.[Id]
          GROUP BY  b.[Category]
        ) AS a;";

        private const string QueryComplexKeysTestSql =
        @"SELECT  a.[Category] ,
        a.[IsValid]
FROM    ( SELECT    b.[Category] ,
                    b.[IsValid]
          FROM      [dbo].[ProductBases] AS c
                    INNER JOIN [dbo].[Products] AS b ON c.[Id] = b.[Id]
          GROUP BY  b.[Category] ,
                    b.[IsValid]
        ) AS a;";

        private const string QueryKeyMembersTestSql =
        @"SELECT  a.[Category] ,
        a.[IsValid]
FROM    ( SELECT    b.[Category] ,
                    b.[IsValid]
          FROM      [dbo].[ProductBases] AS c
                    INNER JOIN [dbo].[Products] AS b ON c.[Id] = b.[Id]
          GROUP BY  b.[Category] ,
                    b.[IsValid]
        ) AS a;";

        private const string QueryKeyAndAggregateCountTestSql =
        @"SELECT  a.[Category] ,
        a.[Count]
FROM    ( SELECT    b.[Category] ,
                    COUNT(1) AS [Count]
          FROM      [dbo].[ProductBases] AS c
                    INNER JOIN [dbo].[Products] AS b ON c.[Id] = b.[Id]
          GROUP BY  b.[Category]
        ) AS a;";

        private const string QueryKeyAndAggregateMaxTestSql =
        @"SELECT  a.[Category] ,
        a.[MaxId]
FROM    ( SELECT    b.[Category] ,
                    MAX(c.[Id]) AS [MaxId]
          FROM      [dbo].[ProductBases] AS c
                    INNER JOIN [dbo].[Products] AS b ON c.[Id] = b.[Id]
          GROUP BY  b.[Category]
        ) AS a;";

        private const string QueryKeyAndAggregateCountMaxTestSql =
        @"SELECT  a.[Category] ,
        a.[Count] ,
        a.[MaxId]
FROM    ( SELECT    b.[Category] ,
                    COUNT(1) AS [Count] ,
                    MAX(c.[Id]) AS [MaxId]
          FROM      [dbo].[ProductBases] AS c
                    INNER JOIN [dbo].[Products] AS b ON c.[Id] = b.[Id]
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
          FROM      [dbo].[ProductBases] AS d
                    INNER JOIN [dbo].[Products] AS c ON d.[Id] = c.[Id]
          GROUP BY  c.[Category]
        ) AS a
        INNER JOIN ( SELECT d.[Id] ,
                            d.[Code] ,
                            d.[Name] ,
                            c.[Category] ,
                            c.[IsValid] ,
                            c.[UpdateDate]
                     FROM   [dbo].[ProductBases] AS d
                            INNER JOIN [dbo].[Products] AS c ON d.[Id] = c.[Id]
                   ) AS b ON b.[Category] = a.[Category];";

    }
}