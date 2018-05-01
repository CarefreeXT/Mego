namespace Caredev.Mego.Tests.Core.Commit.Simple
{
    public partial class RelationTest
    {

        private const string AddObjectSingleTestSql =
@"UPDATE  [Orders]
SET     [CustomerId] = @p0
WHERE   [Id] = @p1";

        private const string AddCollectionSingleTestSql =
@"UPDATE  [OrderDetails]
SET     [OrderId] = @p0
WHERE   [Id] = @p1";

        private const string RemoveObjectSingleTestSql =
@"UPDATE  [Orders]
SET     [CustomerId] = NULL
WHERE   [Id] = @p0";

        private const string RemoveCollectionSingleTestSql =
@"UPDATE  [OrderDetails]
SET     [OrderId] = NULL
WHERE   [Id] = @p0";
        
        private const string AddObjectMultiTestSql =
@"UPDATE  [Orders]
SET     [CustomerId] = @p0
WHERE   [Id] = @p1"; 
        
        private const string AddCollectionMultiTestSql =
@"UPDATE  [OrderDetails]
SET     [OrderId] = @p0
WHERE   [Id] = @p1"; 

        private const string RemoveObjectMultiTestSql =
@"UPDATE  [Orders]
SET     [CustomerId] = NULL
WHERE   [Id] = @p0";

        private const string RemoveCollectionMultiTestSql =
@"UPDATE  [OrderDetails]
SET     [OrderId] = NULL
WHERE   [Id] = @p0";

        private const string AddCompositeSingleTestSql =
@"INSERT  INTO [OrderDetails]
        ( [OrderId], [ProductId] )
VALUES  ( @p0, @p1 )";

        private const string RemoveCompositeSingleTestSql =
@"DELETE  [OrderDetails]
WHERE   [OrderId] = @p0
        AND [ProductId] = @p1";
        
        private const string AddCompositeMultiTestSql =
@"INSERT  INTO [OrderDetails]
        ( [OrderId], [ProductId] )
VALUES  ( @p0, @p1 )";

        private const string RemoveCompositeMultiTestSql =
@"DELETE  [OrderDetails]
WHERE   [OrderId] = @p0
        AND [ProductId] = @p1";
    }
}