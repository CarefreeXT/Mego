using Caredev.Mego.DataAnnotations;
using Caredev.Mego.Resolve.ValueGenerates;
using System;
using System.Linq.Expressions;

namespace Caredev.Mego.Tests.Common
{
    public class GeneratedDateTimeAttribute : GeneratedExpressionAttribute
    {
        public GeneratedDateTimeAttribute(EGeneratedPurpose purpose = EGeneratedPurpose.Update)
            : base(purpose)
        {
            Expression = Expression.MakeMemberAccess(null, typeof(DateTime).GetProperty(nameof(DateTime.Now)));
        }
    }
}
