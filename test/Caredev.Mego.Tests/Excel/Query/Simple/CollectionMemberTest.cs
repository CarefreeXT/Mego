namespace Caredev.Mego.Tests.Core.Query.Simple
{
    public partial class CollectionMemberTest
    {
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
FROM    ( SELECT TOP 5
                    c.[Id] ,
                    c.[CreateDate] ,
                    c.[CustomerId] ,
                    c.[ModifyDate] ,
                    c.[State]
          FROM      [Orders] AS c
          WHERE     c.[Id] NOT IN ( SELECT TOP 5
                                            c.[Id]
                                    FROM    [Orders] AS c
                                    ORDER BY c.[Id] ASC )
          ORDER BY  c.[Id] ASC
        ) AS a
        LEFT JOIN ( SELECT  d.[OrderId] ,
                            d.[Id] ,
                            d.[Discount] ,
                            d.[Key] ,
                            d.[Price] ,
                            d.[ProductId] ,
                            d.[Quantity]
                    FROM    [OrderDetails] AS d
                  ) AS b ON b.[OrderId] = a.[Id]";


        private const string QueryBodyAndAggregateCountMaxTestSql =
@"SELECT  a.[Id] ,
        a.[CreateDate] ,
        a.[CustomerId] ,
        a.[ModifyDate] ,
        a.[State] ,
        b.[Count] ,
        b.[MaxId]
FROM    [Orders] AS a
        LEFT JOIN ( SELECT  c.[OrderId] ,
                            COUNT(1) AS [Count] ,
                            MAX(c.[Id]) AS [MaxId]
                    FROM    [OrderDetails] AS c
                    GROUP BY c.[OrderId]
                  ) AS b ON b.[OrderId] = a.[Id]";
        
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
FROM    [Orders] AS a
        LEFT JOIN ( SELECT  c.[OrderId] ,
                            c.[Id] ,
                            c.[Discount] ,
                            c.[Key] ,
                            c.[Price] ,
                            c.[ProductId] ,
                            c.[Quantity]
                    FROM    [OrderDetails] AS c
                  ) AS b ON b.[OrderId] = a.[Id]";

        private const string QueryBodyAndAggregateMaxTestSql =
@"SELECT  a.[Id] ,
        a.[CreateDate] ,
        a.[CustomerId] ,
        a.[ModifyDate] ,
        a.[State] ,
        b.[MaxId]
FROM    [Orders] AS a
        LEFT JOIN ( SELECT  c.[OrderId] ,
                            MAX(c.[Id]) AS [MaxId]
                    FROM    [OrderDetails] AS c
                    GROUP BY c.[OrderId]
                  ) AS b ON b.[OrderId] = a.[Id]";

        private const string QueryBodyAndAggregateCountTestSql =
@"SELECT  a.[Id] ,
        a.[CreateDate] ,
        a.[CustomerId] ,
        a.[ModifyDate] ,
        a.[State] ,
        b.[Count]
FROM    [Orders] AS a
        LEFT JOIN ( SELECT  c.[OrderId] ,
                            COUNT(1) AS [Count]
                    FROM    [OrderDetails] AS c
                    GROUP BY c.[OrderId]
                  ) AS b ON b.[OrderId] = a.[Id]";

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
FROM    [Orders] AS a
        LEFT JOIN [OrderDetails] AS b ON b.[OrderId] = a.[Id]";

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
FROM    [Orders] AS a
        INNER JOIN [OrderDetails] AS b ON b.[OrderId] = a.[Id]";

    }
}