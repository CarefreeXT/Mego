namespace Caredev.Mego.Tests.Core.Query.Simple
{
    public partial class EntityLinqTest
    {
#if SQLSERVER2012
        private const string QueryListForSkipTestSql =
@"SELECT  a.[Id] ,
        a.[Category] ,
        a.[Code] ,
        a.[IsValid] ,
        a.[Name] ,
        a.[UpdateDate]
FROM    [dbo].[Products] AS a
ORDER BY a.[Id] ASC
        OFFSET 10 ROWS;";

        private const string QueryListForTakeSkipTestSql =
@"SELECT  a.[Id] ,
        a.[Category] ,
        a.[Code] ,
        a.[IsValid] ,
        a.[Name] ,
        a.[UpdateDate]
FROM    [dbo].[Products] AS a
ORDER BY a.[Id] ASC
        OFFSET 10 ROWS
FETCH NEXT 10 ROWS ONLY;";
#else
        private const string QueryListForSkipTestSql =
@"SELECT  a.[Id] ,
        a.[Category] ,
        a.[Code] ,
        a.[IsValid] ,
        a.[Name] ,
        a.[UpdateDate]
FROM    ( SELECT    b.[Id] ,
                    b.[Category] ,
                    b.[Code] ,
                    b.[IsValid] ,
                    b.[Name] ,
                    b.[UpdateDate] ,
                    ROW_NUMBER() OVER ( ORDER BY b.[Id] ASC ) AS RowIndex
          FROM      [dbo].[Products] AS b
        ) a
WHERE   a.RowIndex > 10;";
        
        private const string QueryListForTakeSkipTestSql =
@"SELECT  a.[Id] ,
        a.[Category] ,
        a.[Code] ,
        a.[IsValid] ,
        a.[Name] ,
        a.[UpdateDate]
FROM    ( SELECT    b.[Id] ,
                    b.[Category] ,
                    b.[Code] ,
                    b.[IsValid] ,
                    b.[Name] ,
                    b.[UpdateDate] ,
                    ROW_NUMBER() OVER ( ORDER BY b.[Id] ASC ) AS RowIndex
          FROM      [dbo].[Products] AS b
        ) a
WHERE   a.RowIndex > 10
        AND a.RowIndex <= 20;";
#endif

        private const string QueryListTestSql =
@"SELECT  a.[Id] ,
        a.[Category] ,
        a.[Code] ,
        a.[IsValid] ,
        a.[Name] ,
        a.[UpdateDate]
FROM    [dbo].[Products] AS a;";

        private const string QuerySinglePropertyTestSql =
@"SELECT  a.[Code]
FROM    [dbo].[Products] AS a;";

        private const string QueryMultiPropertyTestSql =
@"SELECT  a.[Id] ,
        a.[Name]
FROM    [dbo].[Products] AS a;";

        private const string QueryPropertyAndExpressionTestSql =
@"SELECT  a.[Id] ,
        a.[Name] ,
        GETDATE() AS [Date]
FROM    [dbo].[Products] AS a;";

        private const string QueryFilterListForConstantTestSql =
@"SELECT  a.[Id] ,
        a.[Category] ,
        a.[Code] ,
        a.[IsValid] ,
        a.[Name] ,
        a.[UpdateDate]
FROM    [dbo].[Products] AS a
WHERE   a.[Code] = @p0;";

        private const string QueryFilterListForVariableTestSql =
@"SELECT  a.[Id] ,
        a.[Category] ,
        a.[Code] ,
        a.[IsValid] ,
        a.[Name] ,
        a.[UpdateDate]
FROM    [dbo].[Products] AS a
WHERE   a.[Code] = @p0;";

        private const string OrderQueryDataTestSql =
@"SELECT  a.[Id] ,
        a.[Category] ,
        a.[Code] ,
        a.[IsValid] ,
        a.[Name] ,
        a.[UpdateDate]
FROM    [dbo].[Products] AS a
ORDER BY a.[Category] DESC;";

        private const string CrossQueryDataTestSql =
@"SELECT  a.[Id] ,
        a.[Name] ,
        b.[Id] AS [Id1] ,
        b.[Name] AS [Name1]
FROM    [dbo].[Products] AS a
        CROSS JOIN [dbo].[Customers] AS b;";

        private const string QueryListForTakeTestSql =
@"SELECT TOP 10
        a.[Id] ,
        a.[Category] ,
        a.[Code] ,
        a.[IsValid] ,
        a.[Name] ,
        a.[UpdateDate]
FROM    [dbo].[Products] AS a;";
        
        private const string QueryMultiPropertyDistinctTestSql =
@"SELECT DISTINCT
        a.[State] ,
        a.[CustomerId]
FROM    [dbo].[Orders] AS a;";
        
        private const string QueryObjectMemberTestSql =
@"SELECT  a.[Id] ,
        a.[Address1] ,
        a.[Address2] ,
        a.[Code] ,
        a.[Name] ,
        a.[Zip]
FROM    [dbo].[Orders] AS b
        INNER JOIN [dbo].[Customers] AS a ON b.[CustomerId] = a.[Id]
WHERE   b.[Id] > @p0
        AND b.[Id] < @p1;";

        private const string QuerySetOperateTestSql =
@"SELECT  a.[Id] ,
        a.[Code] ,
        a.[Name]
FROM    ( SELECT    b.[Id] ,
                    b.[Code] ,
                    b.[Name]
          FROM      [dbo].[Customers] AS b
          UNION ALL
          SELECT    c.[Id] ,
                    c.[Code] ,
                    c.[Name]
          FROM      [dbo].[Products] AS c
        ) AS a;";
    }
}