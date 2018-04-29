namespace Caredev.Mego.Tests.Core.Commit.Simple
{
    partial class MaintenanceTest
    {
        private const string CreateTableTestSql =
@"CREATE TABLE ""SIMPLE"".""TestProduct""(
	""Id"" NUMBER NOT NULL, 
	""Category"" NUMBER NOT NULL, 
	""Code"" NVARCHAR2(2000) NULL, 
	""IsValid"" NUMBER(1) NOT NULL, 
	""Name"" NVARCHAR2(2000) NULL, 
	""UpdateDate"" TIMESTAMP NOT NULL,
	PRIMARY KEY ( ""Id"")
)";

        private const string TableIsExsitTestSql =
@"SELECT CASE WHEN EXISTS (SELECT 1 FROM SYS.ALL_TABLES t WHERE t.OWNER = 'SIMPLE' AND t.TABLE_NAME='Products') THEN 1 ELSE 0 END FROM DUAL";

        private const string CreateRelationTestSql =
@"ALTER TABLE ""SIMPLE"".""Orders"" ADD CONSTRAINT ""FK_Orders_Customers_CustomerId"" FOREIGN KEY(""CustomerId"") REFERENCES ""SIMPLE"".""Customers"" (""Id"")";

        private const string CreateCompositeRelationTestSql =
@"ALTER TABLE ""SIMPLE"".""OrderDetails"" ADD CONSTRAINT ""FK_OrderDetails_Orders_OrderId"" FOREIGN KEY(""OrderId"") REFERENCES ""SIMPLE"".""Orders"" (""Id"")";

        private const string CreateCompositeRelationTestSql1 =
@"ALTER TABLE ""SIMPLE"".""OrderDetails"" ADD CONSTRAINT ""FK_OrderDetails_Products_Produ"" FOREIGN KEY(""ProductId"") REFERENCES ""SIMPLE"".""Products"" (""Id"")";

        private const string DropRelationTestSql =
@"ALTER TABLE ""SIMPLE"".""Orders"" DROP CONSTRAINT ""FK_Orders_Customers_CustomerId""";

        private const string DropCompositeRelationTestSql =
@"ALTER TABLE ""SIMPLE"".""OrderDetails"" DROP CONSTRAINT ""FK_OrderDetails_Orders_OrderId""";

        private const string DropCompositeRelationTestSql1 =
@"ALTER TABLE ""SIMPLE"".""OrderDetails"" DROP CONSTRAINT ""FK_OrderDetails_Products_Produ""";

        private const string CreateTempTableTestSql =
@"CREATE GLOBAL TEMPORARY TABLE ""TestProduct""(
	""Id"" NUMBER NULL, 
	""Category"" NUMBER NULL, 
	""Code"" NCLOB NULL, 
	""IsValid"" NUMBER(1) NULL, 
	""Name"" NCLOB NULL, 
	""UpdateDate"" TIMESTAMP NULL
)";

        private const string CreateTableVariableTestSql = Constants.NotSuppored;

        private const string DropTableTestSql = @"DROP TABLE ""SIMPLE"".""TestProduct""";

        private const string CreateViewTestSql =
@"CREATE VIEW ""SIMPLE"".""TestProduct"" AS 
SELECT
a.""Id"", a.""Category"", a.""Code"", a.""IsValid"", a.""Name"", a.""UpdateDate""
FROM ""SIMPLE"".""Products"" a";

        private const string CreateViewTest2Sql =
@"CREATE VIEW ""SIMPLE"".""TestProduct""
AS
SELECT
  *
FROM products
WHERE ID > 0";

        private const string DropViewTestSql =
@"DROP VIEW ""SIMPLE"".""TestProduct""";

        private const string ViewIsExsitTestSql =
@"SELECT CASE WHEN EXISTS (SELECT 1 FROM SYS.ALL_VIEWS t WHERE t.OWNER = 'SIMPLE' AND t.VIEW_NAME='Products') THEN 1 ELSE 0 END FROM DUAL";

        private const string RenameTableTestSql =
@"RENAME ""SIMPLE"".""Products"" TO ""TestProduct""";

        private const string RenameViewTestSql =
@"RENAME ""SIMPLE"".""Products""  TO ""TestProduct""";
    }
}
