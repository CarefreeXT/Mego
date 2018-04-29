namespace Caredev.Mego.Tests.Core.Query.Simple
{
    public partial class ExpandMemberTest
    {
        private const string ExpandOneLevelObjectMemberPageTestSql =
@"SELECT    a.""Id"", 
            a.""CreateDate"", 
            a.""CustomerId"", 
            a.""ModifyDate"", 
            a.""State"", 
            b.""Id"" AS ""Id1"", 
            b.""Address1"", 
            b.""Address2"", 
            b.""Code"", 
            b.""Name"", 
            b.""Zip""
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
INNER JOIN ""SIMPLE"".""Customers"" b ON a.""CustomerId"" = b.""Id""";

        private const string ExpandTwoLevelCollectionMemberPageTestSql =
@"SELECT    a.""Id"", 
            a.""CreateDate"", 
            a.""CustomerId"", 
            a.""ModifyDate"", 
            a.""State"", 
            b.""Id"" AS ""Id1"", 
            b.""Discount"", 
            b.""Key"", 
            b.""OrderId"", 
            b.""Price"", 
            b.""ProductId"", 
            b.""Quantity"", 
            b.""Id1"" AS ""Id2"", 
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
                        f.""Id"", 
                        f.""Discount"", 
                        f.""Key"", 
                        f.""Price"", 
                        f.""ProductId"", 
                        f.""Quantity"", 
                        g.""Id"" AS ""Id1"", 
                        g.""Category"", 
                        g.""Code"", 
                        g.""IsValid"", 
                        g.""Name"", 
                        g.""UpdateDate""
                FROM ""SIMPLE"".""OrderDetails"" f
                LEFT JOIN ""SIMPLE"".""Products"" g ON f.""ProductId"" = g.""Id""
            ) b ON b.""OrderId"" = a.""Id""";

        private const string ExpandOneLevelObjectMemberStrTestSql =
@"SELECT  a.""Id"" ,
        a.""CreateDate"" ,
        a.""CustomerId"" ,
        a.""ModifyDate"" ,
        a.""State"" ,
        b.""Id"" AS ""Id1"" ,
        b.""Address1"" ,
        b.""Address2"" ,
        b.""Code"" ,
        b.""Name"" ,
        b.""Zip""
FROM    ""SIMPLE"".""Orders"" a
        INNER JOIN ""SIMPLE"".""Customers"" b ON a.""CustomerId"" = b.""Id""";

        private const string ExpandOneLevelCollectionMemberStrTestSql =
@"SELECT  a.""Id"" ,
        a.""CreateDate"" ,
        a.""CustomerId"" ,
        a.""ModifyDate"" ,
        a.""State"" ,
        b.""Id"" AS ""Id1"" ,
        b.""Discount"" ,
        b.""Key"" ,
        b.""OrderId"" ,
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
                  ) b ON b.""OrderId"" = a.""Id""";

        private const string ExpandTwoLevelMemberStrTestSql =
@"SELECT  a.""Id"" ,
        a.""CreateDate"" ,
        a.""CustomerId"" ,
        a.""ModifyDate"" ,
        a.""State"" ,
        b.""Id"" AS ""Id1"" ,
        b.""Address1"" ,
        b.""Address2"" ,
        b.""Code"" ,
        b.""Name"" ,
        b.""Zip"" ,
        c.""Id"" AS ""Id2"" ,
        c.""CreateDate"" AS ""CreateDate1"" ,
        c.""CustomerId"" AS ""CustomerId1"" ,
        c.""ModifyDate"" AS ""ModifyDate1"" ,
        c.""State"" AS ""State1""
FROM    ""SIMPLE"".""Orders"" a
        INNER JOIN ""SIMPLE"".""Customers"" b ON a.""CustomerId"" = b.""Id""
        LEFT JOIN ( SELECT  d.""CustomerId"" ,
                            d.""Id"" ,
                            d.""CreateDate"" ,
                            d.""ModifyDate"" ,
                            d.""State""
                    FROM    ""SIMPLE"".""Orders"" d
                  ) c ON c.""CustomerId"" = b.""Id""";

        
        private const string ExpandOneLevelObjectMemberTestSql =
@"SELECT  a.""Id"" ,
        a.""CreateDate"" ,
        a.""CustomerId"" ,
        a.""ModifyDate"" ,
        a.""State"" ,
        b.""Id"" AS ""Id1"" ,
        b.""Address1"" ,
        b.""Address2"" ,
        b.""Code"" ,
        b.""Name"" ,
        b.""Zip""
FROM    ""SIMPLE"".""Orders"" a
        INNER JOIN ""SIMPLE"".""Customers"" b ON a.""CustomerId"" = b.""Id""";

        private const string ExpandTwoLevelMemberTestSql =
@"SELECT  a.""Id"" ,
        a.""CreateDate"" ,
        a.""CustomerId"" ,
        a.""ModifyDate"" ,
        a.""State"" ,
        b.""Id"" AS ""Id1"" ,
        b.""Address1"" ,
        b.""Address2"" ,
        b.""Code"" ,
        b.""Name"" ,
        b.""Zip"" ,
        c.""Id"" AS ""Id2"" ,
        c.""CreateDate"" AS ""CreateDate1"" ,
        c.""CustomerId"" AS ""CustomerId1"" ,
        c.""ModifyDate"" AS ""ModifyDate1"" ,
        c.""State"" AS ""State1""
FROM    ""SIMPLE"".""Orders"" a
        INNER JOIN ""SIMPLE"".""Customers"" b ON a.""CustomerId"" = b.""Id""
        LEFT JOIN ( SELECT  d.""CustomerId"" ,
                            d.""Id"" ,
                            d.""CreateDate"" ,
                            d.""ModifyDate"" ,
                            d.""State""
                    FROM    ""SIMPLE"".""Orders"" d
                  ) c ON c.""CustomerId"" = b.""Id""";

        private const string ExpandOneLevelCollectionMemberTestSql =
@"SELECT  a.""Id"" ,
        a.""CreateDate"" ,
        a.""CustomerId"" ,
        a.""ModifyDate"" ,
        a.""State"" ,
        b.""Id"" AS ""Id1"" ,
        b.""Discount"" ,
        b.""Key"" ,
        b.""OrderId"" ,
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
                  ) b ON b.""OrderId"" = a.""Id""";
        
        private const string ExpandTwoLevelCollectionMemberTestSql =
@"SELECT  a.""Id"" ,
        a.""CreateDate"" ,
        a.""CustomerId"" ,
        a.""ModifyDate"" ,
        a.""State"" ,
        b.""Id"" AS ""Id1"" ,
        b.""Discount"" ,
        b.""Key"" ,
        b.""OrderId"" ,
        b.""Price"" ,
        b.""ProductId"" ,
        b.""Quantity"" ,
        b.""Id1"" AS ""Id2"" ,
        b.""Category"" ,
        b.""Code"" ,
        b.""IsValid"" ,
        b.""Name"" ,
        b.""UpdateDate""
FROM    ""SIMPLE"".""Orders"" a
        LEFT JOIN ( SELECT  c.""OrderId"" ,
                            c.""Id"" ,
                            c.""Discount"" ,
                            c.""Key"" ,
                            c.""Price"" ,
                            c.""ProductId"" ,
                            c.""Quantity"" ,
                            d.""Id"" AS ""Id1"" ,
                            d.""Category"" ,
                            d.""Code"" ,
                            d.""IsValid"" ,
                            d.""Name"" ,
                            d.""UpdateDate""
                    FROM    ""SIMPLE"".""OrderDetails"" c
                            LEFT JOIN ""SIMPLE"".""Products"" d ON c.""ProductId"" = d.""Id""
                  ) b ON b.""OrderId"" = a.""Id""";

        private const string ExpandOneLevelCollectionMemberFilterTestSql =
@"SELECT  a.""Id"" ,
        a.""CreateDate"" ,
        a.""CustomerId"" ,
        a.""ModifyDate"" ,
        a.""State"" ,
        b.""Id"" AS ""Id1"" ,
        b.""Discount"" ,
        b.""Key"" ,
        b.""OrderId"" ,
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
                    WHERE   c.""Id"" > :p0
                  ) b ON b.""OrderId"" = a.""Id""";

        private const string ExpandTwoLevelCollectionMemberFilterTestSql =
@"SELECT  a.""Id"" ,
        a.""Address1"" ,
        a.""Address2"" ,
        a.""Code"" ,
        a.""Name"" ,
        a.""Zip"" ,
        b.""Id"" AS ""Id1"" ,
        b.""CreateDate"" ,
        b.""CustomerId"" ,
        b.""ModifyDate"" ,
        b.""State"" ,
        b.""Id1"" AS ""Id2"" ,
        b.""Discount"" ,
        b.""Key"" ,
        b.""OrderId"" ,
        b.""Price"" ,
        b.""ProductId"" ,
        b.""Quantity""
FROM    ""SIMPLE"".""Customers"" a
        LEFT JOIN ( SELECT  c.""CustomerId"" ,
                            c.""Id"" ,
                            c.""CreateDate"" ,
                            c.""ModifyDate"" ,
                            c.""State"" ,
                            d.""Id"" AS ""Id1"" ,
                            d.""Discount"" ,
                            d.""Key"" ,
                            d.""OrderId"" ,
                            d.""Price"" ,
                            d.""ProductId"" ,
                            d.""Quantity""
                    FROM    ""SIMPLE"".""Orders"" c
                            LEFT JOIN ( SELECT  e.""OrderId"" ,
                                                e.""Id"" ,
                                                e.""Discount"" ,
                                                e.""Key"" ,
                                                e.""Price"" ,
                                                e.""ProductId"" ,
                                                e.""Quantity""
                                        FROM    ""SIMPLE"".""OrderDetails"" e
                                        WHERE   e.""Id"" > :p0
                                      ) d ON d.""OrderId"" = c.""Id""
                    WHERE   c.""Id"" > :p1
                  ) b ON b.""CustomerId"" = a.""Id""";

    }
}