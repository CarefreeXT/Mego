using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caredev.Mego.Tests
{
    internal partial class Constants
    {
        public const string TestCategoryRootName = "Excel2003";

        public static Models.Simple.OrderManageEntities CreateSimpleContext(bool isinitial = false)
        {
            return new Models.Simple.OrderManageEntities(Constants.ConnectionNameSimple);
        }

        public static Models.Inherit.OrderManageEntities CreateInheritContext(bool isinitial = false)
        {
            return new Models.Inherit.OrderManageEntities(Constants.ConnectionNameInherit);
        }
    }
}