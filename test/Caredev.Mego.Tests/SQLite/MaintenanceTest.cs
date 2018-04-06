namespace Caredev.Mego.Tests.Core.Commit.Simple
{
    partial class MaintenanceTest
    {
        private const string CreateTableTestSql =
@"CREATE TABLE [Products] (
    [Id] INTEGER NOT NULL, 
    [Category] INTEGER NOT NULL, 
    [Code] TEXT NULL, 
    [IsValid] NUMERIC NOT NULL, 
    [Name] TEXT NULL, 
    [UpdateDate] NUMERIC NOT NULL,
    PRIMARY KEY ([Id])
);";

        private const string TableExsitTestSql =
@"SELECT CASE WHEN EXISTS (SELECT 1 FROM [sqlite_master] t WHERE t.type='table' AND t.name='Products') THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END;";

        private const string CreateRelationTestSql =
@";";

        private const string CreateCompositeRelationTestSql =
@";";

        private const string CreateCompositeRelationTestSql1 =
@";";

    }
}
