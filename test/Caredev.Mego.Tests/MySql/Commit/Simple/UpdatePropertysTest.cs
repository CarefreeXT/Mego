namespace Caredev.Mego.Tests.Core.Commit.Simple
{
    public partial class UpdatePropertysTest 
    {
        private const string UpdateSingleObjectTestSql =
@"UPDATE `Customers` AS a
SET a.`Address1` = CONCAT(CONCAT(@p0, @p1), @p2),
    a.`name` = @p3
WHERE a.`Id` = @p4
AND a.`Zip` = @p5;
SELECT
  a.`Address1`
FROM `Customers` AS a
WHERE a.`Id` = @p4;";
        
        private const string UpdateMultiObjectTestSql =
@"CREATE TEMPORARY TABLE `t$1` (
  `Address2` longtext NULL,
  `Address1` longtext NULL,
  `Name` longtext NULL,
  `Id` int NULL,
  `Zip` longtext NULL
);
INSERT INTO `t$1` (`Address2`, `Address1`, `Name`, `Id`, `Zip`)
  VALUES (@p0, @p1, @p2, @p3, @p4),
  (@p5, @p6, @p7, @p8, @p9);
UPDATE `customers` AS a
INNER JOIN `t$1` AS b
  ON a.`Id` = b.`Id`
  AND a.`Zip` = b.`Zip`
SET a.`Address1` = CONCAT(CONCAT(b.`Address2`, @pA), b.`Address1`),
    a.`Name` = b.`Name`;
SELECT
  a.`Address1`
FROM `customers` AS a
  INNER JOIN `t$1` AS c
    ON a.`Id` = c.`Id`;"; 

        private const string UpdateGenerateSingleObjectTestSql =
@"UPDATE `products` AS a
SET a.`Category` = @p0,
    a.`Code` = CONCAT(@p1, @p2),
    a.`Name` = @p1,
    a.`UpdateDate` = NOW()
WHERE a.`Id` = @p3;
SELECT
  a.`Code`,
  a.`UpdateDate`
FROM `products` AS a
WHERE a.`Id` = @p3;";
        
        private const string UpdateGenerateMultiObjectTestSql =
@"CREATE TEMPORARY TABLE `t$1` (
  `Category` int NULL,
  `Name` longtext NULL,
  `Code` longtext NULL,
  `Id` int NULL
);
INSERT INTO `t$1` (`Category`, `Name`, `Code`, `Id`)
  VALUES (@p0, @p1, @p2, @p0),
  (@p0, @p3, @p4, @p5);
UPDATE `products` AS a
INNER JOIN `t$1` AS b
  ON a.`Id` = b.`Id`
SET a.`Category` = b.`Category`,
    a.`Code` = CONCAT(b.`Name`, b.`Code`),
    a.`Name` = b.`Name`,
    a.`UpdateDate` = NOW();
SELECT
  a.`Code`,
  a.`UpdateDate`
FROM `products` AS a
  INNER JOIN `t$1` AS c
    ON a.`Id` = c.`Id`;"; 
    }
}