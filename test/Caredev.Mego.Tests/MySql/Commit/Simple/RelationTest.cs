namespace Caredev.Mego.Tests.Core.Commit.Simple
{
    public partial class RelationTest
    {

        private const string AddObjectSingleTestSql =
@"UPDATE `orders` AS a
SET a.`CustomerId` = @p0
WHERE a.`Id` = @p1;";

        private const string AddCollectionSingleTestSql =
@"UPDATE `orderdetails` AS a
SET a.`OrderId` = @p0
WHERE a.`Id` = @p1;";

        private const string RemoveObjectSingleTestSql =
@"UPDATE  `Orders` AS a
SET     a.`CustomerId` = NULL
WHERE   a.`Id` = @p0;";

        private const string RemoveCollectionSingleTestSql =
@"UPDATE  `OrderDetails` AS a
SET     a.`OrderId` = NULL
WHERE   a.`Id` = @p0;";

        private const string AddObjectMultiTestSql =
@"CREATE TEMPORARY TABLE `t$1` (
  `Id` int NULL,
  `CustomerId` int NULL
);
INSERT INTO `t$1` (`Id`, `CustomerId`)
  VALUES (@p0, @p1),
  (@p2, @p3);
UPDATE `orders` AS a
INNER JOIN `t$1` AS b
  ON a.`Id` = b.`Id`
SET a.`CustomerId` = b.`CustomerId`;"; 
        
        private const string AddCollectionMultiTestSql =
@"CREATE TEMPORARY TABLE `t$1` (
  `Id` int NULL,
  `OrderId` int NULL
);
INSERT INTO `t$1` (`Id`, `OrderId`)
  VALUES (@p0, @p1),
  (@p2, @p3);
UPDATE `orderdetails` AS a
INNER JOIN `t$1` AS b
  ON a.`Id` = b.`Id`
SET a.`OrderId` = b.`OrderId`;";

        private const string RemoveObjectMultiTestSql =
@"UPDATE `orders` AS a
SET a.`CustomerId` = NULL
WHERE a.`Id` IN (@p0, @p1);";

        private const string RemoveCollectionMultiTestSql =
@"UPDATE  `OrderDetails` AS a
SET     a.`OrderId` = NULL
WHERE   a.`Id` IN ( @p0, @p1 );";

        private const string AddCompositeSingleTestSql =
@"INSERT  INTO `OrderDetails`
        ( `OrderId`, `ProductId` )
VALUES  ( @p0, @p1 );";

        private const string RemoveCompositeSingleTestSql =
@"DELETE  a
FROM    `OrderDetails` AS a
WHERE   a.`OrderId` = @p0
        AND a.`ProductId` = @p1;";

        private const string AddCompositeMultiTestSql =
@"INSERT  INTO `OrderDetails`
        ( `OrderId`, `ProductId` )
VALUES  ( @p0, @p1 ),
        ( @p2, @p3 );";

        private const string RemoveCompositeMultiTestSql =
@"CREATE TEMPORARY TABLE `t$1` (
  `OrderId` int NULL,
  `ProductId` int NULL
);
INSERT INTO `t$1` (`OrderId`, `ProductId`)
  VALUES (@p0, @p1),
  (@p2, @p3);
DELETE a
  FROM `orderdetails` AS a
    INNER JOIN `t$1` AS b
      ON a.`OrderId` = b.`OrderId`
      AND a.`ProductId` = b.`ProductId`;"; 
    }
}