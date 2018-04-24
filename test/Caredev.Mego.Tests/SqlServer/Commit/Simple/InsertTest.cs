namespace Caredev.Mego.Tests.Core.Commit.Simple
{
    public partial class InsertTest
    {
        private const string InsertSingleObjectTestSql =
@"INSERT  INTO [dbo].[Customers]
        ( [Id], [Address1], [Address2], [Code], [Name], [Zip] )
VALUES  ( @p0, @p1, @p2, @p3, @p4, @p5 );";

#if SQLSERVER2005
        private const string InsertMultiObjectTestSql =
@"INSERT  INTO [dbo].[Customers]
        ( [Id] ,
          [Address1] ,
          [Address2] ,
          [Code] ,
          [Name] ,
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
                @p1 ,
                @p2 ,
                @p3 ,
                @p7 ,
                @p5;";
#else
        private const string InsertMultiObjectTestSql =
@"INSERT  INTO [dbo].[Customers]
        ( [Id], [Address1], [Address2], [Code], [Name], [Zip] )
VALUES  ( @p0, @p1, @p2, @p3, @p4, @p5 ),
        ( @p6, @p1, @p2, @p3, @p7, @p5 );";
#endif

        private const string InsertIdentitySingleObjectTestSql =
@"INSERT  INTO [dbo].[Products]
        ( [Category] ,[Code] ,[IsValid] ,[Name] ,[UpdateDate]        )
VALUES  ( @p0 ,@p1 ,@p2 ,@p3 ,GETDATE());
SELECT  a.[Id] ,
        a.[UpdateDate]
FROM    [dbo].[Products] AS a
WHERE   a.[Id] = SCOPE_IDENTITY();";

#if SQLSERVER2005
        private const string InsertIdentityMultiObjectTestSql =
@"DECLARE @v0 AS INT;
DECLARE @t1 AS TABLE(
[Id] INT NULL, 
[Category] INT NULL, 
[Code] NVARCHAR(MAX) NULL, 
[IsValid] BIT NULL, 
[Name] NVARCHAR(MAX) NULL, 
[UpdateDate] DATETIME NULL, 
[RowIndex] INT NULL);
INSERT INTO @t1
([Category],[Code],[IsValid],[Name],[UpdateDate],[Id],[RowIndex])
SELECT @p0,@p1,@p2,@p3,GETDATE(),0,1 UNION ALL 
SELECT @p4,@p5,@p6,@p3,GETDATE(),0,2;
SET @v0 = 1;
WHILE (@v0 < 3)
BEGIN
INSERT INTO [dbo].[Products]
([Category],[Code],[IsValid],[Name],[UpdateDate])
SELECT
a.[Category], a.[Code], a.[IsValid], a.[Name], a.[UpdateDate]
FROM @t1 AS a
WHERE a.[RowIndex] = @v0;
UPDATE a
SET [Id] = SCOPE_IDENTITY()
FROM @t1 AS a
WHERE a.[RowIndex] = @v0;
SET @v0 = @v0 + 1;
END;
SELECT
b.[Id], b.[UpdateDate]
FROM [dbo].[Products] AS b
INNER JOIN @t1 AS c ON b.[Id] = c.[Id];";
#else
        private const string InsertIdentityMultiObjectTestSql =
@"DECLARE @v0 AS INT;
DECLARE @t1 AS TABLE
    (
      [Id] INT NULL ,
      [Category] INT NULL ,
      [Code] NVARCHAR(MAX) NULL ,
      [IsValid] BIT NULL ,
      [Name] NVARCHAR(MAX) NULL ,
      [UpdateDate] DATETIME2(7) NULL ,
      [RowIndex] INT NULL
    );

INSERT  INTO @t1
        ( [Category], [Code], [IsValid], [Name], [UpdateDate], [Id],
          [RowIndex] )
VALUES  ( @p0, @p1, @p2, @p3, GETDATE(), 0, 1 ),
        ( @p4, @p5, @p6, @p3, GETDATE(), 0, 2 );
SET @v0 = 1;
WHILE ( @v0 < 3 )
    BEGIN
        INSERT  INTO [dbo].[Products]
                ( [Category] ,
                  [Code] ,
                  [IsValid] ,
                  [Name] ,
                  [UpdateDate]
                )
                SELECT  a.[Category] ,
                        a.[Code] ,
                        a.[IsValid] ,
                        a.[Name] ,
                        a.[UpdateDate]
                FROM    @t1 AS a
                WHERE   a.[RowIndex] = @v0;
        UPDATE  a
        SET     [Id] = SCOPE_IDENTITY()
        FROM    @t1 AS a
        WHERE   a.[RowIndex] = @v0;
        SET @v0 = @v0 + 1;
    END;
SELECT  b.[Id] ,
        b.[UpdateDate]
FROM    [dbo].[Products] AS b
        INNER JOIN @t1 AS c ON b.[Id] = c.[Id];"; 
#endif

        private const string InsertExpressionSingleObjectTestSql =
@"INSERT  INTO [dbo].[Orders]
( [Id] ,[CreateDate] ,[CustomerId] ,[ModifyDate] ,[State])
VALUES  ( @p0 ,GETDATE() ,@p1 ,@p2 ,@p3);
SELECT  a.[CreateDate]
FROM    [dbo].[Orders] AS a
WHERE   a.[Id] = @p0;";

#if SQLSERVER2005
        private const string InsertExpressionMultiObjectTestSql =
@"DECLARE @t0 AS TABLE
    (
      [Id] INT NULL ,
      [CreateDate] DATETIME NULL ,
      [CustomerId] INT NULL ,
      [ModifyDate] DATETIME NULL ,
      [State] INT NULL
    );
INSERT  INTO @t0
        ( [Id] ,
          [CreateDate] ,
          [CustomerId] ,
          [ModifyDate] ,
          [State]
        )
        SELECT  @p0 ,
                GETDATE() ,
                @p1 ,
                @p2 ,
                @p3
        UNION ALL
        SELECT  @p4 ,
                GETDATE() ,
                @p1 ,
                @p2 ,
                @p5;
INSERT  INTO [dbo].[Orders]
        ( [Id] ,
          [CreateDate] ,
          [CustomerId] ,
          [ModifyDate] ,
          [State]
        )
        SELECT  a.[Id] ,
                a.[CreateDate] ,
                a.[CustomerId] ,
                a.[ModifyDate] ,
                a.[State]
        FROM    @t0 AS a;
SELECT  b.[CreateDate]
FROM    [dbo].[Orders] AS b
        INNER JOIN @t0 AS c ON b.[Id] = c.[Id];";
#else
        private const string InsertExpressionMultiObjectTestSql =
@"DECLARE @t0 AS TABLE
    (
      [Id] INT NULL ,
      [CreateDate] DATETIME2(7) NULL ,
      [CustomerId] INT NULL ,
      [ModifyDate] DATETIME2(7) NULL ,
      [State] INT NULL
    );
INSERT  INTO @t0
        ( [Id], [CreateDate], [CustomerId], [ModifyDate], [State] )
VALUES  ( @p0, GETDATE(), @p1, @p2, @p3 ),
        ( @p4, GETDATE(), @p1, @p2, @p5 );
INSERT  INTO [dbo].[Orders]
        ( [Id] ,
          [CreateDate] ,
          [CustomerId] ,
          [ModifyDate] ,
          [State]
        )
        SELECT  a.[Id] ,
                a.[CreateDate] ,
                a.[CustomerId] ,
                a.[ModifyDate] ,
                a.[State]
        FROM    @t0 AS a;
SELECT  b.[CreateDate]
FROM    [dbo].[Orders] AS b
        INNER JOIN @t0 AS c ON b.[Id] = c.[Id];"; 
#endif

        private const string InsertQueryTestSql =
@"INSERT  INTO [dbo].[Customers]
        ( [Id] ,
          [Name] ,
          [Code] ,
          [Address1]
        )
        SELECT  a.[Id] + @p0 AS [Id] ,
                a.[Name] ,
                a.[Name] ,
                a.[Code]
        FROM    [dbo].[Products] AS a
        WHERE   a.[Id] > @p1;";
    }
}