namespace Caredev.Mego.Tests.Core.Query.Simple
{
    public partial class EntityLinqTest
    {
        private const string QueryListForSkipTestSql =
@"SELECT    a.""Id"", 
            a.""Category"", 
            a.""Code"", 
            a.""IsValid"", 
            a.""Name"", 
            a.""UpdateDate"" 
FROM (  SELECT  b.""Id"", 
                b.""Category"", 
                b.""Code"", 
                b.""IsValid"", 
                b.""Name"", 
                b.""UpdateDate"", 
                ROW_NUMBER() OVER ( ORDER BY b.""Id"" ASC ) RowIndex
        FROM ""SIMPLE"".""Products"" b 
    ) a
WHERE a.RowIndex > 10";

        private const string QueryListForTakeSkipTestSql =
@"SELECT    a.""Id"", 
            a.""Category"", 
            a.""Code"", 
            a.""IsValid"", 
            a.""Name"", 
            a.""UpdateDate"" 
FROM    (   SELECT * 
            FROM    ( SELECT    c.""Id"", 
                                c.""Category"", 
                                c.""Code"", 
                                c.""IsValid"", 
                                c.""Name"", 
                                c.""UpdateDate"", 
                                ROW_NUMBER() OVER ( ORDER BY c.""Id"" ASC ) RowIndex
                      FROM ""SIMPLE"".""Products"" c 
                    ) b 
            WHERE b.RowIndex > 10 
        ) a 
WHERE a.RowIndex <= 20";

        private const string QueryListTestSql =
@"SELECT  a.""Id"" ,
        a.""Category"" ,
        a.""Code"" ,
        a.""IsValid"" ,
        a.""Name"" ,
        a.""UpdateDate""
FROM    ""SIMPLE"".""Products"" a";

        private const string QuerySinglePropertyTestSql =
@"SELECT  a.""Code""
FROM    ""SIMPLE"".""Products"" a";

        private const string QueryMultiPropertyTestSql =
@"SELECT  a.""Id"" ,
        a.""Name""
FROM    ""SIMPLE"".""Products"" a";

        private const string QueryPropertyAndExpressionTestSql =
@"SELECT  a.""Id"" ,
        a.""Name"" ,
        SYSDATE AS ""Date""
FROM    ""SIMPLE"".""Products"" a";

        private const string QueryFilterListForConstantTestSql =
@"SELECT  a.""Id"" ,
        a.""Category"" ,
        a.""Code"" ,
        a.""IsValid"" ,
        a.""Name"" ,
        a.""UpdateDate""
FROM    ""SIMPLE"".""Products"" a
WHERE   a.""Code"" = :p0";

        private const string QueryFilterListForVariableTestSql =
@"SELECT  a.""Id"" ,
        a.""Category"" ,
        a.""Code"" ,
        a.""IsValid"" ,
        a.""Name"" ,
        a.""UpdateDate""
FROM    ""SIMPLE"".""Products"" a
WHERE   a.""Code"" = :p0";

        private const string OrderQueryDataTestSql =
@"SELECT  a.""Id"" ,
        a.""Category"" ,
        a.""Code"" ,
        a.""IsValid"" ,
        a.""Name"" ,
        a.""UpdateDate""
FROM    ""SIMPLE"".""Products"" a
ORDER BY a.""Category"" DESC";

        private const string CrossQueryDataTestSql =
@"SELECT  a.""Id"" ,
        a.""Name"" ,
        b.""Id"" AS ""Id1"" ,
        b.""Name"" AS ""Name1""
FROM    ""SIMPLE"".""Products"" a
        CROSS JOIN ""SIMPLE"".""Customers"" b";

        private const string QueryListForTakeTestSql =
@"SELECT    a.* 
FROM (  SELECT  b.""Id"", 
                b.""Category"", 
                b.""Code"", 
                b.""IsValid"", 
                b.""Name"", 
                b.""UpdateDate""
        FROM ""SIMPLE"".""Products"" b
) a 
WHERE ROWNUM <= 10";
        
        private const string QueryMultiPropertyDistinctTestSql =
@"SELECT DISTINCT
        a.""State"" ,
        a.""CustomerId""
FROM    ""SIMPLE"".""Orders"" a";
        
        private const string QueryObjectMemberTestSql =
@"SELECT  a.""Id"" ,
        a.""Address1"" ,
        a.""Address2"" ,
        a.""Code"" ,
        a.""Name"" ,
        a.""Zip""
FROM    ""SIMPLE"".""Orders"" b
        INNER JOIN ""SIMPLE"".""Customers"" a ON b.""CustomerId"" = a.""Id""
WHERE   b.""Id"" > :p0
        AND b.""Id"" < :p1";

        private const string QuerySetOperateTestSql =
@"SELECT  a.""Id"" ,
        a.""Code"" ,
        a.""Name""
FROM    ( SELECT    b.""Id"" ,
                    b.""Code"" ,
                    b.""Name""
          FROM      ""SIMPLE"".""Customers"" b
          UNION ALL
          SELECT    c.""Id"" ,
                    c.""Code"" ,
                    c.""Name""
          FROM      ""SIMPLE"".""Products"" c
        ) a";
    }
}