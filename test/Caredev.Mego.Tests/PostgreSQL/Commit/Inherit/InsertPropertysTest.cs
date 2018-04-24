namespace Caredev.Mego.Tests.Core.Commit.Inherit
{
    public partial class InsertPropertysTest : IInheritTest
    {
        private const string InsertSingleObjectTestSql =
@"INSERT  INTO [dbo].[CustomerBases]
        ( [Id], [Name] )
VALUES  ( @p0, @p1 + @p2 );
INSERT  INTO [dbo].[Customers]
        ( [Id], [Address1] )
VALUES  ( @p0, @p2 + @p3 );
SELECT  a.[Name] ,
        b.[Address1]
FROM    [dbo].[CustomerBases] AS a
        INNER JOIN [dbo].[Customers] AS b ON a.[Id] = b.[Id]
WHERE   a.[Id] = @p0;";

#if SQLSERVER2005
        private const string InsertMultiObjectTestSql =
@"DECLARE @t0 AS TABLE
    (
      [Id] INT NULL ,
      [Code] NVARCHAR(MAX) NULL ,
      [Name] NVARCHAR(MAX) NULL ,
      [Address1] NVARCHAR(MAX) NULL ,
      [Address2] NVARCHAR(MAX) NULL ,
      [Zip] NVARCHAR(MAX) NULL
    );
INSERT  INTO @t0
        ( [Id] ,
          [Name] ,
          [Address1]
        )
        SELECT  @p0 ,
                @p1 + @p2 ,
                @p2 + @p3
        UNION ALL
        SELECT  @p4 ,
                @p5 + @p2 ,
                @p2 + @p3;
INSERT  INTO [dbo].[CustomerBases]
        ( [Id] ,
          [Name]
        )
        SELECT  a.[Id] ,
                a.[Name]
        FROM    @t0 AS a;
INSERT  INTO [dbo].[Customers]
        ( [Id] ,
          [Address1]
        )
        SELECT  a.[Id] ,
                a.[Address1]
        FROM    @t0 AS a;
SELECT  b.[Name] ,
        c.[Address1]
FROM    [dbo].[CustomerBases] AS b
        INNER JOIN [dbo].[Customers] AS c ON b.[Id] = c.[Id]
        INNER JOIN @t0 AS d ON b.[Id] = d.[Id];";
#else
        private const string InsertMultiObjectTestSql =
@"DECLARE @t0 AS TABLE
    (
      [Id] INT NULL ,
      [Code] NVARCHAR(MAX) NULL ,
      [Name] NVARCHAR(MAX) NULL ,
      [Address1] NVARCHAR(MAX) NULL ,
      [Address2] NVARCHAR(MAX) NULL ,
      [Zip] NVARCHAR(MAX) NULL
    );
INSERT  INTO @t0
        ( [Id], [Name], [Address1] )
VALUES  ( @p0, @p1 + @p2, @p2 + @p3 ),
        ( @p4, @p5 + @p2, @p2 + @p3 );
INSERT  INTO [dbo].[CustomerBases]
        ( [Id] ,
          [Name]
        )
        SELECT  a.[Id] ,
                a.[Name]
        FROM    @t0 AS a;
INSERT  INTO [dbo].[Customers]
        ( [Id] ,
          [Address1]
        )
        SELECT  a.[Id] ,
                a.[Address1]
        FROM    @t0 AS a;
SELECT  b.[Name] ,
        c.[Address1]
FROM    [dbo].[CustomerBases] AS b
        INNER JOIN [dbo].[Customers] AS c ON b.[Id] = c.[Id]
        INNER JOIN @t0 AS d ON b.[Id] = d.[Id];"; 
#endif

        private const string InsertIdentitySingleObjectTestSql =
@"DECLARE @v0 AS INT;
INSERT  INTO [dbo].[ProductBases]
        ( [Code], [Name] )
VALUES  ( @p0, @p1 + @p2 );
SET @v0 = SCOPE_IDENTITY();
INSERT  INTO [dbo].[Products]
        ( [Id] ,
          [Category] ,
          [IsValid] ,
          [UpdateDate]
        )
VALUES  ( @v0 ,
          @p3 ,
          @p4 ,
          GETDATE()
        );
SELECT  a.[Id] ,
        a.[Name] ,
        b.[UpdateDate]
FROM    [dbo].[ProductBases] AS a
        INNER JOIN [dbo].[Products] AS b ON a.[Id] = b.[Id]
WHERE   a.[Id] = @v0;";

#if SQLSERVER2005
        private const string InsertIdentityMultiObjectTestSql =
@"DECLARE @v0 AS INT;
DECLARE @t1 AS TABLE
    (
      [Id] INT NULL ,
      [Code] NVARCHAR(MAX) NULL ,
      [Name] NVARCHAR(MAX) NULL ,
      [Category] INT NULL ,
      [IsValid] BIT NULL ,
      [UpdateDate] DATETIME NULL ,
      [RowIndex] INT NULL
    );
INSERT  INTO @t1
        ( [Code] ,
          [Name] ,
          [Category] ,
          [IsValid] ,
          [UpdateDate] ,
          [Id] ,
          [RowIndex]
        )
        SELECT  @p0 ,
                @p1 + @p2 ,
                @p3 ,
                @p4 ,
                GETDATE() ,
                0 ,
                1
        UNION ALL
        SELECT  @p5 ,
                @p1 + @p2 ,
                @p6 ,
                @p7 ,
                GETDATE() ,
                0 ,
                2;
SET @v0 = 1;
WHILE ( @v0 < 3 )
    BEGIN
        INSERT  INTO [dbo].[ProductBases]
                ( [Code] ,
                  [Name]
                )
                SELECT  a.[Code] ,
                        a.[Name]
                FROM    @t1 AS a
                WHERE   a.[RowIndex] = @v0;
        UPDATE  a
        SET     [Id] = SCOPE_IDENTITY()
        FROM    @t1 AS a
        WHERE   a.[RowIndex] = @v0;
        SET @v0 = @v0 + 1;
    END;
INSERT  INTO [dbo].[Products]
        ( [Id] ,
          [Category] ,
          [IsValid] ,
          [UpdateDate]
        )
        SELECT  a.[Id] ,
                a.[Category] ,
                a.[IsValid] ,
                a.[UpdateDate]
        FROM    @t1 AS a;
SELECT  b.[Id] ,
        b.[Name] ,
        c.[UpdateDate]
FROM    [dbo].[ProductBases] AS b
        INNER JOIN [dbo].[Products] AS c ON b.[Id] = c.[Id]
        INNER JOIN @t1 AS d ON b.[Id] = d.[Id];";
#else
        private const string InsertIdentityMultiObjectTestSql =
@"DECLARE @v0 AS INT;
DECLARE @t1 AS TABLE
    (
      [Id] INT NULL ,
      [Code] NVARCHAR(MAX) NULL ,
      [Name] NVARCHAR(MAX) NULL ,
      [Category] INT NULL ,
      [IsValid] BIT NULL ,
      [UpdateDate] DATETIME2(7) NULL ,
      [RowIndex] INT NULL
    );
INSERT  INTO @t1
        ( [Code], [Name], [Category], [IsValid], [UpdateDate], [Id],
          [RowIndex] )
VALUES  ( @p0, @p1 + @p2, @p3, @p4, GETDATE(), 0, 1 ),
        ( @p5, @p1 + @p2, @p6, @p7, GETDATE(), 0, 2 );
SET @v0 = 1;
WHILE ( @v0 < 3 )
    BEGIN
        INSERT  INTO [dbo].[ProductBases]
                ( [Code] ,
                  [Name]
                )
                SELECT  a.[Code] ,
                        a.[Name]
                FROM    @t1 AS a
                WHERE   a.[RowIndex] = @v0;
        UPDATE  a
        SET     [Id] = SCOPE_IDENTITY()
        FROM    @t1 AS a
        WHERE   a.[RowIndex] = @v0;
        SET @v0 = @v0 + 1;
    END;
INSERT  INTO [dbo].[Products]
        ( [Id] ,
          [Category] ,
          [IsValid] ,
          [UpdateDate]
        )
        SELECT  a.[Id] ,
                a.[Category] ,
                a.[IsValid] ,
                a.[UpdateDate]
        FROM    @t1 AS a;
SELECT  b.[Id] ,
        b.[Name] ,
        c.[UpdateDate]
FROM    [dbo].[ProductBases] AS b
        INNER JOIN [dbo].[Products] AS c ON b.[Id] = c.[Id]
        INNER JOIN @t1 AS d ON b.[Id] = d.[Id];"; 
#endif

        private const string InsertExpressionSingleObjectTestSql =
@"INSERT  INTO [dbo].[OrderBases]
        ( [Id], [CreateDate], [ModifyDate] )
VALUES  ( @p0, GETDATE(), GETDATE() );
INSERT  INTO [dbo].[Orders]
        ( [Id], [CustomerId], [State] )
VALUES  ( @p0, @p1, @p2 );
SELECT  a.[CreateDate] ,
        a.[ModifyDate] ,
        b.[CustomerId]
FROM    [dbo].[OrderBases] AS a
        INNER JOIN [dbo].[Orders] AS b ON a.[Id] = b.[Id]
WHERE   a.[Id] = @p0;";

#if SQLSERVER2005
        private const string InsertExpressionMultiObjectTestSql =
@"DECLARE @t0 AS TABLE
    (
      [Id] INT NULL ,
      [CreateDate] DATETIME NULL ,
      [ModifyDate] DATETIME NULL ,
      [CustomerId] INT NULL ,
      [State] INT NULL
    );
INSERT  INTO @t0
        ( [Id] ,
          [CreateDate] ,
          [ModifyDate] ,
          [CustomerId] ,
          [State]
        )
        SELECT  @p0 ,
                GETDATE() ,
                GETDATE() ,
                @p1 ,
                @p2
        UNION ALL
        SELECT  @p3 ,
                GETDATE() ,
                GETDATE() ,
                @p1 ,
                @p4;
INSERT  INTO [dbo].[OrderBases]
        ( [Id] ,
          [CreateDate] ,
          [ModifyDate]
        )
        SELECT  a.[Id] ,
                a.[CreateDate] ,
                a.[ModifyDate]
        FROM    @t0 AS a;
INSERT  INTO [dbo].[Orders]
        ( [Id] ,
          [CustomerId] ,
          [State]
        )
        SELECT  a.[Id] ,
                a.[CustomerId] ,
                a.[State]
        FROM    @t0 AS a;
SELECT  b.[CreateDate] ,
        b.[ModifyDate] ,
        c.[CustomerId]
FROM    [dbo].[OrderBases] AS b
        INNER JOIN [dbo].[Orders] AS c ON b.[Id] = c.[Id]
        INNER JOIN @t0 AS d ON b.[Id] = d.[Id];";
#else
        private const string InsertExpressionMultiObjectTestSql =
@"DECLARE @t0 AS TABLE
    (
      [Id] INT NULL ,
      [CreateDate] DATETIME2(7) NULL ,
      [ModifyDate] DATETIME2(7) NULL ,
      [CustomerId] INT NULL ,
      [State] INT NULL
    );
INSERT  INTO @t0
        ( [Id], [CreateDate], [ModifyDate], [CustomerId], [State] )
VALUES  ( @p0, GETDATE(), GETDATE(), @p1, @p2 ),
        ( @p3, GETDATE(), GETDATE(), @p1, @p4 );
INSERT  INTO [dbo].[OrderBases]
        ( [Id] ,
          [CreateDate] ,
          [ModifyDate]
        )
        SELECT  a.[Id] ,
                a.[CreateDate] ,
                a.[ModifyDate]
        FROM    @t0 AS a;
INSERT  INTO [dbo].[Orders]
        ( [Id] ,
          [CustomerId] ,
          [State]
        )
        SELECT  a.[Id] ,
                a.[CustomerId] ,
                a.[State]
        FROM    @t0 AS a;
SELECT  b.[CreateDate] ,
        b.[ModifyDate] ,
        c.[CustomerId]
FROM    [dbo].[OrderBases] AS b
        INNER JOIN [dbo].[Orders] AS c ON b.[Id] = c.[Id]
        INNER JOIN @t0 AS d ON b.[Id] = d.[Id];"; 
#endif


    }
}