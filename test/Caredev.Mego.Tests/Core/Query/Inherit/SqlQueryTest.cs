namespace Caredev.Mego.Tests.Core.Query.Inherit
{
    using System.Linq;
    using Caredev.Mego.Tests.Models.Inherit;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    [TestClass, TestCategory(Constants.TestCategoryRootName + ".IQuery.Simple")]
    public partial class SqlQueryTest : IInheritTest
    {
        [TestMethod]
        public void SqlQueryValueTest()
        {
            using (var db = CreateContext())
            {
                var data = db.Database.SqlQuery<int>(SqlQueryValueTestSql);
                Assert.IsTrue(data.Any(a => a > 0));
            }
        }

        [TestMethod]
        public void SqlQueryCollectionTest()
        {
            using (var db = CreateContext())
            {
                var data = db.Database.SqlQuery<Product>(SqlQueryCollectionTestSql);
                Assert.IsTrue(data.Any(a => a.Id > 0));
            }
        }
        public OrderManageEntities CreateContext() => Constants.CreateInheritContext();
    }
}
