namespace Caredev.Mego.Tests.Core
{
    using System.Linq;
    using Caredev.Mego.Tests.Models.Simple;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    [TestClass, TestCategory(Constants.TestCategoryRootName + ".Anonymous")]
    public class AnonymousSetTest : ISimpleTest
    {
        [TestMethod]
        public void AnonymousQueryTest()
        {
            using (var db = CreateContext())
            {
                var item = new { Id = 0, Name = "", Code = "" };
                var ds = db.Set(item, "Customers");
                var data = (from a in ds.Take(10)
                            where a.Id > 20 && a.Id < 30
                            select a).ToArray();
                Assert.IsTrue(data.All(a => a.Id > 0 && a.Name.Length > 0));
            }
        }

        [TestMethod]
        public void AnonymousDeleteSingleTest()
        {
            Utility.CommitTest(CreateContext(), db =>
            {
                var ds = db.Set(new { Id = 0 }, "OrderDetails");
                var data = db.OrderDetails.First();

                var operate = ds.Remove(new { data.Id });
                int count = db.Executor.Execute(operate);
                var value = db.OrderDetails.Where(a => a.Id == data.Id).Count();
                Assert.AreEqual(value, 0);
            });
        }

        [TestMethod]
        public void AnonymousInsertSingleTest()
        {
            Utility.CommitTest(CreateContext(), db =>
            {
                var data = new
                {
                    Id = 10000,
                    Name = "Customer 100",
                    Code = "C100",
                    Address1 = "A",
                    Address2 = "B",
                    Zip = "Z"
                };
                var ds = db.Set(data, "Customers");

                var operate = ds.Add(data);
                db.Executor.Execute(operate);
                var test = db.Customers.SingleOrDefault(a => a.Id == 10000);
                Assert.IsNotNull(test);
            });
        }

        [TestMethod]
        public void UpdateSingleObjectTest()
        {
            Utility.CommitTest(CreateContext(), db =>
            {
                var data = db.Customers.First();

                var newitem = new { data.Id, Name = "Customer 100" };
                var ds = db.Set(newitem, "Customers");
                var operate = ds.Update(newitem);

                db.Executor.Execute(operate);
                var id = data.Id;
                var value = db.Customers.FirstOrDefault(a => a.Id == id);
                Assert.IsNotNull(value);
                Assert.AreEqual(value.Name, "Customer 100");
            });
        }

        public OrderManageEntities CreateContext() => Constants.CreateSimpleContext();
    }
}
