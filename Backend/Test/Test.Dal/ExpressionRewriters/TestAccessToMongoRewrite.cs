using System.Linq.Expressions;
using Test.Dal.Persistences;
using Test.Domain.Entities;

namespace Test.Dal.ExpressionRewriters
{
    public class TestAccessToMongoRewrite : ExpressionVisitor
    {
        private readonly ParameterExpression mongoParam = Expression.Parameter(typeof(MongoTestAccess));

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (node.Type == typeof(TestAccess))
            {
                return mongoParam;
            }
            return base.VisitParameter(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            var rewrittenExpr = Visit(node.Expression);

            if (node.Expression is not null &&
                node.Expression.Type == typeof(TestAccess) &&
                rewrittenExpr.Type == typeof(MongoTestAccess))
            {
                var memberName = node.Member.Name;

                var mongoProperty = typeof(MongoTestAccess)
                    .GetProperty(memberName);
                if (mongoProperty is not null)
                {
                    return Expression.MakeMemberAccess(rewrittenExpr, mongoProperty);
                }
            }

            return base.VisitMember(node);
        }

        public Expression<Func<MongoTestAccess, bool>> Rewrite(
            Expression<Func<TestAccess, bool>> original)
        {
            var newBody = Visit(original.Body);
            return Expression.Lambda<Func<MongoTestAccess, bool>>(newBody, mongoParam);
        }
    }
}
