namespace Caredev.Mego.Tests.Core.Commit.Inherit
{
    public partial class InsertPropertysTest 
    {
        private const string InsertSingleObjectTestSql =
@"INSERT INTO `customerbases` (`Id`, `Name`)
  VALUES (@p0, CONCAT(@p1, @p2));
INSERT INTO `customers` (`Id`, `Address1`)
  VALUES (@p0, CONCAT(@p2, @p3));
SELECT
  a.`Name`,
  b.`Address1`
FROM `customerbases` AS a
  INNER JOIN `customers` AS b
    ON a.`Id` = b.`Id`
WHERE a.`Id` = @p0;";

        private const string InsertMultiObjectTestSql =
@"CREATE TEMPORARY TABLE `t$1` (
  `Id` int NULL,
  `Code` longtext NULL,
  `Name` longtext NULL,
  `Address1` longtext NULL,
  `Address2` longtext NULL,
  `Zip` longtext NULL
);
INSERT INTO `t$1` (`Id`, `Name`, `Address1`)
  VALUES (@p0, CONCAT(@p1, @p2), CONCAT(@p2, @p3)),
  (@p4, CONCAT(@p5, @p2), CONCAT(@p2, @p3));
INSERT INTO `customerbases` (`Id`, `Name`)
  SELECT
    a.`Id`,
    a.`Name`
  FROM `t$1` AS a;
INSERT INTO `customers` (`Id`, `Address1`)
  SELECT
    a.`Id`,
    a.`Address1`
  FROM `t$1` AS a;
SELECT
  b.`Name`,
  c.`Address1`
FROM `customerbases` AS b
  INNER JOIN `customers` AS c
    ON b.`Id` = c.`Id`
  INNER JOIN `t$1` AS d
    ON b.`Id` = d.`Id`;"; 

        private const string InsertIdentitySingleObjectTestSql =
@"INSERT INTO `ProductBases` (`Code`, `Name`)
  VALUES (@p0, CONCAT(@p1, @p2));
SET @v0 = LAST_INSERT_ID();
INSERT INTO `products` (`Id`, `Category`, `IsValid`, `UpdateDate`)
  VALUES (@v0, @p3, @p4, NOW());
SELECT
  a.`Id`,
  a.`Name`,
  b.`UpdateDate`
FROM `ProductBases` AS a
  INNER JOIN `products` AS b
    ON a.`Id` = b.`Id`
WHERE a.`Id` = @v0;";

        private const string InsertIdentityMultiObjectTestSql =
@"CREATE TEMPORARY TABLE `t$1` (
  `Id` int NULL,
  `Code` longtext NULL,
  `Name` longtext NULL,
  `Category` int NULL,
  `IsValid` bit(1) NULL,
  `UpdateDate` datetime NULL,
  `RowIndex` int NULL
);
INSERT INTO `t$1` (`Code`, `Name`, `Category`, `IsValid`, `UpdateDate`, `Id`, `RowIndex`)
  VALUES (@p0, CONCAT(@p1, @p2), @p3, @p4, NOW(), 0, 1),
  (@p5, CONCAT(@p1, @p2), @p6, @p7, NOW(), 0, 2);
INSERT INTO `ProductBases` (`Code`, `Name`)
  SELECT
    a.`Code`,
    a.`Name`
  FROM `t$1` AS a
  WHERE a.`RowIndex` = 1;
UPDATE `t$1` AS a
SET a.`Id` = LAST_INSERT_ID()
WHERE a.`RowIndex` = 1;
INSERT INTO `ProductBases` (`Code`, `Name`)
  SELECT
    a.`Code`,
    a.`Name`
  FROM `t$1` AS a
  WHERE a.`RowIndex` = 2;
UPDATE `t$1` AS a
SET a.`Id` = LAST_INSERT_ID()
WHERE a.`RowIndex` = 2;
INSERT INTO `products` (`Id`, `Category`, `IsValid`, `UpdateDate`)
  SELECT
    a.`Id`,
    a.`Category`,
    a.`IsValid`,
    a.`UpdateDate`
  FROM `t$1` AS a;
SELECT
  b.`Id`,
  b.`Name`,
  c.`UpdateDate`
FROM `ProductBases` AS b
  INNER JOIN `products` AS c
    ON b.`Id` = c.`Id`
  INNER JOIN `t$1` AS d
    ON b.`Id` = d.`Id`;"; 

        private const string InsertExpressionSingleObjectTestSql =
@"INSERT  INTO `OrderBases`
        ( `Id`, `CreateDate`, `ModifyDate` )
VALUES  ( @p0, NOW(), NOW() );
INSERT  INTO `Orders`
        ( `Id`, `CustomerId`, `State` )
VALUES  ( @p0, @p1, @p2 );
SELECT  a.`CreateDate` ,
        a.`ModifyDate` ,
        b.`CustomerId`
FROM    `OrderBases` AS a
        INNER JOIN `Orders` AS b ON a.`Id` = b.`Id`
WHERE   a.`Id` = @p0;";

        private const string InsertExpressionMultiObjectTestSql =
@"CREATE TEMPORARY TABLE `t$1` (
  `Id` int NULL,
  `CreateDate` datetime NULL,
  `ModifyDate` datetime NULL,
  `CustomerId` int NULL,
  `State` int NULL
);
INSERT INTO `t$1` (`Id`, `CreateDate`, `ModifyDate`, `CustomerId`, `State`)
  VALUES (@p0, NOW(), NOW(), @p1, @p2),
  (@p3, NOW(), NOW(), @p1, @p4);
INSERT INTO `orderbases` (`Id`, `CreateDate`, `ModifyDate`)
  SELECT
    a.`Id`,
    a.`CreateDate`,
    a.`ModifyDate`
  FROM `t$1` AS a;
INSERT INTO `orders` (`Id`, `CustomerId`, `State`)
  SELECT
    a.`Id`,
    a.`CustomerId`,
    a.`State`
  FROM `t$1` AS a;
SELECT
  b.`CreateDate`,
  b.`ModifyDate`,
  c.`CustomerId`
FROM `orderbases` AS b
  INNER JOIN `orders` AS c
    ON b.`Id` = c.`Id`
  INNER JOIN `t$1` AS d
    ON b.`Id` = d.`Id`;"; 
    }
}