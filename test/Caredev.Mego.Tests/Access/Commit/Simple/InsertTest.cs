namespace Caredev.Mego.Tests.Core.Commit.Simple
{
    public partial class InsertTest
    {
        private const string InsertSingleObjectTestSql =
@"INSERT  INTO [Customers]
        ( [Id], [Address1], [Address2], [Code], [Name], [Zip] )
VALUES  ( @p0, @p1, @p2, @p3, @p4, @p5 )";
        
        private const string InsertMultiObjectTestSql =
@"INSERT  INTO [Customers]
        ( [Id], [Address1], [Address2], [Code], [Name], [Zip] )
VALUES  ( @p0, @p1, @p2, @p3, @p4, @p5 )";

        private const string InsertIdentitySingleObjectTestSql =
@"INSERT  INTO [Products]
        ( [Category] ,[Code] ,[IsValid] ,[Name] ,[UpdateDate]        )
VALUES  ( @p0 ,@p1 ,@p2 ,@p3 ,GETDATE());
SELECT  a.[Id] ,
        a.[UpdateDate]
FROM    [Products] AS a
WHERE   a.[Id] = @@IDENTITY;";
        
        private const string InsertIdentityMultiObjectTestSql =
@"INSERT  INTO [Products]
        ( [Category] ,[Code] ,[IsValid] ,[Name] ,[UpdateDate]        )
VALUES  ( @p0 ,@p1 ,@p2 ,@p3 ,GETDATE());
SELECT  a.[Id] ,
        a.[UpdateDate]
FROM    [Products] AS a
WHERE   a.[Id] = @@IDENTITY;";

        private const string InsertExpressionSingleObjectTestSql =
@"INSERT  INTO [Orders]
( [Id] ,[CreateDate] ,[CustomerId] ,[ModifyDate] ,[State])
VALUES  ( @p0 ,GETDATE() ,@p1 ,@p2 ,@p3);
SELECT  a.[CreateDate]
FROM    [Orders] AS a
WHERE   a.[Id] = @p0;";
        
        private const string InsertExpressionMultiObjectTestSql =
@"INSERT  INTO [Orders]
( [Id] ,[CreateDate] ,[CustomerId] ,[ModifyDate] ,[State] )
VALUES  ( @p0 ,GETDATE() ,@p1 ,@p2 ,@p3);
SELECT  a.[CreateDate]
FROM    [Orders] AS a
WHERE   a.[Id] = @p0;"; 

        private const string InsertQueryTestSql =
@"INp0";
    }
}