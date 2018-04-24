namespace Caredev.Mego.Tests.Core.Query.Inherit
{
    using System.Linq;
    using Caredev.Mego.Exceptions;
    using Caredev.Mego.Tests.Models.Inherit;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    [TestClass, TestCategory(Constants.TestCategoryRootName + ".IQuery.Simple")]
    public partial class SingleEntityTest : IInheritTest
    {
        [TestMethod]
        public void QueryAllDataTest()
        {
            using (var db = CreateContext())
            {
                var query = db.Products;
                Utility.CompareSql(db, query.Expression, QueryAllDataTestSql);
                var data = query.ToArray();
                Assert.IsTrue(data.Any(a => a.Id > 0));
            }
        }

        [TestMethod]
        public void OrderQueryDataTest()
        {
            using (var db = CreateContext())
            {
                var query = db.Products.OrderBy(a => a.Category);
                Utility.CompareSql(db, query.Expression, OrderQueryDataTestSql);
                var data = query.ToArray();
                Assert.IsTrue(data.Any(a => a.Id > 0));
            }
        }

        [TestMethod]
        public void QueryAggregateSumTest()
        {
            using (var db = CreateContext())
            {
                var query = db.Products.Sum(a => a.Id);
                Assert.IsTrue(query > 0);
            }
        }
        
        [TestMethod]
        public void QueryRetrievalValueFirstTest()
        {
            using (var db = CreateContext())
            {
                var query = db.Customers.Where(a => a.Id > 10).Select(a => a.Id).First();
                Assert.IsTrue(query > 10);
            }
        }

        [TestMethod]
        public void QueryFilterContainsTest()
        {
            using (var db = CreateContext())
            {
                var ids = new int[] { 1, 2, 3, 4 };
                var query = from a in db.Products
                            where ids.Contains(a.Id)
                            select a;
                Utility.CompareSql(db, query.Expression, QueryFilterContainsTestSql);
                var data = query.ToArray();
                Assert.IsTrue(data.Any(a => a.Id > 0));
            }
        }

        public OrderManageEntities CreateContext() => Constants.CreateInheritContext();
    }

    public partial class SingleEntityTest
    {

        [TestMethod]
        public void QueryRetrievalObjectFirstTest()
        {
            using (var db = CreateContext())
            {
                var query = db.Customers.Where(a => a.Id >= 10).First();
                Assert.IsTrue(query.Id == 10);
            }
        }

        [TestMethod]
        public void QueryRetrievalObjectFirstNullTest()
        {
            using (var db = CreateContext())
            {
                Assert.ThrowsException<OutputException>(() =>
                {
                    db.Customers.Where(a => a.Id < 0).First();
                });
            }
        }

        [TestMethod]
        public void QueryRetrievalObjectFirstOrDefaultTest()
        {
            using (var db = CreateContext())
            {
                var query = db.Customers.Where(a => a.Id > 10).FirstOrDefault();
                Assert.IsTrue(query.Id > 10);
            }
        }

        [TestMethod]
        public void QueryRetrievalObjectFirstOrDefaultNullTest()
        {
            using (var db = CreateContext())
            {
                var query = db.Customers.Where(a => a.Id < 0).FirstOrDefault();
                Assert.IsNull(query);
            }
        }

        [TestMethod]
        public void QueryRetrievalObjectElementAtTest()
        {
            using (var db = CreateContext())
            {
                var query = db.Customers.Where(a => a.Id > 10).ElementAt(4);
                Assert.IsTrue(query.Id == 15);
            }
        }

        [TestMethod]
        public void QueryRetrievalObjectElementAtOrDefaultTest()
        {
            using (var db = CreateContext())
            {
                var query = db.Customers.Where(a => a.Id > 10).ElementAtOrDefault(4);
                Assert.IsTrue(query.Id == 15);
            }
        }

        [TestMethod]
        public void QueryRetrievalObjectSingleTest()
        {
            using (var db = CreateContext())
            {
                var query = db.Customers.Where(a => a.Id == 10).Single();
                Assert.IsTrue(query.Id == 10);
            }
        }

        [TestMethod]
        public void QueryRetrievalObjectSingleNullTest()
        {
            using (var db = CreateContext())
            {
                Assert.ThrowsException<OutputException>(() =>
                {
                    db.Customers.Where(a => a.Id > 0).Single();
                });
            }
        }

        [TestMethod]
        public void QueryRetrievalObjectSingleOrDefaultTest()
        {
            using (var db = CreateContext())
            {
                var query = db.Customers.Where(a => a.Id == 10).SingleOrDefault();
                Assert.IsTrue(query.Id == 10);
            }
        }

        [TestMethod]
        public void QueryRetrievalObjectSingleOrDefaultNullTest()
        {
            using (var db = CreateContext())
            {
                var query = db.Customers.Where(a => a.Id < 0).SingleOrDefault();
                Assert.IsNull(query);
            }
        }

    }
}
