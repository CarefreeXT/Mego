namespace Caredev.Mego.Tests.Core.Commit.Simple
{
    public partial class InsertTest
    {
        private const string InsertSingleObjectTestSql =
@"INSERT INTO `Customers`
(`Id`,`Address1`,`Address2`,`Code`,`Name`,`Zip`)
VALUES(@p0,@p1,@p2,@p3,@p4,@p5);";
        
        private const string InsertMultiObjectTestSql =
@"INSERT  INTO `Customers`
        ( `Id`, `Address1`, `Address2`, `Code`, `Name`, `Zip` )
VALUES  ( @p0, @p1, @p2, @p3, @p4, @p5 ),
        ( @p6, @p1, @p2, @p3, @p7, @p5 );";

        private const string InsertIdentitySingleObjectTestSql =
@"INSERT INTO `products` (`Category`, `Code`, `IsValid`, `Name`, `UpdateDate`)
  VALUES (@p0, @p1, @p2, @p3, NOW());
SELECT
  a.`Id`,
  a.`UpdateDate`
FROM `products` AS a
WHERE a.`Id` = LAST_INSERT_ID();";

        private const string InsertIdentityMultiObjectTestSql =
@"CREATE TEMPORARY TABLE `t$1` (
  `Id` int NULL,
  `Category` int NULL,
  `Code` longtext NULL,
  `IsValid` bit(1) NULL,
  `Name` longtext NULL,
  `UpdateDate` datetime NULL,
  `RowIndex` int NULL
);
INSERT INTO `t$1` (`Category`, `Code`, `IsValid`, `Name`, `UpdateDate`, `Id`, `RowIndex`)
  VALUES (@p0, @p1, @p2, @p3, NOW(), 0, 1),
  (@p4, @p5, @p6, @p3, NOW(), 0, 2);
INSERT INTO `products` (`Category`, `Code`, `IsValid`, `Name`, `UpdateDate`)
  SELECT
    a.`Category`,
    a.`Code`,
    a.`IsValid`,
    a.`Name`,
    a.`UpdateDate`
  FROM `t$1` AS a
  WHERE a.`RowIndex` = 1;
UPDATE `t$1` AS a
SET a.`Id` = LAST_INSERT_ID()
WHERE a.`RowIndex` = 1;
INSERT INTO `products` (`Category`, `Code`, `IsValid`, `Name`, `UpdateDate`)
  SELECT
    a.`Category`,
    a.`Code`,
    a.`IsValid`,
    a.`Name`,
    a.`UpdateDate`
  FROM `t$1` AS a
  WHERE a.`RowIndex` = 2;
UPDATE `t$1` AS a
SET a.`Id` = LAST_INSERT_ID()
WHERE a.`RowIndex` = 2;
SELECT
  b.`Id`,
  b.`UpdateDate`
FROM `products` AS b
  INNER JOIN `t$1` AS c
    ON b.`Id` = c.`Id`;"; 

        private const string InsertExpressionSingleObjectTestSql =
@"INSERT  INTO `Orders`
( `Id` ,`CreateDate` ,`CustomerId` ,`ModifyDate` ,`State`)
VALUES  ( @p0 ,NOW() ,@p1 ,@p2 ,@p3);
SELECT  a.`CreateDate`
FROM    `Orders` AS a
WHERE   a.`Id` = @p0;";

        private const string InsertExpressionMultiObjectTestSql =
@"CREATE TEMPORARY TABLE `t$1` (
  `Id` int NULL,
  `CreateDate` datetime NULL,
  `CustomerId` int NULL,
  `ModifyDate` datetime NULL,
  `State` int NULL
);
INSERT INTO `t$1` (`Id`, `CreateDate`, `CustomerId`, `ModifyDate`, `State`)
  VALUES (@p0, NOW(), @p1, @p2, @p3),
  (@p4, NOW(), @p1, @p2, @p5);
INSERT INTO `orders` (`Id`, `CreateDate`, `CustomerId`, `ModifyDate`, `State`)
  SELECT
    a.`Id`,
    a.`CreateDate`,
    a.`CustomerId`,
    a.`ModifyDate`,
    a.`State`
  FROM `t$1` AS a;
SELECT
  b.`CreateDate`
FROM `orders` AS b
  INNER JOIN `t$1` AS c
    ON b.`Id` = c.`Id`;"; 

        private const string InsertQueryTestSql =
@"INSERT  INTO `Customers`
        ( `Id` ,
          `Name` ,
          `Code` ,
          `Address1`
        )
        SELECT  a.`Id` + @p0 AS `Id` ,
                a.`Name` ,
                a.`Name` ,
                a.`Code`
        FROM    `Products` AS a
        WHERE   a.`Id` > @p1;";
    }
}