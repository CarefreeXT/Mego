namespace Caredev.Mego.Tests.Core.Commit.Simple
{
    public partial class InsertPropertysTest
    {

        private const string InsertSingleObjectTestSql =
@"INSERT INTO ""public"".""Customers""
(""Id"",""Address1"",""Name"")
VALUES(@p0,@p1 || @p2,@p3 || @p1)
RETURNING ""Address1"",""Name"";";
        
        private const string InsertMultiObjectTestSql =
@"INSERT INTO ""public"".""Customers""
(""Id"",""Address1"",""Name"")
VALUES(@p0,@p1 || @p2,@p3 || @p1),
(@p4,@p1 || @p2,@p5 || @p1)
RETURNING ""Address1"",""Name"";"; 

        private const string InsertIdentitySingleObjectTestSql =
@"INSERT INTO ""public"".""Products""
(""Category"",""Code"",""IsValid"",""Name"",""UpdateDate"")
VALUES(@p0,@p1,@p2,@p3 || @p4,LOCALTIMESTAMP)
RETURNING ""Id"",""Name"",""UpdateDate"";";
        
        private const string InsertIdentityMultiObjectTestSql =
@"INSERT INTO ""public"".""Products""
(""Category"",""Code"",""IsValid"",""Name"",""UpdateDate"")
VALUES(@p0,@p1,@p2,@p3 || @p4,LOCALTIMESTAMP),
(@p5,@p6,@p7,@p3 || @p4,LOCALTIMESTAMP)
RETURNING ""Id"",""Name"",""UpdateDate"";"; 

        private const string InsertExpressionSingleObjectTestSql =
@"INSERT INTO ""public"".""Orders""
(""Id"",""CreateDate"",""CustomerId"",""ModifyDate"",""State"")
VALUES(@p0,LOCALTIMESTAMP,@p1,LOCALTIMESTAMP,@p2)
RETURNING ""CreateDate"",""CustomerId"",""ModifyDate"";";
        
        private const string InsertExpressionMultiObjectTestSql =
@"INSERT INTO ""public"".""Orders""
(""Id"",""CreateDate"",""CustomerId"",""ModifyDate"",""State"")
VALUES(@p0,LOCALTIMESTAMP,@p1,LOCALTIMESTAMP,@p2),
(@p3,LOCALTIMESTAMP,@p1,LOCALTIMESTAMP,@p4)
RETURNING ""CreateDate"",""CustomerId"",""ModifyDate"";"; 
    }
}