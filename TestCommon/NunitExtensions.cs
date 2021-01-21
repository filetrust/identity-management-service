using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework.Constraints;

namespace TestCommon
{
    public static class NunitExtensions
    {
        public static EqualConstraint WithPropEqual(this ConstraintExpression expr, string propName, object expectedVal)
        {
            return expr.With.Property(propName).EqualTo(expectedVal);
        }
    }
}
