namespace Caredev.Mego.Tests.Core.Query.Simple
{
    using System.Linq;
    using Caredev.Mego.Tests.Models.Simple; 
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    [TestClass, TestCategory(Constants.TestCategoryRootName + ".Query.Simple")]
    public partial class EntityLinqTest 
    {
        [TestMethod]
        public void QueryListTest()
        {
            using (var db = CreateContext())
            {
                var query = from a in db.Products
                            select a;
                Utility.CompareSql(db, query.Expression, QueryListTestSql);
                var data = query.ToArray();
                Assert.IsTrue(data.Any(a => a.Id > 0));
            }
        }

        [TestMethod]
        public void QuerySinglePropertyTest()
        {
            using (var db = CreateContext())
            {
                var query = from a in db.Products
                            select a.Code;
                Utility.CompareSql(db, query.Expression, QuerySinglePropertyTestSql);
                var data = query.ToArray();
                Assert.IsTrue(data.Any());
            }
        }

        [TestMethod]
        public void QueryMultiPropertyTest()
        {
            using (var db = CreateContext())
            {
                var query = from a in db.Products
                            select new { a.Id, a.Name };
                Utility.CompareSql(db, query.Expression, QueryMultiPropertyTestSql);
                var data = query.ToArray();
                Assert.IsTrue(data.Any());
            }
        }

        [TestMethod]
        public void QueryMultiPropertyDistinctTest()
        {
            using (var db = CreateContext())
            {
                var query = (from a in db.Orders
                             select new { a.State, a.CustomerId }).Distinct();
                Utility.CompareSql(db, query.Expression, QueryMultiPropertyDistinctTestSql);
                var data = query.ToArray();
                Assert.IsTrue(data.Any());
            }
        }

        [TestMethod]
        public void QueryPropertyAndExpressionTest()
        {
            using (var db = CreateContext())
            {
                var query = from a in db.Products
                            select new { a.Id, a.Name, Date = DateTime.Now };
                Utility.CompareSql(db, query.Expression, QueryPropertyAndExpressionTestSql);
                var data = query.ToArray();
                Assert.IsTrue(data.Any());
            }
        }

        [TestMethod]
        public void QueryFilterListForConstantTest()
        {
            using (var db = CreateContext())
            {
                var query = from a in db.Products
                            where a.Code == "Pro1"
                            select a;
                Utility.CompareSql(db, query.Expression, QueryFilterListForConstantTestSql);
                var data = query.ToArray();
                Assert.IsTrue(data.Length == 1);
            }
        }

        [TestMethod]
        public void QueryFilterListForVariableTest()
        {
            using (var db = CreateContext())
            {
                var code = "Pro1";
                var query = from a in db.Products
                            where a.Code == code
                            select a;
                Utility.CompareSql(db, query.Expression, QueryFilterListForVariableTestSql);
                var data = query.ToArray();
                Assert.IsTrue(data.Length == 1);
            }
        }

        [TestMethod]
        public void OrderQueryDataTest()
        {
            using (var db = CreateContext())
            {
                var query = from a in db.Products
                            orderby a.Category descending
                            select a;
                Utility.CompareSql(db, query.Expression, OrderQueryDataTestSql);
                var data = query.ToArray();
                Assert.IsTrue(data.Any(a => a.Id > 0));
            }
        }

        [TestMethod]
        public void CrossQueryDataTest()
        {
            using (var db = CreateContext())
            {
                var query = from a in db.Products
                            from b in db.Customers
                            select new
                            {
                                ProductId = a.Id,
                                ProductName = a.Name,
                                CustomerId = b.Id,
                                CustomerName = b.Name
                            };
                Utility.CompareSql(db, query.Expression, CrossQueryDataTestSql);
                var data = query.ToArray();
                Assert.IsTrue(data.Any());
            }
        }

        [TestMethod]
        public void QueryListForTakeTest()
        {
            using (var db = CreateContext())
            {
                var query = (from a in db.Products
                             select a).Take(10);
                Utility.CompareSql(db, query.Expression, QueryListForTakeTestSql);
                var data = query.ToArray();
                Assert.IsTrue(data.Length <= 10);
            }
        }

        [TestMethod]
        public void QueryListForSkipTest()
        {
            using (var db = CreateContext())
            {
                var query = (from a in db.Products
                             select a).Skip(10);
                Utility.CompareSql(db, query.Expression, QueryListForSkipTestSql);
                var data = query.ToArray();
                Assert.IsTrue(data.Any(a => a.Id > 10));
            }
        }

        [TestMethod]
        public void QueryListForTakeSkipTest()
        {
            using (var db = CreateContext())
            {
                var query = (from a in db.Products
                             select a).Skip(10).Take(10);
                Utility.CompareSql(db, query.Expression, QueryListForTakeSkipTestSql);
                var data = query.ToArray();
                Assert.IsTrue(data.Length <= 10);
                Assert.IsTrue(data.Any(a => a.Id > 10));
            }
        }

        [TestMethod]
        public void QueryObjectMemberTest()
        {
            using (var db = CreateContext())
            {
                var query = from a in db.Orders
                            where a.Id > 10 && a.Id < 20
                            select a.Customer;
                Utility.CompareSql(db, query.Expression, QueryObjectMemberTestSql);
                var data = query.ToArray();
                Assert.IsTrue(data.Any());
            }
        }

        [TestMethod]
        public void QuerySetOperateTest()
        {
            using (var db = CreateContext())
            {
                var query = (from a in db.Customers
                             select new { a.Id, a.Code, a.Name })
                            .Concat(from b in db.Products
                                    select new { b.Id, b.Code, b.Name });
                Utility.CompareSql(db, query.Expression, QuerySetOperateTestSql);
                var data = query.ToArray();
                Assert.IsTrue(data.Any());
            }
        }

        internal OrderManageEntities CreateContext() => Constants.CreateSimpleContext();
    }
}
