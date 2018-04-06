namespace Caredev.Mego.Tests.Core
{
    public partial class FunctionTest
    {
        private const string CustomFunctionTestSql =
@"SELECT
  a.`Id`,
  a.`Name`,
  ABS(a.`Id`) AS `AbsId`
FROM `customers` AS a;";
    }
}