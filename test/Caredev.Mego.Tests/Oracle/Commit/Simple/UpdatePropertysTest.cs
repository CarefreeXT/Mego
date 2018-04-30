namespace Caredev.Mego.Tests.Core.Commit.Simple
{
    public partial class UpdatePropertysTest
    {
        private const string UpdateSingleObjectTestSql =
@"UPDATE    ""SIMPLE"".""Customers"" a
SET     ""Address1"" = :p0 || :p1 || :p2, 
        ""Name"" = :p3
WHERE   a.""Id"" = :p4 AND a.""Zip"" = :p5 
RETURNING   ""Address1"" INTO :p6";

        private const string UpdateMultiObjectTestSql =
@"UPDATE    ""SIMPLE"".""Customers"" a
SET     ""Address1"" = :p0 || :p1 || :p2, 
        ""Name"" = :p3
WHERE   a.""Id"" = :p4 AND a.""Zip"" = :p5 
RETURNING   ""Address1"" INTO :p6";

        private const string UpdateGenerateSingleObjectTestSql =
@"UPDATE    ""SIMPLE"".""Products"" a
SET     ""Category"" = :p0, 
        ""Code"" = :p1 || :p2, 
        ""Name"" = :p3, 
        ""UpdateDate"" = SYSDATE
WHERE   a.""Id"" = :p4
RETURNING ""Code"",""UpdateDate"" INTO :p5,:p6";

        private const string UpdateGenerateMultiObjectTestSql =
@"UPDATE    ""SIMPLE"".""Products"" a
SET     ""Category"" = :p0, 
        ""Code"" = :p1 || :p2, 
        ""Name"" = :p3, 
        ""UpdateDate"" = SYSDATE
WHERE   a.""Id"" = :p4 
RETURNING ""Code"",""UpdateDate"" INTO :p5,:p6";
    }
}