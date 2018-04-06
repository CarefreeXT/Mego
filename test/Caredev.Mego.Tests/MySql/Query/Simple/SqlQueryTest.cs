namespace Caredev.Mego.Tests.Core.Query.Simple
{
    public partial class SqlQueryTest
    {
        private const string SqlQueryValueTestSql = @"SELECT `Id` FROM `Products`";

        private const string SqlQueryCollectionTestSql = @"SELECT `Id` ,`Code` ,`Name` ,`Category` ,`IsValid` FROM `Products`";
    }
}