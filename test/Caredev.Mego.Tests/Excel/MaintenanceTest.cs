namespace Caredev.Mego.Tests.Core.Commit.Simple
{
    partial class MaintenanceTest
    {
        private const string CreateTableTestSql =
@"CREATE TABLE [TestProduct](
    [Id] INTEGER, 
    [Category] INTEGER, 
    [Code] LONGTEXT, 
    [IsValid] BIT, 
    [Name] LONGTEXT, 
    [UpdateDate] DATETIME
)";

        private const string DropRelationTestSql = Constants.NotSuppored;
        private const string DropCompositeRelationTestSql = Constants.NotSuppored;
        private const string DropCompositeRelationTestSql1 = Constants.NotSuppored;
        private const string CreateTempTableTestSql = Constants.NotSuppored;
        private const string CreateTableVariableTestSql = Constants.NotSuppored;
        private const string TableIsExsitTestSql = Constants.NotSuppored;

        private const string RenameTableTestSql = Constants.NotSuppored;

        private const string DropTableTestSql =
@"DROP TABLE [TestProduct]";

        private const string CreateViewTestSql = Constants.NotSuppored;
        private const string CreateViewTest2Sql = Constants.NotSuppored;
        private const string DropViewTestSql = Constants.NotSuppored;
        private const string ViewIsExsitTestSql = Constants.NotSuppored;

        private const string CreateRelationTestSql = Constants.NotSuppored;
        private const string CreateCompositeRelationTestSql = Constants.NotSuppored;
        private const string CreateCompositeRelationTestSql1 = Constants.NotSuppored;


        private const string RenameViewTestSql = Constants.NotSuppored;

    }
}
