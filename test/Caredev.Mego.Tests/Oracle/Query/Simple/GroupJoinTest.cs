namespace Caredev.Mego.Tests.Core.Query.Simple
{
    public partial class GroupJoinTest
    {
        private const string QueryBodyAndListPageTestSql =
@"SELECT    a.""Id"", 
            a.""CreateDate"", 
            a.""CustomerId"", 
            a.""ModifyDate"", 
            a.""State"", 
            b.""OrderId"", 
            b.""Id"" AS ""Id1"", 
            b.""Discount"", 
            b.""Key"", 
            b.""Price"", 
            b.""ProductId"", 
            b.""Quantity""
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
                    ) c WHERE c.RowIndex <= 10) a
            LEFT JOIN   (   SELECT  f.""OrderId"", 
                                    f.""Id"", 
                                    f.""Discount"", 
                                    f.""Key"", 
                                    f.""Price"", 
                                    f.""ProductId"", 
                                    f.""Quantity""
            FROM ""SIMPLE"".""OrderDetails"" f
        ) b ON a.""Id"" = b.""OrderId""";

        private const string SimpleGroupJoinTestSql =
@"SELECT  a.""Id"" ,
        a.""Category"" ,
        a.""Code"" ,
        a.""IsValid"" ,
        a.""Name"" ,
        a.""UpdateDate"" ,
        b.""Id"" AS ""Id1"" ,
        b.""Address1"" ,
        b.""Address2"" ,
        b.""Code"" AS ""Code1"" ,
        b.""Name"" AS ""Name1"" ,
        b.""Zip""
FROM    ""SIMPLE"".""Products"" a
        INNER JOIN ""SIMPLE"".""Customers"" b ON a.""Id"" = b.""Id""";

        private const string GroupJoinDefaultTestSql =
@"SELECT  a.""Id"" ,
        a.""Category"" ,
        a.""Code"" ,
        a.""IsValid"" ,
        a.""Name"" ,
        a.""UpdateDate"" ,
        b.""Id"" AS ""Id1"" ,
        b.""Address1"" ,
        b.""Address2"" ,
        b.""Code"" AS ""Code1"" ,
        b.""Name"" AS ""Name1"" ,
        b.""Zip""
FROM    ""SIMPLE"".""Products"" a
        LEFT JOIN ""SIMPLE"".""Customers"" b ON a.""Id"" = b.""Id""";
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
                    FROM    ""SIMPLE"".""OrderDetails"" c
                    GROUP BY c.""OrderId""
                  ) b ON a.""Id"" = b.""OrderId""";
        private const string QueryBodyAndAggregateMaxTestSql =
@"SELECT  a.""Id"" ,
        a.""CreateDate"" ,
        a.""CustomerId"" ,
        a.""ModifyDate"" ,
        a.""State"" ,
        b.""MaxId""
FROM    ""SIMPLE"".""Orders"" a
        LEFT JOIN ( SELECT  c.""OrderId"" ,
                            MAX(c.""Id"") AS ""MaxId""
                    FROM    ""SIMPLE"".""OrderDetails"" c
                    GROUP BY c.""OrderId""
                  ) b ON a.""Id"" = b.""OrderId""";
        private const string QueryBodyAndAggregateCountMaxTestSql =
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
                            MAX(c.""Id"") AS ""MaxId""
                    FROM    ""SIMPLE"".""OrderDetails"" c
                    GROUP BY c.""OrderId""
                  ) b ON a.""Id"" = b.""OrderId""";
        private const string QueryBodyAndListTestSql =
@"SELECT  a.""Id"" ,
        a.""CreateDate"" ,
        a.""CustomerId"" ,
        a.""ModifyDate"" ,
        a.""State"" ,
        b.""OrderId"" ,
        b.""Id"" AS ""Id1"" ,
        b.""Discount"" ,
        b.""Key"" ,
        b.""Price"" ,
        b.""ProductId"" ,
        b.""Quantity""
FROM    ""SIMPLE"".""Orders"" a
        LEFT JOIN ( SELECT  c.""OrderId"" ,
                            c.""Id"" ,
                            c.""Discount"" ,
                            c.""Key"" ,
                            c.""Price"" ,
                            c.""ProductId"" ,
                            c.""Quantity""
                    FROM    ""SIMPLE"".""OrderDetails"" c
                  ) b ON a.""Id"" = b.""OrderId""";
    }
}