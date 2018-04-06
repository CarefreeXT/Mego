namespace Caredev.Mego.Tests.Core.Query.Simple
{
    public partial class CollectionMemberTest
    {
#if SQLSERVER2012
        private const string QueryBodyAndListPageTestSql =
@"SELECT  a.[Id] ,
        a.[CreateDate] ,
        a.[CustomerId] ,
        a.[ModifyDate] ,
        a.[State] ,
        b.[Id] AS [Id1] ,
        b.[Discount] ,
        b.[Key] ,
        b.[OrderId] ,
        b.[Price] ,
        b.[ProductId] ,
        b.[Quantity]
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
                            d.[Id] ,
                            d.[Discount] ,
                            d.[Key] ,
                            d.[Price] ,
                            d.[ProductId] ,
                            d.[Quantity]
                    FROM    [dbo].[OrderDetails] AS d
                  ) AS b ON b.[OrderId] = a.[Id];";
#else
        private const string QueryBodyAndListPageTestSql =
@"SELECT  a.[Id] ,
        a.[CreateDate] ,
        a.[CustomerId] ,
        a.[ModifyDate] ,
        a.[State] ,
        b.[Id] AS [Id1] ,
        b.[Discount] ,
        b.[Key] ,
        b.[OrderId] ,
        b.[Price] ,
        b.[ProductId] ,
        b.[Quantity]
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
                            e.[Id] ,
                            e.[Discount] ,
                            e.[Key] ,
                            e.[Price] ,
                            e.[ProductId] ,
                            e.[Quantity]
                    FROM    [dbo].[OrderDetails] AS e
                  ) AS b ON b.[OrderId] = a.[Id];";
#endif

        private const string QueryBodyAndAggregateCountMaxTestSql =
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
                            MAX(c.[Id]) AS [MaxId]
                    FROM    [dbo].[OrderDetails] AS c
                    GROUP BY c.[OrderId]
                  ) AS b ON b.[OrderId] = a.[Id];";
        
        private const string QueryBodyAndListTestSql =
@"SELECT  a.[Id] ,
        a.[CreateDate] ,
        a.[CustomerId] ,
        a.[ModifyDate] ,
        a.[State] ,
        b.[Id] AS [Id1] ,
        b.[Discount] ,
        b.[Key] ,
        b.[OrderId] ,
        b.[Price] ,
        b.[ProductId] ,
        b.[Quantity]
FROM    [dbo].[Orders] AS a
        LEFT JOIN ( SELECT  c.[OrderId] ,
                            c.[Id] ,
                            c.[Discount] ,
                            c.[Key] ,
                            c.[Price] ,
                            c.[ProductId] ,
                            c.[Quantity]
                    FROM    [dbo].[OrderDetails] AS c
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
                            MAX(c.[Id]) AS [MaxId]
                    FROM    [dbo].[OrderDetails] AS c
                    GROUP BY c.[OrderId]
                  ) AS b ON b.[OrderId] = a.[Id];";

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
                    FROM    [dbo].[OrderDetails] AS c
                    GROUP BY c.[OrderId]
                  ) AS b ON b.[OrderId] = a.[Id];";

        private const string SimpleJoinDefaultTestSql =
@"SELECT  a.[Id] ,
        a.[CreateDate] ,
        a.[CustomerId] ,
        a.[ModifyDate] ,
        a.[State] ,
        b.[Id] AS [Id1] ,
        b.[Discount] ,
        b.[Key] ,
        b.[OrderId] ,
        b.[Price] ,
        b.[ProductId] ,
        b.[Quantity]
FROM    [dbo].[Orders] AS a
        LEFT JOIN [dbo].[OrderDetails] AS b ON b.[OrderId] = a.[Id];";

        private const string SimpleJoinTestSql =
@"SELECT  a.[Id] ,
        a.[CreateDate] ,
        a.[CustomerId] ,
        a.[ModifyDate] ,
        a.[State] ,
        b.[Id] AS [Id1] ,
        b.[Discount] ,
        b.[Key] ,
        b.[OrderId] ,
        b.[Price] ,
        b.[ProductId] ,
        b.[Quantity]
FROM    [dbo].[Orders] AS a
        INNER JOIN [dbo].[OrderDetails] AS b ON b.[OrderId] = a.[Id];";

    }
}