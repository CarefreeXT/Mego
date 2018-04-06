namespace Caredev.Mego.Tests.Core.Query.Simple
{
    public partial class ExpandMemberTest
    {
#if SQLSERVER2012
        private const string ExpandOneLevelObjectMemberPageTestSql =
@"SELECT  a.[Id] ,
        a.[CreateDate] ,
        a.[CustomerId] ,
        a.[ModifyDate] ,
        a.[State] ,
        b.[Id] AS [Id1] ,
        b.[Address1] ,
        b.[Address2] ,
        b.[Code] ,
        b.[Name] ,
        b.[Zip]
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
        INNER JOIN [dbo].[Customers] AS b ON a.[CustomerId] = b.[Id];";
        private const string ExpandTwoLevelCollectionMemberPageTestSql =
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
        b.[Quantity] ,
        b.[Id1] AS [Id2] ,
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
                            d.[Id] ,
                            d.[Discount] ,
                            d.[Key] ,
                            d.[Price] ,
                            d.[ProductId] ,
                            d.[Quantity] ,
                            e.[Id] AS [Id1] ,
                            e.[Category] ,
                            e.[Code] ,
                            e.[IsValid] ,
                            e.[Name] ,
                            e.[UpdateDate]
                    FROM    [dbo].[OrderDetails] AS d
                            LEFT JOIN [dbo].[Products] AS e ON d.[ProductId] = e.[Id]
                  ) AS b ON b.[OrderId] = a.[Id];";
#else
        private const string ExpandOneLevelObjectMemberPageTestSql =
@"SELECT  a.[Id] ,
        a.[CreateDate] ,
        a.[CustomerId] ,
        a.[ModifyDate] ,
        a.[State] ,
        b.[Id] AS [Id1] ,
        b.[Address1] ,
        b.[Address2] ,
        b.[Code] ,
        b.[Name] ,
        b.[Zip]
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
        INNER JOIN [dbo].[Customers] AS b ON a.[CustomerId] = b.[Id];";
        private const string ExpandTwoLevelCollectionMemberPageTestSql =
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
        b.[Quantity] ,
        b.[Id1] AS [Id2] ,
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
                            e.[Id] ,
                            e.[Discount] ,
                            e.[Key] ,
                            e.[Price] ,
                            e.[ProductId] ,
                            e.[Quantity] ,
                            f.[Id] AS [Id1] ,
                            f.[Category] ,
                            f.[Code] ,
                            f.[IsValid] ,
                            f.[Name] ,
                            f.[UpdateDate]
                    FROM    [dbo].[OrderDetails] AS e
                            LEFT JOIN [dbo].[Products] AS f ON e.[ProductId] = f.[Id]
                  ) AS b ON b.[OrderId] = a.[Id];";
#endif

        private const string ExpandOneLevelObjectMemberStrTestSql =
@"SELECT  a.[Id] ,
        a.[CreateDate] ,
        a.[CustomerId] ,
        a.[ModifyDate] ,
        a.[State] ,
        b.[Id] AS [Id1] ,
        b.[Address1] ,
        b.[Address2] ,
        b.[Code] ,
        b.[Name] ,
        b.[Zip]
FROM    [dbo].[Orders] AS a
        INNER JOIN [dbo].[Customers] AS b ON a.[CustomerId] = b.[Id];";

        private const string ExpandOneLevelCollectionMemberStrTestSql =
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

        private const string ExpandTwoLevelMemberStrTestSql =
@"SELECT  a.[Id] ,
        a.[CreateDate] ,
        a.[CustomerId] ,
        a.[ModifyDate] ,
        a.[State] ,
        b.[Id] AS [Id1] ,
        b.[Address1] ,
        b.[Address2] ,
        b.[Code] ,
        b.[Name] ,
        b.[Zip] ,
        c.[Id] AS [Id2] ,
        c.[CreateDate] AS [CreateDate1] ,
        c.[CustomerId] AS [CustomerId1] ,
        c.[ModifyDate] AS [ModifyDate1] ,
        c.[State] AS [State1]
FROM    [dbo].[Orders] AS a
        INNER JOIN [dbo].[Customers] AS b ON a.[CustomerId] = b.[Id]
        LEFT JOIN ( SELECT  d.[CustomerId] ,
                            d.[Id] ,
                            d.[CreateDate] ,
                            d.[ModifyDate] ,
                            d.[State]
                    FROM    [dbo].[Orders] AS d
                  ) AS c ON c.[CustomerId] = b.[Id];";

        
        private const string ExpandOneLevelObjectMemberTestSql =
@"SELECT  a.[Id] ,
        a.[CreateDate] ,
        a.[CustomerId] ,
        a.[ModifyDate] ,
        a.[State] ,
        b.[Id] AS [Id1] ,
        b.[Address1] ,
        b.[Address2] ,
        b.[Code] ,
        b.[Name] ,
        b.[Zip]
FROM    [dbo].[Orders] AS a
        INNER JOIN [dbo].[Customers] AS b ON a.[CustomerId] = b.[Id];";

        private const string ExpandTwoLevelMemberTestSql =
@"SELECT  a.[Id] ,
        a.[CreateDate] ,
        a.[CustomerId] ,
        a.[ModifyDate] ,
        a.[State] ,
        b.[Id] AS [Id1] ,
        b.[Address1] ,
        b.[Address2] ,
        b.[Code] ,
        b.[Name] ,
        b.[Zip] ,
        c.[Id] AS [Id2] ,
        c.[CreateDate] AS [CreateDate1] ,
        c.[CustomerId] AS [CustomerId1] ,
        c.[ModifyDate] AS [ModifyDate1] ,
        c.[State] AS [State1]
FROM    [dbo].[Orders] AS a
        INNER JOIN [dbo].[Customers] AS b ON a.[CustomerId] = b.[Id]
        LEFT JOIN ( SELECT  d.[CustomerId] ,
                            d.[Id] ,
                            d.[CreateDate] ,
                            d.[ModifyDate] ,
                            d.[State]
                    FROM    [dbo].[Orders] AS d
                  ) AS c ON c.[CustomerId] = b.[Id];";

        private const string ExpandOneLevelCollectionMemberTestSql =
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
        
        private const string ExpandTwoLevelCollectionMemberTestSql =
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
        b.[Quantity] ,
        b.[Id1] AS [Id2] ,
        b.[Category] ,
        b.[Code] ,
        b.[IsValid] ,
        b.[Name] ,
        b.[UpdateDate]
FROM    [dbo].[Orders] AS a
        LEFT JOIN ( SELECT  c.[OrderId] ,
                            c.[Id] ,
                            c.[Discount] ,
                            c.[Key] ,
                            c.[Price] ,
                            c.[ProductId] ,
                            c.[Quantity] ,
                            d.[Id] AS [Id1] ,
                            d.[Category] ,
                            d.[Code] ,
                            d.[IsValid] ,
                            d.[Name] ,
                            d.[UpdateDate]
                    FROM    [dbo].[OrderDetails] AS c
                            LEFT JOIN [dbo].[Products] AS d ON c.[ProductId] = d.[Id]
                  ) AS b ON b.[OrderId] = a.[Id];";

        private const string ExpandOneLevelCollectionMemberFilterTestSql =
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
                    WHERE   c.[Id] > @p0
                  ) AS b ON b.[OrderId] = a.[Id];";

        private const string ExpandTwoLevelCollectionMemberFilterTestSql =
@"SELECT  a.[Id] ,
        a.[Address1] ,
        a.[Address2] ,
        a.[Code] ,
        a.[Name] ,
        a.[Zip] ,
        b.[Id] AS [Id1] ,
        b.[CreateDate] ,
        b.[CustomerId] ,
        b.[ModifyDate] ,
        b.[State] ,
        b.[Id1] AS [Id2] ,
        b.[Discount] ,
        b.[Key] ,
        b.[OrderId] ,
        b.[Price] ,
        b.[ProductId] ,
        b.[Quantity]
FROM    [dbo].[Customers] AS a
        LEFT JOIN ( SELECT  c.[CustomerId] ,
                            c.[Id] ,
                            c.[CreateDate] ,
                            c.[ModifyDate] ,
                            c.[State] ,
                            d.[Id] AS [Id1] ,
                            d.[Discount] ,
                            d.[Key] ,
                            d.[OrderId] ,
                            d.[Price] ,
                            d.[ProductId] ,
                            d.[Quantity]
                    FROM    [dbo].[Orders] AS c
                            LEFT JOIN ( SELECT  e.[OrderId] ,
                                                e.[Id] ,
                                                e.[Discount] ,
                                                e.[Key] ,
                                                e.[Price] ,
                                                e.[ProductId] ,
                                                e.[Quantity]
                                        FROM    [dbo].[OrderDetails] AS e
                                        WHERE   e.[Id] > @p0
                                      ) AS d ON d.[OrderId] = c.[Id]
                    WHERE   c.[Id] > @p1
                  ) AS b ON b.[CustomerId] = a.[Id];";

    }
}