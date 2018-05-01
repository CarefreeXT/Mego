namespace Caredev.Mego.Tests.Core.Commit.Simple
{
    using System.Linq;
    using Caredev.Mego.Tests.Models.Simple; 
    using Caredev.Mego.Exceptions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    [TestClass, TestCategory(Constants.TestCategoryRootName + ".Commit.Delete")]
    public partial class DeleteTest
    {
        [TestMethod]
        public void DeleteSingleTest()
        {
            Utility.CommitTest(CreateContext(), db =>
            {
                var data = db.OrderDetails.First();
                db.ConcurrencyTest(delegate ()
                {
                    data.Discount += 10000;
                    db.Executor.Execute(db.OrderDetails.Remove(data));
                });
            });
            Utility.CommitTest(CreateContext(), db =>
            {
                var data = db.OrderDetails.First();
                var operate = db.OrderDetails.Remove(data);
                int count = db.Executor.Execute(operate);
                var value = db.OrderDetails.Where(a => a.Id == data.Id).Count();
                Assert.AreEqual(value, 0);
                Utility.CompareSql(db, operate, DeleteSingleTestSql);
            });
        }

        [TestMethod]
        public void DeleteMultiForKeyTest()
        {
            Utility.CommitTest(CreateContext(), db =>
            {
                db.Configuration.EnableConcurrencyCheck = false;
                var list = db.OrderDetails.Take(3).ToArray();
                var operate = db.OrderDetails.RemoveRange(list);
                db.Executor.Execute(operate);
                var value = db.OrderDetails.Where(a => a.Id == list[0].Id).Count();
                Assert.AreEqual(value, 0);
                Utility.CompareSql(db, operate, DeleteMultiForKeyTestSql);
            });
        }

        [TestMethod]
        public void DeleteMultiForKeysTest()
        {
            Utility.CommitTest(CreateContext(), db =>
            {
                var data = db.Warehouses.Take(2).ToArray();
                db.ConcurrencyTest(delegate ()
                {
                    data[0].Address += "A";
                    var operate = db.Warehouses.Remove(data);
                    db.Executor.Execute(operate);
                });
            });
            Utility.CommitTest(CreateContext(), db =>
            {
                var data = db.Warehouses.Take(2).ToArray();
                var operate = db.Warehouses.Remove(data);
                db.Executor.Execute(operate);
                var testid = data[0].Id;
                var testnumber = data[0].Number;
                var count = db.Warehouses.Where(a => a.Id == testid && a.Number == testnumber).Count();
                Assert.AreEqual(count, 0);

                Utility.CompareSql(db, operate, DeleteMultiForKeysTestSql);
            });
        }

        [TestMethod]
        public void DeleteStatementForExpressionTest()
        {
            Utility.CommitTest(CreateContext(), db =>
            {
                var operate = db.Warehouses.Remove(a => a.Id > 0);
                db.Executor.Execute(operate);
                var count = db.Warehouses.Where(a => a.Id > 0).Count();
                Assert.AreEqual(count, 0);

                Utility.CompareSql(db, operate, DeleteStatementForExpressionTestSql);
            });
        }

        [TestMethod]
        public void DeleteStatementForQueryTest()
        {
            Utility.CommitTest(CreateContext(), db =>
            {
                if (db.Configuration.DatabaseFeature.HasCapable(Resolve.EDbCapable.ModifyJoin))
                {
                    var operate = db.Warehouses.Remove(from a in db.Warehouses
                                                       from b in db.Customers
                                                       where a.Id > b.Id && a.Number > 100
                                                       select a);
                    Utility.CompareSql(db, operate, DeleteStatementForQueryTestSql);
                    db.Executor.Execute(operate);
                }
            });
        }

        internal OrderManageEntities CreateContext() => Constants.CreateSimpleContext();
    }
}
