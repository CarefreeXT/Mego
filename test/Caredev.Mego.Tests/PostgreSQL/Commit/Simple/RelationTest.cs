namespace Caredev.Mego.Tests.Core.Commit.Simple
{
    public partial class RelationTest
    {

        private const string AddObjectSingleTestSql =
@"UPDATE  ""public"".""Orders"" AS a
SET     ""CustomerId"" = @p0
WHERE   a.""Id"" = @p1;";

        private const string AddCollectionSingleTestSql =
@"UPDATE  ""public"".""OrderDetails"" AS a
SET     ""OrderId"" = @p0
WHERE   a.""Id"" = @p1;";

        private const string RemoveObjectSingleTestSql =
@"UPDATE  ""public"".""Orders"" AS a
SET     ""CustomerId"" = NULL
WHERE   a.""Id"" = @p0;";

        private const string RemoveCollectionSingleTestSql =
@"UPDATE  ""public"".""OrderDetails"" AS a
SET     ""OrderId"" = NULL
WHERE   a.""Id"" = @p0;";
        
        private const string AddObjectMultiTestSql =
@"UPDATE  ""public"".""Orders"" AS a
SET     ""CustomerId"" = b.""CustomerId""
FROM    (VALUES  ( @p0, @p1 ),
        ( @p2, @p3 )) AS b ( ""Id"", ""CustomerId"" )
WHERE    a.""Id"" = b.""Id"";"; 
        
        private const string AddCollectionMultiTestSql =
@"UPDATE  ""public"".""OrderDetails"" AS a
SET     ""OrderId"" = b.""OrderId""
FROM    (   VALUES  
            ( @p0, @p1 ),
            ( @p2, @p3 )
        ) AS b ( ""Id"",""OrderId"" )
WHERE a.""Id"" = b.""Id"";"; 

        private const string RemoveObjectMultiTestSql =
@"UPDATE  ""public"".""Orders"" AS a
SET     ""CustomerId"" = NULL
WHERE   a.""Id"" IN ( @p0, @p1 );";

        private const string RemoveCollectionMultiTestSql =
@"UPDATE  ""public"".""OrderDetails"" AS a
SET     ""OrderId"" = NULL
WHERE   a.""Id"" IN ( @p0, @p1 );";

        private const string AddCompositeSingleTestSql =
@"INSERT  INTO ""public"".""OrderDetails""
        ( ""OrderId"", ""ProductId"" )
VALUES  ( @p0, @p1 );";

        private const string RemoveCompositeSingleTestSql =
@"DELETE FROM ""public"".""OrderDetails"" AS a
WHERE a.""OrderId"" = @p0 AND a.""ProductId"" = @p1;";
        
        private const string AddCompositeMultiTestSql =
@"INSERT  INTO ""public"".""OrderDetails""
        ( ""OrderId"", ""ProductId"" )
VALUES  ( @p0, @p1 ),
        ( @p2, @p3 );";

        private const string RemoveCompositeMultiTestSql =
@"DELETE FROM   ""public"".""OrderDetails"" AS a
USING   (   VALUES
            ( @p0, @p1 ),
            ( @p2, @p3 )
        ) AS b (""OrderId"",""ProductId"")
WHERE a.""OrderId"" = b.""OrderId"" AND a.""ProductId"" = b.""ProductId"";"; 
    }
}