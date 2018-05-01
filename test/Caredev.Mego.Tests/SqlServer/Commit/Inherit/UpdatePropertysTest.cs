namespace Caredev.Mego.Tests.Core.Commit.Inherit
{
    public partial class UpdatePropertysTest 
    {
        private const string UpdateSingleObjectTestSql =
@"UPDATE  a
SET     [Name] = @p0
FROM    [dbo].[CustomerBases] AS a
WHERE   a.[Id] = @p1;
UPDATE  b
SET     [Address1] = @p2 + @p3 + @p4
FROM    [dbo].[Customers] AS b
WHERE   b.[Id] = @p1
        AND b.[Zip] = @p5;
SELECT  c.[Address1]
FROM    [dbo].[CustomerBases] AS d
        INNER JOIN [dbo].[Customers] AS c ON d.[Id] = c.[Id]
WHERE   d.[Id] = @p1;";

#if SQLSERVER2005
        private const string UpdateMultiObjectTestSql =
@"DECLARE @t0 AS TABLE
    (
      [Name] NVARCHAR(MAX) NULL ,
      [Id] INT NULL ,
      [Address2] NVARCHAR(MAX) NULL ,
      [Address1] NVARCHAR(MAX) NULL
    );
INSERT  INTO @t0
        ( [Name] ,
          [Id] ,
          [Address2] ,
          [Address1]
        )
        SELECT  @p0 ,
                @p1 ,
                @p2 ,
                @p3
        UNION ALL
        SELECT  @p4 ,
                @p5 ,
                @p6 ,
                @p7;
UPDATE  a
SET     [Name] = b.[Name]
FROM    [dbo].[CustomerBases] AS a
        INNER JOIN @t0 AS b ON a.[Id] = b.[Id];
UPDATE  c
SET     [Address1] = d.[Address2] + @p8 + d.[Address1]
FROM    [dbo].[Customers] AS c
        INNER JOIN @t0 AS d ON c.[Id] = d.[Id];
SELECT  e.[Address1]
FROM    [dbo].[CustomerBases] AS f
        INNER JOIN [dbo].[Customers] AS e ON f.[Id] = e.[Id]
        INNER JOIN @t0 AS g ON f.[Id] = g.[Id];";
#else
        private const string UpdateMultiObjectTestSql =
@"DECLARE @t0 AS TABLE
    (
      [Name] NVARCHAR(MAX) NULL ,
      [Id] INT NULL ,
      [Address2] NVARCHAR(MAX) NULL ,
      [Address1] NVARCHAR(MAX) NULL ,
      [Zip] NVARCHAR(MAX) NULL
    );
INSERT  INTO @t0
        ( [Name], [Id], [Address2], [Address1], [Zip] )
VALUES  ( @p0, @p1, @p2, @p3, @p4 ),
        ( @p5, @p6, @p7, @p8, @p9 );
UPDATE  a
SET     [Name] = b.[Name]
FROM    [dbo].[CustomerBases] AS a
        INNER JOIN @t0 AS b ON a.[Id] = b.[Id];
UPDATE  c
SET     [Address1] = d.[Address2] + @pA + d.[Address1]
FROM    [dbo].[Customers] AS c
        INNER JOIN @t0 AS d ON c.[Id] = d.[Id]
                               AND c.[Zip] = d.[Zip];
SELECT  e.[Address1]
FROM    [dbo].[CustomerBases] AS f
        INNER JOIN [dbo].[Customers] AS e ON f.[Id] = e.[Id]
        INNER JOIN @t0 AS g ON f.[Id] = g.[Id];"; 
#endif

        private const string UpdateGenerateSingleObjectTestSql =
@"UPDATE  a
SET     [Code] = @p0 + @p1 ,
        [Name] = @p0
FROM    [dbo].[ProductBases] AS a
WHERE   a.[Id] = @p2;
UPDATE  b
SET     [Category] = @p3 ,
        [UpdateDate] = GETDATE()
FROM    [dbo].[Products] AS b
WHERE   b.[Id] = @p2;
SELECT  c.[Code] ,
        d.[UpdateDate]
FROM    [dbo].[ProductBases] AS c
        INNER JOIN [dbo].[Products] AS d ON c.[Id] = d.[Id]
WHERE   c.[Id] = @p2;";

#if SQLSERVER2005
        private const string UpdateGenerateMultiObjectTestSql =
@"DECLARE @t0 AS TABLE
    (
      [Name] NVARCHAR(MAX) NULL ,
      [Code] NVARCHAR(MAX) NULL ,
      [Id] INT NULL ,
      [Category] INT NULL
    );
INSERT  INTO @t0
        ( [Name] ,
          [Code] ,
          [Id] ,
          [Category]
        )
        SELECT  @p0 ,
                @p1 ,
                @p2 ,
                @p2
        UNION ALL
        SELECT  @p3 ,
                @p4 ,
                @p5 ,
                @p2;
UPDATE  a
SET     [Code] = b.[Name] + b.[Code] ,
        [Name] = b.[Name]
FROM    [dbo].[ProductBases] AS a
        INNER JOIN @t0 AS b ON a.[Id] = b.[Id];
UPDATE  c
SET     [Category] = d.[Category] ,
        [UpdateDate] = GETDATE()
FROM    [dbo].[Products] AS c
        INNER JOIN @t0 AS d ON c.[Id] = d.[Id];
SELECT  e.[Code] ,
        f.[UpdateDate]
FROM    [dbo].[ProductBases] AS e
        INNER JOIN [dbo].[Products] AS f ON e.[Id] = f.[Id]
        INNER JOIN @t0 AS g ON e.[Id] = g.[Id];";
#else
        private const string UpdateGenerateMultiObjectTestSql =
@"DECLARE @t0 AS TABLE
    (
      [Name] NVARCHAR(MAX) NULL ,
      [Code] NVARCHAR(MAX) NULL ,
      [Id] INT NULL ,
      [Category] INT NULL
    );
INSERT  INTO @t0
        ( [Name], [Code], [Id], [Category] )
VALUES  ( @p0, @p1, @p2, @p2 ),
        ( @p3, @p4, @p5, @p2 );
UPDATE  a
SET     [Code] = b.[Name] + b.[Code] ,
        [Name] = b.[Name]
FROM    [dbo].[ProductBases] AS a
        INNER JOIN @t0 AS b ON a.[Id] = b.[Id];
UPDATE  c
SET     [Category] = d.[Category] ,
        [UpdateDate] = GETDATE()
FROM    [dbo].[Products] AS c
        INNER JOIN @t0 AS d ON c.[Id] = d.[Id];
SELECT  e.[Code] ,
        f.[UpdateDate]
FROM    [dbo].[ProductBases] AS e
        INNER JOIN [dbo].[Products] AS f ON e.[Id] = f.[Id]
        INNER JOIN @t0 AS g ON e.[Id] = g.[Id];"; 
#endif
    }
}