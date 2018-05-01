namespace Caredev.Mego.Tests.Core.Commit.Simple
{
    public partial class UpdatePropertysTest 
    {
        private const string UpdateSingleObjectTestSql =
@"UPDATE  a
SET     [Address1] = @p0 + @p1 + @p2 ,
        [Name] = @p3
FROM    [dbo].[Customers] AS a
WHERE   a.[Id] = @p4
        AND a.[Zip] = @p5;
SELECT  a.[Address1]
FROM    [dbo].[Customers] AS a
WHERE   a.[Id] = @p4;";

#if SQLSERVER2005
        private const string UpdateMultiObjectTestSql =
@"DECLARE @t0 AS TABLE
    (
      [Address2] NVARCHAR(MAX) NULL ,
      [Address1] NVARCHAR(MAX) NULL ,
      [Name] NVARCHAR(MAX) NULL ,
      [Id] INT NULL
    );
INSERT  INTO @t0
        ( [Address2] ,
          [Address1] ,
          [Name] ,
          [Id]
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
SET     [Address1] = b.[Address2] + @p8 + b.[Address1] ,
        [Name] = b.[Name]
FROM    [dbo].[Customers] AS a
        INNER JOIN @t0 AS b ON a.[Id] = b.[Id];
SELECT  a.[Address1]
FROM    [dbo].[Customers] AS a
        INNER JOIN @t0 AS c ON a.[Id] = c.[Id];";
#else
        private const string UpdateMultiObjectTestSql =
@"DECLARE @t0 AS TABLE
    (
      [Address2] NVARCHAR(MAX) NULL ,
      [Address1] NVARCHAR(MAX) NULL ,
      [Name] NVARCHAR(MAX) NULL ,
      [Id] INT NULL ,
      [Zip] NVARCHAR(MAX) NULL
    );
INSERT  INTO @t0
        ( [Address2], [Address1], [Name], [Id], [Zip] )
VALUES  ( @p0, @p1, @p2, @p3, @p4 ),
        ( @p5, @p6, @p7, @p8, @p9 );
UPDATE  a
SET     [Address1] = b.[Address2] + @pA + b.[Address1] ,
        [Name] = b.[Name]
FROM    [dbo].[Customers] AS a
        INNER JOIN @t0 AS b ON a.[Id] = b.[Id]
                               AND a.[Zip] = b.[Zip];
SELECT  a.[Address1]
FROM    [dbo].[Customers] AS a
        INNER JOIN @t0 AS c ON a.[Id] = c.[Id];"; 
#endif

        private const string UpdateGenerateSingleObjectTestSql =
@"UPDATE  a
SET     [Category] = @p0 ,
        [Code] = @p1 + @p2 ,
        [Name] = @p1 ,
        [UpdateDate] = GETDATE()
FROM    [dbo].[Products] AS a
WHERE   a.[Id] = @p3;
SELECT  a.[Code] ,
        a.[UpdateDate]
FROM    [dbo].[Products] AS a
WHERE   a.[Id] = @p3;";

#if SQLSERVER2005
        private const string UpdateGenerateMultiObjectTestSql =
@"DECLARE @t0 AS TABLE
    (
      [Category] INT NULL ,
      [Name] NVARCHAR(MAX) NULL ,
      [Code] NVARCHAR(MAX) NULL ,
      [Id] INT NULL
    );
INSERT  INTO @t0
        ( [Category] ,
          [Name] ,
          [Code] ,
          [Id]
        )
        SELECT  @p0 ,
                @p1 ,
                @p2 ,
                @p0
        UNION ALL
        SELECT  @p0 ,
                @p3 ,
                @p4 ,
                @p5;
UPDATE  a
SET     [Category] = b.[Category] ,
        [Code] = b.[Name] + b.[Code] ,
        [Name] = b.[Name] ,
        [UpdateDate] = GETDATE()
FROM    [dbo].[Products] AS a
        INNER JOIN @t0 AS b ON a.[Id] = b.[Id];
SELECT  a.[Code] ,
        a.[UpdateDate]
FROM    [dbo].[Products] AS a
        INNER JOIN @t0 AS c ON a.[Id] = c.[Id];";
#else
        private const string UpdateGenerateMultiObjectTestSql =
@"DECLARE @t0 AS TABLE
    (
      [Category] INT NULL ,
      [Name] NVARCHAR(MAX) NULL ,
      [Code] NVARCHAR(MAX) NULL ,
      [Id] INT NULL
    );
INSERT  INTO @t0
        ( [Category], [Name], [Code], [Id] )
VALUES  ( @p0, @p1, @p2, @p0 ),
        ( @p0, @p3, @p4, @p5 );
UPDATE  a
SET     [Category] = b.[Category] ,
        [Code] = b.[Name] + b.[Code] ,
        [Name] = b.[Name] ,
        [UpdateDate] = GETDATE()
FROM    [dbo].[Products] AS a
        INNER JOIN @t0 AS b ON a.[Id] = b.[Id];
SELECT  a.[Code] ,
        a.[UpdateDate]
FROM    [dbo].[Products] AS a
        INNER JOIN @t0 AS c ON a.[Id] = c.[Id];"; 
#endif
    }
}