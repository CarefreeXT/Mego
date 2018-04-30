namespace Caredev.Mego.Tests.Core.Commit.Simple
{
    public partial class DeleteTest
    {
        private const string DeleteSingleTestSql =
@"DELETE FROM    ""SIMPLE"".""OrderDetails"" a
WHERE   a.""Id"" = :p0
        AND a.""Discount"" = :p1";

        private const string DeleteMultiForKeyTestSql =
@"DELETE FROM ""SIMPLE"".""OrderDetails"" a
WHERE a.""Id"" = :p0";

        private const string DeleteMultiForKeysTestSql =
@"DELETE FROM ""SIMPLE"".""Warehouses"" a
WHERE a.""Id"" = :p0 AND a.""Number"" = :p1 AND a.""Address"" = :p2"; 

        private const string DeleteStatementForExpressionTestSql =
@"DELETE FROM    ""SIMPLE"".""Warehouses"" a
WHERE   a.""Id"" > :p0";

        private const string DeleteStatementForQueryTestSql =
@"DELETE FROM   ""SIMPLE"".""Warehouses"" a 
WHERE ROWID IN
    (   SELECT a.ROWID 
        FROM ""SIMPLE"".""Warehouses"" a
        CROSS JOIN ""SIMPLE"".""Customers"" b
        WHERE a.""Id"" > b.""Id"" AND a.""Number"" > :p0
    )";
    }
}