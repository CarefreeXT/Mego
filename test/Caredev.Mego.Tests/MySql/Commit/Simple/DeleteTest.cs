namespace Caredev.Mego.Tests.Core.Commit.Simple
{
    public partial class DeleteTest
    {
        private const string DeleteSingleTestSql =
@"DELETE a
  FROM `orderdetails` AS a
WHERE a.`Id` = @p0
  AND a.`Discount` = @p1;";

        private const string DeleteMultiForKeyTestSql =
@"DELETE  a
FROM    `OrderDetails` AS a
WHERE   a.`Id` IN ( @p0, @p1, @p2 );";

        private const string DeleteMultiForKeysTestSql =
@"CREATE TEMPORARY TABLE `t$1` (
  `Id` int NULL,
  `Number` int NULL,
  `Address` longtext NULL
);
INSERT INTO `t$1` (`Id`, `Number`, `Address`)
  VALUES (@p0, @p0, @p1),
  (@p0, @p2, @p3);
DELETE a
  FROM `warehouses` AS a
    INNER JOIN `t$1` AS b
      ON a.`Id` = b.`Id`
      AND a.`Number` = b.`Number`
      AND a.`Address` = b.`Address`;"; 

        private const string DeleteStatementForExpressionTestSql =
@"DELETE  a
FROM    `Warehouses` AS a
WHERE   a.`Id` > @p0;";

        private const string DeleteStatementForQueryTestSql =
@"DELETE  a
FROM    `Warehouses` AS a
        CROSS JOIN `Customers` AS b
WHERE   a.`Id` > b.`Id`
        AND a.`Number` > @p0;";
    }
}