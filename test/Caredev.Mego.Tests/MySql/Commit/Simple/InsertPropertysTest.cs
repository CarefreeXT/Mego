namespace Caredev.Mego.Tests.Core.Commit.Simple
{
    public partial class InsertPropertysTest
    {

        private const string InsertSingleObjectTestSql =
@"INSERT INTO `customers` (`Id`, `Address1`, `Name`)
  VALUES (@p0, CONCAT(@p1, @p2), CONCAT(@p3, @p1));
SELECT
  a.`Address1`,
  a.`Name`
FROM `customers` AS a
WHERE a.`Id` = @p0;";

        private const string InsertMultiObjectTestSql =
@"CREATE TEMPORARY TABLE `t$1` (
  `Id` int NULL,
  `Address1` longtext NULL,
  `Address2` longtext NULL,
  `Code` longtext NULL,
  `Name` longtext NULL,
  `Zip` longtext NULL
);
INSERT INTO `t$1` (`Id`, `Address1`, `Name`)
  VALUES (@p0, CONCAT(@p1, @p2), CONCAT(@p3, @p1)),
  (@p4, CONCAT(@p1, @p2), CONCAT(@p5, @p1));
INSERT INTO `customers` (`Id`, `Address1`, `Name`)
  SELECT
    a.`Id`,
    a.`Address1`,
    a.`Name`
  FROM `t$1` AS a;
SELECT
  b.`Address1`,
  b.`Name`
FROM `customers` AS b
  INNER JOIN `t$1` AS c
    ON b.`Id` = c.`Id`;"; 

        private const string InsertIdentitySingleObjectTestSql =
@"INSERT INTO `products` (`Category`, `Code`, `IsValid`, `Name`, `UpdateDate`)
  VALUES (@p0, @p1, @p2, CONCAT(@p3, @p4), NOW());
SELECT
  a.`Id`,
  a.`Name`,
  a.`UpdateDate`
FROM `products` AS a
WHERE a.`Id` =LAST_INSERT_ID();";

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
  VALUES (@p0, @p1, @p2, CONCAT(@p3, @p4), NOW(), 0, 1),
  (@p5, @p6, @p7, CONCAT(@p3, @p4), NOW(), 0, 2);
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
  b.`Name`,
  b.`UpdateDate`
FROM `products` AS b
  INNER JOIN `t$1` AS c
    ON b.`Id` = c.`Id`;"; 

        private const string InsertExpressionSingleObjectTestSql =
@"INSERT  INTO `Orders`
        ( `Id` ,
          `CreateDate` ,
          `CustomerId` ,
          `ModifyDate` ,
          `State`
        )
VALUES  ( @p0 ,
          NOW() ,
          @p1 ,
          NOW() ,
          @p2
        );
SELECT  a.`CreateDate` ,
        a.`CustomerId` ,
        a.`ModifyDate`
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
  VALUES (@p0, NOW(), @p1, NOW(), @p2),
  (@p3, NOW(), @p1, NOW(), @p4);
INSERT INTO `orders` (`Id`, `CreateDate`, `CustomerId`, `ModifyDate`, `State`)
  SELECT
    a.`Id`,
    a.`CreateDate`,
    a.`CustomerId`,
    a.`ModifyDate`,
    a.`State`
  FROM `t$1` AS a;
SELECT
  b.`CreateDate`,
  b.`CustomerId`,
  b.`ModifyDate`
FROM `orders` AS b
  INNER JOIN `t$1` AS c
    ON b.`Id` = c.`Id`;"; 
    }
}