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
    }
}
