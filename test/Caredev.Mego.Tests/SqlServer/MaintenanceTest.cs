namespace Caredev.Mego.Tests.Core.Commit.Simple
{
    partial class MaintenanceTest
    {
        private const string CreateTableTestSql =
@"CREATE TABLE [dbo].[Products]
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

        private const string TableExsitTestSql =
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
