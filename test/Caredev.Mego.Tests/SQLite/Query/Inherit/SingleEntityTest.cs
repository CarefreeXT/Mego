namespace Caredev.Mego.Tests.Core.Query.Inherit
{
    public partial class SingleEntityTest
    {
        private const string QueryAllDataTestSql =
@"SELECT  a.[Id] ,
        a.[Code] ,
        a.[Name] ,
        b.[Category] ,
        b.[IsValid] ,
        b.[UpdateDate]
FROM    [ProductBases] AS a
        INNER JOIN [Products] AS b ON a.[Id] = b.[Id];";
        private const string OrderQueryDataTestSql =
@"SELECT  a.[Id] ,
        a.[Code] ,
        a.[Name] ,
        b.[Category] ,
        b.[IsValid] ,
        b.[UpdateDate]
FROM    [ProductBases] AS a
        INNER JOIN [Products] AS b ON a.[Id] = b.[Id]
ORDER BY b.[Category] ASC;";
        private const string QueryFilterContainsTestSql =
@"SELECT  a.[Id] ,
        a.[Code] ,
        a.[Name] ,
        b.[Category] ,
        b.[IsValid] ,
        b.[UpdateDate]
FROM    [ProductBases] AS a
        INNER JOIN [Products] AS b ON a.[Id] = b.[Id]
WHERE   a.[Id] IN ( @p0, @p1, @p2, @p3 );";
    }
}