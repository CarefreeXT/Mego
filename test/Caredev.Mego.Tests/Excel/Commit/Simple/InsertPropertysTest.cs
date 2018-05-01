namespace Caredev.Mego.Tests.Core.Commit.Simple
{
    public partial class InsertPropertysTest
    {

        private const string InsertSingleObjectTestSql =
@"INSERT  INTO [Customers]
        ( [Id], [Address1], [Name] )
VALUES  ( @p0, @p1 + @p2, @p3 + @p4 );
SELECT  a.[Address1] ,
        a.[Name]
FROM    [Customers] AS a
WHERE   a.[Id] = @p0;";
        
        private const string InsertMultiObjectTestSql =
@"INSERT  INTO [Customers]
        ( [Id], [Address1], [Name] )
VALUES  ( @p0, @p1 + @p2, @p3 + @p4 );
SELECT  a.[Address1] ,
        a.[Name]
FROM    [Customers] AS a
WHERE   a.[Id] = @p0;"; 

        private const string InsertIdentitySingleObjectTestSql =
@"INSERT  INTO [Products]
        ( [Category] ,
          [Code] ,
          [IsValid] ,
          [Name] ,
          [UpdateDate]
        )
VALUES  ( @p0 ,
          @p1 ,
          @p2 ,
          @p3 + @p4 ,
          GETDATE()
        );
SELECT  a.[Id] ,
        a.[Name] ,
        a.[UpdateDate]
FROM    [Products] AS a
WHERE   a.[Id] = @@IDENTITY;";
        
        private const string InsertIdentityMultiObjectTestSql =
@"INSERT  INTO [Products]
        ( [Category] ,
          [Code] ,
          [IsValid] ,
          [Name] ,
          [UpdateDate]
        )
VALUES  ( @p0 ,
          @p1 ,
          @p2 ,
          @p3 + @p4 ,
          GETDATE()
        );
SELECT  a.[Id] ,
        a.[Name] ,
        a.[UpdateDate]
FROM    [Products] AS a
WHERE   a.[Id] = @@IDENTITY;";

        private const string InsertExpressionSingleObjectTestSql =
@"INSERT  INTO [Orders]
        ( [Id] ,
          [CreateDate] ,
          [CustomerId] ,
          [ModifyDate] ,
          [State]
        )
VALUES  ( @p0 ,
          GETDATE() ,
          @p1 ,
          GETDATE() ,
          @p2
        );
SELECT  a.[CreateDate] ,
        a.[CustomerId] ,
        a.[ModifyDate]
FROM    [Orders] AS a
WHERE   a.[Id] = @p0;";
        
        private const string InsertExpressionMultiObjectTestSql =
@"INSERT  INTO [Orders]
        ( [Id] ,
          [CreateDate] ,
          [CustomerId] ,
          [ModifyDate] ,
          [State]
        )
VALUES  ( @p0 ,
          GETDATE() ,
          @p1 ,
          GETDATE() ,
          @p2
        );
SELECT  a.[CreateDate] ,
        a.[CustomerId] ,
        a.[ModifyDate]
FROM    [Orders] AS a
WHERE   a.[Id] = @p0;";
    }
}