namespace Caredev.Mego.Tests.Core.Commit.Simple
{
    public partial class UpdatePropertysTest : ISimpleTest
    {
        private const string UpdateSingleObjectTestSql =
@"UPDATE    ""public"".""Customers"" AS a
SET     ""Address1"" = @p0 || @p1 || @p2, 
        ""Name"" = @p3
WHERE   a.""Id"" = @p4 AND a.""Zip"" = @p5
RETURNING   a.""Address1"";";

        private const string UpdateMultiObjectTestSql =
@"UPDATE    ""public"".""Customers"" AS a
SET     ""Address1"" = b.""Address2"" || @p0 || b.""Address1"", 
        ""Name"" = b.""Name""
FROM    (   VALUES
            (@p1,@p2,@p3,@p4,@p5),
            (@p6,@p7,@p8,@p9,@pA)
        ) AS b (""Address2"",""Address1"",""Name"",""Id"",""Zip"")
WHERE   a.""Id"" = b.""Id"" AND a.""Zip"" = b.""Zip""
RETURNING   a.""Address1"";"; 

        private const string UpdateGenerateSingleObjectTestSql =
@"UPDATE    ""public"".""Products"" AS a
SET     ""Category"" = @p0, 
        ""Code"" = @p1 || @p2, 
        ""Name"" = @p1, 
        ""UpdateDate"" = LOCALTIMESTAMP
WHERE   a.""Id"" = @p3
RETURNING   a.""Code"",a.""UpdateDate"";";

        private const string UpdateGenerateMultiObjectTestSql =
@"UPDATE    ""public"".""Products"" AS a
SET     ""Category"" = b.""Category"", 
        ""Code"" = b.""Name"" || b.""Code"", 
        ""Name"" = b.""Name"", ""UpdateDate"" = LOCALTIMESTAMP
FROM    (   VALUES
            (@p0,@p1,@p2,@p0),
            (@p0,@p3,@p4,@p5)
        ) AS b (""Category"",""Name"",""Code"",""Id"")
WHERE   a.""Id"" = b.""Id""
RETURNING   a.""Code"",a.""UpdateDate"";"; 
    }
}