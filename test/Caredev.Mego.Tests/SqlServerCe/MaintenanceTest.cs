namespace Caredev.Mego.Tests.Core.Commit.Simple
{
    partial class MaintenanceTest
    {
        private const string CreateTableTestSql =
@"CREATE TABLE [TestProduct]
    (
      [Id] INT NOT NULL
               IDENTITY(1, 1) ,
      [Category] INT NOT NULL ,
      [Code] NVARCHAR(4000) NULL ,
      [IsValid] BIT NOT NULL ,
      [Name] NVARCHAR(4000) NULL ,
      [UpdateDate] DATETIME NOT NULL ,
      PRIMARY KEY ( [Id] )
    )";

        private const string DropRelationTestSql =
@"ALTER TABLE [Orders] DROP CONSTRAINT [FK_Orders_Customers_CustomerId_Id]";

        private const string DropCompositeRelationTestSql =
@"ALTER TABLE [OrderDetails] DROP CONSTRAINT [FK_OrderDetails_Orders_OrderId_Id]";

        private const string DropCompositeRelationTestSql1 =
@"ALTER TABLE [OrderDetails] DROP CONSTRAINT [FK_OrderDetails_Products_ProductId_Id]";

        private const string CreateTempTableTestSql =
@"CREATE TABLE #TestProduct
    (
      [Id] INT NULL ,
      [Category] INT NULL ,
      [Code] NVARCHAR(MAX) NULL ,
      [IsValid] BIT NULL ,
      [Name] NVARCHAR(MAX) NULL ,
      [UpdateDate] DATETIME2(7) NULL
    )";

        private const string CreateTableVariableTestSql =
@"DECLARE @TestProduct AS TABLE
    (
      [Id] INT NULL ,
      [Category] INT NULL ,
      [Code] NVARCHAR(MAX) NULL ,
      [IsValid] BIT NULL ,
      [Name] NVARCHAR(MAX) NULL ,
      [UpdateDate] DATETIME2(7) NULL
    )";

        private const string DropTableTestSql =
@"DROP TABLE [TestProduct]";

        private const string CreateViewTestSql = Constants.NotSuppored;
        private const string CreateViewTest2Sql = Constants.NotSuppored;
        private const string DropViewTestSql = Constants.NotSuppored;
        private const string ViewIsExsitTestSql = Constants.NotSuppored;

        private const string RenameTableTestSql =
@"EXEC sp_rename 'Products', 'TestProduct'";

        private const string RenameViewTestSql = Constants.NotSuppored;

        private const string TableIsExsitTestSql =
@"SELECT CASE WHEN EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES t WHERE t.TABLE_TYPE='TABLE' AND t.TABLE_NAME='Products') THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END";

        private const string CreateRelationTestSql =
@"ALTER TABLE [Orders] ADD CONSTRAINT [FK_Orders_Customers_CustomerId_Id] FOREIGN KEY([CustomerId]) REFERENCES [Customers] ([Id])";

        private const string CreateCompositeRelationTestSql =
@"ALTER TABLE [OrderDetails] ADD CONSTRAINT [FK_OrderDetails_Orders_OrderId_Id] FOREIGN KEY([OrderId]) REFERENCES [Orders] ([Id])";

        private const string CreateCompositeRelationTestSql1 =
@"ALTER TABLE [OrderDetails] ADD CONSTRAINT [FK_OrderDetails_Products_ProductId_Id] FOREIGN KEY([ProductId]) REFERENCES [Products] ([Id])";

    }
}
