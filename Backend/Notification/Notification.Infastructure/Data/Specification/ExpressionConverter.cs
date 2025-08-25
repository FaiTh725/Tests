using System.Linq.Expressions;

namespace Notification.Infrastructure.Data.Specification
{
    public class ExpressionConverter<TIn, TOut>: ExpressionVisitor
    {
        private readonly ParameterExpression mongoParam = Expression.Parameter(typeof(TOut));

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (node.Type == typeof(TIn))
            {
                return mongoParam;
            }

            return base.VisitParameter(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            var rewriteExpr = Visit(node.Expression);

            if (node.Expression is not null &&
                node.Expression.Type == typeof(TIn) &&
                rewriteExpr.Type == typeof(TOut))
            {
                var memberName = node.Member.Name;

                var mongoProperty = typeof(TOut).GetProperty(memberName);
                if (mongoProperty is not null)
                {
                    return Expression.MakeMemberAccess(rewriteExpr, mongoProperty);
                }
            }

            return base.VisitMember(node);
        }

        public Expression<Func<TOut, bool>> Rewrite(
            Expression<Func<TIn, bool>> original)
        {
            var newBody = Visit(original.Body);
            return Expression.Lambda<Func<TOut, bool>>(newBody, mongoParam);
        }

        public Expression<Func<TOut, object>> Rewrite(
            Expression<Func<TIn, object>> original)
        {
            var newBody = Visit(original.Body);

            if (newBody.Type.IsValueType)
            {
                newBody = Expression.Convert(newBody, typeof(object));
            }

            return Expression.Lambda<Func<TOut, object>>(newBody, mongoParam);
        }
    }
}
