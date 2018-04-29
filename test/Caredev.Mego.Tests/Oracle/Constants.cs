using Caredev.Mego.Resolve.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caredev.Mego.Tests
{
    internal partial class Constants
    {
        public const string TestCategoryRootName = "Oracle11";

        public static Models.Simple2.OrderManageEntities CreateSimpleContext(bool isinitial = false)
        {
            var db = new Models.Simple2.OrderManageEntities(Constants.ConnectionNameSimple);
            if (!isinitial)
            {
                db.Configuration.DatabaseFeature.DefaultSchema = "SIMPLE";
                db.Configuration.Metadata.Register(typeof(BooleanToInt32Converter));
                db.Configuration.Metadata.Register(typeof(GuidToByteArrayConverter));
            }
            return db;
        }

        public static Models.Inherit2.OrderManageEntities CreateInheritContext(bool isinitial = false)
        {
            var db = new Models.Inherit2.OrderManageEntities(Constants.ConnectionNameInherit);
            if (!isinitial)
            {
                db.Configuration.DatabaseFeature.DefaultSchema = "INHERIT";
            }
            return db;
        }
    }
}
