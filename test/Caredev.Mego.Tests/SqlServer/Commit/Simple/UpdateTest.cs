namespace Caredev.Mego.Tests.Core.Commit.Simple
{
    public partial class UpdateTest
    {
        private const string UpdateSingleObjectTestSql =
@"UPDATE  a
SET     [Address1] = @p0 ,
        [Address2] = @p1 ,
        [Code] = @p2 ,
        [Name] = @p3 ,
        [Zip] = @p4
FROM    [dbo].[Customers] AS a
WHERE   a.[Id] = @p5 AND a.[Zip] = @p4;";

#if SQLSERVER2005
        private const string UpdateMultiObjectTestSql =
@"DECLARE @t0 AS TABLE
    (
      [Address1] NVARCHAR(MAX) NULL ,
      [Address2] NVARCHAR(MAX) NULL ,
      [Code] NVARCHAR(MAX) NULL ,
      [Name] NVARCHAR(MAX) NULL ,
      [Zip] NVARCHAR(MAX) NULL ,
      [Id] INT NULL
    );
INSERT  INTO @t0
        ( [Address1] ,
          [Address2] ,
          [Code] ,
          [Name] ,
          [Zip] ,
          [Id]
        )
        SELECT  @p0 ,
                @p1 ,
                @p2 ,
                @p3 ,
                @p4 ,
                @p5
        UNION ALL
        SELECT  @p6 ,
                @p1 ,
                @p7 ,
                @p8 ,
                @p4 ,
                @p9;
UPDATE  a
SET     [Address1] = b.[Address1] ,
        [Address2] = b.[Address2] ,
        [Code] = b.[Code] ,
        [Name] = b.[Name] ,
        [Zip] = b.[Zip]
FROM    [dbo].[Customers] AS a
        INNER JOIN @t0 AS b ON a.[Id] = b.[Id];";
#else
        private const string UpdateMultiObjectTestSql =
@"DECLARE @t0 AS TABLE
    (
      [Address1] NVARCHAR(MAX) NULL ,
      [Address2] NVARCHAR(MAX) NULL ,
      [Code] NVARCHAR(MAX) NULL ,
      [Name] NVARCHAR(MAX) NULL ,
      [Zip] NVARCHAR(MAX) NULL ,
      [Id] INT NULL
    );
INSERT  INTO @t0
        ( [Address1], [Address2], [Code], [Name], [Zip], [Id] )
VALUES  ( @p0, @p1, @p2, @p3, @p4, @p5 ),
        ( @p6, @p7, @p8, @p9, @pA, @pB );
UPDATE  a
SET     [Address1] = b.[Address1] ,
        [Address2] = b.[Address2] ,
        [Code] = b.[Code] ,
        [Name] = b.[Name] ,
        [Zip] = b.[Zip]
FROM    [dbo].[Customers] AS a
        INNER JOIN @t0 AS b ON a.[Id] = b.[Id]
                               AND a.[Zip] = b.[Zip];"; 
#endif

        private const string UpdateGenerateSingleObjectTestSql =
@"UPDATE  a
SET     [Category] = @p0 ,
        [Code] = @p1 ,
        [IsValid] = @p2 ,
        [Name] = @p3 ,
        [UpdateDate] = GETDATE()
FROM    [dbo].[Products] AS a
WHERE   a.[Id] = @p4;
SELECT  a.[UpdateDate]
FROM    [dbo].[Products] AS a
WHERE   a.[Id] = @p4;";

#if SQLSERVER2005
        private const string UpdateGenerateMultiObjectTestSql =
@"DECLARE @t0 AS TABLE
    (
      [Category] INT NULL ,
      [Code] NVARCHAR(MAX) NULL ,
      [IsValid] BIT NULL ,
      [Name] NVARCHAR(MAX) NULL ,
      [Id] INT NULL
    );
INSERT  INTO @t0
        ( [Category] ,
          [Code] ,
          [IsValid] ,
          [Name] ,
          [Id]
        )
        SELECT  @p0 ,
                @p1 ,
                @p2 ,
                @p3 ,
                @p4
        UNION ALL
        SELECT  @p0 ,
                @p5 ,
                @p6 ,
                @p7 ,
                @p8;
UPDATE  a
SET     [Category] = b.[Category] ,
        [Code] = b.[Code] ,
        [IsValid] = b.[IsValid] ,
        [Name] = b.[Name] ,
        [UpdateDate] = GETDATE()
FROM    [dbo].[Products] AS a
        INNER JOIN @t0 AS b ON a.[Id] = b.[Id];
SELECT  a.[UpdateDate]
FROM    [dbo].[Products] AS a
        INNER JOIN @t0 AS c ON a.[Id] = c.[Id];";
#else
        private const string UpdateGenerateMultiObjectTestSql =
@"DECLARE @t0 AS TABLE
    (
      [Category] INT NULL ,
      [Code] NVARCHAR(MAX) NULL ,
      [IsValid] BIT NULL ,
      [Name] NVARCHAR(MAX) NULL ,
      [Id] INT NULL
    );

INSERT  INTO @t0
        ( [Category], [Code], [IsValid], [Name], [Id] )
VALUES  ( @p0, @p1, @p2, @p3, @p4 ),
        ( @p0, @p5, @p6, @p7, @p8 );
UPDATE  a
SET     [Category] = b.[Category] ,
        [Code] = b.[Code] ,
        [IsValid] = b.[IsValid] ,
        [Name] = b.[Name] ,
        [UpdateDate] = GETDATE()
FROM    [dbo].[Products] AS a
        INNER JOIN @t0 AS b ON a.[Id] = b.[Id];
SELECT  a.[UpdateDate]
FROM    [dbo].[Products] AS a
        INNER JOIN @t0 AS c ON a.[Id] = c.[Id];"; 
#endif

        private const string UpdateQueryTestSql =
@"UPDATE  a
SET     [Name] = b.[Name] ,
        [Code] = b.[Name] ,
        [Address1] = b.[Code]
FROM    [dbo].[Customers] AS a
        INNER JOIN [dbo].[Products] AS b ON a.[Id] = b.[Id]
WHERE   b.[Id] > @p0;";
    }
}