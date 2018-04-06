namespace Caredev.Mego.Tests.Core.Commit.Inherit
{
    public partial class UpdateTest : IInheritTest
    {
        private const string UpdateSingleObjectTestSql =
@"UPDATE  a
SET     [Code] = @p0 ,
        [Name] = @p1
FROM    [dbo].[CustomerBases] AS a
WHERE   a.[Id] = @p2;
UPDATE  b
SET     [Address1] = @p3 ,
        [Address2] = @p4 ,
        [Zip] = @p5
FROM    [dbo].[Customers] AS b
WHERE   b.[Id] = @p2
        AND b.[Zip] = @p5;";

#if SQLSERVER2005
        private const string UpdateMultiObjectTestSql =
@"DECLARE @t0 AS TABLE
    (
      [Code] NVARCHAR(MAX) NULL ,
      [Name] NVARCHAR(MAX) NULL ,
      [Id] INT NULL ,
      [Address1] NVARCHAR(MAX) NULL ,
      [Address2] NVARCHAR(MAX) NULL ,
      [Zip] NVARCHAR(MAX) NULL
    );
INSERT  INTO @t0
        ( [Code] ,
          [Name] ,
          [Id] ,
          [Address1] ,
          [Address2] ,
          [Zip]
        )
        SELECT  @p0 ,
                @p1 ,
                @p2 ,
                @p3 ,
                @p4 ,
                @p5
        UNION ALL
        SELECT  @p6 ,
                @p7 ,
                @p8 ,
                @p9 ,
                @p4 ,
                @p5;
UPDATE  a
SET     [Code] = b.[Code] ,
        [Name] = b.[Name]
FROM    [dbo].[CustomerBases] AS a
        INNER JOIN @t0 AS b ON a.[Id] = b.[Id];
UPDATE  c
SET     [Address1] = d.[Address1] ,
        [Address2] = d.[Address2] ,
        [Zip] = d.[Zip]
FROM    [dbo].[Customers] AS c
        INNER JOIN @t0 AS d ON c.[Id] = d.[Id];";
#else
        private const string UpdateMultiObjectTestSql =
@"DECLARE @t0 AS TABLE
    (
      [Code] NVARCHAR(MAX) NULL ,
      [Name] NVARCHAR(MAX) NULL ,
      [Id] INT NULL ,
      [Address1] NVARCHAR(MAX) NULL ,
      [Address2] NVARCHAR(MAX) NULL ,
      [Zip] NVARCHAR(MAX) NULL
    );
INSERT  INTO @t0
        ( [Code], [Name], [Id], [Address1], [Address2], [Zip] )
VALUES  ( @p0, @p1, @p2, @p3, @p4, @p5 ),
        ( @p6, @p7, @p8, @p9, @pA, @pB );
UPDATE  a
SET     [Code] = b.[Code] ,
        [Name] = b.[Name]
FROM    [dbo].[CustomerBases] AS a
        INNER JOIN @t0 AS b ON a.[Id] = b.[Id];
UPDATE  c
SET     [Address1] = d.[Address1] ,
        [Address2] = d.[Address2] ,
        [Zip] = d.[Zip]
FROM    [dbo].[Customers] AS c
        INNER JOIN @t0 AS d ON c.[Id] = d.[Id]
                               AND c.[Zip] = d.[Zip];"; 
#endif

        private const string UpdateGenerateSingleObjectTestSql =
@"UPDATE  a
SET     [Code] = @p0 ,
        [Name] = @p1
FROM    [dbo].[ProductBases] AS a
WHERE   a.[Id] = @p2;
UPDATE  b
SET     [Category] = @p3 ,
        [IsValid] = @p4 ,
        [UpdateDate] = GETDATE()
FROM    [dbo].[Products] AS b
WHERE   b.[Id] = @p2;
SELECT  c.[UpdateDate]
FROM    [dbo].[ProductBases] AS d
        INNER JOIN [dbo].[Products] AS c ON d.[Id] = c.[Id]
WHERE   d.[Id] = @p2;";

#if SQLSERVER2005
        private const string UpdateGenerateMultiObjectTestSql =
@"DECLARE @t0 AS TABLE
    (
      [Code] NVARCHAR(MAX) NULL ,
      [Name] NVARCHAR(MAX) NULL ,
      [Id] INT NULL ,
      [Category] INT NULL ,
      [IsValid] BIT NULL
    );
INSERT  INTO @t0
        ( [Code] ,
          [Name] ,
          [Id] ,
          [Category] ,
          [IsValid]
        )
        SELECT  @p0 ,
                @p1 ,
                @p2 ,
                @p3 ,
                @p4
        UNION ALL
        SELECT  @p5 ,
                @p6 ,
                @p7 ,
                @p3 ,
                @p8;
UPDATE  a
SET     [Code] = b.[Code] ,
        [Name] = b.[Name]
FROM    [dbo].[ProductBases] AS a
        INNER JOIN @t0 AS b ON a.[Id] = b.[Id];
UPDATE  c
SET     [Category] = d.[Category] ,
        [IsValid] = d.[IsValid] ,
        [UpdateDate] = GETDATE()
FROM    [dbo].[Products] AS c
        INNER JOIN @t0 AS d ON c.[Id] = d.[Id];
SELECT  e.[UpdateDate]
FROM    [dbo].[ProductBases] AS f
        INNER JOIN [dbo].[Products] AS e ON f.[Id] = e.[Id]
        INNER JOIN @t0 AS g ON f.[Id] = g.[Id];";
#else
        private const string UpdateGenerateMultiObjectTestSql =
@"DECLARE @t0 AS TABLE
    (
      [Code] NVARCHAR(MAX) NULL ,
      [Name] NVARCHAR(MAX) NULL ,
      [Id] INT NULL ,
      [Category] INT NULL ,
      [IsValid] BIT NULL
    );
INSERT  INTO @t0
        ( [Code], [Name], [Id], [Category], [IsValid] )
VALUES  ( @p0, @p1, @p2, @p3, @p4 ),
        ( @p5, @p6, @p7, @p3, @p8 );
UPDATE  a
SET     [Code] = b.[Code] ,
        [Name] = b.[Name]
FROM    [dbo].[ProductBases] AS a
        INNER JOIN @t0 AS b ON a.[Id] = b.[Id];
UPDATE  c
SET     [Category] = d.[Category] ,
        [IsValid] = d.[IsValid] ,
        [UpdateDate] = GETDATE()
FROM    [dbo].[Products] AS c
        INNER JOIN @t0 AS d ON c.[Id] = d.[Id];
SELECT  e.[UpdateDate]
FROM    [dbo].[ProductBases] AS f
        INNER JOIN [dbo].[Products] AS e ON f.[Id] = e.[Id]
        INNER JOIN @t0 AS g ON f.[Id] = g.[Id];"; 
#endif

        private const string UpdateQueryTestSql =
@"UPDATE  a
SET     [Name] = b.[Name] ,
        [Code] = b.[Name]
FROM    [dbo].[CustomerBases] AS a
        CROSS JOIN [dbo].[ProductBases] AS b
        INNER JOIN [dbo].[Products] AS c ON b.[Id] = c.[Id]
WHERE   b.[Id] > @p0;
UPDATE  d
SET     [Address1] = b.[Code]
FROM    [dbo].[Customers] AS d
        CROSS JOIN [dbo].[ProductBases] AS b
        INNER JOIN [dbo].[Products] AS c ON b.[Id] = c.[Id]
WHERE   b.[Id] > @p0;";
    }
}