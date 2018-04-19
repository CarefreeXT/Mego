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
                var operate = manager.CreateTable<Product>("TestProduct");

                Utility.CompareSql(db, operate, CreateTableTestSql);
            });
        }

        [TestMethod]
        public void DropTableTest()
        {
            Utility.CommitTest(CreateContext(), db =>
            {
                var manager = db.Database.Manager;
                var operate = manager.DropTable<Product>("TestProduct");

                Utility.CompareSql(db, operate, DropTableTestSql);
            });
        }

        [TestMethod]
        public void CreateTempTableTest()
        {
            Utility.CommitTest(CreateContext(), db =>
            {
                if (db.Configuration.DatabaseFeature.HasCapable(Resolve.EDbCapable.TemporaryTable))
                {
                    var manager = db.Database.Manager;
                    var operate = manager.CreateTempTable<Product>("TestProduct");

                    Utility.CompareSql(db, operate, CreateTempTableTestSql);
                }
            });
        }

        [TestMethod]
        public void CreateTableVariableTest()
        {
            Utility.CommitTest(CreateContext(), db =>
            {
                if (db.Configuration.DatabaseFeature.HasCapable(Resolve.EDbCapable.TableVariable))
                {
                    var manager = db.Database.Manager;
                    var operate = manager.CreateTableVariable<Product>("TestProduct");

                    Utility.CompareSql(db, operate, CreateTableVariableTestSql);
                }
            });
        }

        [TestMethod]
        public void TableIsExsitTest()
        {
            Utility.CommitTest(CreateContext(), db =>
            {
                var manager = db.Database.Manager;
                var operate = manager.TableIsExsit<Product>();

                Utility.CompareSql(db, operate, TableIsExsitTestSql);
            });
        }

        [TestMethod]
        public void RenameTableTest()
        {
            Utility.CommitTest(CreateContext(), db =>
            {
                var manager = db.Database.Manager;
                var operate = manager.RenameTable<Product>("TestProduct");

                Utility.CompareSql(db, operate, RenameTableTestSql);
            });
        }

        [TestMethod]
        public void CreateViewTest()
        {
            Utility.CommitTest(CreateContext(), db =>
            {
                var manager = db.Database.Manager;
                var operate = manager.CreateView<Product>(from a in db.Products
                                                          select a, "TestProduct");

                Utility.CompareSql(db, operate, CreateViewTestSql);
            });
        }

        [TestMethod]
        public void CreateViewTest2()
        {
            Utility.CommitTest(CreateContext(), db =>
            {
                var manager = db.Database.Manager;
                var operate = manager.CreateView<Product>("SELECT * FROM Products WHERE Id>0", "TestProduct");

                Utility.CompareSql(db, operate, CreateViewTest2Sql);
            });
        }

        [TestMethod]
        public void DropViewTest()
        {
            Utility.CommitTest(CreateContext(), db =>
            {
                var manager = db.Database.Manager;
                var operate = manager.DropView<Product>("TestProduct");

                Utility.CompareSql(db, operate, DropViewTestSql);
            });
        }

        [TestMethod]
        public void ViewIsExsitTest()
        {
            Utility.CommitTest(CreateContext(), db =>
            {
                var manager = db.Database.Manager;
                var operate = manager.ViewIsExsit<Product>();

                Utility.CompareSql(db, operate, ViewIsExsitTestSql);
            });
        }

#if !SQLITE
        [TestMethod]
        public void RenameViewTest()
        {
            Utility.CommitTest(CreateContext(), db =>
            {
                var manager = db.Database.Manager;
                var operate = manager.RenameView<Product>("TestProduct");

                Utility.CompareSql(db, operate, RenameViewTestSql);
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
        public void DropRelationTest()
        {
            Utility.CommitTest(CreateContext(), db =>
            {
                var manager = db.Database.Manager;
                var operate = manager.DropRelation((Order order) => order.Customer).ToArray();

                Utility.CompareSql(db, operate[0], DropRelationTestSql);
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

        [TestMethod]
        public void DropCompositeRelationTest()
        {
            Utility.CommitTest(CreateContext(), db =>
            {
                var manager = db.Database.Manager;
                var operate = manager.DropRelation((Order order) => order.Products).ToArray();

                Utility.CompareSql(db, operate[0], DropCompositeRelationTestSql);
                Utility.CompareSql(db, operate[1], DropCompositeRelationTestSql1);
            });
        }
#endif

        public OrderManageEntities CreateContext()
        {
            return new OrderManageEntities(Constants.ConnectionNameSimple);
        }
    }
}