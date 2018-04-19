namespace Caredev.Mego.Tests.Core.Commit.Simple
{
    partial class MaintenanceTest
    {
        private const string CreateTableTestSql =
@"CREATE TABLE [dbo].[TestProduct]
    (
      [Id] INT NOT NULL
               IDENTITY(1, 1) ,
      [Category] INT NOT NULL ,
      [Code] NVARCHAR(MAX) NULL ,
      [IsValid] BIT NOT NULL ,
      [Name] NVARCHAR(MAX) NULL ,
      [UpdateDate] DATETIME2(7) NOT NULL ,
      PRIMARY KEY ( [Id] )
    );";

        private const string DropRelationTestSql =
@"ALTER TABLE [dbo].[Orders] DROP CONSTRAINT [FK_Orders_Customers_CustomerId_Id];";

        private const string DropCompositeRelationTestSql =
@"ALTER TABLE [dbo].[OrderDetails] DROP CONSTRAINT [FK_OrderDetails_Orders_OrderId_Id];";

        private const string DropCompositeRelationTestSql1 =
@"ALTER TABLE [dbo].[OrderDetails] DROP CONSTRAINT [FK_OrderDetails_Products_ProductId_Id];";

        private const string CreateTempTableTestSql =
@"CREATE TABLE #TestProduct
    (
      [Id] INT NULL ,
      [Category] INT NULL ,
      [Code] NVARCHAR(MAX) NULL ,
      [IsValid] BIT NULL ,
      [Name] NVARCHAR(MAX) NULL ,
      [UpdateDate] DATETIME2(7) NULL
    );";

        private const string CreateTableVariableTestSql =
@"DECLARE @TestProduct AS TABLE
    (
      [Id] INT NULL ,
      [Category] INT NULL ,
      [Code] NVARCHAR(MAX) NULL ,
      [IsValid] BIT NULL ,
      [Name] NVARCHAR(MAX) NULL ,
      [UpdateDate] DATETIME2(7) NULL
    );";

        private const string DropTableTestSql = 
@"DROP TABLE [dbo].[TestProduct];";

        private const string CreateViewTestSql =
@"CREATE VIEW [dbo].[TestProduct]
AS
    SELECT  a.[Id] ,
            a.[Category] ,
            a.[Code] ,
            a.[IsValid] ,
            a.[Name] ,
            a.[UpdateDate]
    FROM    [dbo].[Products] AS a;";

        private const string CreateViewTest2Sql =
@"CREATE VIEW [dbo].[TestProduct]
AS
    SELECT  *
    FROM    Products
    WHERE   Id > 0;";

        private const string DropViewTestSql =
@"DROP VIEW [dbo].[TestProduct];";

        private const string ViewIsExsitTestSql =
@"SELECT  CASE WHEN OBJECT_ID('[dbo].[Products]', 'V') IS NULL
             THEN CAST(0 AS BIT)
             ELSE CAST(1 AS BIT)
        END;";

        private const string RenameTableTestSql =
@"EXEC sp_rename '[dbo].[Products]', 'TestProduct';";

        private const string RenameViewTestSql =
@"EXEC sp_rename '[dbo].[Products]', 'TestProduct';";

        private const string TableIsExsitTestSql =
@"SELECT  CASE WHEN OBJECT_ID('[dbo].[Products]', 'U') IS NULL
             THEN CAST(0 AS BIT)
             ELSE CAST(1 AS BIT)
        END;";

        private const string CreateRelationTestSql =
@"ALTER TABLE [dbo].[Orders] ADD CONSTRAINT [FK_Orders_Customers_CustomerId_Id] FOREIGN KEY([CustomerId]) 
REFERENCES [dbo].[Customers] ([Id]);";

        private const string CreateCompositeRelationTestSql =
@"ALTER TABLE [dbo].[OrderDetails] ADD CONSTRAINT [FK_OrderDetails_Orders_OrderId_Id] FOREIGN KEY([OrderId]) REFERENCES [dbo].[Orders] ([Id]);";

        private const string CreateCompositeRelationTestSql1 =
@"ALTER TABLE [dbo].[OrderDetails] ADD CONSTRAINT [FK_OrderDetails_Products_ProductId_Id] FOREIGN KEY([ProductId]) REFERENCES [dbo].[Products] ([Id]);";

    }
}
