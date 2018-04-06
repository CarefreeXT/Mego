namespace Caredev.Mego.Tests.Core.Query.Inherit
{
    public partial class CollectionMemberTest
    {
        private const string QueryBodyAndListPageTestSql =
@"SELECT
  a.[Id],
  a.[CreateDate],
  a.[ModifyDate],
  a.[CustomerId],
  a.[State],
  b.[Id] AS [Id1],
  b.[Key],
  b.[Price],
  b.[Quantity],
  b.[Discount],
  b.[OrderId],
  b.[ProductId]
FROM (SELECT
    c.[Id],
    c.[CreateDate],
    c.[ModifyDate],
    d.[CustomerId],
    d.[State]
  FROM [orderbases] AS c
    INNER JOIN [orders] AS d
      ON c.[Id] = d.[Id]
  ORDER BY c.[Id] ASC
  LIMIT 5, 5) AS a
  LEFT JOIN (SELECT
      e.[OrderId],
      f.[Id],
      f.[Key],
      f.[Price],
      f.[Quantity],
      e.[Discount],
      e.[ProductId]
    FROM [orderdetailbases] AS f
      INNER JOIN [orderdetails] AS e
        ON f.[Id] = e.[Id]) AS b
    ON b.[OrderId] = a.[Id]";

        private const string SimpleJoinTestSql =
@"SELECT
  a.[Id],
  a.[CreateDate],
  a.[ModifyDate],
  b.[CustomerId],
  b.[State],
  c.[Id] AS [Id1],
  c.[Key],
  c.[Price],
  c.[Quantity],
  d.[Discount],
  d.[OrderId],
  d.[ProductId]
FROM [orderbases] AS a
  INNER JOIN [orders] AS b
    ON a.[Id] = b.[Id]
  INNER JOIN [orderdetailbases] AS c
  INNER JOIN [orderdetails] AS d
    ON c.[Id] = d.[Id]
    AND d.[OrderId] = a.[Id]";

        private const string SimpleJoinDefaultTestSql =
        @"SELECT  a.[Id] ,
        a.[CreateDate] ,
        a.[ModifyDate] ,
        b.[CustomerId] ,
        b.[State] ,
        c.[Id] AS [Id1] ,
        c.[Key] ,
        c.[Price] ,
        c.[Quantity] ,
        d.[Discount] ,
        d.[OrderId] ,
        d.[ProductId]
FROM    [OrderBases] AS a
        INNER JOIN [Orders] AS b ON a.[Id] = b.[Id]
        LEFT JOIN [OrderDetailBases] AS c
        INNER JOIN [OrderDetails] AS d ON c.[Id] = d.[Id] ON d.[OrderId] = a.[Id];";

        private const string QueryBodyAndAggregateCountTestSql =
        @"SELECT  a.[Id] ,
        a.[CreateDate] ,
        a.[ModifyDate] ,
        b.[CustomerId] ,
        b.[State] ,
        c.[Count]
FROM    [OrderBases] AS a
        INNER JOIN [Orders] AS b ON a.[Id] = b.[Id]
        LEFT JOIN ( SELECT  d.[OrderId] ,
                            COUNT(1) AS [Count]
                    FROM    [OrderDetailBases] AS e
                            INNER JOIN [OrderDetails] AS d ON e.[Id] = d.[Id]
                    GROUP BY d.[OrderId]
                  ) AS c ON c.[OrderId] = a.[Id];";

        private const string QueryBodyAndAggregateMaxTestSql =
        @"SELECT  a.[Id] ,
        a.[CreateDate] ,
        a.[ModifyDate] ,
        b.[CustomerId] ,
        b.[State] ,
        c.[MaxId]
FROM    [OrderBases] AS a
        INNER JOIN [Orders] AS b ON a.[Id] = b.[Id]
        LEFT JOIN ( SELECT  d.[OrderId] ,
                            MAX(e.[Id]) AS [MaxId]
                    FROM    [OrderDetailBases] AS e
                            INNER JOIN [OrderDetails] AS d ON e.[Id] = d.[Id]
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
FROM    [OrderBases] AS a
        INNER JOIN [Orders] AS b ON a.[Id] = b.[Id]
        LEFT JOIN ( SELECT  d.[OrderId] ,
                            COUNT(1) AS [Count] ,
                            MAX(e.[Id]) AS [MaxId]
                    FROM    [OrderDetailBases] AS e
                            INNER JOIN [OrderDetails] AS d ON e.[Id] = d.[Id]
                    GROUP BY d.[OrderId]
                  ) AS c ON c.[OrderId] = a.[Id];";

        
        private const string QueryBodyAndListTestSql =
        @"SELECT  a.[Id] ,
        a.[CreateDate] ,
        a.[ModifyDate] ,
        b.[CustomerId] ,
        b.[State] ,
        c.[Id] AS [Id1] ,
        c.[Key] ,
        c.[Price] ,
        c.[Quantity] ,
        c.[Discount] ,
        c.[OrderId] ,
        c.[ProductId]
FROM    [OrderBases] AS a
        INNER JOIN [Orders] AS b ON a.[Id] = b.[Id]
        LEFT JOIN ( SELECT  d.[OrderId] ,
                            e.[Id] ,
                            e.[Key] ,
                            e.[Price] ,
                            e.[Quantity] ,
                            d.[Discount] ,
                            d.[ProductId]
                    FROM    [OrderDetailBases] AS e
                            INNER JOIN [OrderDetails] AS d ON e.[Id] = d.[Id]
                  ) AS c ON c.[OrderId] = a.[Id];";

    }
}