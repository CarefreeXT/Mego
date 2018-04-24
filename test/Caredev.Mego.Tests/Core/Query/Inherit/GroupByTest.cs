namespace Caredev.Mego.Tests.Core.Query.Inherit
{
    using System.Linq;
    using Caredev.Mego.Tests.Models.Inherit;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    [TestClass, TestCategory(Constants.TestCategoryRootName + ".IQuery.GroupBy")]
    public partial class GroupByTest : IInheritTest
    {
        [TestMethod]
        public void QuerySimpleKeyTest()
        {
            using (var db = CreateContext())
            {
                var query = from a in db.Products
                            group a by a.Category into g
                            select new { g.Key };
                Utility.CompareSql(db, query.Expression, QuerySimpleKeyTestSql);
                var data = query.ToArray();
                Assert.IsTrue(data.Any(a => a.Key > 0));
            }
        }

        [TestMethod]
        public void QueryComplexKeysTest()
        {
            using (var db = CreateContext())
            {
                var query = from a in db.Products
                            group a by new { a.Category, a.IsValid } into g
                            select g.Key;
                Utility.CompareSql(db, query.Expression, QueryComplexKeysTestSql);
                var data = query.ToArray();
                Assert.IsTrue(data.Any(a => a.Category > 0));
            }
        }

        [TestMethod]
        public void QueryKeyMembersTest()
        {
            using (var db = CreateContext())
            {
                var query = from a in db.Products
                            group a by new { a.Category, a.IsValid } into g
                            select new { g.Key, g.Key.IsValid };
                Utility.CompareSql(db, query.Expression, QueryKeyMembersTestSql);
                var data = query.ToArray();
                Assert.IsTrue(data.Any(a => a.Key.Category > 0));
            }
        }

        [TestMethod]
        public void QueryKeyAndAggregateCountTest()
        {
            using (var db = CreateContext())
            {
                var query = from a in db.Products
                            group a by a.Category into g
                            select new { g.Key, Count = g.Count() };
                Utility.CompareSql(db, query.Expression, QueryKeyAndAggregateCountTestSql);
                var data = query.ToArray();
                Assert.IsTrue(data.Any(a => a.Key > 0));
            }
        }

        [TestMethod]
        public void QueryKeyAndAggregateMaxTest()
        {
            using (var db = CreateContext())
            {
                var query = from a in db.Products
                            group a by a.Category into g
                            select new { g.Key, Max = g.Max(a => a.Id) };
                Utility.CompareSql(db, query.Expression, QueryKeyAndAggregateMaxTestSql);
                var data = query.ToArray();
                Assert.IsTrue(data.Any(a => a.Key > 0));
            }
        }
        [TestMethod]
        public void QueryKeyAndAggregateCountMaxTest()
        {
            using (var db = CreateContext())
            {
                var query = from a in db.Products
                            group a by a.Category into g
                            select new { g.Key, Count = g.Count(), Max = g.Max(a => a.Id) };
                Utility.CompareSql(db, query.Expression, QueryKeyAndAggregateCountMaxTestSql);
                var data = query.ToArray();
                Assert.IsTrue(data.Any(a => a.Key > 0));
            }
        }
        [TestMethod]
        public void QuerySimpleKeyAndListTest()
        {
            using (var db = CreateContext())
            {
                var query = from a in db.Products
                            group a by a.Category into g
                            select new { g.Key, List = g };
                Utility.CompareSql(db, query.Expression, QuerySimpleKeyAndListTestSql);
                var data = query.ToArray();
                Assert.IsTrue(data.Any(a => a.Key > 0));
            }
        }
        [TestMethod]
        public void QuerySimpleKeyAndListPageTest()
        {
            using (var db = CreateContext())
            {
                var query = (from a in db.Products
                             group a by a.Category into g
                             select new { g.Key, List = g }).Skip(2).Take(2);
                Utility.CompareSql(db, query.Expression, QuerySimpleKeyAndListPageTestSql);
                var data = query.ToArray();
                Assert.IsTrue(data.Any(a => a.Key > 0));
            }
        }

        public OrderManageEntities CreateContext() => Constants.CreateInheritContext();
    }
}
