namespace Caredev.Mego.Tests.Core.Commit.Simple
{
    using System.Linq;
    using Caredev.Mego.Tests.Models.Simple; 
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    [TestClass, TestCategory(Constants.TestCategoryRootName + ".Commit.Relation")]
    public partial class RelationTest
    {
        [TestMethod]
        public void AddObjectSingleTest()
        {
            using (var db = CreateContext())
            {
                var source = new Order() { Id = 1 };
                var target = new Customer { Id = 2 };
                var operate = db.Orders.AddRelation(source, s => s.Customer, target);
                Utility.CompareSql(db, operate, AddObjectSingleTestSql);
            }
        }

        [TestMethod]
        public void AddCollectionSingleTest()
        {
            using (var db = CreateContext())
            {
                var source = new Order() { Id = 1 };
                var target = new OrderDetail { Id = 2 };
                var operate = db.Orders.AddRelation(source, s => s.Details, target);
                Utility.CompareSql(db, operate, AddCollectionSingleTestSql);
            }
        }

        [TestMethod]
        public void RemoveObjectSingleTest()
        {
            using (var db = CreateContext())
            {
                var source = new Order() { Id = 1 };
                var target = new Customer { Id = 2 };
                var operate = db.Orders.RemoveRelation(source, s => s.Customer, target);
                Utility.CompareSql(db, operate, RemoveObjectSingleTestSql);
            }
        }

        [TestMethod]
        public void RemoveCollectionSingleTest()
        {
            using (var db = CreateContext())
            {
                var source = new Order() { Id = 1 };
                var target = new OrderDetail { Id = 2 };
                var operate = db.Orders.RemoveRelation(source, s => s.Details, target);
                Utility.CompareSql(db, operate, RemoveCollectionSingleTestSql);
            }
        }

        [TestMethod]
        public void AddObjectMultiTest()
        {
            using (var db = CreateContext())
            {
                var source = new Order() { Id = 1 };
                var target = new Customer { Id = 2 };
                var operate = db.Orders.AddRelation(source, s => s.Customer, target);
                operate.Add(new Order() { Id = 3 }, new Customer { Id = 4 });
                Utility.CompareSql(db, operate, AddObjectMultiTestSql);
            }
        }

        [TestMethod]
        public void AddCollectionMultiTest()
        {
            using (var db = CreateContext())
            {
                var source = new Order() { Id = 1 };
                var target = new OrderDetail { Id = 2 };
                var operate = db.Orders.AddRelation(source, s => s.Details, target);
                operate.Add(new Order() { Id = 3 }, new OrderDetail { Id = 4 });
                Utility.CompareSql(db, operate, AddCollectionMultiTestSql);
            }
        }

        [TestMethod]
        public void RemoveObjectMultiTest()
        {
            using (var db = CreateContext())
            {
                var source = new Order() { Id = 1 };
                var target = new Customer { Id = 2 };
                var operate = db.Orders.RemoveRelation(source, s => s.Customer, target);
                operate.Add(new Order() { Id = 3 }, new Customer { Id = 4 });
                Utility.CompareSql(db, operate, RemoveObjectMultiTestSql);
            }
        }

        [TestMethod]
        public void RemoveCollectionMultiTest()
        {
            using (var db = CreateContext())
            {
                var source = new Order() { Id = 1 };
                var target = new OrderDetail { Id = 2 };
                var operate = db.Orders.RemoveRelation(source, s => s.Details, target);
                operate.Add(new Order() { Id = 3 }, new OrderDetail { Id = 4 });
                Utility.CompareSql(db, operate, RemoveCollectionMultiTestSql);
            }
        }

        [TestMethod]
        public void AddCompositeSingleTest()
        {
            using (var db = CreateContext())
            {
                var source = new Order() { Id = 1 };
                var target = new Product { Id = 2 };
                var operate = db.Orders.AddRelation(source, s => s.Products, target);
                Utility.CompareSql(db, operate, AddCompositeSingleTestSql);
            }
        }

        [TestMethod]
        public void RemoveCompositeSingleTest()
        {
            using (var db = CreateContext())
            {
                var source = new Order() { Id = 1 };
                var target = new Product { Id = 2 };
                var operate = db.Orders.RemoveRelation(source, s => s.Products, target);
                Utility.CompareSql(db, operate, RemoveCompositeSingleTestSql);
            }
        }

        [TestMethod]
        public void AddCompositeMultiTest()
        {
            using (var db = CreateContext())
            {
                var source = new Order() { Id = 1 };
                var target = new Product { Id = 2 };
                var operate = db.Orders.AddRelation(source, s => s.Products, target);
                operate.Add(new Order() { Id = 3 }, new Product { Id = 4 });
                Utility.CompareSql(db, operate, AddCompositeMultiTestSql);
            }
        }

        [TestMethod]
        public void RemoveCompositeMultiTest()
        {
            using (var db = CreateContext())
            {
                var source = new Order() { Id = 1 };
                var target = new Product { Id = 2 };
                var operate = db.Orders.RemoveRelation(source, s => s.Products, target);
                operate.Add(new Order() { Id = 3 }, new Product { Id = 4 });
                Utility.CompareSql(db, operate, RemoveCompositeMultiTestSql);
            }
        }

        internal OrderManageEntities CreateContext() => Constants.CreateSimpleContext();
    }
}
