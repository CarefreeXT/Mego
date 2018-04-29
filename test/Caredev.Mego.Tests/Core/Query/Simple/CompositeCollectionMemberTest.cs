namespace Caredev.Mego.Tests.Core.Query.Simple
{
    using System.Linq;
#if ORACLE || FIREBIRD
    using Caredev.Mego.Tests.Models.Simple2;
#else
    using Caredev.Mego.Tests.Models.Simple; 
#endif
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    [TestClass, TestCategory(Constants.TestCategoryRootName + ".Query.Collection2")]
    public partial class CompositeCollectionMemberTest
    {
        [TestMethod]
        public void SimpleJoinTest()
        {
            using (var db = CreateContext())
            {
                var query = from a in db.Orders
                            from b in a.Products
                            select new { a, b };
                Utility.CompareSql(db, query.Expression, SimpleJoinTestSql);
                var data = query.ToArray();
                Assert.IsTrue(data.Any());
            }
        }

        [TestMethod]
        public void SimpleJoinDefaultTest()
        {
            using (var db = CreateContext())
            {
                var query = from a in db.Orders
                            from b in a.Products.DefaultIfEmpty()
                            select new { a, b };
                Utility.CompareSql(db, query.Expression, SimpleJoinDefaultTestSql);
                var data = query.ToArray();
                Assert.IsTrue(data.Any());
            }
        }
        [TestMethod]
        public void QueryBodyAndAggregateCountTest()
        {
            using (var db = CreateContext())
            {
                var query = from a in db.Orders
                            select new { Order = a, Count = a.Products.Count() };
                Utility.CompareSql(db, query.Expression, QueryBodyAndAggregateCountTestSql);
                var data = query.ToArray();
                Assert.IsTrue(data.Any());
            }
        }

        [TestMethod]
        public void QueryBodyAndAggregateMaxTest()
        {
            using (var db = CreateContext())
            {
                var query = from a in db.Orders
                            select new { Order = a, MaxId = a.Products.Max(m => m.Id) };
                Utility.CompareSql(db, query.Expression, QueryBodyAndAggregateMaxTestSql);
                var data = query.ToArray();
                Assert.IsTrue(data.Any());
            }
        }

        [TestMethod]
        public void QueryBodyAndAggregateCountMaxTest()
        {
            using (var db = CreateContext())
            {
                var query = from a in db.Orders
                            select new { Order = a, Count = a.Products.Count(), Max = a.Products.Max(b => b.Id) };
                Utility.CompareSql(db, query.Expression, QueryBodyAndAggregateCountMaxSql);
                var data = query.ToArray();
                Assert.IsTrue(data.Any());
            }
        }

        [TestMethod]
        public void QueryBodyAndListTest()
        {
            using (var db = CreateContext())
            {
                var query = from a in db.Orders
                            select new { Order = a, List = a.Products };
                Utility.CompareSql(db, query.Expression, QueryBodyAndListTestSql);
                var data = query.ToArray();
                Assert.IsTrue(data.Any(a => a.List.Count > 0));
            }
        }

        [TestMethod]
        public void QueryBodyAndListPageTest()
        {
            using (var db = CreateContext())
            {
                var query = (from a in db.Orders
                             select new { Order = a, List = a.Products }).Skip(5).Take(5);
                Utility.CompareSql(db, query.Expression, QueryBodyAndListPageTestSql);
                var data = query.ToArray();
                Assert.IsTrue(data.Any(a => a.List.Count > 0));
            }
        }

        public OrderManageEntities CreateContext() => Constants.CreateSimpleContext();
    }
}
