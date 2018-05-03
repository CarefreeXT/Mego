namespace Caredev.Mego.Tests.Core.Commit.Simple
{
    public partial class DeleteTest
    {
        private const string DeleteSingleTestSql =
@"DELETE FROM [OrderDetails] AS a
WHERE a.[Id] = @p0";

        private const string DeleteMultiForKeyTestSql =
@"DELETE FROM [OrderDetails] AS a
WHERE a.[Id] = @p0";

        private const string DeleteMultiForKeysTestSql =
@"DELETE FROM [Warehouses] AS a
WHERE   a.[Id] = @p0
        AND a.[Number] = @p1";

        private const string DeleteStatementForExpressionTestSql =
@"DELETE FROM [Warehouses] AS a
WHERE a.[Id] > @p0";

        private const string DeleteStatementForQueryTestSql =
@"DELETE  a.*
FROM    [Warehouses] AS a
        CROSS JOIN [Customers] AS b
WHERE   a.[Id] > b.[Id]
        AND a.[Number] > @p0;";
    }
}