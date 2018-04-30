namespace Caredev.Mego.Tests.Core.Commit.Simple
{
    public partial class InsertPropertysTest
    {

        private const string InsertSingleObjectTestSql =
@"INSERT INTO ""SIMPLE"".""Customers""
(""Id"",""Address1"",""Name"")
VALUES(:p0,:p1 || :p2,:p3 || :p4) 
RETURNING ""Address1"",""Name"" INTO :p5,:p6";

        private const string InsertMultiObjectTestSql =
@"INSERT INTO ""SIMPLE"".""Customers""
(""Id"",""Address1"",""Name"")
VALUES(:p0,:p1 || :p2,:p3 || :p4) 
RETURNING ""Address1"",""Name"" INTO :p5,:p6";

        private const string InsertIdentitySingleObjectTestSql =
@"INSERT INTO ""SIMPLE"".""Products""
(""Id"",""Category"",""Code"",""IsValid"",""Name"",""UpdateDate"")
VALUES(""SIMPLE"".""ProductSequence"".NEXTVAL,:p0,:p1,:p2,:p3 || :p4,SYSDATE) 
RETURNING ""Id"",""Name"",""UpdateDate"" INTO :p5,:p6,:p7";

        private const string InsertIdentityMultiObjectTestSql =
@"INSERT INTO ""SIMPLE"".""Products""
(""Id"",""Category"",""Code"",""IsValid"",""Name"",""UpdateDate"")
VALUES(""SIMPLE"".""ProductSequence"".NEXTVAL,:p0,:p1,:p2,:p3 || :p4,SYSDATE) 
RETURNING ""Id"",""Name"",""UpdateDate"" INTO :p5,:p6,:p7";

        private const string InsertExpressionSingleObjectTestSql =
@"INSERT INTO ""SIMPLE"".""Orders""
(""Id"",""CreateDate"",""CustomerId"",""ModifyDate"",""State"")
VALUES(:p0,SYSDATE,:p1,SYSDATE,:p2) 
RETURNING ""CreateDate"",""CustomerId"",""ModifyDate"" INTO :p3,:p4,:p5";

        private const string InsertExpressionMultiObjectTestSql =
@"INSERT INTO ""SIMPLE"".""Orders""
(""Id"",""CreateDate"",""CustomerId"",""ModifyDate"",""State"")
VALUES(:p0,SYSDATE,:p1,SYSDATE,:p2) 
RETURNING ""CreateDate"",""CustomerId"",""ModifyDate"" INTO :p3,:p4,:p5";
    }
}