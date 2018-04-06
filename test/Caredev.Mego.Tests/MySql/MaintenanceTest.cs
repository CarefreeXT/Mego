namespace Caredev.Mego.Tests.Core.Commit.Simple
{
    partial class MaintenanceTest
    {
        private const string CreateTableTestSql =
@"CREATE TABLE `Products` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Category` int NOT NULL,
  `Code` longtext NULL,
  `IsValid` bit(1) NOT NULL,
  `Name` longtext NULL,
  `UpdateDate` datetime NOT NULL,
  PRIMARY KEY (`Id`)
);";

        private const string TableExsitTestSql =
@"SELECT CASE WHEN EXISTS (SELECT 1 FROM information_schema.TABLES t WHERE t.TABLE_SCHEMA=DATABASE() AND t.TABLE_NAME='Products') THEN TRUE ELSE FALSE END;";

        private const string CreateRelationTestSql =
@"ALTER TABLE `Orders` ADD CONSTRAINT `FK_Orders_Customers_CustomerId_Id` FOREIGN KEY(`CustomerId`) REFERENCES `Customers` (`Id`);";

        private const string CreateCompositeRelationTestSql =
@"ALTER TABLE `OrderDetails` ADD CONSTRAINT `FK_OrderDetails_Orders_OrderId_Id` FOREIGN KEY(`OrderId`) REFERENCES `Orders` (`Id`);";

        private const string CreateCompositeRelationTestSql1 =
@"ALTER TABLE `OrderDetails` ADD CONSTRAINT `FK_OrderDetails_Products_ProductId_Id` FOREIGN KEY(`ProductId`) REFERENCES `Products` (`Id`);";

    }
}
