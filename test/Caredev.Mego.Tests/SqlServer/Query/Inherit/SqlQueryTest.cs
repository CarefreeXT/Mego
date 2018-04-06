namespace Caredev.Mego.Tests.Core.Query.Inherit
{
    public partial class SqlQueryTest
    {
        private const string SqlQueryValueTestSql = @"SELECT Id FROM dbo.ProductBases";

        private const string SqlQueryCollectionTestSql =
@"SELECT  a.Id ,
        a.Code ,
        a.Name ,
        b.Category ,
        b.IsValid ,
        b.UpdateDate
FROM    dbo.ProductBases a
        INNER JOIN dbo.Products b ON b.Id = a.Id;";
    }
}