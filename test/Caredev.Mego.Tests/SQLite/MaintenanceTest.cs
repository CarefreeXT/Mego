namespace Caredev.Mego.Tests.Core.Commit.Simple
{
    partial class MaintenanceTest
    {
        private const string CreateTableTestSql =
@"CREATE TABLE [TestProduct] (
    [Id] INTEGER NOT NULL, 
    [Category] INTEGER NOT NULL, 
    [Code] TEXT NULL, 
    [IsValid] NUMERIC NOT NULL, 
    [Name] TEXT NULL, 
    [UpdateDate] NUMERIC NOT NULL,
    PRIMARY KEY ([Id])
);";

        private const string TableIsExsitTestSql =
@"SELECT CASE WHEN EXISTS (SELECT 1 FROM [sqlite_master] t WHERE t.type='table' AND t.name='Products') THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END;";
        
        private const string CreateTempTableTestSql =
@"CREATE TEMPORARY TABLE [TestProduct](
    [Id] INTEGER NULL, 
    [Category] INTEGER NULL, 
    [Code] TEXT NULL, 
    [IsValid] NUMERIC NULL, 
    [Name] TEXT NULL, 
    [UpdateDate] NUMERIC NULL
);";

        private const string CreateTableVariableTestSql = @"";

        private const string DropTableTestSql = 
@"DROP TABLE [TestProduct];";

        private const string CreateViewTestSql =
@"CREATE VIEW [TestProduct] AS 
SELECT
a.[Id], a.[Category], a.[Code], a.[IsValid], a.[Name], a.[UpdateDate]
FROM [Products] AS a;";

        private const string CreateViewTest2Sql =
@"CREATE VIEW [TestProduct] AS 
SELECT * FROM Products WHERE Id>0;";

        private const string DropViewTestSql =
@"DROP VIEW [TestProduct];";

        private const string ViewIsExsitTestSql =
@"SELECT CASE WHEN EXISTS (SELECT 1 FROM [sqlite_master] t WHERE t.type='view' AND t.name='Products') THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END;";

        private const string RenameTableTestSql =
@"ALTER TABLE [Products] RENAME TO [TestProduct];";

        private const string RenameViewTestSql = @"";
    }
}
