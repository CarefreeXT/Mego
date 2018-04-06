namespace Caredev.Mego.Tests.Core.Commit.Simple
{
    using System.Linq;
    using Caredev.Mego.Tests.Models.Simple;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    [TestClass, TestCategory(Constants.TestCategoryRootName + ".Commit.Maintenance")]
    public partial class MaintenanceTest : ISimpleTest
    {
        [TestMethod]
        public void CreateTableTest()
        {
            Utility.CommitTest(CreateContext(), db =>
            {
                var manager = db.Database.Manager;
                var operate = manager.CreateTable<Product>();

                Utility.CompareSql(db, operate, CreateTableTestSql);
            });
        }

        [TestMethod]
        public void TableExsitTest()
        {
            Utility.CommitTest(CreateContext(), db =>
            {
                var manager = db.Database.Manager;
                var operate = manager.Exsit<Product>();

                Utility.CompareSql(db, operate, TableExsitTestSql);
            });
        }

        [TestMethod]
        public void CreateRelationTest()
        {
            Utility.CommitTest(CreateContext(), db =>
            {
                var manager = db.Database.Manager;
                var operate = manager.CreateRelation((Order order) => order.Customer).ToArray();

                Utility.CompareSql(db, operate[0], CreateRelationTestSql);
            });
        }

        [TestMethod]
        public void CreateCompositeRelationTest()
        {
            Utility.CommitTest(CreateContext(), db =>
            {
                var manager = db.Database.Manager;
                var operate = manager.CreateRelation((Order order) => order.Products).ToArray();

                Utility.CompareSql(db, operate[0], CreateCompositeRelationTestSql);
                Utility.CompareSql(db, operate[1], CreateCompositeRelationTestSql1);
            });
        }



        public OrderManageEntities CreateContext()
        {
            return new OrderManageEntities(Constants.ConnectionNameSimple);
        }
    }
}
