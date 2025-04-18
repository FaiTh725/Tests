using System.Linq.Expressions;

namespace Test.Dal.Specifications
{
    public class ParameterReplacer : ExpressionVisitor
    {
        private readonly ParameterExpression parameter;

        public ParameterReplacer(ParameterExpression parameter)
        {
            this.parameter = parameter;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return parameter;
        }
    }
}
