namespace Caredev.Mego.Tests.Core.Query.Simple
{
    public partial class EntityLinqTest
    {

        private const string QueryListForSkipTestSql =
@"SELECT
  a.[Id],
  a.[Category],
  a.[Code],
  a.[IsValid],
  a.[Name],
  a.[UpdateDate]
FROM [products] AS a
ORDER BY a.[Id] ASC
LIMIT 2147483647 OFFSET 10;";

        private const string QueryListForTakeSkipTestSql =
@"SELECT
  a.[Id],
  a.[Category],
  a.[Code],
  a.[IsValid],
  a.[Name],
  a.[UpdateDate]
FROM [Products] AS a
ORDER BY a.[Id] ASC
LIMIT 10 OFFSET 10;";

        private const string QueryListTestSql =
@"SELECT
  a.[Id],
  a.[Category],
  a.[Code],
  a.[IsValid],
  a.[Name],
  a.[UpdateDate]
FROM [Products] AS a;";

        private const string QuerySinglePropertyTestSql =
@"SELECT
  a.[Code]
FROM [Products] AS a;";

        private const string QueryMultiPropertyTestSql =
@"SELECT
  a.[Id],
  a.[Name]
FROM [Products] AS a;";

        private const string QueryPropertyAndExpressionTestSql =
@"SELECT  a.[Id] ,
        a.[Name] ,
        datetime('now') AS [Date]
FROM    [Products] AS a;";

        private const string QueryFilterListForConstantTestSql =
@"SELECT  a.[Id] ,
        a.[Category] ,
        a.[Code] ,
        a.[IsValid] ,
        a.[Name] ,
        a.[UpdateDate]
FROM    [Products] AS a
WHERE   a.[Code] = @p0;";

        private const string QueryFilterListForVariableTestSql =
@"SELECT  a.[Id] ,
        a.[Category] ,
        a.[Code] ,
        a.[IsValid] ,
        a.[Name] ,
        a.[UpdateDate]
FROM    [Products] AS a
WHERE   a.[Code] = @p0;";

        private const string OrderQueryDataTestSql =
@"SELECT  a.[Id] ,
        a.[Category] ,
        a.[Code] ,
        a.[IsValid] ,
        a.[Name] ,
        a.[UpdateDate]
FROM    [Products] AS a
ORDER BY a.[Category] DESC;";

        private const string CrossQueryDataTestSql =
@"SELECT  a.[Id] ,
        a.[Name] ,
        b.[Id] AS [Id1] ,
        b.[Name] AS [Name1]
FROM    [Products] AS a
        CROSS JOIN [Customers] AS b;";

        private const string QueryListForTakeTestSql =
@"SELECT
  a.[Id],
  a.[Category],
  a.[Code],
  a.[IsValid],
  a.[Name],
  a.[UpdateDate]
FROM [Products] AS a
LIMIT 10;";
        
        private const string QueryMultiPropertyDistinctTestSql =
@"SELECT DISTINCT
        a.[State] ,
        a.[CustomerId]
FROM    [Orders] AS a;";
        
        private const string QueryObjectMemberTestSql =
@"SELECT  a.[Id] ,
        a.[Address1] ,
        a.[Address2] ,
        a.[Code] ,
        a.[Name] ,
        a.[Zip]
FROM    [Orders] AS b
        INNER JOIN [Customers] AS a ON b.[CustomerId] = a.[Id]
WHERE   b.[Id] > @p0
        AND b.[Id] < @p1;";

        private const string QuerySetOperateTestSql =
@"SELECT  a.[Id] ,
        a.[Code] ,
        a.[Name]
FROM    ( SELECT    b.[Id] ,
                    b.[Code] ,
                    b.[Name]
          FROM      [Customers] AS b
          UNION ALL
          SELECT    c.[Id] ,
                    c.[Code] ,
                    c.[Name]
          FROM      [Products] AS c
        ) AS a;";
    }
}