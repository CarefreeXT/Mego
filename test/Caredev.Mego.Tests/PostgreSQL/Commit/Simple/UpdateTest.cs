namespace Caredev.Mego.Tests.Core.Commit.Simple
{
    public partial class UpdateTest
    {
        private const string UpdateSingleObjectTestSql =
@"UPDATE  ""public"".""Customers"" AS a
SET     ""Address1"" = @p0 ,
        ""Address2"" = @p1 ,
        ""Code"" = @p2 ,
        ""Name"" = @p3 ,
        ""Zip"" = @p4
WHERE   a.""Id"" = @p5 AND a.""Zip"" = @p4;";
        
        private const string UpdateMultiObjectTestSql =
@"UPDATE  ""public"".""Customers"" AS a
SET     ""Address1"" = b.""Address1"", 
        ""Address2"" = b.""Address2"", 
        ""Code"" = b.""Code"", 
        ""Name"" = b.""Name"", 
        ""Zip"" = b.""Zip""
FROM    (   VALUES
            (@p0,@p1,@p2,@p3,@p4,@p5),
            (@p6,@p7,@p8,@p9,@pA,@pB)
        ) AS b (""Address1"",""Address2"",""Code"",""Name"",""Zip"",""Id"")
WHERE   a.""Id"" = b.""Id"" AND a.""Zip"" = b.""Zip"";"; 

        private const string UpdateGenerateSingleObjectTestSql =
@"UPDATE ""public"".""Products"" AS a
SET     ""Category"" = @p0, 
        ""Code"" = @p1, 
        ""IsValid"" = @p2, 
        ""Name"" = @p3, 
        ""UpdateDate"" = LOCALTIMESTAMP
WHERE a.""Id"" = @p4
RETURNING a.""UpdateDate"";";
        
        private const string UpdateGenerateMultiObjectTestSql =
@"UPDATE    ""public"".""Products"" AS a
SET     ""Category"" = b.""Category"", 
        ""Code"" = b.""Code"", 
        ""IsValid"" = b.""IsValid"", 
        ""Name"" = b.""Name"", 
        ""UpdateDate"" = LOCALTIMESTAMP
FROM    (   VALUES
            (@p0,@p1,@p2,@p3,@p4),
            (@p0,@p5,@p6,@p7,@p8)
        ) AS b (""Category"",""Code"",""IsValid"",""Name"",""Id"")
WHERE   a.""Id"" = b.""Id""
RETURNING   a.""UpdateDate"";"; 

        private const string UpdateQueryTestSql =
@"UPDATE  ""public"".""Customers"" AS a
SET     ""Name"" = b.""Name"", 
        ""Code"" = b.""Name"", 
        ""Address1"" = b.""Code""
FROM    ""public"".""Products"" AS b
WHERE   a.""Id"" = b.""Id"" AND b.""Id"" > @p0;";
    }
}