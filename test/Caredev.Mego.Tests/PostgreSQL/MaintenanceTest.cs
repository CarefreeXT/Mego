namespace Caredev.Mego.Tests.Core.Commit.Simple
{
    partial class MaintenanceTest
    {
        private const string CreateTableTestSql =
@"CREATE TABLE ""public"".""TestProduct""(
	""Id"" SERIAL NOT NULL, 
	""Category"" INTEGER NOT NULL, 
	""Code"" TEXT NULL, 
	""IsValid"" BOOLEAN NOT NULL, 
	""Name"" TEXT NULL, 
	""UpdateDate"" TIMESTAMP NOT NULL,
	PRIMARY KEY ( ""Id"")
);";

        private const string TableIsExsitTestSql =
@"SELECT CASE WHEN EXISTS (SELECT 1 FROM information_schema.TABLES t WHERE t.TABLE_SCHEMA='public' AND t.TABLE_NAME='Products') THEN CAST(1 AS BOOLEAN) ELSE CAST(0 AS BOOLEAN) END;";

        private const string CreateRelationTestSql =
@"ALTER TABLE ""public"".""Orders"" ADD CONSTRAINT ""FK_Orders_Customers_CustomerId_Id"" FOREIGN KEY(""CustomerId"") REFERENCES ""public"".""Customers"" (""Id"");";

        private const string CreateCompositeRelationTestSql =
@"ALTER TABLE ""public"".""OrderDetails"" ADD CONSTRAINT ""FK_OrderDetails_Orders_OrderId_Id"" FOREIGN KEY(""OrderId"") REFERENCES ""public"".""Orders"" (""Id"");";

        private const string CreateCompositeRelationTestSql1 =
@"ALTER TABLE ""public"".""OrderDetails"" ADD CONSTRAINT ""FK_OrderDetails_Products_ProductId_Id"" FOREIGN KEY(""ProductId"") REFERENCES ""public"".""Products"" (""Id"");";

        private const string DropRelationTestSql =
@"ALTER TABLE ""public"".""Orders"" DROP CONSTRAINT ""FK_Orders_Customers_CustomerId_Id"";";

        private const string DropCompositeRelationTestSql =
@"ALTER TABLE ""public"".""OrderDetails"" DROP CONSTRAINT ""FK_OrderDetails_Orders_OrderId_Id"";";

        private const string DropCompositeRelationTestSql1 =
@"ALTER TABLE ""public"".""OrderDetails"" DROP CONSTRAINT ""FK_OrderDetails_Products_ProductId_Id"";";

        private const string CreateTempTableTestSql =
@"CREATE TEMPORARY TABLE ""TestProduct""(
    ""Id"" SERIAL, 
    ""Category"" INTEGER, 
    ""Code"" TEXT, 
    ""IsValid"" BOOLEAN, 
    ""Name"" TEXT, 
    ""UpdateDate"" TIMESTAMP
);";

        private const string CreateTableVariableTestSql = @"";

        private const string DropTableTestSql = @"DROP TABLE ""public"".""TestProduct"";";

        private const string CreateViewTestSql =
@"CREATE VIEW ""public"".""TestProduct"" AS 
SELECT
a.""Id"", a.""Category"", a.""Code"", a.""IsValid"", a.""Name"", a.""UpdateDate""
FROM ""public"".""Products"" AS a;";

        private const string CreateViewTest2Sql =
@"CREATE VIEW ""public"".""TestProduct""
AS
SELECT
  *
FROM products
WHERE ID > 0;";

        private const string DropViewTestSql =
@"DROP VIEW ""public"".""TestProduct"";";

        private const string ViewIsExsitTestSql =
@"SELECT CASE WHEN EXISTS (SELECT 1 FROM information_schema.VIEWS t WHERE t.TABLE_SCHEMA='public' AND t.TABLE_NAME='Products') THEN CAST(1 AS BOOLEAN) ELSE CAST(0 AS BOOLEAN) END;";

        private const string RenameTableTestSql =
@"AlTER TABLE ""public"".""Products"" RENAME TO ""TestProduct"";";

        private const string RenameViewTestSql =
@"AlTER VIEW ""public"".""Products"" RENAME TO ""TestProduct"";";
    }
}
