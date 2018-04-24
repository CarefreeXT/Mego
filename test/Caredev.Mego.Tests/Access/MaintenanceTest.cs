namespace Caredev.Mego.Tests.Core.Commit.Simple
{
    partial class MaintenanceTest
    {
        private const string CreateTableTestSql =
@"CREATE TABLE [TestProduct](
    [Id] INTEGER NOT NULL IDENTITY (1, 1), 
    [Category] INTEGER NOT NULL, 
    [Code] LONGTEXT NULL, 
    [IsValid] BIT NOT NULL, 
    [Name] LONGTEXT NULL, 
    [UpdateDate] DATETIME NOT NULL,
    PRIMARY KEY ( [Id])
);";

        private const string DropRelationTestSql =
@"ALTER TABLE [Orders] DROP CONSTRAINT [FK_Orders_Customers_CustomerId_Id];";

        private const string DropCompositeRelationTestSql =
@"ALTER TABLE [OrderDetails] DROP CONSTRAINT [FK_OrderDetails_Orders_OrderId_Id];";

        private const string DropCompositeRelationTestSql1 =
@"ALTER TABLE [OrderDetails] DROP CONSTRAINT [FK_OrderDetails_Products_ProductId_Id];";

        private const string CreateTempTableTestSql = Constants.NotSuppored;

        private const string CreateTableVariableTestSql = Constants.NotSuppored;

        private const string DropTableTestSql = 
@"DROP TABLE [TestProduct];";

        private const string CreateViewTestSql =
@"CREATE VIEW [TestProduct]
AS
    SELECT  a.[Id] ,
            a.[Category] ,
            a.[Code] ,
            a.[IsValid] ,
            a.[Name] ,
            a.[UpdateDate]
    FROM    [Products] AS a;";

        private const string CreateViewTest2Sql =
@"CREATE VIEW [TestProduct]
AS
    SELECT  *
    FROM    Products
    WHERE   Id > 0;";

        private const string DropViewTestSql =
@"DROP VIEW [TestProduct];";

        private const string ViewIsExsitTestSql =
@"SELECT COUNT(1) FROM MSysObjects Where Type=5 AND Name='Products';";

        private const string RenameTableTestSql = Constants.NotSuppored;

        private const string RenameViewTestSql = Constants.NotSuppored;

        private const string TableIsExsitTestSql =
@"SELECT COUNT(1) FROM MSysObjects Where Type=1 AND Name='Products';";

        private const string CreateRelationTestSql =
@"ALTER TABLE [Orders] ADD CONSTRAINT [FK_Orders_Customers_CustomerId_Id] FOREIGN KEY([CustomerId]) 
REFERENCES [Customers] ([Id]);";

        private const string CreateCompositeRelationTestSql =
@"ALTER TABLE [OrderDetails] ADD CONSTRAINT [FK_OrderDetails_Orders_OrderId_Id] FOREIGN KEY([OrderId]) REFERENCES [Orders] ([Id]);";

        private const string CreateCompositeRelationTestSql1 =
@"ALTER TABLE [OrderDetails] ADD CONSTRAINT [FK_OrderDetails_Products_ProductId_Id] FOREIGN KEY([ProductId]) REFERENCES [Products] ([Id]);";
    }
}
