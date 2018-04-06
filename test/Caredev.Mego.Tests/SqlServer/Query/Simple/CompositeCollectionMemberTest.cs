namespace Caredev.Mego.Tests.Core.Query.Simple
{
    public partial class CompositeCollectionMemberTest
    {
#if SQLSERVER2012
        private const string QueryBodyAndListPageTestSql =
@"SELECT  a.[Id] ,
        a.[CreateDate] ,
        a.[CustomerId] ,
        a.[ModifyDate] ,
        a.[State] ,
        b.[Id] AS [Id1] ,
        b.[Category] ,
        b.[Code] ,
        b.[IsValid] ,
        b.[Name] ,
        b.[UpdateDate]
FROM    ( SELECT    c.[Id] ,
                    c.[CreateDate] ,
                    c.[CustomerId] ,
                    c.[ModifyDate] ,
                    c.[State]
          FROM      [dbo].[Orders] AS c
          ORDER BY  c.[Id] ASC
                    OFFSET 5 ROWS
FETCH NEXT 5 ROWS ONLY
        ) AS a
        LEFT JOIN ( SELECT  d.[OrderId] ,
                            e.[Id] ,
                            e.[Category] ,
                            e.[Code] ,
                            e.[IsValid] ,
                            e.[Name] ,
                            e.[UpdateDate]
                    FROM    [dbo].[Products] AS e
                            INNER JOIN [dbo].[OrderDetails] AS d ON e.[Id] = d.[ProductId]
                  ) AS b ON b.[OrderId] = a.[Id];";
#else
        private const string QueryBodyAndListPageTestSql =
@"SELECT  a.[Id] ,
        a.[CreateDate] ,
        a.[CustomerId] ,
        a.[ModifyDate] ,
        a.[State] ,
        b.[Id] AS [Id1] ,
        b.[Category] ,
        b.[Code] ,
        b.[IsValid] ,
        b.[Name] ,
        b.[UpdateDate]
FROM    ( SELECT    c.[Id] ,
                    c.[CreateDate] ,
                    c.[CustomerId] ,
                    c.[ModifyDate] ,
                    c.[State]
          FROM      ( SELECT    d.[Id] ,
                                d.[CreateDate] ,
                                d.[CustomerId] ,
                                d.[ModifyDate] ,
                                d.[State] ,
                                ROW_NUMBER() OVER ( ORDER BY d.[Id] ASC ) AS RowIndex
                      FROM      [dbo].[Orders] AS d
                    ) c
          WHERE     c.RowIndex > 5
                    AND c.RowIndex <= 10
        ) AS a
        LEFT JOIN ( SELECT  e.[OrderId] ,
                            f.[Id] ,
                            f.[Category] ,
                            f.[Code] ,
                            f.[IsValid] ,
                            f.[Name] ,
                            f.[UpdateDate]
                    FROM    [dbo].[Products] AS f
                            INNER JOIN [dbo].[OrderDetails] AS e ON f.[Id] = e.[ProductId]
                  ) AS b ON b.[OrderId] = a.[Id];";
#endif

        private const string SimpleJoinTestSql =
@"SELECT  a.[Id] ,
        a.[CreateDate] ,
        a.[CustomerId] ,
        a.[ModifyDate] ,
        a.[State] ,
        b.[Id] AS [Id1] ,
        b.[Category] ,
        b.[Code] ,
        b.[IsValid] ,
        b.[Name] ,
        b.[UpdateDate]
FROM    [dbo].[Orders] AS a
        INNER JOIN [dbo].[OrderDetails] AS c ON c.[OrderId] = a.[Id]
        INNER JOIN [dbo].[Products] AS b ON b.[Id] = c.[ProductId];";

        private const string SimpleJoinDefaultTestSql =
@"SELECT  a.[Id] ,
        a.[CreateDate] ,
        a.[CustomerId] ,
        a.[ModifyDate] ,
        a.[State] ,
        b.[Id] AS [Id1] ,
        b.[Category] ,
        b.[Code] ,
        b.[IsValid] ,
        b.[Name] ,
        b.[UpdateDate]
FROM    [dbo].[Orders] AS a
        LEFT JOIN [dbo].[OrderDetails] AS c ON c.[OrderId] = a.[Id]
        INNER JOIN [dbo].[Products] AS b ON b.[Id] = c.[ProductId];";

        private const string QueryBodyAndAggregateCountTestSql =
@"SELECT  a.[Id] ,
        a.[CreateDate] ,
        a.[CustomerId] ,
        a.[ModifyDate] ,
        a.[State] ,
        b.[Count]
FROM    [dbo].[Orders] AS a
        LEFT JOIN ( SELECT  c.[OrderId] ,
                            COUNT(1) AS [Count]
                    FROM    [dbo].[Products] AS d
                            INNER JOIN [dbo].[OrderDetails] AS c ON d.[Id] = c.[ProductId]
                    GROUP BY c.[OrderId]
                  ) AS b ON b.[OrderId] = a.[Id];";

        private const string QueryBodyAndAggregateMaxTestSql =
@"SELECT  a.[Id] ,
        a.[CreateDate] ,
        a.[CustomerId] ,
        a.[ModifyDate] ,
        a.[State] ,
        b.[MaxId]
FROM    [dbo].[Orders] AS a
        LEFT JOIN ( SELECT  c.[OrderId] ,
                            MAX(d.[Id]) AS [MaxId]
                    FROM    [dbo].[Products] AS d
                            INNER JOIN [dbo].[OrderDetails] AS c ON d.[Id] = c.[ProductId]
                    GROUP BY c.[OrderId]
                  ) AS b ON b.[OrderId] = a.[Id];";

        private const string QueryBodyAndAggregateCountMaxSql =
@"SELECT  a.[Id] ,
        a.[CreateDate] ,
        a.[CustomerId] ,
        a.[ModifyDate] ,
        a.[State] ,
        b.[Count] ,
        b.[MaxId]
FROM    [dbo].[Orders] AS a
        LEFT JOIN ( SELECT  c.[OrderId] ,
                            COUNT(1) AS [Count] ,
                            MAX(d.[Id]) AS [MaxId]
                    FROM    [dbo].[Products] AS d
                            INNER JOIN [dbo].[OrderDetails] AS c ON d.[Id] = c.[ProductId]
                    GROUP BY c.[OrderId]
                  ) AS b ON b.[OrderId] = a.[Id];";
        
        private const string QueryBodyAndListTestSql =
@"SELECT  a.[Id] ,
        a.[CreateDate] ,
        a.[CustomerId] ,
        a.[ModifyDate] ,
        a.[State] ,
        b.[Id] AS [Id1] ,
        b.[Category] ,
        b.[Code] ,
        b.[IsValid] ,
        b.[Name] ,
        b.[UpdateDate]
FROM    [dbo].[Orders] AS a
        LEFT JOIN ( SELECT  c.[OrderId] ,
                            d.[Id] ,
                            d.[Category] ,
                            d.[Code] ,
                            d.[IsValid] ,
                            d.[Name] ,
                            d.[UpdateDate]
                    FROM    [dbo].[Products] AS d
                            INNER JOIN [dbo].[OrderDetails] AS c ON d.[Id] = c.[ProductId]
                  ) AS b ON b.[OrderId] = a.[Id];";

    }
}