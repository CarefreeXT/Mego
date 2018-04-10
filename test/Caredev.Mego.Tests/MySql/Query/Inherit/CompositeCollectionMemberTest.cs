namespace Caredev.Mego.Tests.Core.Query.Inherit
{
    public partial class CompositeCollectionMemberTest
    {
        private const string QueryBodyAndListPageTestSql =
@"SELECT
  a.`Id`,
  a.`CreateDate`,
  a.`ModifyDate`,
  a.`CustomerId`,
  a.`State`,
  b.`Id` AS `Id1`,
  b.`Code`,
  b.`Name`,
  b.`Category`,
  b.`IsValid`,
  b.`UpdateDate`
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
  LIMIT 5 OFFSET 5) AS a
  LEFT JOIN (SELECT
      e.`OrderId`,
      f.`Id`,
      f.`Code`,
      f.`Name`,
      g.`Category`,
      g.`IsValid`,
      g.`UpdateDate`
    FROM `productbases` AS f
      INNER JOIN `products` AS g
        ON f.`Id` = g.`Id`
      INNER JOIN `orderdetailbases` AS h
      INNER JOIN `orderdetails` AS e
        ON h.`Id` = e.`Id`
        AND f.`Id` = e.`ProductId`) AS b
    ON b.`OrderId` = a.`Id`;";

        private const string SimpleJoinTestSql =
        @"SELECT
  a.`Id`,
  a.`CreateDate`,
  a.`ModifyDate`,
  b.`CustomerId`,
  b.`State`,
  c.`Id` AS `Id1`,
  c.`Code`,
  c.`Name`,
  d.`Category`,
  d.`IsValid`,
  d.`UpdateDate`
FROM `orderbases` AS a
  INNER JOIN `orders` AS b
    ON a.`Id` = b.`Id`
  INNER JOIN `orderdetailbases` AS e
  INNER JOIN `orderdetails` AS f
    ON e.`Id` = f.`Id`
    AND f.`OrderId` = a.`Id`
  INNER JOIN `productbases` AS c
  INNER JOIN `products` AS d
    ON c.`Id` = d.`Id`
    AND c.`Id` = f.`ProductId`;";

        private const string SimpleJoinDefaultTestSql =
        @"SELECT
  a.`Id`,
  a.`CreateDate`,
  a.`ModifyDate`,
  b.`CustomerId`,
  b.`State`,
  c.`Id` AS `Id1`,
  c.`Code`,
  c.`Name`,
  d.`Category`,
  d.`IsValid`,
  d.`UpdateDate`
FROM `orderbases` AS a
  INNER JOIN `orders` AS b
    ON a.`Id` = b.`Id`
  LEFT JOIN `orderdetailbases` AS e
  INNER JOIN `orderdetails` AS f
    ON e.`Id` = f.`Id`
    ON f.`OrderId` = a.`Id`
  INNER JOIN `productbases` AS c
  INNER JOIN `products` AS d
    ON c.`Id` = d.`Id`
    AND c.`Id` = f.`ProductId`;";

        private const string QueryBodyAndAggregateCountTestSql =
        @"SELECT
  a.`Id`,
  a.`CreateDate`,
  a.`ModifyDate`,
  b.`CustomerId`,
  b.`State`,
  c.`COUNT`
FROM `orderbases` AS a
  INNER JOIN `orders` AS b
    ON a.`Id` = b.`Id`
  LEFT JOIN (SELECT
      d.`OrderId`,
      COUNT(1) AS `Count`
    FROM `productbases` AS e
      INNER JOIN `products` AS f
        ON e.`Id` = f.`Id`
      INNER JOIN `orderdetailbases` AS g
      INNER JOIN `orderdetails` AS d
        ON g.`Id` = d.`Id`
        AND e.`Id` = d.`ProductId`
    GROUP BY d.`OrderId`) AS c
    ON c.`OrderId` = a.`Id`;";

        private const string QueryBodyAndAggregateMaxTestSql =
        @"SELECT
  a.`Id`,
  a.`CreateDate`,
  a.`ModifyDate`,
  b.`CustomerId`,
  b.`State`,
  c.`MaxId`
FROM `orderbases` AS a
  INNER JOIN `orders` AS b
    ON a.`Id` = b.`Id`
  LEFT JOIN (SELECT
      d.`OrderId`,
      MAX(e.`Id`) AS `MaxId`
    FROM `productbases` AS e
      INNER JOIN `products` AS f
        ON e.`Id` = f.`Id`
      INNER JOIN `orderdetailbases` AS g
      INNER JOIN `orderdetails` AS d
        ON g.`Id` = d.`Id`
        AND e.`Id` = d.`ProductId`
    GROUP BY d.`OrderId`) AS c
    ON c.`OrderId` = a.`Id`;";

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
                    FROM    `ProductBases` AS e
                            INNER JOIN `Products` AS f ON e.`Id` = f.`Id`
                            INNER JOIN `OrderDetailBases` AS g
                            INNER JOIN `OrderDetails` AS d ON g.`Id` = d.`Id` AND e.`Id` = d.`ProductId`
                    GROUP BY d.`OrderId`
                  ) AS c ON c.`OrderId` = a.`Id`;";

        
        private const string QueryBodyAndListTestSql =
        @"SELECT
  a.`Id`,
  a.`CreateDate`,
  a.`ModifyDate`,
  b.`CustomerId`,
  b.`State`,
  c.`Id` AS `Id1`,
  c.`Code`,
  c.`Name`,
  c.`Category`,
  c.`IsValid`,
  c.`UpdateDate`
FROM `orderbases` AS a
  INNER JOIN `orders` AS b
    ON a.`Id` = b.`Id`
  LEFT JOIN (SELECT
      d.`OrderId`,
      e.`Id`,
      e.`Code`,
      e.`Name`,
      f.`Category`,
      f.`IsValid`,
      f.`UpdateDate`
    FROM `productbases` AS e
      INNER JOIN `products` AS f
        ON e.`Id` = f.`Id`
      INNER JOIN `orderdetailbases` AS g
      INNER JOIN `orderdetails` AS d
        ON g.`Id` = d.`Id`
        AND e.`Id` = d.`ProductId`) AS c
    ON c.`OrderId` = a.`Id`;";

    }
}