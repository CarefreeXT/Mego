namespace Caredev.Mego.Tests.Core.Query.Simple
{
    public partial class EntityLinqTest
    {
        private const string QueryListForSkipTestSql =
@"SELECT  a.""Id"" ,
        a.""Category"" ,
        a.""Code"" ,
        a.""IsValid"" ,
        a.""Name"" ,
        a.""UpdateDate""
FROM    ""public"".""Products"" AS a
ORDER BY a.""Id"" ASC
        OFFSET 10;";

        private const string QueryListForTakeSkipTestSql =
@"SELECT  a.""Id"" ,
        a.""Category"" ,
        a.""Code"" ,
        a.""IsValid"" ,
        a.""Name"" ,
        a.""UpdateDate""
FROM    ""public"".""Products"" AS a
ORDER BY a.""Id"" ASC
        LIMIT 10 OFFSET 10;";

        private const string QueryListTestSql =
@"SELECT  a.""Id"" ,
        a.""Category"" ,
        a.""Code"" ,
        a.""IsValid"" ,
        a.""Name"" ,
        a.""UpdateDate""
FROM    ""public"".""Products"" AS a;";

        private const string QuerySinglePropertyTestSql =
@"SELECT  a.""Code""
FROM    ""public"".""Products"" AS a;";

        private const string QueryMultiPropertyTestSql =
@"SELECT  a.""Id"" ,
        a.""Name""
FROM    ""public"".""Products"" AS a;";

        private const string QueryPropertyAndExpressionTestSql =
@"SELECT  a.""Id"" ,
        a.""Name"" ,
        LOCALTIMESTAMP AS ""Date""
FROM    ""public"".""Products"" AS a;";

        private const string QueryFilterListForConstantTestSql =
@"SELECT  a.""Id"" ,
        a.""Category"" ,
        a.""Code"" ,
        a.""IsValid"" ,
        a.""Name"" ,
        a.""UpdateDate""
FROM    ""public"".""Products"" AS a
WHERE   a.""Code"" = @p0;";

        private const string QueryFilterListForVariableTestSql =
@"SELECT  a.""Id"" ,
        a.""Category"" ,
        a.""Code"" ,
        a.""IsValid"" ,
        a.""Name"" ,
        a.""UpdateDate""
FROM    ""public"".""Products"" AS a
WHERE   a.""Code"" = @p0;";

        private const string OrderQueryDataTestSql =
@"SELECT  a.""Id"" ,
        a.""Category"" ,
        a.""Code"" ,
        a.""IsValid"" ,
        a.""Name"" ,
        a.""UpdateDate""
FROM    ""public"".""Products"" AS a
ORDER BY a.""Category"" DESC;";

        private const string CrossQueryDataTestSql =
@"SELECT  a.""Id"" ,
        a.""Name"" ,
        b.""Id"" AS ""Id1"" ,
        b.""Name"" AS ""Name1""
FROM    ""public"".""Products"" AS a
        CROSS JOIN ""public"".""Customers"" AS b;";

        private const string QueryListForTakeTestSql =
@"SELECT a.""Id"" ,
        a.""Category"" ,
        a.""Code"" ,
        a.""IsValid"" ,
        a.""Name"" ,
        a.""UpdateDate""
FROM    ""public"".""Products"" AS a LIMIT 10;";
        
        private const string QueryMultiPropertyDistinctTestSql =
@"SELECT DISTINCT
        a.""State"" ,
        a.""CustomerId""
FROM    ""public"".""Orders"" AS a;";
        
        private const string QueryObjectMemberTestSql =
@"SELECT  a.""Id"" ,
        a.""Address1"" ,
        a.""Address2"" ,
        a.""Code"" ,
        a.""Name"" ,
        a.""Zip""
FROM    ""public"".""Orders"" AS b
        INNER JOIN ""public"".""Customers"" AS a ON b.""CustomerId"" = a.""Id""
WHERE   b.""Id"" > @p0
        AND b.""Id"" < @p1;";

        private const string QuerySetOperateTestSql =
@"SELECT  a.""Id"" ,
        a.""Code"" ,
        a.""Name""
FROM    ( SELECT    b.""Id"" ,
                    b.""Code"" ,
                    b.""Name""
          FROM      ""public"".""Customers"" AS b
          UNION ALL
          SELECT    c.""Id"" ,
                    c.""Code"" ,
                    c.""Name""
          FROM      ""public"".""Products"" AS c
        ) AS a;";
    }
}