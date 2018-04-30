namespace Caredev.Mego.Tests.Core.Commit.Simple
{
    public partial class RelationTest
    {

        private const string AddObjectSingleTestSql =
@"UPDATE  ""SIMPLE"".""Orders"" a
SET     ""CustomerId"" = :p0
WHERE   a.""Id"" = :p1";

        private const string AddCollectionSingleTestSql =
@"UPDATE  ""SIMPLE"".""OrderDetails"" a
SET     ""OrderId"" = :p0
WHERE   a.""Id"" = :p1";

        private const string RemoveObjectSingleTestSql =
@"UPDATE  ""SIMPLE"".""Orders"" a
SET     ""CustomerId"" = NULL
WHERE   a.""Id"" = :p0";

        private const string RemoveCollectionSingleTestSql =
@"UPDATE  ""SIMPLE"".""OrderDetails"" a
SET     ""OrderId"" = NULL
WHERE   a.""Id"" = :p0";
        
        private const string AddObjectMultiTestSql =
@"UPDATE  ""SIMPLE"".""Orders"" a
SET     ""CustomerId"" = :p0
WHERE    a.""Id"" = :p1"; 
        
        private const string AddCollectionMultiTestSql =
@"UPDATE  ""SIMPLE"".""OrderDetails"" a
SET     ""OrderId"" = :p0
WHERE a.""Id"" = :p1"; 

        private const string RemoveObjectMultiTestSql =
@"UPDATE  ""SIMPLE"".""Orders"" a
SET     ""CustomerId"" = NULL
WHERE   a.""Id"" = :p0";

        private const string RemoveCollectionMultiTestSql =
@"UPDATE  ""SIMPLE"".""OrderDetails"" a
SET     ""OrderId"" = NULL
WHERE   a.""Id"" = :p0";

        private const string AddCompositeSingleTestSql =
@"INSERT  INTO ""SIMPLE"".""OrderDetails""
        ( ""OrderId"", ""ProductId"" )
VALUES  ( :p0, :p1 )";

        private const string RemoveCompositeSingleTestSql =
@"DELETE FROM ""SIMPLE"".""OrderDetails"" a
WHERE a.""OrderId"" = :p0 AND a.""ProductId"" = :p1";
        
        private const string AddCompositeMultiTestSql =
@"INSERT  INTO ""SIMPLE"".""OrderDetails""
        ( ""OrderId"", ""ProductId"" )
VALUES  ( :p0, :p1 )";

        private const string RemoveCompositeMultiTestSql =
@"DELETE FROM   ""SIMPLE"".""OrderDetails"" a
WHERE a.""OrderId"" = :p0 AND a.""ProductId"" = :p1"; 
    }
}