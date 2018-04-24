namespace Caredev.Mego.Tests.Core.Commit.Simple
{
    public partial class UpdateTest
    {
        private const string UpdateSingleObjectTestSql =
@"UPDATE [Customers]
SET [Address1] = @p0, [Address2] = @p1, [Code] = @p2, [Name] = @p3, [Zip] = @p4
WHERE [Id] = @p5;";

        private const string UpdateMultiObjectTestSql =
@"CREATE TEMPORARY TABLE [t$1]([Id] INTEGER NULL);
INSERT INTO [t$1]([Id])
VALUES(@p0),(@p1);
UPDATE [Customers]
SET [Address1] = @p2, [Address2] = @p3, [Code] = @p4, [Name] = @p5, [Zip] = @p6
WHERE [Id] = @p0;
UPDATE [Customers]
SET [Address1] = @p7, [Address2] = @p8, [Code] = @p9, [Name] = @pA, [Zip] = @pB
WHERE [Id] = @p1;";

        private const string UpdateGenerateSingleObjectTestSql =
@"UPDATE [Products]
SET [Category] = @p0,
    [Code] = @p1,
    [IsValid] = @p2,
    [Name] = @p3,
    [UpdateDate] = datetime('now')
WHERE [Id] = @p4;
SELECT
  a.[UpdateDate]
FROM [products] AS a
WHERE a.[Id] = @p4;";
        
        private const string UpdateGenerateMultiObjectTestSql =
@"CREATE TEMPORARY TABLE [t$1]([Id] INTEGER NULL);
INSERT INTO [t$1]([Id])
VALUES(@p0),(@p1);
UPDATE [Products]
SET [Category] = @p2, [Code] = @p3, [IsValid] = @p4, [Name] = @p5, [UpdateDate] = datetime('now')
WHERE [Id] = @p0;
UPDATE [Products]
SET [Category] = @p2, [Code] = @p6, [IsValid] = @p7, [Name] = @p8, [UpdateDate] = datetime('now')
WHERE [Id] = @p1;
SELECT
a.[UpdateDate]
FROM [Products] AS a
INNER JOIN [t$1] AS b ON a.[Id] = b.[Id];"; 

        private const string UpdateQueryTestSql =
@"UPDATE [customers] AS a
CROSS JOIN [products] AS b
SET a.[Name] = b.[Name],
    a.[Code] = b.[Name],
    a.[Address1] = b.[Code]
WHERE b.[Id] > @p0";
    }
}