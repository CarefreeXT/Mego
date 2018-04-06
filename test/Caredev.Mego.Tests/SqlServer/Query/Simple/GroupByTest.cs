namespace Caredev.Mego.Tests.Core.Query.Simple
{
    public partial class GroupByTest
    {
#if SQLSERVER2012
        private const string QuerySimpleKeyAndListPageTestSql =
@"SELECT  a.[Category] ,
        b.[Id] ,
        b.[Category] AS [Category1] ,
        b.[Code] ,
        b.[IsValid] ,
        b.[Name] ,
        b.[UpdateDate]
FROM    ( SELECT    c.[Category]
          FROM      ( SELECT    d.[Category]
                      FROM      [dbo].[Products] AS d
                      GROUP BY  d.[Category]
                    ) AS c
          ORDER BY  c.[Category] ASC
                    OFFSET 2 ROWS
FETCH NEXT 2 ROWS ONLY
        ) AS a
        INNER JOIN ( SELECT d.[Id] ,
                            d.[Category] ,
                            d.[Code] ,
                            d.[IsValid] ,
                            d.[Name] ,
                            d.[UpdateDate]
                     FROM   [dbo].[Products] AS d
                   ) AS b ON b.[Category] = a.[Category];";
#else
        private const string QuerySimpleKeyAndListPageTestSql =
@"SELECT  a.[Category] ,
        b.[Id] ,
        b.[Category] AS [Category1] ,
        b.[Code] ,
        b.[IsValid] ,
        b.[Name] ,
        b.[UpdateDate]
FROM    ( SELECT    c.[Category]
          FROM      ( SELECT    d.[Category] ,
                                ROW_NUMBER() OVER ( ORDER BY d.[Category] ASC ) AS RowIndex
                      FROM      ( SELECT    e.[Category]
                                  FROM      [dbo].[Products] AS e
                                  GROUP BY  e.[Category]
                                ) AS d
                    ) c
          WHERE     c.RowIndex > 2
                    AND c.RowIndex <= 4
        ) AS a
        INNER JOIN ( SELECT e.[Id] ,
                            e.[Category] ,
                            e.[Code] ,
                            e.[IsValid] ,
                            e.[Name] ,
                            e.[UpdateDate]
                     FROM   [dbo].[Products] AS e
                   ) AS b ON b.[Category] = a.[Category];";
#endif

        private const string QuerySimpleKeyTestSql =
      @"SELECT  a.[Category]
FROM    ( SELECT    b.[Category]
          FROM      [dbo].[Products] AS b
          GROUP BY  b.[Category]
        ) AS a;";

        private const string QueryComplexKeysTestSql =
@"SELECT  a.[Category] ,
        a.[IsValid]
FROM    ( SELECT    b.[Category] ,
                    b.[IsValid]
          FROM      [dbo].[Products] AS b
          GROUP BY  b.[Category] ,
                    b.[IsValid]
        ) AS a;";

        private const string QueryKeyMembersTestSql =
@"SELECT  a.[Category] ,
        a.[IsValid]
FROM    ( SELECT    b.[Category] ,
                    b.[IsValid]
          FROM      [dbo].[Products] AS b
          GROUP BY  b.[Category] ,
                    b.[IsValid]
        ) AS a;";

        private const string QueryKeyAndAggregateCountTestSql =
@"SELECT  a.[Category] ,
        a.[Count]
FROM    ( SELECT    b.[Category] ,
                    COUNT(1) AS [Count]
          FROM      [dbo].[Products] AS b
          GROUP BY  b.[Category]
        ) AS a;";

        private const string QueryKeyAndAggregateMaxTestSql =
@"SELECT  a.[Category] ,
        a.[MaxId]
FROM    ( SELECT    b.[Category] ,
                    MAX(b.[Id]) AS [MaxId]
          FROM      [dbo].[Products] AS b
          GROUP BY  b.[Category]
        ) AS a;";

        private const string QueryKeyAndAggregateCountMaxTestSql =
@"SELECT  a.[Category] ,
        a.[Count] ,
        a.[MaxId]
FROM    ( SELECT    b.[Category] ,
                    COUNT(1) AS [Count] ,
                    MAX(b.[Id]) AS [MaxId]
          FROM      [dbo].[Products] AS b
          GROUP BY  b.[Category]
        ) AS a;";

        private const string QuerySimpleKeyAndListTestSql =
@"SELECT  a.[Category] ,
        b.[Id] ,
        b.[Category] AS [Category1] ,
        b.[Code] ,
        b.[IsValid] ,
        b.[Name] ,
        b.[UpdateDate]
FROM    ( SELECT    c.[Category]
          FROM      [dbo].[Products] AS c
          GROUP BY  c.[Category]
        ) AS a
        INNER JOIN ( SELECT c.[Id] ,
                            c.[Category] ,
                            c.[Code] ,
                            c.[IsValid] ,
                            c.[Name] ,
                            c.[UpdateDate]
                     FROM   [dbo].[Products] AS c
                   ) AS b ON b.[Category] = a.[Category];";

        

    }
}