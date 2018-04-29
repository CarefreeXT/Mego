namespace Caredev.Mego.Tests.Core.Query.Simple
{
    public partial class SingleEntityTest
    {
        private const string QueryAllDataTestSql =
@"SELECT  a.""Id"" ,
        a.""Category"" ,
        a.""Code"" ,
        a.""IsValid"" ,
        a.""Name"" ,
        a.""UpdateDate""
FROM    ""SIMPLE"".""Products"" a";

        private const string OrderQueryDataTestSql =
@"SELECT  a.""Id"" ,
        a.""Category"" ,
        a.""Code"" ,
        a.""IsValid"" ,
        a.""Name"" ,
        a.""UpdateDate""
FROM    ""SIMPLE"".""Products"" a
ORDER BY a.""Category"" ASC";

        private const string QueryFilterContainsTestSql =
@"SELECT  a.""Id"" ,
        a.""Category"" ,
        a.""Code"" ,
        a.""IsValid"" ,
        a.""Name"" ,
        a.""UpdateDate""
FROM    ""SIMPLE"".""Products"" a
WHERE   a.""Id"" IN ( :p0, :p1, :p2, :p3 )";
    }
}