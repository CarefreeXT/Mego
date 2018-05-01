namespace Caredev.Mego.Tests.Core.Commit.Simple
{
    public partial class DeleteTest
    {
        private const string DeleteSingleTestSql =
@"DELETE  [OrderDetails]
WHERE   [Id] = @p0";

        private const string DeleteMultiForKeyTestSql =
@"DELETE  [OrderDetails]
WHERE   [Id] = @p0";
        
        private const string DeleteMultiForKeysTestSql =
@"DELETE  [Warehouses]
WHERE   [Id] = @p0
        AND [Number] = @p1"; 

        private const string DeleteStatementForExpressionTestSql =
@"DELETE [Warehouses]
WHERE [Id] > @p0";

        private const string DeleteStatementForQueryTestSql =
@"DELETE  a
FROM    [Warehouses] AS a
        CROSS JOIN [Customers] AS b
WHERE   a.[Id] > b.[Id]
        AND a.[Number] > @p0;";
    }
}