namespace Caredev.Mego.Tests.Core.Query.Simple
{
    public partial class CompositeCollectionMemberTest
    {
        private const string QueryBodyAndListPageTestSql =
@"SELECT    a.""Id"", 
            a.""CreateDate"", 
            a.""CustomerId"", 
            a.""ModifyDate"", 
            a.""State"", 
            b.""Id"" AS ""Id1"", 
            b.""Category"", 
            b.""Code"", 
            b.""IsValid"", 
            b.""Name"", 
            b.""UpdateDate""
FROM    (   SELECT  c.""Id"", 
                    c.""CreateDate"", 
                    c.""CustomerId"", 
                    c.""ModifyDate"", 
                    c.""State"" 
            FROM    (   SELECT * 
                        FROM    (   SELECT  e.""Id"", 
                                            e.""CreateDate"", 
                                            e.""CustomerId"", 
                                            e.""ModifyDate"", 
                                            e.""State"", 
                                            ROW_NUMBER() OVER ( ORDER BY e.""Id"" ASC ) RowIndex
                                    FROM ""SIMPLE"".""Orders"" e 
                                ) d WHERE d.RowIndex > 5 
                    ) c WHERE c.RowIndex <= 10
        ) a
LEFT JOIN   (   SELECT  f.""OrderId"", 
                        g.""Id"", 
                        g.""Category"", 
                        g.""Code"", 
                        g.""IsValid"", 
                        g.""Name"", 
                        g.""UpdateDate""
                FROM ""SIMPLE"".""Products"" g
                INNER JOIN ""SIMPLE"".""OrderDetails"" f ON g.""Id"" = f.""ProductId""
            ) b ON b.""OrderId"" = a.""Id""";

        private const string SimpleJoinTestSql =
@"SELECT  a.""Id"" ,
        a.""CreateDate"" ,
        a.""CustomerId"" ,
        a.""ModifyDate"" ,
        a.""State"" ,
        b.""Id"" AS ""Id1"" ,
        b.""Category"" ,
        b.""Code"" ,
        b.""IsValid"" ,
        b.""Name"" ,
        b.""UpdateDate""
FROM    ""SIMPLE"".""Orders"" a
        INNER JOIN ""SIMPLE"".""OrderDetails"" c ON c.""OrderId"" = a.""Id""
        INNER JOIN ""SIMPLE"".""Products"" b ON b.""Id"" = c.""ProductId""";

        private const string SimpleJoinDefaultTestSql =
@"SELECT  a.""Id"" ,
        a.""CreateDate"" ,
        a.""CustomerId"" ,
        a.""ModifyDate"" ,
        a.""State"" ,
        b.""Id"" AS ""Id1"" ,
        b.""Category"" ,
        b.""Code"" ,
        b.""IsValid"" ,
        b.""Name"" ,
        b.""UpdateDate""
FROM    ""SIMPLE"".""Orders"" a
        LEFT JOIN ""SIMPLE"".""OrderDetails"" c ON c.""OrderId"" = a.""Id""
        INNER JOIN ""SIMPLE"".""Products"" b ON b.""Id"" = c.""ProductId""";

        private const string QueryBodyAndAggregateCountTestSql =
@"SELECT  a.""Id"" ,
        a.""CreateDate"" ,
        a.""CustomerId"" ,
        a.""ModifyDate"" ,
        a.""State"" ,
        b.""Count""
FROM    ""SIMPLE"".""Orders"" a
        LEFT JOIN ( SELECT  c.""OrderId"" ,
                            COUNT(1) AS ""Count""
                    FROM    ""SIMPLE"".""Products"" d
                            INNER JOIN ""SIMPLE"".""OrderDetails"" c ON d.""Id"" = c.""ProductId""
                    GROUP BY c.""OrderId""
                  ) b ON b.""OrderId"" = a.""Id""";

        private const string QueryBodyAndAggregateMaxTestSql =
@"SELECT  a.""Id"" ,
        a.""CreateDate"" ,
        a.""CustomerId"" ,
        a.""ModifyDate"" ,
        a.""State"" ,
        b.""MaxId""
FROM    ""SIMPLE"".""Orders"" a
        LEFT JOIN ( SELECT  c.""OrderId"" ,
                            MAX(d.""Id"") AS ""MaxId""
                    FROM    ""SIMPLE"".""Products"" d
                            INNER JOIN ""SIMPLE"".""OrderDetails"" c ON d.""Id"" = c.""ProductId""
                    GROUP BY c.""OrderId""
                  ) b ON b.""OrderId"" = a.""Id""";

        private const string QueryBodyAndAggregateCountMaxSql =
@"SELECT  a.""Id"" ,
        a.""CreateDate"" ,
        a.""CustomerId"" ,
        a.""ModifyDate"" ,
        a.""State"" ,
        b.""Count"" ,
        b.""MaxId""
FROM    ""SIMPLE"".""Orders"" a
        LEFT JOIN ( SELECT  c.""OrderId"" ,
                            COUNT(1) AS ""Count"" ,
                            MAX(d.""Id"") AS ""MaxId""
                    FROM    ""SIMPLE"".""Products"" d
                            INNER JOIN ""SIMPLE"".""OrderDetails"" c ON d.""Id"" = c.""ProductId""
                    GROUP BY c.""OrderId""
                  ) b ON b.""OrderId"" = a.""Id""";
        
        private const string QueryBodyAndListTestSql =
@"SELECT  a.""Id"" ,
        a.""CreateDate"" ,
        a.""CustomerId"" ,
        a.""ModifyDate"" ,
        a.""State"" ,
        b.""Id"" AS ""Id1"" ,
        b.""Category"" ,
        b.""Code"" ,
        b.""IsValid"" ,
        b.""Name"" ,
        b.""UpdateDate""
FROM    ""SIMPLE"".""Orders"" a
        LEFT JOIN ( SELECT  c.""OrderId"" ,
                            d.""Id"" ,
                            d.""Category"" ,
                            d.""Code"" ,
                            d.""IsValid"" ,
                            d.""Name"" ,
                            d.""UpdateDate""
                    FROM    ""SIMPLE"".""Products"" d
                            INNER JOIN ""SIMPLE"".""OrderDetails"" c ON d.""Id"" = c.""ProductId""
                  ) b ON b.""OrderId"" = a.""Id""";

    }
}