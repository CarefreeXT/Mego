namespace Caredev.Mego.Tests.Core.Query.Inherit
{
    public partial class ExpandMemberTest
    {
#if SQLSERVER2012
        private const string ExpandOneLevelObjectMemberPageTestSql =
@"SELECT  a.[Id] ,
        a.[CreateDate] ,
        a.[ModifyDate] ,
        a.[CustomerId] ,
        a.[State] ,
        b.[Id] AS [Id1] ,
        b.[Code] ,
        b.[Name] ,
        c.[Address1] ,
        c.[Address2] ,
        c.[Zip]
FROM    ( SELECT    d.[Id] ,
                    d.[CreateDate] ,
                    d.[ModifyDate] ,
                    e.[CustomerId] ,
                    e.[State]
          FROM      [dbo].[OrderBases] AS d
                    INNER JOIN [dbo].[Orders] AS e ON d.[Id] = e.[Id]
          ORDER BY  d.[Id] ASC
                    OFFSET 5 ROWS
FETCH NEXT 5 ROWS ONLY
        ) AS a
        INNER JOIN [dbo].[CustomerBases] AS b
        INNER JOIN [dbo].[Customers] AS c ON b.[Id] = c.[Id] ON a.[CustomerId] = b.[Id];";

        private const string ExpandTwoLevelCollectionMemberPageTestSql =
@"SELECT  a.[Id] ,
        a.[CreateDate] ,
        a.[ModifyDate] ,
        a.[CustomerId] ,
        a.[State] ,
        b.[Id] AS [Id1] ,
        b.[Key] ,
        b.[Price] ,
        b.[Quantity] ,
        b.[Discount] ,
        b.[OrderId] ,
        b.[ProductId] ,
        b.[Id1] AS [Id2] ,
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
                            f.[Key] ,
                            f.[Price] ,
                            f.[Quantity] ,
                            e.[Discount] ,
                            e.[ProductId] ,
                            g.[Id] AS [Id1] ,
                            g.[Code] ,
                            g.[Name] ,
                            h.[Category] ,
                            h.[IsValid] ,
                            h.[UpdateDate]
                    FROM    [dbo].[OrderDetailBases] AS f
                            INNER JOIN [dbo].[OrderDetails] AS e ON f.[Id] = e.[Id]
                            LEFT JOIN [dbo].[ProductBases] AS g
                            INNER JOIN [dbo].[Products] AS h ON g.[Id] = h.[Id] ON e.[ProductId] = g.[Id]
                  ) AS b ON b.[OrderId] = a.[Id];";
#else
        private const string ExpandOneLevelObjectMemberPageTestSql =
@"SELECT  a.[Id] ,
        a.[CreateDate] ,
        a.[ModifyDate] ,
        a.[CustomerId] ,
        a.[State] ,
        b.[Id] AS [Id1] ,
        b.[Code] ,
        b.[Name] ,
        c.[Address1] ,
        c.[Address2] ,
        c.[Zip]
FROM    ( SELECT    d.[Id] ,
                    d.[CreateDate] ,
                    d.[ModifyDate] ,
                    d.[CustomerId] ,
                    d.[State]
          FROM      ( SELECT    e.[Id] ,
                                e.[CreateDate] ,
                                e.[ModifyDate] ,
                                f.[CustomerId] ,
                                f.[State] ,
                                ROW_NUMBER() OVER ( ORDER BY e.[Id] ASC ) AS RowIndex
                      FROM      [dbo].[OrderBases] AS e
                                INNER JOIN [dbo].[Orders] AS f ON e.[Id] = f.[Id]
                    ) d
          WHERE     d.RowIndex > 5
                    AND d.RowIndex <= 10
        ) AS a
        INNER JOIN [dbo].[CustomerBases] AS b
        INNER JOIN [dbo].[Customers] AS c ON b.[Id] = c.[Id] ON a.[CustomerId] = b.[Id];";

        private const string ExpandTwoLevelCollectionMemberPageTestSql =
@"SELECT  a.[Id] ,
        a.[CreateDate] ,
        a.[ModifyDate] ,
        a.[CustomerId] ,
        a.[State] ,
        b.[Id] AS [Id1] ,
        b.[Key] ,
        b.[Price] ,
        b.[Quantity] ,
        b.[Discount] ,
        b.[OrderId] ,
        b.[ProductId] ,
        b.[Id1] AS [Id2] ,
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
                            g.[Key] ,
                            g.[Price] ,
                            g.[Quantity] ,
                            f.[Discount] ,
                            f.[ProductId] ,
                            h.[Id] AS [Id1] ,
                            h.[Code] ,
                            h.[Name] ,
                            i.[Category] ,
                            i.[IsValid] ,
                            i.[UpdateDate]
                    FROM    [dbo].[OrderDetailBases] AS g
                            INNER JOIN [dbo].[OrderDetails] AS f ON g.[Id] = f.[Id]
                            LEFT JOIN [dbo].[ProductBases] AS h
                            INNER JOIN [dbo].[Products] AS i ON h.[Id] = i.[Id] ON f.[ProductId] = h.[Id]
                  ) AS b ON b.[OrderId] = a.[Id];";
#endif

        private const string ExpandOneLevelObjectMemberStrTestSql =
        @"SELECT  a.[Id] ,
        a.[CreateDate] ,
        a.[ModifyDate] ,
        b.[CustomerId] ,
        b.[State] ,
        c.[Id] AS [Id1] ,
        c.[Code] ,
        c.[Name] ,
        d.[Address1] ,
        d.[Address2] ,
        d.[Zip]
FROM    [dbo].[OrderBases] AS a
        INNER JOIN [dbo].[Orders] AS b ON a.[Id] = b.[Id]
        INNER JOIN [dbo].[CustomerBases] AS c
        INNER JOIN [dbo].[Customers] AS d ON c.[Id] = d.[Id] ON b.[CustomerId] = c.[Id];";

        private const string ExpandOneLevelCollectionMemberStrTestSql =
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
                  ) AS c ON c.[OrderId] = a.[Id];";

        private const string ExpandTwoLevelMemberStrTestSql =
        @"SELECT  a.[Id] ,
        a.[CreateDate] ,
        a.[ModifyDate] ,
        b.[CustomerId] ,
        b.[State] ,
        c.[Id] AS [Id1] ,
        c.[Code] ,
        c.[Name] ,
        d.[Address1] ,
        d.[Address2] ,
        d.[Zip] ,
        e.[Id] AS [Id2] ,
        e.[CreateDate] AS [CreateDate1] ,
        e.[ModifyDate] AS [ModifyDate1] ,
        e.[CustomerId] AS [CustomerId1] ,
        e.[State] AS [State1]
FROM    [dbo].[OrderBases] AS a
        INNER JOIN [dbo].[Orders] AS b ON a.[Id] = b.[Id]
        INNER JOIN [dbo].[CustomerBases] AS c
        INNER JOIN [dbo].[Customers] AS d ON c.[Id] = d.[Id] ON b.[CustomerId] = c.[Id]
        LEFT JOIN ( SELECT  f.[CustomerId] ,
                            g.[Id] ,
                            g.[CreateDate] ,
                            g.[ModifyDate] ,
                            f.[State]
                    FROM    [dbo].[OrderBases] AS g
                            INNER JOIN [dbo].[Orders] AS f ON g.[Id] = f.[Id]
                  ) AS e ON e.[CustomerId] = c.[Id];";

       

        private const string ExpandOneLevelObjectMemberTestSql =
        @"SELECT  a.[Id] ,
        a.[CreateDate] ,
        a.[ModifyDate] ,
        b.[CustomerId] ,
        b.[State] ,
        c.[Id] AS [Id1] ,
        c.[Code] ,
        c.[Name] ,
        d.[Address1] ,
        d.[Address2] ,
        d.[Zip]
FROM    [dbo].[OrderBases] AS a
        INNER JOIN [dbo].[Orders] AS b ON a.[Id] = b.[Id]
        INNER JOIN [dbo].[CustomerBases] AS c
        INNER JOIN [dbo].[Customers] AS d ON c.[Id] = d.[Id] ON b.[CustomerId] = c.[Id];";

        private const string ExpandTwoLevelMemberTestSql =
        @"SELECT  a.[Id] ,
        a.[CreateDate] ,
        a.[ModifyDate] ,
        b.[CustomerId] ,
        b.[State] ,
        c.[Id] AS [Id1] ,
        c.[Code] ,
        c.[Name] ,
        d.[Address1] ,
        d.[Address2] ,
        d.[Zip] ,
        e.[Id] AS [Id2] ,
        e.[CreateDate] AS [CreateDate1] ,
        e.[ModifyDate] AS [ModifyDate1] ,
        e.[CustomerId] AS [CustomerId1] ,
        e.[State] AS [State1]
FROM    [dbo].[OrderBases] AS a
        INNER JOIN [dbo].[Orders] AS b ON a.[Id] = b.[Id]
        INNER JOIN [dbo].[CustomerBases] AS c
        INNER JOIN [dbo].[Customers] AS d ON c.[Id] = d.[Id] ON b.[CustomerId] = c.[Id]
        LEFT JOIN ( SELECT  f.[CustomerId] ,
                            g.[Id] ,
                            g.[CreateDate] ,
                            g.[ModifyDate] ,
                            f.[State]
                    FROM    [dbo].[OrderBases] AS g
                            INNER JOIN [dbo].[Orders] AS f ON g.[Id] = f.[Id]
                  ) AS e ON e.[CustomerId] = c.[Id];";

        private const string ExpandOneLevelCollectionMemberTestSql =
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
                  ) AS c ON c.[OrderId] = a.[Id];";

        
        private const string ExpandTwoLevelCollectionMemberTestSql =
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
        c.[ProductId] ,
        c.[Id1] AS [Id2] ,
        c.[Code] ,
        c.[Name] ,
        c.[Category] ,
        c.[IsValid] ,
        c.[UpdateDate]
FROM    [dbo].[OrderBases] AS a
        INNER JOIN [dbo].[Orders] AS b ON a.[Id] = b.[Id]
        LEFT JOIN ( SELECT  d.[OrderId] ,
                            e.[Id] ,
                            e.[Key] ,
                            e.[Price] ,
                            e.[Quantity] ,
                            d.[Discount] ,
                            d.[ProductId] ,
                            f.[Id] AS [Id1] ,
                            f.[Code] ,
                            f.[Name] ,
                            g.[Category] ,
                            g.[IsValid] ,
                            g.[UpdateDate]
                    FROM    [dbo].[OrderDetailBases] AS e
                            INNER JOIN [dbo].[OrderDetails] AS d ON e.[Id] = d.[Id]
                            LEFT JOIN [dbo].[ProductBases] AS f
                            INNER JOIN [dbo].[Products] AS g ON f.[Id] = g.[Id] ON d.[ProductId] = f.[Id]
                  ) AS c ON c.[OrderId] = a.[Id];";

        private const string ExpandOneLevelCollectionMemberFilterTestSql =
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
                    WHERE   e.[Id] > @p0
                  ) AS c ON c.[OrderId] = a.[Id];";

        private const string ExpandTwoLevelCollectionMemberFilterTestSql =
        @"SELECT  a.[Id] ,
        a.[Code] ,
        a.[Name] ,
        b.[Address1] ,
        b.[Address2] ,
        b.[Zip] ,
        c.[Id] AS [Id1] ,
        c.[CreateDate] ,
        c.[ModifyDate] ,
        c.[CustomerId] ,
        c.[State] ,
        c.[Id1] AS [Id2] ,
        c.[Key] ,
        c.[Price] ,
        c.[Quantity] ,
        c.[Discount] ,
        c.[OrderId] ,
        c.[ProductId]
FROM    [dbo].[CustomerBases] AS a
        INNER JOIN [dbo].[Customers] AS b ON a.[Id] = b.[Id]
        LEFT JOIN ( SELECT  d.[CustomerId] ,
                            e.[Id] ,
                            e.[CreateDate] ,
                            e.[ModifyDate] ,
                            d.[State] ,
                            f.[Id] AS [Id1] ,
                            f.[Key] ,
                            f.[Price] ,
                            f.[Quantity] ,
                            f.[Discount] ,
                            f.[OrderId] ,
                            f.[ProductId]
                    FROM    [dbo].[OrderBases] AS e
                            INNER JOIN [dbo].[Orders] AS d ON e.[Id] = d.[Id]
                            LEFT JOIN ( SELECT  g.[OrderId] ,
                                                h.[Id] ,
                                                h.[Key] ,
                                                h.[Price] ,
                                                h.[Quantity] ,
                                                g.[Discount] ,
                                                g.[ProductId]
                                        FROM    [dbo].[OrderDetailBases] AS h
                                                INNER JOIN [dbo].[OrderDetails]
                                                AS g ON h.[Id] = g.[Id]
                                        WHERE   h.[Id] > @p0
                                      ) AS f ON f.[OrderId] = e.[Id]
                    WHERE   e.[Id] > @p1
                  ) AS c ON c.[CustomerId] = a.[Id];";

    }
}