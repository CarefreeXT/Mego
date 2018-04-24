namespace Caredev.Mego.Tests.Core.Query.Simple
{
    public partial class GroupJoinTest
    {
        private const string QueryBodyAndListPageTestSql =
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
FROM    ( SELECT    c.""Id"" ,
                    c.""CreateDate"" ,
                    c.""CustomerId"" ,
                    c.""ModifyDate"" ,
                    c.""State""
          FROM      ""public"".""Orders"" AS c
          ORDER BY  c.""Id"" ASC
                    LIMIT 5 OFFSET 5
        ) AS a
        LEFT JOIN ( SELECT  d.""OrderId"" ,
                            d.""Id"" ,
                            d.""Discount"" ,
                            d.""Key"" ,
                            d.""Price"" ,
                            d.""ProductId"" ,
                            d.""Quantity""
                    FROM    ""public"".""OrderDetails"" AS d
                  ) AS b ON a.""Id"" = b.""OrderId"";";

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
FROM    ""public"".""Products"" AS a
        INNER JOIN ""public"".""Customers"" AS b ON a.""Id"" = b.""Id"";";

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
FROM    ""public"".""Products"" AS a
        LEFT JOIN ""public"".""Customers"" AS b ON a.""Id"" = b.""Id"";";
        private const string QueryBodyAndAggregateCountTestSql =
@"SELECT  a.""Id"" ,
        a.""CreateDate"" ,
        a.""CustomerId"" ,
        a.""ModifyDate"" ,
        a.""State"" ,
        b.""Count""
FROM    ""public"".""Orders"" AS a
        LEFT JOIN ( SELECT  c.""OrderId"" ,
                            COUNT(1) AS ""Count""
                    FROM    ""public"".""OrderDetails"" AS c
                    GROUP BY c.""OrderId""
                  ) AS b ON a.""Id"" = b.""OrderId"";";
        private const string QueryBodyAndAggregateMaxTestSql =
@"SELECT  a.""Id"" ,
        a.""CreateDate"" ,
        a.""CustomerId"" ,
        a.""ModifyDate"" ,
        a.""State"" ,
        b.""MaxId""
FROM    ""public"".""Orders"" AS a
        LEFT JOIN ( SELECT  c.""OrderId"" ,
                            MAX(c.""Id"") AS ""MaxId""
                    FROM    ""public"".""OrderDetails"" AS c
                    GROUP BY c.""OrderId""
                  ) AS b ON a.""Id"" = b.""OrderId"";";
        private const string QueryBodyAndAggregateCountMaxTestSql =
@"SELECT  a.""Id"" ,
        a.""CreateDate"" ,
        a.""CustomerId"" ,
        a.""ModifyDate"" ,
        a.""State"" ,
        b.""Count"" ,
        b.""MaxId""
FROM    ""public"".""Orders"" AS a
        LEFT JOIN ( SELECT  c.""OrderId"" ,
                            COUNT(1) AS ""Count"" ,
                            MAX(c.""Id"") AS ""MaxId""
                    FROM    ""public"".""OrderDetails"" AS c
                    GROUP BY c.""OrderId""
                  ) AS b ON a.""Id"" = b.""OrderId"";";
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
FROM    ""public"".""Orders"" AS a
        LEFT JOIN ( SELECT  c.""OrderId"" ,
                            c.""Id"" ,
                            c.""Discount"" ,
                            c.""Key"" ,
                            c.""Price"" ,
                            c.""ProductId"" ,
                            c.""Quantity""
                    FROM    ""public"".""OrderDetails"" AS c
                  ) AS b ON a.""Id"" = b.""OrderId"";";
    }
}