namespace Caredev.Mego.Tests.Core.Query.Inherit
{
    public partial class ExpandMemberTest
    {
        private const string ExpandOneLevelObjectMemberPageTestSql =
@"SELECT
  a.`Id`,
  a.`CreateDate`,
  a.`ModifyDate`,
  a.`CustomerId`,
  a.`State`,
  b.`Id` AS `Id1`,
  b.`Code`,
  b.`Name`,
  c.`Address1`,
  c.`Address2`,
  c.`Zip`
FROM (SELECT
    d.`Id`,
    d.`CreateDate`,
    d.`ModifyDate`,
    e.`CustomerId`,
    e.`State`
  FROM `orderbases` AS d
    INNER JOIN `orders` AS e
      ON d.`Id` = e.`Id`
  ORDER BY d.`Id` ASC
  LIMIT 5 OFFSET 5) AS a
  INNER JOIN `customerbases` AS b
  INNER JOIN `customers` AS c
    ON b.`Id` = c.`Id`
    AND a.`CustomerId` = b.`Id`;";

        private const string ExpandTwoLevelCollectionMemberPageTestSql =
@"SELECT
  a.`Id`,
  a.`CreateDate`,
  a.`ModifyDate`,
  a.`CustomerId`,
  a.`State`,
  b.`Id` AS `Id1`,
  b.`Key`,
  b.`Price`,
  b.`Quantity`,
  b.`Discount`,
  b.`OrderId`,
  b.`ProductId`,
  b.`Id1` AS `Id2`,
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
      f.`Key`,
      f.`Price`,
      f.`Quantity`,
      e.`Discount`,
      e.`ProductId`,
      g.`Id` AS `Id1`,
      g.`Code`,
      g.`Name`,
      h.`Category`,
      h.`IsValid`,
      h.`UpdateDate`
    FROM `orderdetailbases` AS f
      INNER JOIN `orderdetails` AS e
        ON f.`Id` = e.`Id`
      LEFT JOIN `productbases` AS g
      INNER JOIN `products` AS h
        ON g.`Id` = h.`Id`
        ON e.`ProductId` = g.`Id`) AS b
    ON b.`OrderId` = a.`Id`;";

        private const string ExpandOneLevelObjectMemberStrTestSql =
        @"SELECT
  a.`Id`,
  a.`CreateDate`,
  a.`ModifyDate`,
  b.`CustomerId`,
  b.`State`,
  c.`Id` AS `Id1`,
  c.`Code`,
  c.`Name`,
  d.`Address1`,
  d.`Address2`,
  d.`Zip`
FROM `orderbases` AS a
  INNER JOIN `orders` AS b
    ON a.`Id` = b.`Id`
  INNER JOIN `customerbases` AS c
  INNER JOIN `customers` AS d
    ON c.`Id` = d.`Id`
    AND b.`CustomerId` = c.`Id`;";

        private const string ExpandOneLevelCollectionMemberStrTestSql =
        @"SELECT  a.`Id` ,
        a.`CreateDate` ,
        a.`ModifyDate` ,
        b.`CustomerId` ,
        b.`State` ,
        c.`Id` AS `Id1` ,
        c.`Key` ,
        c.`Price` ,
        c.`Quantity` ,
        c.`Discount` ,
        c.`OrderId` ,
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
                  ) AS c ON c.`OrderId` = a.`Id`;";

        private const string ExpandTwoLevelMemberStrTestSql =
        @"SELECT
  a.`Id`,
  a.`CreateDate`,
  a.`ModifyDate`,
  b.`CustomerId`,
  b.`State`,
  c.`Id` AS `Id1`,
  c.`Code`,
  c.`Name`,
  d.`Address1`,
  d.`Address2`,
  d.`Zip`,
  e.`Id` AS `Id2`,
  e.`CreateDate` AS `CreateDate1`,
  e.`ModifyDate` AS `ModifyDate1`,
  e.`CustomerId` AS `CustomerId1`,
  e.`State` AS `State1`
FROM `orderbases` AS a
  INNER JOIN `orders` AS b
    ON a.`Id` = b.`Id`
  INNER JOIN `customerbases` AS c
  INNER JOIN `customers` AS d
    ON c.`Id` = d.`Id`
    AND b.`CustomerId` = c.`Id`
  LEFT JOIN (SELECT
      f.`CustomerId`,
      g.`Id`,
      g.`CreateDate`,
      g.`ModifyDate`,
      f.`State`
    FROM `orderbases` AS g
      INNER JOIN `orders` AS f
        ON g.`Id` = f.`Id`) AS e
    ON e.`CustomerId` = c.`Id`;";

       

        private const string ExpandOneLevelObjectMemberTestSql =
        @"SELECT
  a.`Id`,
  a.`CreateDate`,
  a.`ModifyDate`,
  b.`CustomerId`,
  b.`State`,
  c.`Id` AS `Id1`,
  c.`Code`,
  c.`Name`,
  d.`Address1`,
  d.`Address2`,
  d.`Zip`
FROM `orderbases` AS a
  INNER JOIN `orders` AS b
    ON a.`Id` = b.`Id`
  INNER JOIN `customerbases` AS c
  INNER JOIN `customers` AS d
    ON c.`Id` = d.`Id`
    AND b.`CustomerId` = c.`Id`;";

        private const string ExpandTwoLevelMemberTestSql =
        @"SELECT
  a.`Id`,
  a.`CreateDate`,
  a.`ModifyDate`,
  b.`CustomerId`,
  b.`State`,
  c.`Id` AS `Id1`,
  c.`Code`,
  c.`Name`,
  d.`Address1`,
  d.`Address2`,
  d.`Zip`,
  e.`Id` AS `Id2`,
  e.`CreateDate` AS `CreateDate1`,
  e.`ModifyDate` AS `ModifyDate1`,
  e.`CustomerId` AS `CustomerId1`,
  e.`State` AS `State1`
FROM `orderbases` AS a
  INNER JOIN `orders` AS b
    ON a.`Id` = b.`Id`
  INNER JOIN `customerbases` AS c
  INNER JOIN `customers` AS d
    ON c.`Id` = d.`Id`
    AND b.`CustomerId` = c.`Id`
  LEFT JOIN (SELECT
      f.`CustomerId`,
      g.`Id`,
      g.`CreateDate`,
      g.`ModifyDate`,
      f.`State`
    FROM `orderbases` AS g
      INNER JOIN `orders` AS f
        ON g.`Id` = f.`Id`) AS e
    ON e.`CustomerId` = c.`Id`;";

        private const string ExpandOneLevelCollectionMemberTestSql =
        @"SELECT  a.`Id` ,
        a.`CreateDate` ,
        a.`ModifyDate` ,
        b.`CustomerId` ,
        b.`State` ,
        c.`Id` AS `Id1` ,
        c.`Key` ,
        c.`Price` ,
        c.`Quantity` ,
        c.`Discount` ,
        c.`OrderId` ,
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
                  ) AS c ON c.`OrderId` = a.`Id`;";

        
        private const string ExpandTwoLevelCollectionMemberTestSql =
        @"SELECT
  a.`Id`,
  a.`CreateDate`,
  a.`ModifyDate`,
  b.`CustomerId`,
  b.`State`,
  c.`Id` AS `Id1`,
  c.`Key`,
  c.`Price`,
  c.`Quantity`,
  c.`Discount`,
  c.`OrderId`,
  c.`ProductId`,
  c.`Id1` AS `Id2`,
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
      e.`Key`,
      e.`Price`,
      e.`Quantity`,
      d.`Discount`,
      d.`ProductId`,
      f.`Id` AS `Id1`,
      f.`Code`,
      f.`Name`,
      g.`Category`,
      g.`IsValid`,
      g.`UpdateDate`
    FROM `orderdetailbases` AS e
      INNER JOIN `orderdetails` AS d
        ON e.`Id` = d.`Id`
      LEFT JOIN `productbases` AS f
      INNER JOIN `products` AS g
        ON f.`Id` = g.`Id`
        ON d.`ProductId` = f.`Id`) AS c
    ON c.`OrderId` = a.`Id`;";

        private const string ExpandOneLevelCollectionMemberFilterTestSql =
        @"SELECT  a.`Id` ,
        a.`CreateDate` ,
        a.`ModifyDate` ,
        b.`CustomerId` ,
        b.`State` ,
        c.`Id` AS `Id1` ,
        c.`Key` ,
        c.`Price` ,
        c.`Quantity` ,
        c.`Discount` ,
        c.`OrderId` ,
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
                    WHERE   e.`Id` > @p0
                  ) AS c ON c.`OrderId` = a.`Id`;";

        private const string ExpandTwoLevelCollectionMemberFilterTestSql =
        @"SELECT  a.`Id` ,
        a.`Code` ,
        a.`Name` ,
        b.`Address1` ,
        b.`Address2` ,
        b.`Zip` ,
        c.`Id` AS `Id1` ,
        c.`CreateDate` ,
        c.`ModifyDate` ,
        c.`CustomerId` ,
        c.`State` ,
        c.`Id1` AS `Id2` ,
        c.`Key` ,
        c.`Price` ,
        c.`Quantity` ,
        c.`Discount` ,
        c.`OrderId` ,
        c.`ProductId`
FROM    `CustomerBases` AS a
        INNER JOIN `Customers` AS b ON a.`Id` = b.`Id`
        LEFT JOIN ( SELECT  d.`CustomerId` ,
                            e.`Id` ,
                            e.`CreateDate` ,
                            e.`ModifyDate` ,
                            d.`State` ,
                            f.`Id` AS `Id1` ,
                            f.`Key` ,
                            f.`Price` ,
                            f.`Quantity` ,
                            f.`Discount` ,
                            f.`OrderId` ,
                            f.`ProductId`
                    FROM    `OrderBases` AS e
                            INNER JOIN `Orders` AS d ON e.`Id` = d.`Id`
                            LEFT JOIN ( SELECT  g.`OrderId` ,
                                                h.`Id` ,
                                                h.`Key` ,
                                                h.`Price` ,
                                                h.`Quantity` ,
                                                g.`Discount` ,
                                                g.`ProductId`
                                        FROM    `OrderDetailBases` AS h
                                                INNER JOIN `OrderDetails`
                                                AS g ON h.`Id` = g.`Id`
                                        WHERE   h.`Id` > @p0
                                      ) AS f ON f.`OrderId` = e.`Id`
                    WHERE   e.`Id` > @p1
                  ) AS c ON c.`CustomerId` = a.`Id`;";

    }
}