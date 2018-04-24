namespace Caredev.Mego.Tests.Core.Query.Inherit
{
    public partial class CompositeCollectionMemberTest
    {
#if SQLSERVER2012
        private const string QueryBodyAndListPageTestSql =
@"SELECT  a.[Id] ,
        a.[CreateDate] ,
        a.[ModifyDate] ,
        a.[CustomerId] ,
        a.[State] ,
        b.[Id] AS [Id1] ,
        b.[Code] ,
        b.[Name] ,
        b.[Category] ,
        b.[IsValid] ,
        b.[UpdateDate]
FROM    ( SELECT    c.[Id] ,
                    c.[CreateDate] ,
                    c.[ModifyDate] ,
                    d.[CustomerId] ,
                    d.[State]
          FROM      [dbo].[OrderBases] AS c
                    INNER JOIN [dbo].[Orders] AS d ON c.[Id] = d.[Id]
          ORDER BY  c.[Id] ASC
                    OFFSET 5 ROWS
FETCH NEXT 5 ROWS ONLY
        ) AS a
        LEFT JOIN ( SELECT  e.[OrderId] ,
                            f.[Id] ,
                            f.[Code] ,
                            f.[Name] ,
                            g.[Category] ,
                            g.[IsValid] ,
                            g.[UpdateDate]
                    FROM    [dbo].[ProductBases] AS f
                            INNER JOIN [dbo].[Products] AS g ON f.[Id] = g.[Id]
                            INNER JOIN [dbo].[OrderDetailBases] AS h
                            INNER JOIN [dbo].[OrderDetails] AS e ON h.[Id] = e.[Id] ON f.[Id] = e.[ProductId]
                  ) AS b ON b.[OrderId] = a.[Id];";
#else
        private const string QueryBodyAndListPageTestSql =
@"SELECT  a.[Id] ,
        a.[CreateDate] ,
        a.[ModifyDate] ,
        a.[CustomerId] ,
        a.[State] ,
        b.[Id] AS [Id1] ,
        b.[Code] ,
        b.[Name] ,
        b.[Category] ,
        b.[IsValid] ,
        b.[UpdateDate]
FROM    ( SELECT    c.[Id] ,
                    c.[CreateDate] ,
                    c.[ModifyDate] ,
                    c.[CustomerId] ,
                    c.[State]
          FROM      ( SELECT    d.[Id] ,
                                d.[CreateDate] ,
                                d.[ModifyDate] ,
                                e.[CustomerId] ,
                                e.[State] ,
                                ROW_NUMBER() OVER ( ORDER BY d.[Id] ASC ) AS RowIndex
                      FROM      [dbo].[OrderBases] AS d
                                INNER JOIN [dbo].[Orders] AS e ON d.[Id] = e.[Id]
                    ) c
          WHERE     c.RowIndex > 5
                    AND c.RowIndex <= 10
        ) AS a
        LEFT JOIN ( SELECT  f.[OrderId] ,
                            g.[Id] ,
                            g.[Code] ,
                            g.[Name] ,
                            h.[Category] ,
                            h.[IsValid] ,
                            h.[UpdateDate]
                    FROM    [dbo].[ProductBases] AS g
                            INNER JOIN [dbo].[Products] AS h ON g.[Id] = h.[Id]
                            INNER JOIN [dbo].[OrderDetailBases] AS i
                            INNER JOIN [dbo].[OrderDetails] AS f ON i.[Id] = f.[Id] ON g.[Id] = f.[ProductId]
                  ) AS b ON b.[OrderId] = a.[Id];";
#endif
        private const string SimpleJoinTestSql =
        @"SELECT  a.[Id] ,
        a.[CreateDate] ,
        a.[ModifyDate] ,
        b.[CustomerId] ,
        b.[State] ,
        c.[Id] AS [Id1] ,
        c.[Code] ,
        c.[Name] ,
        d.[Category] ,
        d.[IsValid] ,
        d.[UpdateDate]
FROM    [dbo].[OrderBases] AS a
        INNER JOIN [dbo].[Orders] AS b ON a.[Id] = b.[Id]
        INNER JOIN [dbo].[OrderDetailBases] AS e
        INNER JOIN [dbo].[OrderDetails] AS f ON e.[Id] = f.[Id] ON f.[OrderId] = a.[Id]
        INNER JOIN [dbo].[ProductBases] AS c
        INNER JOIN [dbo].[Products] AS d ON c.[Id] = d.[Id] ON c.[Id] = f.[ProductId];";

        private const string SimpleJoinDefaultTestSql =
        @"SELECT  a.[Id] ,
        a.[CreateDate] ,
        a.[ModifyDate] ,
        b.[CustomerId] ,
        b.[State] ,
        c.[Id] AS [Id1] ,
        c.[Code] ,
        c.[Name] ,
        d.[Category] ,
        d.[IsValid] ,
        d.[UpdateDate]
FROM    [dbo].[OrderBases] AS a
        INNER JOIN [dbo].[Orders] AS b ON a.[Id] = b.[Id]
        LEFT JOIN [dbo].[OrderDetailBases] AS e
        INNER JOIN [dbo].[OrderDetails] AS f ON e.[Id] = f.[Id] ON f.[OrderId] = a.[Id]
        INNER JOIN [dbo].[ProductBases] AS c
        INNER JOIN [dbo].[Products] AS d ON c.[Id] = d.[Id] ON c.[Id] = f.[ProductId];";

        private const string QueryBodyAndAggregateCountTestSql =
        @"SELECT  a.[Id] ,
        a.[CreateDate] ,
        a.[ModifyDate] ,
        b.[CustomerId] ,
        b.[State] ,
        c.[Count]
FROM    [dbo].[OrderBases] AS a
        INNER JOIN [dbo].[Orders] AS b ON a.[Id] = b.[Id]
        LEFT JOIN ( SELECT  d.[OrderId] ,
                            COUNT(1) AS [Count]
                    FROM    [dbo].[ProductBases] AS e
                            INNER JOIN [dbo].[Products] AS f ON e.[Id] = f.[Id]
                            INNER JOIN [dbo].[OrderDetailBases] AS g
                            INNER JOIN [dbo].[OrderDetails] AS d ON g.[Id] = d.[Id] ON e.[Id] = d.[ProductId]
                    GROUP BY d.[OrderId]
                  ) AS c ON c.[OrderId] = a.[Id];";

        private const string QueryBodyAndAggregateMaxTestSql =
        @"SELECT  a.[Id] ,
        a.[CreateDate] ,
        a.[ModifyDate] ,
        b.[CustomerId] ,
        b.[State] ,
        c.[MaxId]
FROM    [dbo].[OrderBases] AS a
        INNER JOIN [dbo].[Orders] AS b ON a.[Id] = b.[Id]
        LEFT JOIN ( SELECT  d.[OrderId] ,
                            MAX(e.[Id]) AS [MaxId]
                    FROM    [dbo].[ProductBases] AS e
                            INNER JOIN [dbo].[Products] AS f ON e.[Id] = f.[Id]
                            INNER JOIN [dbo].[OrderDetailBases] AS g
                            INNER JOIN [dbo].[OrderDetails] AS d ON g.[Id] = d.[Id] ON e.[Id] = d.[ProductId]
                    GROUP BY d.[OrderId]
                  ) AS c ON c.[OrderId] = a.[Id];";

        private const string QueryBodyAndAggregateCountMaxTestSql =
        @"SELECT  a.[Id] ,
        a.[CreateDate] ,
        a.[ModifyDate] ,
        b.[CustomerId] ,
        b.[State] ,
        c.[Count] ,
        c.[MaxId]
FROM    [dbo].[OrderBases] AS a
        INNER JOIN [dbo].[Orders] AS b ON a.[Id] = b.[Id]
        LEFT JOIN ( SELECT  d.[OrderId] ,
                            COUNT(1) AS [Count] ,
                            MAX(e.[Id]) AS [MaxId]
                    FROM    [dbo].[ProductBases] AS e
                            INNER JOIN [dbo].[Products] AS f ON e.[Id] = f.[Id]
                            INNER JOIN [dbo].[OrderDetailBases] AS g
                            INNER JOIN [dbo].[OrderDetails] AS d ON g.[Id] = d.[Id] ON e.[Id] = d.[ProductId]
                    GROUP BY d.[OrderId]
                  ) AS c ON c.[OrderId] = a.[Id];";

        
        private const string QueryBodyAndListTestSql =
        @"SELECT  a.[Id] ,
        a.[CreateDate] ,
        a.[ModifyDate] ,
        b.[CustomerId] ,
        b.[State] ,
        c.[Id] AS [Id1] ,
        c.[Code] ,
        c.[Name] ,
        c.[Category] ,
        c.[IsValid] ,
        c.[UpdateDate]
FROM    [dbo].[OrderBases] AS a
        INNER JOIN [dbo].[Orders] AS b ON a.[Id] = b.[Id]
        LEFT JOIN ( SELECT  d.[OrderId] ,
                            e.[Id] ,
                            e.[Code] ,
                            e.[Name] ,
                            f.[Category] ,
                            f.[IsValid] ,
                            f.[UpdateDate]
                    FROM    [dbo].[ProductBases] AS e
                            INNER JOIN [dbo].[Products] AS f ON e.[Id] = f.[Id]
                            INNER JOIN [dbo].[OrderDetailBases] AS g
                            INNER JOIN [dbo].[OrderDetails] AS d ON g.[Id] = d.[Id] ON e.[Id] = d.[ProductId]
                  ) AS c ON c.[OrderId] = a.[Id];";

    }
}