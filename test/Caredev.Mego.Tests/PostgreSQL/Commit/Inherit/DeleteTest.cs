namespace Caredev.Mego.Tests.Core.Commit.Inherit
{
    public partial class DeleteTest : IInheritTest
    {
        private const string DeleteSingleTestSql =
@"DELETE  a
FROM    [dbo].[OrderDetails] AS a
WHERE   a.[Id] = @p0 AND a.[Discount] = @p1;
DELETE  b
FROM    [dbo].[OrderDetailBases] AS b
WHERE   b.[Id] = @p0;";

        private const string DeleteMultiForKeyTestSql =
@"DELETE  a
FROM    [dbo].[OrderDetails] AS a
WHERE   a.[Id] IN ( @p0, @p1, @p2 );
DELETE  b
FROM    [dbo].[OrderDetailBases] AS b
WHERE   b.[Id] IN ( @p0, @p1, @p2 );";

#if SQLSERVER2005
        private const string DeleteMultiForKeysTestSql =
@"DECLARE @t0 AS TABLE
    (
      [Id] INT NULL ,
      [Number] INT NULL
    );
INSERT  INTO @t0
        ( [Id] ,
          [Number]
        )
        SELECT  @p0 ,
                @p1
        UNION ALL
        SELECT  @p2 ,
                @p3;
DELETE  a
FROM    [dbo].[Warehouses] AS a
        INNER JOIN @t0 AS b ON a.[Id] = b.[Id]
                               AND a.[Number] = b.[Number];
DELETE  c
FROM    [dbo].[WarehouseBases] AS c
        INNER JOIN @t0 AS d ON c.[Id] = d.[Id]
                               AND c.[Number] = d.[Number];";
#else
        private const string DeleteMultiForKeysTestSql =
@"DECLARE @t0 AS TABLE
    (
      [Id] INT NULL ,
      [Number] INT NULL ,
      [Address] NVARCHAR(MAX) NULL
    );
INSERT  INTO @t0
        ( [Id], [Number], [Address] )
VALUES  ( @p0, @p0, @p1 ),
        ( @p0, @p2, @p3 );
DELETE  a
FROM    [dbo].[Warehouses] AS a
        INNER JOIN @t0 AS b ON a.[Id] = b.[Id]
                               AND a.[Number] = b.[Number]
                               AND a.[Address] = b.[Address];
DELETE  c
FROM    [dbo].[WarehouseBases] AS c
        INNER JOIN @t0 AS d ON c.[Id] = d.[Id]
                               AND c.[Number] = d.[Number];"; 
#endif

        private const string DeleteStatementForExpressionTestSql =
@"DECLARE @t0 AS TABLE
    (
      [Id] INT NULL ,
      [Number] INT NULL
    );

INSERT  INTO @t0
        ( [Id] ,
          [Number]
        )
        SELECT  a.[Id] ,
                a.[Number]
        FROM    [dbo].[WarehouseBases] AS a
                INNER JOIN [dbo].[Warehouses] AS b ON a.[Id] = b.[Id]
                                                      AND a.[Number] = b.[Number]
        WHERE   a.[Id] > @p0;
DELETE  c
FROM    [dbo].[Warehouses] AS c
        INNER JOIN @t0 AS d ON c.[Id] = d.[Id]
                                              AND c.[Number] = d.[Number];
DELETE  e
FROM    [dbo].[WarehouseBases] AS e
        INNER JOIN @t0 AS f ON e.[Id] = f.[Id]
                                              AND e.[Number] = f.[Number];";

        private const string DeleteStatementForQueryTestSql =
@"DECLARE @t0 AS TABLE
    (
      [Id] INT NULL ,
      [Number] INT NULL
    );

INSERT  INTO @t0
        ( [Id] ,
          [Number]
        )
        SELECT  a.[Id] ,
                a.[Number]
        FROM    [dbo].[WarehouseBases] AS a
                INNER JOIN [dbo].[Warehouses] AS b ON a.[Id] = b.[Id]
                                                      AND a.[Number] = b.[Number]
                CROSS JOIN [dbo].[CustomerBases] AS c
                INNER JOIN [dbo].[Customers] AS d ON c.[Id] = d.[Id]
        WHERE   a.[Id] > c.[Id]
                AND a.[Number] > @p0;
DELETE  e
FROM    [dbo].[Warehouses] AS e
        INNER JOIN @t0 AS f ON e.[Id] = f.[Id]
                                              AND e.[Number] = f.[Number];
DELETE  g
FROM    [dbo].[WarehouseBases] AS g
        INNER JOIN @t0 AS h ON g.[Id] = h.[Id]
                                              AND g.[Number] = h.[Number];";
    }
}