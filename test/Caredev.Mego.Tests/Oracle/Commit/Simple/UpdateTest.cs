namespace Caredev.Mego.Tests.Core.Commit.Simple
{
    public partial class UpdateTest
    {
        private const string UpdateSingleObjectTestSql =
@"UPDATE  ""SIMPLE"".""Customers"" a
SET     ""Address1"" = :p0 ,
        ""Address2"" = :p1 ,
        ""Code"" = :p2 ,
        ""Name"" = :p3 ,
        ""Zip"" = :p4
WHERE   a.""Id"" = :p5 AND a.""Zip"" = :p6";
        
        private const string UpdateMultiObjectTestSql =
@"UPDATE  ""SIMPLE"".""Customers"" a
SET     ""Address1"" = :p0 ,
        ""Address2"" = :p1 ,
        ""Code"" = :p2 ,
        ""Name"" = :p3 ,
        ""Zip"" = :p4
WHERE   a.""Id"" = :p5 AND a.""Zip"" = :p6";

        private const string UpdateGenerateSingleObjectTestSql =
@"UPDATE ""SIMPLE"".""Products"" a
SET     ""Category"" = :p0, 
        ""Code"" = :p1, 
        ""IsValid"" = :p2, 
        ""Name"" = :p3, 
        ""UpdateDate"" = SYSDATE
WHERE a.""Id"" = :p4
RETURNING ""UpdateDate"" INTO :p5";
        
        private const string UpdateGenerateMultiObjectTestSql =
@"UPDATE ""SIMPLE"".""Products"" a
SET     ""Category"" = :p0, 
        ""Code"" = :p1, 
        ""IsValid"" = :p2, 
        ""Name"" = :p3, 
        ""UpdateDate"" = SYSDATE
WHERE a.""Id"" = :p4
RETURNING ""UpdateDate"" INTO :p5";

        private const string UpdateQueryTestSql =
@"UPDATE ""SIMPLE"".""Customers"" a
SET (   ""Name"", ""Code"", ""Address1"") = 
    (   SELECT b.""Name"", b.""Name"", b.""Code""
        FROM ""SIMPLE"".""Customers"" a
        INNER JOIN ""SIMPLE"".""Products"" b ON a.""Id"" = b.""Id""
        WHERE b.""Id"" > :p0
    )";
    }
}