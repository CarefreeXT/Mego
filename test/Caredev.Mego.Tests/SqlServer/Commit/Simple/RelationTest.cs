namespace Caredev.Mego.Tests.Core.Commit.Simple
{
    public partial class RelationTest
    {

        private const string AddObjectSingleTestSql =
@"UPDATE  a
SET     [CustomerId] = @p0
FROM    [dbo].[Orders] AS a
WHERE   a.[Id] = @p1;";

        private const string AddCollectionSingleTestSql =
@"UPDATE  a
SET     [OrderId] = @p0
FROM    [dbo].[OrderDetails] AS a
WHERE   a.[Id] = @p1;";

        private const string RemoveObjectSingleTestSql =
@"UPDATE  a
SET     [CustomerId] = NULL
FROM    [dbo].[Orders] AS a
WHERE   a.[Id] = @p0;";

        private const string RemoveCollectionSingleTestSql =
@"UPDATE  a
SET     [OrderId] = NULL
FROM    [dbo].[OrderDetails] AS a
WHERE   a.[Id] = @p0;";
        
        private const string AddObjectMultiTestSql =
@"DECLARE @t0 AS TABLE
    (
      [Id] INT NULL ,
      [CustomerId] INT NULL
    );
INSERT  INTO @t0
        ( [Id], [CustomerId] )
VALUES  ( @p0, @p1 ),
        ( @p2, @p3 );
UPDATE  a
SET     [CustomerId] = b.[CustomerId]
FROM    [dbo].[Orders] AS a
        INNER JOIN @t0 AS b ON a.[Id] = b.[Id];"; 
        
        private const string AddCollectionMultiTestSql =
@"DECLARE @t0 AS TABLE
    (
      [Id] INT NULL ,
      [OrderId] INT NULL
    );
INSERT  INTO @t0
        ( [Id], [OrderId] )
VALUES  ( @p0, @p1 ),
        ( @p2, @p3 );
UPDATE  a
SET     [OrderId] = b.[OrderId]
FROM    [dbo].[OrderDetails] AS a
        INNER JOIN @t0 AS b ON a.[Id] = b.[Id];"; 

        private const string RemoveObjectMultiTestSql =
@"UPDATE  a
SET     [CustomerId] = NULL
FROM    [dbo].[Orders] AS a
WHERE   a.[Id] IN ( @p0, @p1 );";

        private const string RemoveCollectionMultiTestSql =
@"UPDATE  a
SET     [OrderId] = NULL
FROM    [dbo].[OrderDetails] AS a
WHERE   a.[Id] IN ( @p0, @p1 );";

        private const string AddCompositeSingleTestSql =
@"INSERT  INTO [dbo].[OrderDetails]
        ( [OrderId], [ProductId] )
VALUES  ( @p0, @p1 );";

        private const string RemoveCompositeSingleTestSql =
@"DELETE  a
FROM    [dbo].[OrderDetails] AS a
WHERE   a.[OrderId] = @p0
        AND a.[ProductId] = @p1;";
        
        private const string AddCompositeMultiTestSql =
@"INSERT  INTO [dbo].[OrderDetails]
        ( [OrderId], [ProductId] )
VALUES  ( @p0, @p1 ),
        ( @p2, @p3 );";

        private const string RemoveCompositeMultiTestSql =
@"DECLARE @t0 AS TABLE
    (
      [OrderId] INT NULL ,
      [ProductId] INT NULL
    );
INSERT  INTO @t0
        ( [OrderId], [ProductId] )
VALUES  ( @p0, @p1 ),
        ( @p2, @p3 );
DELETE  a
FROM    [dbo].[OrderDetails] AS a
        INNER JOIN @t0 AS b ON a.[OrderId] = b.[OrderId]
                               AND a.[ProductId] = b.[ProductId];"; 
    }
}