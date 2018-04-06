namespace Caredev.Mego.Tests.Core.Query.Inherit
{
    public partial class GroupJoinTest
    {
        private const string QueryBodyAndListPageTestSql =
@"SELECT
  a.`Id`,
  a.`CreateDate`,
  a.`ModifyDate`,
  a.`CustomerId`,
  a.`State`,
  b.`OrderId`,
  b.`Id` AS `Id1`,
  b.`Key`,
  b.`Price`,
  b.`Quantity`,
  b.`Discount`,
  b.`ProductId`
FROM (SELECT
    c.`Id`,
    c.`CreateDate`,
    c.`ModifyDate`,
    d.`CustomerId`,
    d.`State`
  FROM `orderbases` AS c
    INNER JOIN `orders` AS d
      ON c.`Id` = d.`Id`
  ORDER BY c.`Id` ASC
  LIMIT 5, 5) AS a
  LEFT JOIN (SELECT
      e.`OrderId`,
      f.`Id`,
      f.`Key`,
      f.`Price`,
      f.`Quantity`,
      e.`Discount`,
      e.`ProductId`
    FROM `orderdetailbases` AS f
      INNER JOIN `orderdetails` AS e
        ON f.`Id` = e.`Id`) AS b
    ON a.`Id` = b.`OrderId`;";

        private const string SimpleGroupJoinTestSql =
        @"SELECT
  a.`Id`,
  a.`Code`,
  a.`Name`,
  b.`Category`,
  b.`IsValid`,
  b.`UpdateDate`,
  c.`Id` AS `Id1`,
  c.`Code` AS `Code1`,
  c.`Name` AS `Name1`,
  d.`Address1`,
  d.`Address2`,
  d.`Zip`
FROM `productbases` AS a
  INNER JOIN `products` AS b
    ON a.`Id` = b.`Id`
  INNER JOIN `customerbases` AS c
  INNER JOIN `customers` AS d
    ON c.`Id` = d.`Id`
    AND a.`Id` = c.`Id`;";

        private const string GroupJoinDefaultTestSql =
        @"SELECT  a.`Id` ,
        a.`Code` ,
        a.`Name` ,
        b.`Category` ,
        b.`IsValid` ,
        b.`UpdateDate` ,
        c.`Id` AS `Id1` ,
        c.`Code` AS `Code1` ,
        c.`Name` AS `Name1` ,
        d.`Address1` ,
        d.`Address2` ,
        d.`Zip`
FROM    `ProductBases` AS a
        INNER JOIN `Products` AS b ON a.`Id` = b.`Id`
        LEFT JOIN `CustomerBases` AS c
        INNER JOIN `Customers` AS d ON c.`Id` = d.`Id` ON a.`Id` = c.`Id`;";

        private const string QueryBodyAndAggregateCountTestSql =
        @"SELECT  a.`Id` ,
        a.`CreateDate` ,
        a.`ModifyDate` ,
        b.`CustomerId` ,
        b.`State` ,
        c.`Count`
FROM    `OrderBases` AS a
        INNER JOIN `Orders` AS b ON a.`Id` = b.`Id`
        LEFT JOIN ( SELECT  d.`OrderId` ,
                            COUNT(1) AS `Count`
                    FROM    `OrderDetailBases` AS e
                            INNER JOIN `OrderDetails` AS d ON e.`Id` = d.`Id`
                    GROUP BY d.`OrderId`
                  ) AS c ON a.`Id` = c.`OrderId`;";

        private const string QueryBodyAndAggregateMaxTestSql =
        @"SELECT  a.`Id` ,
        a.`CreateDate` ,
        a.`ModifyDate` ,
        b.`CustomerId` ,
        b.`State` ,
        c.`MaxId`
FROM    `OrderBases` AS a
        INNER JOIN `Orders` AS b ON a.`Id` = b.`Id`
        LEFT JOIN ( SELECT  d.`OrderId` ,
                            MAX(e.`Id`) AS `MaxId`
                    FROM    `OrderDetailBases` AS e
                            INNER JOIN `OrderDetails` AS d ON e.`Id` = d.`Id`
                    GROUP BY d.`OrderId`
                  ) AS c ON a.`Id` = c.`OrderId`;";

        private const string QueryBodyAndAggregateCountMaxTestSql =
        @"SELECT  a.`Id` ,
        a.`CreateDate` ,
        a.`ModifyDate` ,
        b.`CustomerId` ,
        b.`State` ,
        c.`Count` ,
        c.`MaxId`
FROM    `OrderBases` AS a
        INNER JOIN `Orders` AS b ON a.`Id` = b.`Id`
        LEFT JOIN ( SELECT  d.`OrderId` ,
                            COUNT(1) AS `Count` ,
                            MAX(e.`Id`) AS `MaxId`
                    FROM    `OrderDetailBases` AS e
                            INNER JOIN `OrderDetails` AS d ON e.`Id` = d.`Id`
                    GROUP BY d.`OrderId`
                  ) AS c ON a.`Id` = c.`OrderId`;";
       
        private const string QueryBodyAndListTestSql =
        @"SELECT  a.`Id` ,
        a.`CreateDate` ,
        a.`ModifyDate` ,
        b.`CustomerId` ,
        b.`State` ,
        c.`OrderId` ,
        c.`Id` AS `Id1` ,
        c.`Key` ,
        c.`Price` ,
        c.`Quantity` ,
        c.`Discount` ,
        c.`ProductId`
FROM    `OrderBases` AS a
        INNER JOIN `Orders` AS b ON a.`Id` = b.`Id`
        LEFT JOIN ( SELECT  d.`OrderId` ,
                            e.`Id` ,
                            e.`Key` ,
                            e.`Price` ,
                            e.`Quantity` ,
                            d.`Discount` ,
                            d.`ProductId`
                    FROM    `OrderDetailBases` AS e
                            INNER JOIN `OrderDetails` AS d ON e.`Id` = d.`Id`
                  ) AS c ON a.`Id` = c.`OrderId`;";

    }
}