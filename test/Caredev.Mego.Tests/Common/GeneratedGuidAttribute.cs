using Caredev.Mego.DataAnnotations;
using Caredev.Mego.Resolve.ValueGenerates;
using System;
using System.Linq.Expressions;
namespace Caredev.Mego.Tests.Common
{
    public class GeneratedGuidAttribute : GeneratedExpressionAttribute
    {
        public GeneratedGuidAttribute(EGeneratedPurpose purpose = EGeneratedPurpose.Insert)
            : base(purpose)
        {
            Expression = Expression.Call(null, typeof(Guid).GetMethod(nameof(Guid.NewGuid)));
        }
    }
}
