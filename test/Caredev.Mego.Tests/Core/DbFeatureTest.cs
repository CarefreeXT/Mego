namespace Caredev.Mego.Tests.Core
{
    using System;
    using System.Data.Common;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Caredev.Mego.Resolve.Generators;
    using Caredev.Mego.Resolve.Operates;
    using System.Text;

    [TestClass, TestCategory(Constants.TestCategoryRootName + ".Database")]
    public partial class DbFeatureTest
    {
        [TestMethod]
        public void MaxInsertRowCountTest()
        {
            using (var db = CreateSimpleContext())
            {
                db.NoCommit((con, tran) =>
                {
                    var com = con.CreateCommand();
                    com.Transaction = tran;
                    com.CommandText = MaxInsertRowCountTestSql("t0", MaxInsertRowCount);
                    com.ExecuteNonQuery();

                    com.CommandText = MaxInsertRowCountTestSql("t1", MaxInsertRowCount + 1);
                    try
                    {
                        com.ExecuteNonQuery();
                        if (HasMaxInsertRowCount)
                        {
                            Assert.Fail();
                        }
                    }
                    catch (Exception e)
                    {
                        Assert.IsInstanceOfType(e, typeof(DbException));
                    }
                });
            }
        }

        [TestMethod]
        public void MaxParameterCountTest()
        {
            Utility.CommitTest(CreateSimpleContext(), db =>
            {
                var con = db.Database.Connection;
                con.Open();
                var com = con.CreateCommand();
                StringBuilder builder = new StringBuilder();
                builder.Append("SELECT * FROM Customers WHERE Id IN (");
                for (int i = 0; i < MaxParameterCount; i++)
                {
                    com.AddParameter("p" + i.ToString(), i);
                    if (i == 0)
                    {
                        builder.Append("@p" + i.ToString());
                    }
                    else
                    {
                        builder.Append(",@p" + i.ToString());
                    }
                }
                builder.Append(");");

                com.CommandText = builder.ToString();
                com.ExecuteNonQuery();
                com.AddParameter("p" + MaxParameterCount.ToString(), MaxParameterCount);

                builder.Insert(builder.Length - 2, ",@p" + MaxParameterCount.ToString());
                com.CommandText = builder.ToString();
                try
                {
                    com.ExecuteNonQuery();
                    if (HasMaxParameterCount)
                    {
                        Assert.Fail();
                    }
                }
                catch (Exception e)
                {
                    Assert.IsInstanceOfType(e, typeof(DbException));
                }
            });
        }

        [TestMethod]
        public void InitialSimpleDatabaseTest()
        {
            using (var db = CreateSimpleContext())
            {
                CreateDatabaseIfNoExsits(db);
                db.InitialTable();
                db.InitialData();
            }
        }

        [TestMethod]
        public void InitialInheritDataBaseTest()
        {
            using (var db = CreateInheritContext())
            {
                CreateDatabaseIfNoExsits(db);
                db.InitialTable();
                db.InitialData();
            }
        }

        public Models.Simple.OrderManageEntities CreateSimpleContext()
        {
            return new Models.Simple.OrderManageEntities(Constants.ConnectionNameSimple);
        }

        public Models.Inherit.OrderManageEntities CreateInheritContext()
        {
            return new Models.Inherit.OrderManageEntities(Constants.ConnectionNameInherit);
        }
    }
}
