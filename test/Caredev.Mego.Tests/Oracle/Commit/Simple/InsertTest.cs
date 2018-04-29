namespace Caredev.Mego.Tests.Core.Commit.Simple
{
    public partial class InsertTest
    {
        private const string InsertSingleObjectTestSql =
@"INSERT  INTO ""SIMPLE"".""Customers""
        ( ""Id"", ""Address1"", ""Address2"", ""Code"", ""Name"", ""Zip"" )
VALUES  ( :p0, :p1, :p2, :p3, :p4, :p5 )";

        private const string InsertMultiObjectTestSql =
@"INSERT  INTO ""SIMPLE"".""Customers""
        ( ""Id"", ""Address1"", ""Address2"", ""Code"", ""Name"", ""Zip"" )
VALUES  ( :p0, :p1, :p2, :p3, :p4, :p5 )";

        private const string InsertIdentitySingleObjectTestSql =
@"INSERT INTO ""SIMPLE"".""Products""
(""Id"",""Category"",""Code"",""IsValid"",""Name"",""UpdateDate"")
VALUES(""SIMPLE"".""ProductSequence"".NEXTVAL,:p0,:p1,:p2,:p3,SYSDATE) 
RETURNING ""Id"",""UpdateDate"" INTO :p4,:p5";
        
        private const string InsertIdentityMultiObjectTestSql =
@"INSERT INTO ""SIMPLE"".""Products""
(""Id"",""Category"",""Code"",""IsValid"",""Name"",""UpdateDate"")
VALUES(""SIMPLE"".""ProductSequence"".NEXTVAL,:p0,:p1,:p2,:p3,SYSDATE) 
RETURNING ""Id"",""UpdateDate"" INTO :p4,:p5"; 

        private const string InsertExpressionSingleObjectTestSql =
@"INSERT INTO ""SIMPLE"".""Orders""
(""Id"",""CreateDate"",""CustomerId"",""ModifyDate"",""State"")
VALUES(:p0,SYSDATE,:p1,:p2,:p3) RETURNING ""CreateDate"" INTO :p4";
        
        private const string InsertExpressionMultiObjectTestSql =
@"INSERT INTO ""SIMPLE"".""Orders""
(""Id"",""CreateDate"",""CustomerId"",""ModifyDate"",""State"")
VALUES(:p0,SYSDATE,:p1,:p2,:p3) RETURNING ""CreateDate"" INTO :p4"; 

        private const string InsertQueryTestSql =
@"INSERT  INTO ""SIMPLE"".""Customers""
        ( ""Id"" ,
          ""Name"" ,
          ""Code"" ,
          ""Address1""
        )
        SELECT  a.""Id"" + :p0 AS ""Id"" ,
                a.""Name"" ,
                a.""Name"" ,
                a.""Code""
        FROM    ""SIMPLE"".""Products"" a
        WHERE   a.""Id"" > :p1";
    }
}