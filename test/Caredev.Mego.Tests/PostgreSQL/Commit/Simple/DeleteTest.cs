namespace Caredev.Mego.Tests.Core.Commit.Simple
{
    public partial class DeleteTest
    {
        private const string DeleteSingleTestSql =
@"DELETE FROM    ""public"".""OrderDetails"" AS a
WHERE   a.""Id"" = @p0
        AND a.""Discount"" = @p1;";

        private const string DeleteMultiForKeyTestSql =
@"DELETE FROM    ""public"".""OrderDetails"" AS a
WHERE   a.""Id"" IN ( @p0, @p1, @p2 );";

        private const string DeleteMultiForKeysTestSql =
@"DELETE FROM ""public"".""Warehouses"" AS a
USING       (   VALUES  
                (@p0,@p0,@p1),
                (@p0,@p2,@p3)
            ) AS b (""Id"",""Number"",""Address"")
WHERE       a.""Id"" = b.""Id"" AND a.""Number"" = b.""Number"" AND a.""Address"" = b.""Address"";"; 

        private const string DeleteStatementForExpressionTestSql =
@"DELETE FROM    ""public"".""Warehouses"" AS a
WHERE   a.""Id"" > @p0;";

        private const string DeleteStatementForQueryTestSql =
@"DELETE FROM   ""public"".""Warehouses"" AS a
USING           ""public"".""Customers"" AS b
WHERE   a.""Id"" > b.""Id"" AND a.""Number"" > @p0;";
    }
}