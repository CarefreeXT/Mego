namespace Caredev.Mego.Tests.Core.Commit.Simple
{
    public partial class UpdateTest
    {
        private const string UpdateSingleObjectTestSql =
@"UPDATE `customers` AS a
SET a.`Address1` = @p0,
    a.`Address2` = @p1,
    a.`Code` = @p2,
    a.`Name` = @p3,
    a.`Zip` = @p4
WHERE a.`Id` = @p5
AND a.`Zip` = @p4;";

        private const string UpdateMultiObjectTestSql =
@"CREATE TEMPORARY TABLE `t$1` (
  `Address1` longtext NULL,
  `Address2` longtext NULL,
  `Code` longtext NULL,
  `Name` longtext NULL,
  `Zip` longtext NULL,
  `Id` int NULL
);
INSERT INTO `t$1` (`Address1`, `Address2`, `Code`, `Name`, `Zip`, `Id`)
  VALUES (@p0, @p1, @p2, @p3, @p4, @p5),
  (@p6, @p7, @p8, @p9, @pA, @pB);
UPDATE `customers` AS a
INNER JOIN `t$1` AS b
  ON a.`Id` = b.`Id`
  AND a.`Zip` = b.`Zip`
SET a.`Address1` = b.`Address1`,
    a.`Address2` = b.`Address2`,
    a.`Code` = b.`Code`,
    a.`Name` = b.`Name`,
    a.`Zip` = b.`Zip`;"; 

        private const string UpdateGenerateSingleObjectTestSql =
@"UPDATE `products` AS a
SET a.`Category` = @p0,
    a.`Code` = @p1,
    a.`IsValid` = @p2,
    a.`Name` = @p3,
    a.`UpdateDate` = NOW()
WHERE a.`Id` = @p4;
SELECT
  a.`UpdateDate`
FROM `products` AS a
WHERE a.`Id` = @p4;";
        
        private const string UpdateGenerateMultiObjectTestSql =
@"CREATE TEMPORARY TABLE `t$1` (
  `Category` int NULL,
  `Code` longtext NULL,
  `IsValid` bit(1) NULL,
  `Name` longtext NULL,
  `Id` int NULL
);
INSERT INTO `t$1` (`Category`, `Code`, `IsValid`, `Name`, `Id`)
  VALUES (@p0, @p1, @p2, @p3, @p4),
  (@p0, @p5, @p6, @p7, @p8);
UPDATE `products` AS a
INNER JOIN `t$1` AS b
  ON a.`Id` = b.`Id`
SET a.`Category` = b.`Category`,
    a.`Code` = b.`Code`,
    a.`IsValid` = b.`IsValid`,
    a.`Name` = b.`Name`,
    a.`UpdateDate` = NOW();
SELECT
  a.`UpdateDate`
FROM `products` AS a
  INNER JOIN `t$1` AS c
    ON a.`Id` = c.`Id`;"; 

        private const string UpdateQueryTestSql =
@"UPDATE `Customers` AS a
INNER JOIN `Products` AS b ON a.`Id` = b.`Id`
SET a.`Name` = b.`Name`, a.`Code` = b.`Name`, a.`Address1` = b.`Code`
WHERE b.`Id` > @p0;";
    }
}