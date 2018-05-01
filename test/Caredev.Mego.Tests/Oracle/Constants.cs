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

        public static Models.Simple.OrderManageEntities CreateSimpleContext(bool isinitial = false)
        {
            var db = new Models.Simple.OrderManageEntities(Constants.ConnectionNameSimple);
            if (!isinitial)
            {
                db.Configuration.DatabaseFeature.DefaultSchema = "SIMPLE";
                db.Configuration.Metadata.Register(typeof(BooleanToInt32Converter));
                db.Configuration.Metadata.Register(typeof(GuidToByteArrayConverter));
            }
            return db;
        }

        public static Models.Inherit.OrderManageEntities CreateInheritContext(bool isinitial = false)
        {
            var db = new Models.Inherit.OrderManageEntities(Constants.ConnectionNameInherit);
            if (!isinitial)
            {
                db.Configuration.DatabaseFeature.DefaultSchema = "INHERIT";
            }
            return db;
        }
    }
}
