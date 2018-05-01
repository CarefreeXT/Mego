namespace Caredev.Mego.Tests.Core
{
    using System;
    using System.Linq;
    using Caredev.Mego.DataAnnotations;
    using Caredev.Mego.Tests.Models.Simple;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    [TestClass, TestCategory(Constants.TestCategoryRootName + ".Database")]
    public partial class FunctionTest
    {
        [TestMethod]
        public void CustomFunctionTest()
        {
            using (var db = CreateSimpleContext())
            {
                var query = from a in db.Customers
                            select new
                            {
                                a.Id, 
                                a.Name,
                                AbsId = CustomAbs(a.Id)
                            };
                Utility.CompareSql(db, query.Expression, CustomFunctionTestSql);
                var data = query.ToArray();
                Assert.IsTrue(data.Any(a => a.Id > 0));
            }
        }

        [DbFunction("ABS", IsSystemFunction = true)]
        public int CustomAbs(int value)
        {
            throw new NotImplementedException();
        }

        internal OrderManageEntities CreateSimpleContext() => Constants.CreateSimpleContext();

    }
}
