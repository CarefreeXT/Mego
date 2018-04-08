namespace Caredev.Mego.Tests.Core.Commit.Inherit
{
    using System.Linq;
    using Caredev.Mego.Tests.Models.Inherit;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    [TestClass, TestCategory(Constants.TestCategoryRootName + ".ICommit.Update")]
    public partial class UpdateTest : IInheritTest
    {
        [TestMethod]
        public void UpdateSingleObjectTest()
        {
            Utility.CommitTest(CreateContext(), db =>
            {
                var data = db.Customers.First();
                db.ConcurrencyTest(delegate ()
                {
                    data.Zip += "A";
                    db.Executor.Execute(db.Customers.Update(data));
                });
            });
            Utility.CommitTest(CreateContext(), db =>
            {
                var data = db.Customers.First();
                data.Name = "Customer 100";
                var operate = db.Customers.Update(data);
                Utility.CompareSql(db, operate, UpdateSingleObjectTestSql);

                db.Executor.Execute(operate);
                var id = data.Id;
                var value = db.Customers.FirstOrDefault(a => a.Id == id);
                Assert.IsNotNull(value);
                Assert.AreEqual(value.Name, "Customer 100");
            });
        }

        [TestMethod]
        public void UpdateMultiObjectTest()
        {
            Utility.CommitTest(CreateContext(), db =>
            {
                var data = db.Customers.Take(2).ToArray();
                data[0].Name = "Customer 100";
                var operate = db.Customers.Update(data);
                Utility.CompareSql(db, operate, UpdateMultiObjectTestSql);

                db.Executor.Execute(operate);
                var id = data[0].Id;
                var value = db.Customers.FirstOrDefault(a => a.Id == id);
                Assert.IsNotNull(value);
                Assert.AreEqual(value.Name, "Customer 100");
            });
        }

        [TestMethod]
        public void UpdateGenerateSingleObjectTest()
        {
            Utility.CommitTest(CreateContext(), db =>
            {
                var operate = db.Products.Update(new Product()
                {
                    Id = 10,
                    Code = "sdfsdf",
                    Name = "Customer 100",
                    IsValid = true,
                    UpdateDate = System.DateTime.Now,
                    Category = 12
                });
                Utility.CompareSql(db, operate, UpdateGenerateSingleObjectTestSql);

                db.Executor.Execute(operate);
                var value = db.Products.FirstOrDefault(a => a.Id == 10);
                Assert.IsNotNull(value);
                Assert.AreEqual(value.Name, "Customer 100");
            });
        }
        
        [TestMethod]
        public void UpdateGenerateMultiObjectTest()
        {
            Utility.CommitTest(CreateContext(), db =>
            {
                var operate = db.Products.Update(new Product()
                {
                    Id = 10,
                    Code = "sdfsdf",
                    Name = "Customer 100",
                    IsValid = true,
                    UpdateDate = System.DateTime.Now,
                    Category = 12
                }, new Product()
                {
                    Id = 13,
                    Code = "sdfsdf1",
                    Name = "Prod 100",
                    IsValid = false,
                    UpdateDate = System.DateTime.Now,
                    Category = 12
                });
                Utility.CompareSql(db, operate, UpdateGenerateMultiObjectTestSql);

                db.Executor.Execute(operate);
                var value = db.Products.FirstOrDefault(a => a.Id == 10);
                Assert.IsNotNull(value);
                Assert.AreEqual(value.Name, "Customer 100");
            });
        }

        [TestMethod]
        public void UpdateQueryTest()
        {
            Utility.CommitTest(CreateContext(), db =>
            {
                var operate = db.Customers.Update(from b in db.Customers
                                                  join a in db.Products on b.Id equals a.Id
                                                  where a.Id > 100
                                                  select new Customer()
                                                  {
                                                      Name = a.Name,
                                                      Code = a.Name,
                                                      Address1 = a.Code,
                                                  });
                Utility.CompareSql(db, operate, UpdateQueryTestSql);

                db.Executor.Execute(operate);
            });
        }

        public OrderManageEntities CreateContext()
        {
            return new OrderManageEntities(Constants.ConnectionNameInherit);
        }
    }
}