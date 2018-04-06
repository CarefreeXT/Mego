namespace Caredev.Mego.Tests.Core.Query.Simple
{
    using System.Linq;
    using Caredev.Mego.Tests.Models.Simple;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    [TestClass, TestCategory(Constants.TestCategoryRootName + ".Query.GroupJoin")]
    public partial class GroupJoinTest : ISimpleTest
    {

        [TestMethod]
        public void SimpleGroupJoinTest()
        {
            using (var db = CreateContext())
            {
                var query = from a in db.Products
                            join b in db.Customers on a.Id equals b.Id
                            select new { a, b };
                Utility.CompareSql(db, query.Expression, SimpleGroupJoinTestSql);
                var data = query.ToArray();
                Assert.IsTrue(data.Any());
            }
        }

        [TestMethod]
        public void GroupJoinDefaultTest()
        {
            using (var db = CreateContext())
            {
                var query = from a in db.Products
                            join b in db.Customers on a.Id equals b.Id into g
                            from b in g.DefaultIfEmpty()
                            select new { a, b };
                Utility.CompareSql(db, query.Expression, GroupJoinDefaultTestSql);
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
                            join b in db.OrderDetails on a.Id equals b.OrderId into g
                            select new { Order = a, Count = g.Count() };
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
                            join b in db.OrderDetails on a.Id equals b.OrderId into g
                            select new { Order = a, MaxId = g.Max(m => m.Id) };
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
                            join b in db.OrderDetails on a.Id equals b.OrderId into g
                            select new { Order = a, Count = g.Count(), MaxId = g.Max(m => m.Id) };
                Utility.CompareSql(db, query.Expression, QueryBodyAndAggregateCountMaxTestSql);
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
                            join b in db.OrderDetails on a.Id equals b.OrderId into g
                            select new { Order = a, List = g };
                Utility.CompareSql(db, query.Expression, QueryBodyAndListTestSql);
                var data = query.ToArray();
                Assert.IsTrue(data.Any());
            }
        }

        [TestMethod]
        public void QueryBodyAndListPageTest()
        {
            using (var db = CreateContext())
            {
                var query = (from a in db.Orders
                            join b in db.OrderDetails on a.Id equals b.OrderId into g
                            select new { Order = a, List = g }).Skip(5).Take(5);
                Utility.CompareSql(db, query.Expression, QueryBodyAndListPageTestSql);
                var data = query.ToArray();
                Assert.IsTrue(data.Any());
            }
        }

        public OrderManageEntities CreateContext()
        {
            return new OrderManageEntities(Constants.ConnectionNameSimple);
        }
    }
}
