namespace Caredev.Mego.Tests.Core.Query.Inherit
{
    public partial class SqlQueryTest
    {
        private const string SqlQueryValueTestSql = @"SELECT Id FROM ProductBases";

        private const string SqlQueryCollectionTestSql =
@"SELECT  a.Id ,
        a.Code ,
        a.Name ,
        b.Category ,
        b.IsValid ,
        b.UpdateDate
FROM    ProductBases a
        INNER JOIN Products b ON b.Id = a.Id;";
    }
}