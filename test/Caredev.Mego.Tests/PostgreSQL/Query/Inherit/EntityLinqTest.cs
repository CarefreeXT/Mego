namespace Caredev.Mego.Tests.Core.Query.Inherit
{
    public partial class EntityLinqTest
    {
#if SQLSERVER2012
        private const string QueryListForSkipTestSql =
@"SELECT  a.[Id] ,
        a.[Code] ,
        a.[Name] ,
        b.[Category] ,
        b.[IsValid] ,
        b.[UpdateDate]
FROM    [dbo].[ProductBases] AS a
        INNER JOIN [dbo].[Products] AS b ON a.[Id] = b.[Id]
ORDER BY a.[Id] ASC
        OFFSET 10 ROWS;";

        private const string QueryListForTakeSkipTestSql =
        @"SELECT  a.[Id] ,
        a.[Code] ,
        a.[Name] ,
        b.[Category] ,
        b.[IsValid] ,
        b.[UpdateDate]
FROM    [dbo].[ProductBases] AS a
        INNER JOIN [dbo].[Products] AS b ON a.[Id] = b.[Id]
ORDER BY a.[Id] ASC
        OFFSET 10 ROWS
FETCH NEXT 10 ROWS ONLY;";
#else
        private const string QueryListForSkipTestSql =
        @"SELECT  a.[Id] ,
        a.[Code] ,
        a.[Name] ,
        a.[Category] ,
        a.[IsValid] ,
        a.[UpdateDate]
FROM    ( SELECT    b.[Id] ,
                    b.[Code] ,
                    b.[Name] ,
                    c.[Category] ,
                    c.[IsValid] ,
                    c.[UpdateDate] ,
                    ROW_NUMBER() OVER ( ORDER BY b.[Id] ASC ) AS RowIndex
          FROM      [dbo].[ProductBases] AS b
                    INNER JOIN [dbo].[Products] AS c ON b.[Id] = c.[Id]
        ) a
WHERE   a.RowIndex > 10;";

        private const string QueryListForTakeSkipTestSql =
        @"SELECT  a.[Id] ,
        a.[Code] ,
        a.[Name] ,
        a.[Category] ,
        a.[IsValid] ,
        a.[UpdateDate]
FROM    ( SELECT    b.[Id] ,
                    b.[Code] ,
                    b.[Name] ,
                    c.[Category] ,
                    c.[IsValid] ,
                    c.[UpdateDate] ,
                    ROW_NUMBER() OVER ( ORDER BY b.[Id] ASC ) AS RowIndex
          FROM      [dbo].[ProductBases] AS b
                    INNER JOIN [dbo].[Products] AS c ON b.[Id] = c.[Id]
        ) a
WHERE   a.RowIndex > 10
        AND a.RowIndex <= 20;";
#endif
        private const string QueryListTestSql =
@"SELECT  a.[Id] ,
        a.[Code] ,
        a.[Name] ,
        b.[Category] ,
        b.[IsValid] ,
        b.[UpdateDate]
FROM    [dbo].[ProductBases] AS a
        INNER JOIN [dbo].[Products] AS b ON a.[Id] = b.[Id];";

        private const string QuerySinglePropertyTestSql =
        @"SELECT  a.[Code]
FROM    [dbo].[ProductBases] AS a
        INNER JOIN [dbo].[Products] AS b ON a.[Id] = b.[Id];";

        private const string QueryMultiPropertyTestSql =
        @"SELECT  a.[Id] ,
        a.[Name]
FROM    [dbo].[ProductBases] AS a
        INNER JOIN [dbo].[Products] AS b ON a.[Id] = b.[Id];";

        private const string QueryMultiPropertyDistinctTestSql =
        @"SELECT DISTINCT
        a.[State] ,
        a.[CustomerId]
FROM    [dbo].[OrderBases] AS b
        INNER JOIN [dbo].[Orders] AS a ON b.[Id] = a.[Id];";

        private const string QueryPropertyAndExpressionTestSql =
        @"SELECT  a.[Id] ,
        a.[Name] ,
        GETDATE() AS [Date]
FROM    [dbo].[ProductBases] AS a
        INNER JOIN [dbo].[Products] AS b ON a.[Id] = b.[Id];";

        private const string QueryFilterListForConstantTestSql =
        @"SELECT  a.[Id] ,
        a.[Code] ,
        a.[Name] ,
        b.[Category] ,
        b.[IsValid] ,
        b.[UpdateDate]
FROM    [dbo].[ProductBases] AS a
        INNER JOIN [dbo].[Products] AS b ON a.[Id] = b.[Id]
WHERE   a.[Code] = @p0;";

        private const string QueryFilterListForVariableTestSql =
        @"SELECT  a.[Id] ,
        a.[Code] ,
        a.[Name] ,
        b.[Category] ,
        b.[IsValid] ,
        b.[UpdateDate]
FROM    [dbo].[ProductBases] AS a
        INNER JOIN [dbo].[Products] AS b ON a.[Id] = b.[Id]
WHERE   a.[Code] = @p0;";

        private const string OrderQueryDataTestSql =
        @"SELECT  a.[Id] ,
        a.[Code] ,
        a.[Name] ,
        b.[Category] ,
        b.[IsValid] ,
        b.[UpdateDate]
FROM    [dbo].[ProductBases] AS a
        INNER JOIN [dbo].[Products] AS b ON a.[Id] = b.[Id]
ORDER BY b.[Category] DESC;";

        private const string CrossQueryDataTestSql =
        @"SELECT  a.[Id] ,
        a.[Name] ,
        b.[Id] AS [Id1] ,
        b.[Name] AS [Name1]
FROM    [dbo].[ProductBases] AS a
        INNER JOIN [dbo].[Products] AS c ON a.[Id] = c.[Id]
        CROSS JOIN [dbo].[CustomerBases] AS b
        INNER JOIN [dbo].[Customers] AS d ON b.[Id] = d.[Id];";

        private const string QueryListForTakeTestSql =
        @"SELECT TOP 10
        a.[Id] ,
        a.[Code] ,
        a.[Name] ,
        b.[Category] ,
        b.[IsValid] ,
        b.[UpdateDate]
FROM    [dbo].[ProductBases] AS a
        INNER JOIN [dbo].[Products] AS b ON a.[Id] = b.[Id];";

        private const string QuerySetOperateTestSql =
@"SELECT  a.[Id] ,
        a.[Code] ,
        a.[Name]
FROM    ( SELECT    b.[Id] ,
                    b.[Code] ,
                    b.[Name]
          FROM      [dbo].[CustomerBases] AS b
                    INNER JOIN [dbo].[Customers] AS c ON b.[Id] = c.[Id]
          UNION ALL
          SELECT    d.[Id] ,
                    d.[Code] ,
                    d.[Name]
          FROM      [dbo].[ProductBases] AS d
                    INNER JOIN [dbo].[Products] AS e ON d.[Id] = e.[Id]
        ) AS a;";

        private const string QueryObjectMemberTestSql =
        @"SELECT  a.[Id] ,
        a.[Code] ,
        a.[Name] ,
        b.[Address1] ,
        b.[Address2] ,
        b.[Zip]
FROM    [dbo].[OrderBases] AS c
        INNER JOIN [dbo].[Orders] AS d ON c.[Id] = d.[Id]
        INNER JOIN [dbo].[CustomerBases] AS a
        INNER JOIN [dbo].[Customers] AS b ON a.[Id] = b.[Id] ON d.[CustomerId] = a.[Id]
WHERE   c.[Id] > @p0
        AND c.[Id] < @p1;";

    }
}