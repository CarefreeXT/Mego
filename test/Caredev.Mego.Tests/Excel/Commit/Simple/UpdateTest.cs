namespace Caredev.Mego.Tests.Core.Commit.Simple
{
    public partial class UpdateTest
    {
        private const string UpdateSingleObjectTestSql =
@"UPDATE  [Customers]
SET     [Address1] = @p0 ,
        [Address2] = @p1 ,
        [Code] = @p2 ,
        [Name] = @p3 ,
        [Zip] = @p4
WHERE   [Id] = @p5";
        
        private const string UpdateMultiObjectTestSql =
@"UPDATE  [Customers]
SET     [Address1] = @p0 ,
        [Address2] = @p1 ,
        [Code] = @p2 ,
        [Name] = @p3 ,
        [Zip] = @p4
WHERE   [Id] = @p5";

        private const string UpdateGenerateSingleObjectTestSql =
@"UPDATE [Products]
SET     [Category] = @p0 ,
        [Code] = @p1 ,
        [IsValid] = @p2 ,
        [Name] = @p3 ,
        [UpdateDate] = NOW()
WHERE   [Id] = @p4;
SELECT  a.[UpdateDate]
FROM    [Products] AS a
WHERE   a.[Id] = @p4;";
        
        private const string UpdateGenerateMultiObjectTestSql =
@"UPDATE [Products]
SET     [Category] = @p0 ,
        [Code] = @p1 ,
        [IsValid] = @p2 ,
        [Name] = @p3 ,
        [UpdateDate] = NOW()
WHERE   [Id] = @p4;
SELECT  a.[UpdateDate]
FROM    [Products] AS a
WHERE   a.[Id] = @p4;";

        private const string UpdateQueryTestSql =
@"UPDATE  
SET     [Name] = b.[Name] ,
        [Code] = b.[Name] ,
        [Address1] = b.[Code]
FROM    [Customers] AS a
        INNER JOIN [Products] AS b ON a.[Id] = b.[Id]
WHERE   b.[Id] > @p0;";
    }
}