namespace Caredev.Mego.Tests.Core.Query.Simple
{
    public partial class CompositeCollectionMemberTest
    {
        private const string QueryBodyAndListPageTestSql =
@"SELECT
  a.`Id`,
  a.`CreateDate`,
  a.`CustomerId`,
  a.`ModifyDate`,
  a.`State`,
  b.`Id` AS `Id1`,
  b.`Category`,
  b.`Code`,
  b.`IsValid`,
  b.`Name`,
  b.`UpdateDate`
FROM (SELECT
    c.`Id`,
    c.`CreateDate`,
    c.`CustomerId`,
    c.`ModifyDate`,
    c.`State`
  FROM `orders` AS c
  ORDER BY c.`Id` ASC
  LIMIT 5 OFFSET 5) AS a
  LEFT JOIN (SELECT
      d.`OrderId`,
      e.`Id`,
      e.`Category`,
      e.`Code`,
      e.`IsValid`,
      e.`Name`,
      e.`UpdateDate`
    FROM `products` AS e
      INNER JOIN `orderdetails` AS d
        ON e.`Id` = d.`ProductId`) AS b
    ON b.`OrderId` = a.`Id`;";

        private const string SimpleJoinTestSql =
@"SELECT  a.`Id` ,
        a.`CreateDate` ,
        a.`CustomerId` ,
        a.`ModifyDate` ,
        a.`State` ,
        b.`Id` AS `Id1` ,
        b.`Category` ,
        b.`Code` ,
        b.`IsValid` ,
        b.`Name` ,
        b.`UpdateDate`
FROM    `Orders` AS a
        INNER JOIN `OrderDetails` AS c ON c.`OrderId` = a.`Id`
        INNER JOIN `Products` AS b ON b.`Id` = c.`ProductId`;";

        private const string SimpleJoinDefaultTestSql =
@"SELECT  a.`Id` ,
        a.`CreateDate` ,
        a.`CustomerId` ,
        a.`ModifyDate` ,
        a.`State` ,
        b.`Id` AS `Id1` ,
        b.`Category` ,
        b.`Code` ,
        b.`IsValid` ,
        b.`Name` ,
        b.`UpdateDate`
FROM    `Orders` AS a
        LEFT JOIN `OrderDetails` AS c ON c.`OrderId` = a.`Id`
        INNER JOIN `Products` AS b ON b.`Id` = c.`ProductId`;";

        private const string QueryBodyAndAggregateCountTestSql =
@"SELECT  a.`Id` ,
        a.`CreateDate` ,
        a.`CustomerId` ,
        a.`ModifyDate` ,
        a.`State` ,
        b.`Count`
FROM    `Orders` AS a
        LEFT JOIN ( SELECT  c.`OrderId` ,
                            COUNT(1) AS `Count`
                    FROM    `Products` AS d
                            INNER JOIN `OrderDetails` AS c ON d.`Id` = c.`ProductId`
                    GROUP BY c.`OrderId`
                  ) AS b ON b.`OrderId` = a.`Id`;";

        private const string QueryBodyAndAggregateMaxTestSql =
@"SELECT  a.`Id` ,
        a.`CreateDate` ,
        a.`CustomerId` ,
        a.`ModifyDate` ,
        a.`State` ,
        b.`MaxId`
FROM    `Orders` AS a
        LEFT JOIN ( SELECT  c.`OrderId` ,
                            MAX(d.`Id`) AS `MaxId`
                    FROM    `Products` AS d
                            INNER JOIN `OrderDetails` AS c ON d.`Id` = c.`ProductId`
                    GROUP BY c.`OrderId`
                  ) AS b ON b.`OrderId` = a.`Id`;";

        private const string QueryBodyAndAggregateCountMaxSql =
@"SELECT  a.`Id` ,
        a.`CreateDate` ,
        a.`CustomerId` ,
        a.`ModifyDate` ,
        a.`State` ,
        b.`Count` ,
        b.`MaxId`
FROM    `Orders` AS a
        LEFT JOIN ( SELECT  c.`OrderId` ,
                            COUNT(1) AS `Count` ,
                            MAX(d.`Id`) AS `MaxId`
                    FROM    `Products` AS d
                            INNER JOIN `OrderDetails` AS c ON d.`Id` = c.`ProductId`
                    GROUP BY c.`OrderId`
                  ) AS b ON b.`OrderId` = a.`Id`;";
        
        private const string QueryBodyAndListTestSql =
@"SELECT  a.`Id` ,
        a.`CreateDate` ,
        a.`CustomerId` ,
        a.`ModifyDate` ,
        a.`State` ,
        b.`Id` AS `Id1` ,
        b.`Category` ,
        b.`Code` ,
        b.`IsValid` ,
        b.`Name` ,
        b.`UpdateDate`
FROM    `Orders` AS a
        LEFT JOIN ( SELECT  c.`OrderId` ,
                            d.`Id` ,
                            d.`Category` ,
                            d.`Code` ,
                            d.`IsValid` ,
                            d.`Name` ,
                            d.`UpdateDate`
                    FROM    `Products` AS d
                            INNER JOIN `OrderDetails` AS c ON d.`Id` = c.`ProductId`
                  ) AS b ON b.`OrderId` = a.`Id`;";

    }
}