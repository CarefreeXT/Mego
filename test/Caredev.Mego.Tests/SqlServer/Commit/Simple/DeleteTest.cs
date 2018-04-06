namespace Caredev.Mego.Tests.Core.Commit.Simple
{
    public partial class DeleteTest
    {
        private const string DeleteSingleTestSql =
@"DELETE  a
FROM    [dbo].[OrderDetails] AS a
WHERE   a.[Id] = @p0
        AND a.[Discount] = @p1;";

        private const string DeleteMultiForKeyTestSql =
@"DELETE  a
FROM    [dbo].[OrderDetails] AS a
WHERE   a.[Id] IN ( @p0, @p1, @p2 );";

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
                               AND a.[Number] = b.[Number];";
#else
        private const string DeleteMultiForKeysTestSql =
@"DECLARE @t0 AS TABLE
    (
      [Id] INT NULL ,
      [Number] INT NULL,
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
                               AND a.[Address] = b.[Address];"; 
#endif

        private const string DeleteStatementForExpressionTestSql =
@"DELETE  a
FROM    [dbo].[Warehouses] AS a
WHERE   a.[Id] > @p0;";

        private const string DeleteStatementForQueryTestSql =
@"DELETE  a
FROM    [dbo].[Warehouses] AS a
        CROSS JOIN [dbo].[Customers] AS b
WHERE   a.[Id] > b.[Id]
        AND a.[Number] > @p0;";
    }
}