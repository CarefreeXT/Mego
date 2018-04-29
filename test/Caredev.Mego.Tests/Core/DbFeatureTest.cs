namespace Caredev.Mego.Tests.Core
{
    using System;
    using System.Data.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
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
                if (con.State != System.Data.ConnectionState.Open)
                {
                    con.Open();
                }
                var com = con.CreateCommand();
                StringBuilder builder = new StringBuilder();
                builder.Append(MaxParameterCountTestSql);
                builder.Append("(");
                for (int i = 0; i < MaxParameterCount; i++)
                {
                    com.AddParameter("p" + i.ToString(), i);
                    if (i == 0)
                    {
                        builder.Append(ParameterPrefix + i.ToString());
                    }
                    else
                    {
                        builder.Append("," + ParameterPrefix + i.ToString());
                    }
                }
                builder.Append(")");

                com.CommandText = builder.ToString();
                com.ExecuteNonQuery();
                com.AddParameter("p" + MaxParameterCount.ToString(), MaxParameterCount);

                builder.Insert(builder.Length - 2, "," + ParameterPrefix + MaxParameterCount.ToString());
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
            using (var db = Constants.CreateSimpleContext(true))
            {
                CreateDatabaseIfNoExsits(db);
            }
            using (var db = CreateSimpleContext())
            {
                db.InitialTable();
                db.InitialData();
            }
        }

#if MYSQL || SQLSERVER
        [TestMethod]
        public void InitialInheritDataBaseTest()
        {
            using (var db = Constants.CreateInheritContext(true))
            {
                CreateDatabaseIfNoExsits(db);
            }
            using (var db = CreateInheritContext())
            {
                db.InitialTable();
                db.InitialData();
            }
        } 
#endif

#if ORACLE || FIREBIRD
        public Models.Simple2.OrderManageEntities CreateSimpleContext() => Constants.CreateSimpleContext();

        public Models.Inherit2.OrderManageEntities CreateInheritContext() => Constants.CreateInheritContext();
#else
        public Models.Simple.OrderManageEntities CreateSimpleContext() => Constants.CreateSimpleContext();

        public Models.Inherit.OrderManageEntities CreateInheritContext() => Constants.CreateInheritContext();
#endif
    }
}
