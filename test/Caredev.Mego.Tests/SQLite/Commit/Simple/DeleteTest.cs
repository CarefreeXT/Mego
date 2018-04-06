namespace Caredev.Mego.Tests.Core.Commit.Simple
{
    public partial class DeleteTest
    {
        private const string DeleteSingleTestSql =
@"DELETE FROM    [OrderDetails]
WHERE   [Id] = @p0;";

        private const string DeleteMultiForKeyTestSql =
@"DELETE FROM    [OrderDetails]
WHERE   [Id] IN ( @p0, @p1, @p2 );";

        private const string DeleteMultiForKeysTestSql =
@"DELETE FROM [Warehouses]
WHERE [Id] = @p0 AND [Number] = @p1;
DELETE FROM [Warehouses]
WHERE [Id] = @p2 AND [Number] = @p3;"; 

        private const string DeleteStatementForExpressionTestSql =
@"DELETE FROM    [Warehouses]
WHERE   [Id] > @p0;";

        private const string DeleteStatementForQueryTestSql =
@"DELETE  a
FROM    [Warehouses] AS a
        CROSS JOIN [Customers] AS b
WHERE   a.[Id] > b.[Id]
        AND a.[Number] > @p0;";
    }
}