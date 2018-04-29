﻿namespace Caredev.Mego.Tests.Core.Query.Simple
{
    using System.Linq;
#if ORACLE || FIREBIRD
    using Caredev.Mego.Tests.Models.Simple2;
#else
    using Caredev.Mego.Tests.Models.Simple; 
#endif
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    [TestClass, TestCategory(Constants.TestCategoryRootName + ".Query.Simple")]
    public partial class SqlQueryTest 
    {
        [TestMethod]
        public void SqlQueryValueTest()
        {
            using (var db = CreateContext())
            {
                var data = db.Database.SqlQuery<int>(SqlQueryValueTestSql);
                Assert.IsTrue(data.Any(a => a > 0));
            }
        }

        [TestMethod]
        public void SqlQueryCollectionTest()
        {
            using (var db = CreateContext())
            {
                var data = db.Database.SqlQuery<Product>(SqlQueryCollectionTestSql);
                Assert.IsTrue(data.Any(a => a.Id > 0));
            }
        }
        public OrderManageEntities CreateContext() => Constants.CreateSimpleContext();
    }
}
