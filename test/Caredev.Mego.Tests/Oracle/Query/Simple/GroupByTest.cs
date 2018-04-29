namespace Caredev.Mego.Tests.Core.Query.Simple
{
    public partial class GroupByTest
    {
        private const string QuerySimpleKeyAndListPageTestSql =
@"SELECT    a.""Category"", 
            b.""Id"", 
            b.""Category"" AS ""Category1"", 
            b.""Code"", 
            b.""IsValid"", 
            b.""Name"", 
            b.""UpdateDate""
FROM    (   SELECT  c.""Category"" 
            FROM    (   SELECT * 
                        FROM    (   SELECT  e.""Category"", 
                                            ROW_NUMBER() OVER ( ORDER BY e.""Category"" ASC ) RowIndex
                                    FROM    (   SELECT  f.""Category""
                                                FROM ""SIMPLE"".""Products"" f
                                                GROUP BY f.""Category""
                                            ) e 
                                ) d WHERE d.RowIndex > 2 
                    ) c WHERE c.RowIndex <= 4
        ) a
INNER JOIN  (   SELECT  f.""Id"", 
                        f.""Category"", 
                        f.""Code"", 
                        f.""IsValid"", 
                        f.""Name"", 
                        f.""UpdateDate""
                FROM ""SIMPLE"".""Products"" f
            ) b ON b.""Category"" = a.""Category""";

        private const string QuerySimpleKeyTestSql =
      @"SELECT  a.""Category""
FROM    ( SELECT    b.""Category""
          FROM      ""SIMPLE"".""Products"" b
          GROUP BY  b.""Category""
        ) a";

        private const string QueryComplexKeysTestSql =
@"SELECT  a.""Category"" ,
        a.""IsValid""
FROM    ( SELECT    b.""Category"" ,
                    b.""IsValid""
          FROM      ""SIMPLE"".""Products"" b
          GROUP BY  b.""Category"" ,
                    b.""IsValid""
        ) a";

        private const string QueryKeyMembersTestSql =
@"SELECT  a.""Category"" ,
        a.""IsValid""
FROM    ( SELECT    b.""Category"" ,
                    b.""IsValid""
          FROM      ""SIMPLE"".""Products"" b
          GROUP BY  b.""Category"" ,
                    b.""IsValid""
        ) a";

        private const string QueryKeyAndAggregateCountTestSql =
@"SELECT  a.""Category"" ,
        a.""Count""
FROM    ( SELECT    b.""Category"" ,
                    COUNT(1) AS ""Count""
          FROM      ""SIMPLE"".""Products"" b
          GROUP BY  b.""Category""
        ) a";

        private const string QueryKeyAndAggregateMaxTestSql =
@"SELECT  a.""Category"" ,
        a.""MaxId""
FROM    ( SELECT    b.""Category"" ,
                    MAX(b.""Id"") AS ""MaxId""
          FROM      ""SIMPLE"".""Products"" b
          GROUP BY  b.""Category""
        ) a";

        private const string QueryKeyAndAggregateCountMaxTestSql =
@"SELECT  a.""Category"" ,
        a.""Count"" ,
        a.""MaxId""
FROM    ( SELECT    b.""Category"" ,
                    COUNT(1) AS ""Count"" ,
                    MAX(b.""Id"") AS ""MaxId""
          FROM      ""SIMPLE"".""Products"" b
          GROUP BY  b.""Category""
        ) a";

        private const string QuerySimpleKeyAndListTestSql =
@"SELECT  a.""Category"" ,
        b.""Id"" ,
        b.""Category"" AS ""Category1"" ,
        b.""Code"" ,
        b.""IsValid"" ,
        b.""Name"" ,
        b.""UpdateDate""
FROM    ( SELECT    c.""Category""
          FROM      ""SIMPLE"".""Products"" c
          GROUP BY  c.""Category""
        ) a
        INNER JOIN ( SELECT c.""Id"" ,
                            c.""Category"" ,
                            c.""Code"" ,
                            c.""IsValid"" ,
                            c.""Name"" ,
                            c.""UpdateDate""
                     FROM   ""SIMPLE"".""Products"" c
                   ) b ON b.""Category"" = a.""Category""";

        

    }
}