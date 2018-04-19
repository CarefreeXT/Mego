namespace Caredev.Mego.Tests.Core.Commit.Simple
{
    partial class MaintenanceTest
    {
        private const string CreateTableTestSql =
@"CREATE TABLE `TestProduct` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Category` int NOT NULL,
  `Code` longtext NULL,
  `IsValid` bit(1) NOT NULL,
  `Name` longtext NULL,
  `UpdateDate` datetime NOT NULL,
  PRIMARY KEY (`Id`)
);";

        private const string TableIsExsitTestSql =
@"SELECT CASE WHEN EXISTS (SELECT 1 FROM information_schema.TABLES t WHERE t.TABLE_SCHEMA=DATABASE() AND t.TABLE_NAME='Products') THEN TRUE ELSE FALSE END;";

        private const string CreateRelationTestSql =
@"ALTER TABLE `Orders` ADD CONSTRAINT `FK_Orders_Customers_CustomerId_Id` FOREIGN KEY(`CustomerId`) REFERENCES `Customers` (`Id`);";

        private const string CreateCompositeRelationTestSql =
@"ALTER TABLE `OrderDetails` ADD CONSTRAINT `FK_OrderDetails_Orders_OrderId_Id` FOREIGN KEY(`OrderId`) REFERENCES `Orders` (`Id`);";

        private const string CreateCompositeRelationTestSql1 =
@"ALTER TABLE `OrderDetails` ADD CONSTRAINT `FK_OrderDetails_Products_ProductId_Id` FOREIGN KEY(`ProductId`) REFERENCES `Products` (`Id`);";
        
        private const string DropRelationTestSql =
@"ALTER TABLE `Orders` DROP CONSTRAINT `FK_Orders_Customers_CustomerId_Id`;";

        private const string DropCompositeRelationTestSql =
@"ALTER TABLE `OrderDetails` DROP CONSTRAINT `FK_OrderDetails_Orders_OrderId_Id`;";

        private const string DropCompositeRelationTestSql1 =
@"ALTER TABLE `OrderDetails` DROP CONSTRAINT `FK_OrderDetails_Products_ProductId_Id`;";

        private const string CreateTempTableTestSql =
@"CREATE TEMPORARY TABLE `TestProduct` (
  `Id` int NULL,
  `Category` int NULL,
  `Code` longtext NULL,
  `IsValid` bit(1) NULL,
  `Name` longtext NULL,
  `UpdateDate` datetime NULL
);";

        private const string CreateTableVariableTestSql = @"";

        private const string DropTableTestSql = @"DROP TABLE `TestProduct`;";

        private const string CreateViewTestSql =
@"CREATE VIEW `TestProduct`
AS
SELECT
  a.`ID`,
  a.`Category`,
  a.`Code`,
  a.`IsValid`,
  a.`NAME`,
  a.`UpdateDate`
FROM `products` AS a;";

        private const string CreateViewTest2Sql =
@"CREATE VIEW `TestProduct`
AS
SELECT
  *
FROM products
WHERE ID > 0;";

        private const string DropViewTestSql =
@"DROP VIEW `TestProduct`;";

        private const string ViewIsExsitTestSql =
@"SELECT
  CASE WHEN EXISTS (SELECT
          1
        FROM information_schema.VIEWS t
        WHERE t.TABLE_SCHEMA = DATABASE()
        AND t.TABLE_NAME = 'Products') THEN TRUE ELSE FALSE END;";

        private const string RenameTableTestSql = 
@"RENAME TABLE `Products` TO `TestProduct`;";

        private const string RenameViewTestSql =
@"RENAME TABLE `Products` TO `TestProduct`;";
    }
}
