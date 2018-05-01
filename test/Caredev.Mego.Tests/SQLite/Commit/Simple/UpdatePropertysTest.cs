namespace Caredev.Mego.Tests.Core.Commit.Simple
{
    public partial class UpdatePropertysTest 
    {
        private const string UpdateSingleObjectTestSql =
@"UPDATE [Customers]
SET [Address1] = @p0 || @p1 || @p2,
    [Name] = @p3
WHERE [Id] = @p4;
SELECT
  a.[Address1]
FROM [customers] AS a
WHERE a.[Id] = @p4;";
        
        private const string UpdateMultiObjectTestSql =
@"CREATE TEMPORARY TABLE [t$1]([Id] INTEGER NULL);
INSERT INTO [t$1]([Id])
VALUES(@p0),(@p1);
UPDATE [Customers]
SET [Address1] = @p2 || @p3 || @p4, [Name] = @p5
WHERE [Id] = @p0;
UPDATE [Customers]
SET [Address1] = @p6 || @p3 || @p7, [Name] = @p8
WHERE [Id] = @p1;
SELECT
a.[Address1]
FROM [Customers] AS a
INNER JOIN [t$1] AS b ON a.[Id] = b.[Id];

"; 

        private const string UpdateGenerateSingleObjectTestSql =
@"UPDATE [products]
SET [Category] = @p0,
    [Code] = @p1 || @p2,
    [Name] = @p1,
    [UpdateDate] = datetime('now')
WHERE [Id] = @p3;
SELECT
  a.[Code],
  a.[UpdateDate]
FROM [products] AS a
WHERE a.[Id] = @p3;";
        
        private const string UpdateGenerateMultiObjectTestSql =
@"CREATE TEMPORARY TABLE [t$1]([Id] INTEGER NULL);
INSERT INTO [t$1]([Id])
VALUES(@p0),(@p1);
UPDATE [Products]
SET [Category] = @p0, [Code] = @p2 || @p3, [Name] = @p2, [UpdateDate] = datetime('now')
WHERE [Id] = @p0;
UPDATE [Products]
SET [Category] = @p0, [Code] = @p4 || @p5, [Name] = @p4, [UpdateDate] = datetime('now')
WHERE [Id] = @p1;
SELECT
a.[Code], a.[UpdateDate]
FROM [Products] AS a
INNER JOIN [t$1] AS b ON a.[Id] = b.[Id];"; 
    }
}