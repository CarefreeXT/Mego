using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Caredev.Mego.Common;
using System.Reflection;
using System.Collections;


namespace Caredev.Mego.Tests.TestBase
{
    [TestCategory(Constants.TestCategoryFoundation)]
    [TestClass]
    public class SupportMembersInitializationTest
    {
        [TestMethod]
        public void TypeInitializationQueryableTest()
        {
            InitializationTest(typeof(SupportMembers.Queryable));
        }

        [TestMethod]
        public void TypeInitializationEnumerableTest()
        {
            InitializationTest(typeof(SupportMembers.Enumerable));
        }

        [TestMethod]
        public void TypeInitializationStringTest()
        {
            InitializationTest(typeof(SupportMembers.String));
        }

        [TestMethod]
        public void TypeInitializationMathTest()
        {
            InitializationTest(typeof(SupportMembers.Math));
        }

        [TestMethod]
        public void TypeInitializationDateTimeTest()
        {
            InitializationTest(typeof(SupportMembers.DateTime));
        }


        private void InitializationTest(Type type)
        {
            foreach (var field in type.GetFields())
            {
                if (field.FieldType.IsArray)
                {
                    var members = field.GetValue(null) as IEnumerable;
                    Assert.IsNotNull(members);
                    foreach (var item in members)
                    {
                        var member = item as MemberInfo;
                        Assert.IsNotNull(member);
                        Assert.IsTrue(field.Name.StartsWith(member.Name));
                    }
                }
                else
                {
                    var member = field.GetValue(null) as MemberInfo;
                    Assert.IsNotNull(member);
                    Assert.IsTrue(field.Name.StartsWith(member.Name));
                }
            }
        }
    }
}
