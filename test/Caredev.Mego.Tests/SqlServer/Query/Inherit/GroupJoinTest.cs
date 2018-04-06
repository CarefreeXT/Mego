namespace Caredev.Mego.Tests.Core.Query.Inherit
{
    public partial class GroupJoinTest
    {
#if SQLSERVER2012
        private const string QueryBodyAndListPageTestSql =
@"SELECT  a.[Id] ,
        a.[CreateDate] ,
        a.[ModifyDate] ,
        a.[CustomerId] ,
        a.[State] ,
        b.[OrderId] ,
        b.[Id] AS [Id1] ,
        b.[Key] ,
        b.[Price] ,
        b.[Quantity] ,
        b.[Discount] ,
        b.[ProductId]
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
                            f.[Key] ,
                            f.[Price] ,
                            f.[Quantity] ,
                            e.[Discount] ,
                            e.[ProductId]
                    FROM    [dbo].[OrderDetailBases] AS f
                            INNER JOIN [dbo].[OrderDetails] AS e ON f.[Id] = e.[Id]
                  ) AS b ON a.[Id] = b.[OrderId];";
#else
        private const string QueryBodyAndListPageTestSql =
@"SELECT  a.[Id] ,
        a.[CreateDate] ,
        a.[ModifyDate] ,
        a.[CustomerId] ,
        a.[State] ,
        b.[OrderId] ,
        b.[Id] AS [Id1] ,
        b.[Key] ,
        b.[Price] ,
        b.[Quantity] ,
        b.[Discount] ,
        b.[ProductId]
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
                            g.[Key] ,
                            g.[Price] ,
                            g.[Quantity] ,
                            f.[Discount] ,
                            f.[ProductId]
                    FROM    [dbo].[OrderDetailBases] AS g
                            INNER JOIN [dbo].[OrderDetails] AS f ON g.[Id] = f.[Id]
                  ) AS b ON a.[Id] = b.[OrderId];";
#endif

        private const string SimpleGroupJoinTestSql =
        @"SELECT  a.[Id] ,
        a.[Code] ,
        a.[Name] ,
        b.[Category] ,
        b.[IsValid] ,
        b.[UpdateDate] ,
        c.[Id] AS [Id1] ,
        c.[Code] AS [Code1] ,
        c.[Name] AS [Name1] ,
        d.[Address1] ,
        d.[Address2] ,
        d.[Zip]
FROM    [dbo].[ProductBases] AS a
        INNER JOIN [dbo].[Products] AS b ON a.[Id] = b.[Id]
        INNER JOIN [dbo].[CustomerBases] AS c
        INNER JOIN [dbo].[Customers] AS d ON c.[Id] = d.[Id] ON a.[Id] = c.[Id];";

        private const string GroupJoinDefaultTestSql =
        @"SELECT  a.[Id] ,
        a.[Code] ,
        a.[Name] ,
        b.[Category] ,
        b.[IsValid] ,
        b.[UpdateDate] ,
        c.[Id] AS [Id1] ,
        c.[Code] AS [Code1] ,
        c.[Name] AS [Name1] ,
        d.[Address1] ,
        d.[Address2] ,
        d.[Zip]
FROM    [dbo].[ProductBases] AS a
        INNER JOIN [dbo].[Products] AS b ON a.[Id] = b.[Id]
        LEFT JOIN [dbo].[CustomerBases] AS c
        INNER JOIN [dbo].[Customers] AS d ON c.[Id] = d.[Id] ON a.[Id] = c.[Id];";

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
                    FROM    [dbo].[OrderDetailBases] AS e
                            INNER JOIN [dbo].[OrderDetails] AS d ON e.[Id] = d.[Id]
                    GROUP BY d.[OrderId]
                  ) AS c ON a.[Id] = c.[OrderId];";

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
                    FROM    [dbo].[OrderDetailBases] AS e
                            INNER JOIN [dbo].[OrderDetails] AS d ON e.[Id] = d.[Id]
                    GROUP BY d.[OrderId]
                  ) AS c ON a.[Id] = c.[OrderId];";

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
                    FROM    [dbo].[OrderDetailBases] AS e
                            INNER JOIN [dbo].[OrderDetails] AS d ON e.[Id] = d.[Id]
                    GROUP BY d.[OrderId]
                  ) AS c ON a.[Id] = c.[OrderId];";
       
        private const string QueryBodyAndListTestSql =
        @"SELECT  a.[Id] ,
        a.[CreateDate] ,
        a.[ModifyDate] ,
        b.[CustomerId] ,
        b.[State] ,
        c.[OrderId] ,
        c.[Id] AS [Id1] ,
        c.[Key] ,
        c.[Price] ,
        c.[Quantity] ,
        c.[Discount] ,
        c.[ProductId]
FROM    [dbo].[OrderBases] AS a
        INNER JOIN [dbo].[Orders] AS b ON a.[Id] = b.[Id]
        LEFT JOIN ( SELECT  d.[OrderId] ,
                            e.[Id] ,
                            e.[Key] ,
                            e.[Price] ,
                            e.[Quantity] ,
                            d.[Discount] ,
                            d.[ProductId]
                    FROM    [dbo].[OrderDetailBases] AS e
                            INNER JOIN [dbo].[OrderDetails] AS d ON e.[Id] = d.[Id]
                  ) AS c ON a.[Id] = c.[OrderId];";

    }
}