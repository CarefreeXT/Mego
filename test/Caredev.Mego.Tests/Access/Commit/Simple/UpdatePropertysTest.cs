namespace Caredev.Mego.Tests.Core.Commit.Simple
{
    public partial class UpdatePropertysTest 
    {
        private const string UpdateSingleObjectTestSql =
@"UPDATE  [Customers]
SET     [Address1] = @p0 + @p1 + @p2 ,
        [Name] = @p3
WHERE   [Id] = @p4;
SELECT  a.[Address1]
FROM    [Customers] AS a
WHERE   a.[Id] = @p4;";
        
        private const string UpdateMultiObjectTestSql =
@"UPDATE  [Customers]
SET     [Address1] = @p0 + @p1 + @p2 ,
        [Name] = @p3
WHERE   [Id] = @p4;
SELECT  a.[Address1]
FROM    [Customers] AS a
WHERE   a.[Id] = @p4;";

        private const string UpdateGenerateSingleObjectTestSql =
@"UPDATE  [Products]
SET     [Category] = @p0 ,
        [Code] = @p1 + @p2 ,
        [Name] = @p1 ,
        [UpdateDate] = GETDATE()
WHERE   [Id] = @p3;
SELECT  a.[Code] ,
        a.[UpdateDate]
FROM    [Products] AS a
WHERE   a.[Id] = @p3;";
        
        private const string UpdateGenerateMultiObjectTestSql =
@"UPDATE  [Products]
SET     [Category] = @p0 ,
        [Code] = @p1 + @p2 ,
        [Name] = @p1 ,
        [UpdateDate] = GETDATE()
WHERE   [Id] = @p3;
SELECT  a.[Code] ,
        a.[UpdateDate]
FROM    [Products] AS a
WHERE   a.[Id] = @p3;";
    }
}