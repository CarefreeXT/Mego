namespace Caredev.Mego.Tests.Core.Query.Simple
{
    public partial class GroupByTest
    {
        private const string QuerySimpleKeyAndListPageTestSql =
@"SELECT
  a.`Category`,
  b.`Id`,
  b.`Category` AS `Category1`,
  b.`Code`,
  b.`IsValid`,
  b.`Name`,
  b.`UpdateDate`
FROM (SELECT
    c.`Category`
  FROM (SELECT
      d.`Category`
    FROM `products` AS d
    GROUP BY d.`Category`) AS c
  ORDER BY c.`Category` ASC
  LIMIT 2, 2) AS a
  INNER JOIN (SELECT
      d.`Id`,
      d.`Category`,
      d.`Code`,
      d.`IsValid`,
      d.`Name`,
      d.`UpdateDate`
    FROM `products` AS d) AS b
    ON b.`Category` = a.`Category`;";

        private const string QuerySimpleKeyTestSql =
      @"SELECT  a.`Category`
FROM    ( SELECT    b.`Category`
          FROM      `Products` AS b
          GROUP BY  b.`Category`
        ) AS a;";

        private const string QueryComplexKeysTestSql =
@"SELECT  a.`Category` ,
        a.`IsValid`
FROM    ( SELECT    b.`Category` ,
                    b.`IsValid`
          FROM      `Products` AS b
          GROUP BY  b.`Category` ,
                    b.`IsValid`
        ) AS a;";

        private const string QueryKeyMembersTestSql =
@"SELECT  a.`Category` ,
        a.`IsValid`
FROM    ( SELECT    b.`Category` ,
                    b.`IsValid`
          FROM      `Products` AS b
          GROUP BY  b.`Category` ,
                    b.`IsValid`
        ) AS a;";

        private const string QueryKeyAndAggregateCountTestSql =
@"SELECT  a.`Category` ,
        a.`Count`
FROM    ( SELECT    b.`Category` ,
                    COUNT(1) AS `Count`
          FROM      `Products` AS b
          GROUP BY  b.`Category`
        ) AS a;";

        private const string QueryKeyAndAggregateMaxTestSql =
@"SELECT  a.`Category` ,
        a.`MaxId`
FROM    ( SELECT    b.`Category` ,
                    MAX(b.`Id`) AS `MaxId`
          FROM      `Products` AS b
          GROUP BY  b.`Category`
        ) AS a;";

        private const string QueryKeyAndAggregateCountMaxTestSql =
@"SELECT  a.`Category` ,
        a.`Count` ,
        a.`MaxId`
FROM    ( SELECT    b.`Category` ,
                    COUNT(1) AS `Count` ,
                    MAX(b.`Id`) AS `MaxId`
          FROM      `Products` AS b
          GROUP BY  b.`Category`
        ) AS a;";

        private const string QuerySimpleKeyAndListTestSql =
@"SELECT  a.`Category` ,
        b.`Id` ,
        b.`Category` AS `Category1` ,
        b.`Code` ,
        b.`IsValid` ,
        b.`Name` ,
        b.`UpdateDate`
FROM    ( SELECT    c.`Category`
          FROM      `Products` AS c
          GROUP BY  c.`Category`
        ) AS a
        INNER JOIN ( SELECT c.`Id` ,
                            c.`Category` ,
                            c.`Code` ,
                            c.`IsValid` ,
                            c.`Name` ,
                            c.`UpdateDate`
                     FROM   `Products` AS c
                   ) AS b ON b.`Category` = a.`Category`;";

        

    }
}