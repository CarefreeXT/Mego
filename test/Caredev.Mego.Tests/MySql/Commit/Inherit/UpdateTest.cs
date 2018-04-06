namespace Caredev.Mego.Tests.Core.Commit.Inherit
{
    public partial class UpdateTest : IInheritTest
    {
        private const string UpdateSingleObjectTestSql =
@"UPDATE `CustomerBases` AS a
SET a.`Code` = @p0,
    a.`name` = @p1
WHERE a.`Id` = @p2;
UPDATE `Customers` AS b
SET b.`Address1` = @p3,
    b.`Address2` = @p4,
    b.`Zip` = @p5
WHERE b.`Id` = @p2
AND b.`Zip` = @p5;";

        private const string UpdateMultiObjectTestSql =
@"CREATE TEMPORARY TABLE `t$1` (
  `Code` longtext NULL,
  `Name` longtext NULL,
  `Id` int NULL,
  `Address1` longtext NULL,
  `Address2` longtext NULL,
  `Zip` longtext NULL
);
INSERT INTO `t$1` (`Code`, `Name`, `Id`, `Address1`, `Address2`, `Zip`)
  VALUES (@p0, @p1, @p2, @p3, @p4, @p5),
  (@p6, @p7, @p8, @p9, @pA, @pB);
UPDATE `CustomerBases` AS a
INNER JOIN `t$1` AS b
  ON a.`Id` = b.`Id`
SET a.`Code` = b.`Code`,
    a.`Name` = b.`Name`;
UPDATE `Customers` AS c
INNER JOIN `t$1` AS d
  ON c.`Id` = d.`Id`
  AND c.`Zip` = d.`Zip`
SET c.`Address1` = d.`Address1`,
    c.`Address2` = d.`Address2`,
    c.`Zip` = d.`Zip`;"; 

        private const string UpdateGenerateSingleObjectTestSql =
@"UPDATE `productbases` AS a
SET a.`Code` = @p0,
    a.`Name` = @p1
WHERE a.`Id` = @p2;
UPDATE `products` AS b
SET b.`Category` = @p3,
    b.`IsValid` = @p4,
    b.`UpdateDate` = NOW()
WHERE b.`Id` = @p2;
SELECT
  c.`UpdateDate`
FROM `productbases` AS d
  INNER JOIN `products` AS c
    ON d.`Id` = c.`Id`
WHERE d.`Id` = @p2;";

        private const string UpdateGenerateMultiObjectTestSql =
@"CREATE TEMPORARY TABLE `t$1` (
  `Code` longtext NULL,
  `Name` longtext NULL,
  `Id` int NULL,
  `Category` int NULL,
  `IsValid` bit(1) NULL
);
INSERT INTO `t$1` (`Code`, `Name`, `Id`, `Category`, `IsValid`)
  VALUES (@p0, @p1, @p2, @p3, @p4),
  (@p5, @p6, @p7, @p3, @p8);
UPDATE `productbases` AS a
INNER JOIN `t$1` AS b
  ON a.`Id` = b.`Id`
SET a.`Code` = b.`Code`,
    a.`Name` = b.`Name`;
UPDATE `products` AS c
INNER JOIN `t$1` AS d
  ON c.`Id` = d.`Id`
SET c.`Category` = d.`Category`,
    c.`IsValid` = d.`IsValid`,
    c.`UpdateDate` = NOW();
SELECT
  e.`UpdateDate`
FROM `productbases` AS f
  INNER JOIN `products` AS e
    ON f.`Id` = e.`Id`
  INNER JOIN `t$1` AS g
    ON f.`Id` = g.`Id`;"; 

        private const string UpdateQueryTestSql =
@"UPDATE `CustomerBases` AS a CROSS JOIN `ProductBases` AS b
INNER JOIN `Products` AS c ON b.`Id` = c.`Id`
SET a.`Name` = b.`Name`, a.`Code` = b.`Name`
WHERE b.`Id` > @p0;
UPDATE `Customers` AS d CROSS JOIN `ProductBases` AS b
INNER JOIN `Products` AS c ON b.`Id` = c.`Id`
SET d.`Address1` = b.`Code`
WHERE b.`Id` > @p0;";
    }
}