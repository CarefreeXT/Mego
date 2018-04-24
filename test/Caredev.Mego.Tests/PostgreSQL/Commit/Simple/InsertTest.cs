namespace Caredev.Mego.Tests.Core.Commit.Simple
{
    public partial class InsertTest
    {
        private const string InsertSingleObjectTestSql =
@"INSERT INTO ""public"".""Customers""
(""Id"",""Address1"",""Address2"",""Code"",""Name"",""Zip"")
VALUES(@p0,@p1,@p2,@p3,@p4,@p5);";

        private const string InsertMultiObjectTestSql =
@"INSERT  INTO ""public"".""Customers""
        ( ""Id"", ""Address1"", ""Address2"", ""Code"", ""Name"", ""Zip"" )
VALUES  ( @p0, @p1, @p2, @p3, @p4, @p5 ),
        ( @p6, @p1, @p2, @p3, @p7, @p5 );";

        private const string InsertIdentitySingleObjectTestSql =
@"INSERT INTO ""public"".""Products""
(""Category"",""Code"",""IsValid"",""Name"",""UpdateDate"")
VALUES(@p0,@p1,@p2,@p3,LOCALTIMESTAMP)
RETURNING ""Id"",""UpdateDate"";";

        private const string InsertIdentityMultiObjectTestSql =
@"INSERT INTO ""public"".""Products""
(""Category"",""Code"",""IsValid"",""Name"",""UpdateDate"")
VALUES(@p0,@p1,@p2,@p3,LOCALTIMESTAMP),
(@p4,@p5,@p6,@p3,LOCALTIMESTAMP)
RETURNING ""Id"",""UpdateDate"";"; 


        private const string InsertExpressionSingleObjectTestSql =
@"INSERT INTO ""public"".""Orders""
(""Id"",""CreateDate"",""CustomerId"",""ModifyDate"",""State"")
VALUES(@p0,LOCALTIMESTAMP,@p1,@p2,@p3)
RETURNING ""CreateDate"";";
        
        private const string InsertExpressionMultiObjectTestSql =
@"INSERT INTO ""public"".""Orders""
(""Id"",""CreateDate"",""CustomerId"",""ModifyDate"",""State"")
VALUES(@p0,LOCALTIMESTAMP,@p1,@p2,@p3),
(@p4,LOCALTIMESTAMP,@p1,@p2,@p5)
RETURNING ""CreateDate"";"; 

        private const string InsertQueryTestSql =
@"INSERT  INTO ""public"".""Customers""
        ( ""Id"" ,
          ""Name"" ,
          ""Code"" ,
          ""Address1""
        )
        SELECT  a.""Id"" + @p0 AS ""Id"" ,
                a.""Name"" ,
                a.""Name"" ,
                a.""Code""
        FROM    ""public"".""Products"" AS a
        WHERE   a.""Id"" > @p1;";
    }
}