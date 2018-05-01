using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caredev.Mego.Tests
{
    internal partial class Constants
    {
#if SQLSERVER2012
        public const string TestCategoryRootName = "SqlServer2012";
#else
        public const string TestCategoryRootName = "SqlServer2005";
#endif

        internal static Models.Simple.OrderManageEntities CreateSimpleContext(bool isinitial = false)
        {
            return new Models.Simple.OrderManageEntities(Constants.ConnectionNameSimple);
        }

        internal static Models.Inherit.OrderManageEntities CreateInheritContext(bool isinitial = false)
        {
            return new Models.Inherit.OrderManageEntities(Constants.ConnectionNameInherit);
        }
    }
}