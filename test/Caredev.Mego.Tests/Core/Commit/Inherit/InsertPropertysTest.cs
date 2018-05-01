namespace Caredev.Mego.Tests.Core.Commit.Inherit
{
    using System;
    using System.Linq;
    using Caredev.Mego.Tests.Models.Inherit;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    [TestClass, TestCategory(Constants.TestCategoryRootName + ".ICommit.InsertPropertys")]
    public partial class InsertPropertysTest 
    {
        [TestMethod]
        public void InsertSingleObjectTest()
        {
            Utility.CommitTest(CreateContext(), db =>
            {
                var operate = db.Customers.Add(a => new Customer()
                {
                    Id = a.Id,
                    Name = a.Name + "a",
                    Address1 = a.Address1 + a.Address2
                }, new Customer()
                {
                    Id = 10000,
                    Name = "sdfdsf",
                    Address1 = "a",
                    Address2 = "b"
                });
                Utility.CompareSql(db, operate, InsertSingleObjectTestSql);

                db.Executor.Execute(operate);
                var test = db.Customers.FirstOrDefault(a => a.Id == 10000);
                Assert.IsNotNull(test);
            });
        }

        [TestMethod]
        public void InsertMultiObjectTest()
        {
            Utility.CommitTest(CreateContext(), db =>
            {
                var operate = db.Customers.Add(a => new Customer()
                {
                    Id = a.Id,
                    Name = a.Name + "a",
                    Address1 = a.Address1 + a.Address2
                }, new Customer()
                {
                    Id = 10100,
                    Name = "Customer 100",
                    Address1 = "a",
                    Address2 = "b"
                }, new Customer()
                {
                    Id = 10110,
                    Name = "Customer 110",
                    Address1 = "a",
                    Address2 = "b"
                });
                Utility.CompareSql(db, operate, InsertMultiObjectTestSql);

                db.Executor.Execute(operate);
                var test = db.Customers.Where(a => a.Id > 10000).ToArray();
                Assert.IsTrue(test.Length == 2);
                Assert.AreEqual(test[0].Id, 10100);
                Assert.AreEqual(test[1].Id, 10110);
            });
        }

        [TestMethod]
        public void InsertIdentitySingleObjectTest()
        {
            Utility.CommitTest(CreateContext(), db =>
            {
                var operate = db.Products.Add(a => new Product()
                {
                    Code = a.Code,
                    Name = a.Name + "a",
                    Category = a.Category,
                    IsValid = a.IsValid,
                }, new Product()
                {
                    Code = "TP",
                    Name = "Product Test",
                    Category = 3,
                    IsValid = true
                });
                Utility.CompareSql(db, operate, InsertIdentitySingleObjectTestSql);

                db.Executor.Execute(operate);
                var test = db.Products.FirstOrDefault(a => a.Code == "TP");
                Assert.IsNotNull(test);
                Assert.IsTrue(test.Name.EndsWith("a"));
            });
        }

        [TestMethod]
        public void InsertIdentityMultiObjectTest()
        {
            Utility.CommitTest(CreateContext(), db =>
            {
                var operate = db.Products.Add(a => new Product()
                {
                    Code = a.Code,
                    Name = a.Name + "a",
                    Category = a.Category,
                    IsValid = a.IsValid,
                }, new Product()
                {
                    Code = "TP",
                    Name = "Product Test",
                    Category = 3,
                    IsValid = true
                }, new Product()
                {
                    Code = "OP",
                    Name = "Product Test",
                    Category = 2,
                    IsValid = false
                });
                Utility.CompareSql(db, operate, InsertIdentityMultiObjectTestSql);

                db.Executor.Execute(operate);
                var test = db.Products.FirstOrDefault(a => a.Code == "TP");
                Assert.IsNotNull(test);
                Assert.IsTrue(test.Name.EndsWith("a"));
            });
        }

        [TestMethod]
        public void InsertExpressionSingleObjectTest()
        {
            Utility.CommitTest(CreateContext(), db =>
            {
                var operate = db.Orders.Add(a => new Order()
                {
                    Id = a.Id,
                    CustomerId = 6,
                    ModifyDate = DateTime.Now,
                    State = a.State,
                }, new Order()
                {
                    Id = 10000,
                    State = 2,
                });
                Utility.CompareSql(db, operate, InsertExpressionSingleObjectTestSql);

                db.Executor.Execute(operate);
                var test = db.Orders.Single(a => a.Id == 10000);
            });
        }

        [TestMethod]
        public void InsertExpressionMultiObjectTest()
        {
            Utility.CommitTest(CreateContext(), db =>
            {
                var operate = db.Orders.Add(a => new Order()
                {
                    Id = a.Id,
                    CustomerId = 6,
                    ModifyDate = DateTime.Now,
                    State = a.State,
                }, new Order()
                {
                    Id = 10000,
                    State = 2,
                }, new Order()
                {
                    Id = 10001,
                    State = 3,
                });
                Utility.CompareSql(db, operate, InsertExpressionMultiObjectTestSql);

                db.Executor.Execute(operate);
                var test = db.Orders.Where(a => a.Id >= 10000).ToArray();
                Assert.IsTrue(test.Length == 2);
            });
        }

        internal OrderManageEntities CreateContext() => Constants.CreateInheritContext();
    }
}