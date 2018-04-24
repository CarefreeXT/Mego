namespace Caredev.Mego.Tests.Core.Query.Inherit
{
    using System.Linq;
    using Caredev.Mego.Tests.Models.Inherit;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    [TestClass, TestCategory(Constants.TestCategoryRootName + ".IQuery.ExpandMember")]
    public partial class ExpandMemberTest : IInheritTest
    {
        [TestMethod]
        public void ExpandOneLevelObjectMemberStrTest()
        {
            using (var db = CreateContext())
            {
                var query = from a in db.Orders.Include("Customer")
                            select a;
                Utility.CompareSql(db, query.Expression, ExpandOneLevelObjectMemberStrTestSql);
                var data = query.ToArray();
                Assert.IsTrue(data.All(a => a.Customer != null));
            }
        }

        [TestMethod]
        public void ExpandOneLevelCollectionMemberStrTest()
        {
            using (var db = CreateContext())
            {
                var query = from a in db.Orders.Include("Details")
                            select a;
                Utility.CompareSql(db, query.Expression, ExpandOneLevelCollectionMemberStrTestSql);
                var data = query.ToArray();
                Assert.IsTrue(data.Any(a => a.Details.Count > 0));
            }
        }

        [TestMethod]
        public void ExpandTwoLevelMemberStrTest()
        {
            using (var db = CreateContext())
            {
                var query = from a in db.Orders.Include("Customer.Orders")
                            select a;
                Utility.CompareSql(db, query.Expression, ExpandTwoLevelMemberStrTestSql);
                var data = query.ToArray();
                Assert.IsTrue(data.Any(a => a.Customer != null && a.Customer.Orders.Count > 0));
            }
        }

        [TestMethod]
        public void ExpandOneLevelObjectMemberTest()
        {
            using (var db = CreateContext())
            {
                var query = from a in db.Orders.Include(a => a.Customer)
                            select a;
                Utility.CompareSql(db, query.Expression, ExpandOneLevelObjectMemberTestSql);
                var data = query.ToArray();
                Assert.IsTrue(data.All(a => a.Customer != null));
            }
        }

        [TestMethod]
        public void ExpandOneLevelObjectMemberPageTest()
        {
            using (var db = CreateContext())
            {
                var query = (from a in db.Orders.Include(a => a.Customer)
                             select a).Skip(5).Take(5);
                Utility.CompareSql(db, query.Expression, ExpandOneLevelObjectMemberPageTestSql);
                var data = query.ToArray();
                Assert.IsTrue(data.All(a => a.Customer != null));
            }
        }

        [TestMethod]
        public void ExpandTwoLevelMemberTest()
        {
            using (var db = CreateContext())
            {
                var query = from a in db.Orders.Include(a => a.Customer.Orders)
                            select a;
                Utility.CompareSql(db, query.Expression, ExpandTwoLevelMemberTestSql);
                var data = query.ToArray();
                Assert.IsTrue(data.Any(a => a.Customer != null && a.Customer.Orders.Count > 0));
            }
        }

        [TestMethod]
        public void ExpandOneLevelCollectionMemberTest()
        {
            using (var db = CreateContext())
            {
                var query = from a in db.Orders.Include(a => a.Details)
                            select a;
                Utility.CompareSql(db, query.Expression, ExpandOneLevelCollectionMemberTestSql);
                var data = query.ToArray();
                Assert.IsTrue(data.Any(a => a.Details.Count > 0));
            }
        }
        [TestMethod]
        public void ExpandTwoLevelCollectionMemberTest()
        {
            using (var db = CreateContext())
            {
                var query = from a in db.Orders.Include(a => a.Details.Include(b => b.Product))
                            select a;
                Utility.CompareSql(db, query.Expression, ExpandTwoLevelCollectionMemberTestSql);
                var data = query.ToArray();
                Assert.IsTrue(data.Any(a => a.Details.Count > 0 && a.Details.Any(b => b.Product != null)));
            }
        }
        [TestMethod]
        public void ExpandTwoLevelCollectionMemberPageTest()
        {
            using (var db = CreateContext())
            {
                var query = (from a in db.Orders.Include(a => a.Details.Include(b => b.Product))
                             select a).Skip(5).Take(5);
                Utility.CompareSql(db, query.Expression, ExpandTwoLevelCollectionMemberPageTestSql);
                var data = query.ToArray();
                Assert.IsTrue(data.Any(a => a.Details.Count > 0 && a.Details.Any(b => b.Product != null)));
            }
        }
        [TestMethod]
        public void ExpandOneLevelCollectionMemberFilterTest()
        {
            using (var db = CreateContext())
            {
                var query = from a in db.Orders.Include(a => a.Details.Where(b => b.Id > 10))
                            select a;
                Utility.CompareSql(db, query.Expression, ExpandOneLevelCollectionMemberFilterTestSql);
                var data = query.ToArray();
                Assert.IsTrue(data.Any());
            }
        }

        [TestMethod]
        public void ExpandTwoLevelCollectionMemberFilterTest()
        {
            using (var db = CreateContext())
            {
                var query = from a in db.Customers.Include(
                                b => b.Orders.Where(d => d.Id > 2).Include(
                                    c => c.Details.Where(d => d.Id > 3)
                                )
                             )
                            select a;
                Utility.CompareSql(db, query.Expression, ExpandTwoLevelCollectionMemberFilterTestSql);
                var data = query.ToArray();
                Assert.IsTrue(data.Any());
            }
        }
        public OrderManageEntities CreateContext() => Constants.CreateInheritContext();
    }
}
