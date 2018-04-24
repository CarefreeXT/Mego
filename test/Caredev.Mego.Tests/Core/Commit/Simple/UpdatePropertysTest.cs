namespace Caredev.Mego.Tests.Core.Commit.Simple
{
    using System.Linq;
    using Caredev.Mego.Tests.Models.Simple;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    [TestClass, TestCategory(Constants.TestCategoryRootName + ".Commit.UpdatePropertys")]
    public partial class UpdatePropertysTest : ISimpleTest
    {
        [TestMethod]
        public void UpdateSingleObjectTest()
        {
            Utility.CommitTest(CreateContext(), db =>
            {
                var data = db.Customers.First();
                data.Address1 = "aaaa";
                data.Address2 = "bbbb";
                var operate = db.Customers.Update(a => new Customer()
                {
                    Id = a.Id,
                    Name = a.Name,
                    Address1 = a.Address2 + "-" + a.Address1
                }, data);
                Utility.CompareSql(db, operate, UpdateSingleObjectTestSql);

                db.Executor.Execute(operate);
                var id = data.Id;
                var value = db.Customers.Single(a => a.Id == id);
                Assert.AreEqual(value.Address1, "bbbb-aaaa");
            });
        }

        [TestMethod]
        public void UpdateMultiObjectTest()
        {
            Utility.CommitTest(CreateContext(), db =>
            {
                var data = db.Customers.Take(2).ToArray();
                data[0].Address1 = "aaaa";
                data[0].Address2 = "bbbb";
                var operate = db.Customers.Update(a => new Customer()
                {
                    Id = a.Id,
                    Name = a.Name,
                    Address1 = a.Address2 + "-" + a.Address1
                }, data);
                Utility.CompareSql(db, operate, UpdateMultiObjectTestSql);

                db.Executor.Execute(operate);
                var id = data[0].Id;
                var value = db.Customers.Single(a => a.Id == id);
                Assert.AreEqual(value.Address1, "bbbb-aaaa");
            });
        }

        [TestMethod]
        public void UpdateGenerateSingleObjectTest()
        {
            Utility.CommitTest(CreateContext(), db =>
            {
                var data = new Product()
                {
                    Id = 10,
                    Name = "Customer 100",
                    Category = 12,
                    Code = "A"
                };
                var operate = db.Products.Update(a => new Product()
                {
                    Id = a.Id,
                    Name = a.Name,
                    Category = a.Category,
                    Code = a.Name + a.Code
                }, data);
                Utility.CompareSql(db, operate, UpdateGenerateSingleObjectTestSql);

                db.Executor.Execute(operate);
                var value = db.Products.FirstOrDefault(a => a.Id == data.Id);
                Assert.IsNotNull(value);
                Assert.AreEqual(data.Code, value.Code);
            });
        }

        [TestMethod]
        public void UpdateGenerateMultiObjectTest()
        {
            Utility.CommitTest(CreateContext(), db =>
            {
                var list = new Product[] {
                    new Product()
                    {
                        Id = 12,
                        Name = "Customer 100",
                        Category = 12,
                        Code = "A"
                    }, new Product()
                    {
                        Id = 13,
                        Name = "Cus 101",
                        Category = 12,
                        Code = "B"
                    }
                };
                var operate = db.Products.Update(a => new Product()
                {
                    Id = a.Id,
                    Name = a.Name,
                    Category = a.Category,
                    Code = a.Name + a.Code
                }, list);
                Utility.CompareSql(db, operate, UpdateGenerateMultiObjectTestSql);

                db.Executor.Execute(operate);
                var value = db.Products.FirstOrDefault(a => a.Id == list[0].Id);
                Assert.IsNotNull(value);
                Assert.AreEqual(list[0].Code, value.Code);
            });
        }

        public OrderManageEntities CreateContext() => Constants.CreateSimpleContext();
    }
}
