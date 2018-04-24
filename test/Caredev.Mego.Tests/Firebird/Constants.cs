using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caredev.Mego.Tests
{
    internal partial class Constants
    {
        public const string TestCategoryRootName = "Firebird25";

        public static Models.Simple2.OrderManageEntities CreateSimpleContext(bool isinitial = false)
        {
            return new Models.Simple2.OrderManageEntities(Constants.ConnectionNameSimple);
        }

        public static Models.Inherit2.OrderManageEntities CreateInheritContext(bool isinitial = false)
        {
            return new Models.Inherit2.OrderManageEntities(Constants.ConnectionNameInherit);
        }
    }
}
