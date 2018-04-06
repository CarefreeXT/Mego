namespace Caredev.Mego.Tests.Core.Commit.Simple
{
    using System;
    using System.Linq;
    using Caredev.Mego.Tests.Models.Simple;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    [TestClass, TestCategory(Constants.TestCategoryRootName + ".Commit.Insert")]
    public partial class InsertTest : ISimpleTest
    {
        [TestMethod]
        public void InsertSingleObjectTest()
        {
            Utility.CommitTest(CreateContext(), db =>
            {
                var operate = db.Customers.Add(new Customer()
                {
                    Id = 10000,
                    Name = "Customer 100",
                    Code = "C100",
                    Address1 = "A",
                    Address2 = "B",
                    Zip = "Z"
                });
                Utility.CompareSql(db, operate, InsertSingleObjectTestSql);
                db.Executor.Execute(operate);
                var test = db.Customers.SingleOrDefault(a => a.Id == 10000);
                Assert.IsNotNull(test);
            });
        }

        [TestMethod]
        public void InsertMultiObjectTest()
        {
            Utility.CommitTest(CreateContext(), db =>
            {
                var operate = db.Customers.Add(new Customer()
                {
                    Id = 10100,
                    Name = "Customer 100",
                    Code = "C100",
                    Address1 = "A",
                    Address2 = "B",
                    Zip = "Z"
                }, new Customer()
                {
                    Id = 10200,
                    Name = "Customer 200",
                    Code = "C100",
                    Address1 = "A",
                    Address2 = "B",
                    Zip = "Z"
                });
                Utility.CompareSql(db, operate, InsertMultiObjectTestSql);

                db.Executor.Execute(operate);
                var test = db.Customers.Where(a => a.Id > 10000).ToArray();
                Assert.IsTrue(test.Length == 2);
                Assert.AreEqual(test[0].Id, 10100);
                Assert.AreEqual(test[1].Id, 10200);
            });
        }

        [TestMethod]
        public void InsertIdentitySingleObjectTest()
        {
            Utility.CommitTest(CreateContext(), db =>
            {
                var operate = db.Products.Add(new Product()
                {
                    Code = "TP",
                    Name = "Product Test",
                    Category = 3,
                    IsValid = true
                });
                Utility.CompareSql(db, operate, InsertIdentitySingleObjectTestSql);

                db.Executor.Execute(operate);
                var teste = db.Products.Where(a => a.Name == "Product Test").ToArray();
                Assert.IsTrue(teste.Length > 0);
            });
        }

        [TestMethod]
        public void InsertIdentityMultiObjectTest()
        {
            Utility.CommitTest(CreateContext(), db =>
            {
                var data = new Product[] {
                    new Product()
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
                }
                };
                var operate = db.Products.Add(data);
                Utility.CompareSql(db, operate, InsertIdentityMultiObjectTestSql);

                db.Executor.Execute(operate);
                Assert.IsTrue(data.All(a => a.Id > 0));
                var teste = db.Products.Where(a => a.Name == "Product Test").ToArray();
                Assert.IsTrue(teste.Length > 1);
            });
        }

        [TestMethod]
        public void InsertExpressionSingleObjectTest()
        {
            Utility.CommitTest(CreateContext(), db =>
            {
                var operate = db.Orders.Add(new Order()
                {
                    Id = 10000,
                    CustomerId = 6,
                    State = 2,
                    ModifyDate = DateTime.Now
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
                var date = DateTime.Now;
                var operate = db.Orders.Add(new Order()
                {
                    Id = 10000,
                    CustomerId = 6,
                    State = 2,
                    ModifyDate = date
                }, new Order()
                {
                    Id = 10001,
                    CustomerId = 6,
                    State = 3,
                    ModifyDate = date
                });
                Utility.CompareSql(db, operate, InsertExpressionMultiObjectTestSql);

                db.Executor.Execute(operate);
                var test = db.Orders.Where(a => a.Id >= 10000).ToArray();
                Assert.IsTrue(test.Length == 2);
            });
        }

        [TestMethod]
        public void InsertQueryTest()
        {
            Utility.CommitTest(CreateContext(), db =>
            {
                var operate = db.Customers.Add(from a in db.Products
                                               where a.Id > 5
                                               select new Customer()
                                               {
                                                   Id = a.Id + 10000,
                                                   Name = a.Name,
                                                   Code = a.Name,
                                                   Address1 = a.Code
                                               });
                Utility.CompareSql(db, operate, InsertQueryTestSql);

                db.Executor.Execute(operate);
                var list = db.Customers.Where(a => a.Id > 10000).ToList();
                Assert.IsTrue(list.Count > 0);
            });
        }

        public OrderManageEntities CreateContext()
        {
            return new OrderManageEntities(Constants.ConnectionNameSimple);
        }
    }
}

