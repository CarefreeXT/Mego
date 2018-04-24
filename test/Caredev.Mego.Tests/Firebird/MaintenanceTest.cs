namespace Caredev.Mego.Tests.Core.Commit.Simple
{
    partial class MaintenanceTest
    {
        private const string CreateTableTestSql =
@"CREATE TABLE ""TestProduct""(
    ""Id"" INTEGER NOT NULL, 
    ""Category"" INTEGER NOT NULL, 
    ""Code"" BLOB SUB_TYPE 1, 
    ""IsValid"" SMALLINT NOT NULL, 
    ""Name"" BLOB SUB_TYPE 1, 
    ""UpdateDate"" TIMESTAMP NOT NULL,
    PRIMARY KEY ( ""Id"")
);";

        private const string DropRelationTestSql =
@"ALTER TABLE ""Orders"" DROP CONSTRAINT ""FK_Orders_Customers_CustomerId_"";";

        private const string DropCompositeRelationTestSql =
@"ALTER TABLE ""OrderDetails"" DROP CONSTRAINT ""FK_OrderDetails_Orders_OrderId_"";";

        private const string DropCompositeRelationTestSql1 =
@"ALTER TABLE ""OrderDetails"" DROP CONSTRAINT ""FK_OrderDetails_Products_Produc"";";

        private const string CreateTempTableTestSql =
@"CREATE TABLE #TestProduct
    (
      Id INT NULL ,
      Category INT NULL ,
      Code NVARCHAR(MAX) NULL ,
      IsValid BIT NULL ,
      Name NVARCHAR(MAX) NULL ,
      UpdateDate DATETIME2(7) NULL
    );";

        private const string CreateTableVariableTestSql = Constants.NotSuppored;

        private const string DropTableTestSql =
@"DROP TABLE ""TestProduct"";";

        private const string CreateViewTestSql =
@"CREATE VIEW ""TestProduct"" AS 
SELECT
a.""Id"", a.""Category"", a.""Code"", a.""IsValid"", a.""Name"", a.""UpdateDate""
FROM ""Products"" AS a;";

        private const string CreateViewTest2Sql =
@"CREATE VIEW ""TestProduct"" AS 
SELECT * FROM Products WHERE Id>0;";

        private const string DropViewTestSql =
@"DROP VIEW ""TestProduct"";";

        private const string ViewIsExsitTestSql =
@"SELECT COUNT(1) FROM RDB$RELATIONS WHERE RDB$VIEW_BLR IS NOT NULL AND (RDB$SYSTEM_FLAG IS NULL OR RDB$SYSTEM_FLAG = 0) AND RDB$RELATION_NAME='PRODUCTS';";

        private const string RenameTableTestSql = Constants.NotSuppored;

        private const string RenameViewTestSql = Constants.NotSuppored;

        private const string TableIsExsitTestSql =
@"SELECT COUNT(1) FROM RDB$RELATIONS WHERE RDB$VIEW_BLR IS NULL AND (RDB$SYSTEM_FLAG IS NULL OR RDB$SYSTEM_FLAG = 0) AND RDB$RELATION_NAME='PRODUCTS';";

        private const string CreateRelationTestSql =
@"ALTER TABLE ""Orders"" ADD CONSTRAINT ""FK_Orders_Customers_CustomerId_"" FOREIGN KEY(""CustomerId"") REFERENCES ""Customers"" (""Id"");";

        private const string CreateCompositeRelationTestSql =
@"ALTER TABLE ""OrderDetails"" ADD CONSTRAINT ""FK_OrderDetails_Orders_OrderId_"" FOREIGN KEY(""OrderId"") REFERENCES ""Orders"" (""Id"");";

        private const string CreateCompositeRelationTestSql1 =
@"ALTER TABLE ""OrderDetails"" ADD CONSTRAINT ""FK_OrderDetails_Products_Produc"" FOREIGN KEY(""ProductId"") REFERENCES ""Products"" (""Id"");";

    }
}
