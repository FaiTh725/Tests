using System.Linq.Expressions;
using Test.Dal.Persistences;
using Test.Domain.Entities;
using TestEntity = Test.Domain.Entities.Test;

namespace Test.Dal.ExpressionRewriters
{
    public class TestToMongoRewriter: ExpressionVisitor
    {
        private readonly ParameterExpression mongoParam = Expression.Parameter(typeof(MongoTest));

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (node.Type == typeof(TestEntity))
            {
                return mongoParam;
            }
            return base.VisitParameter(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            var rewrittenExpr = Visit(node.Expression);

            if (node.Expression is not null &&
                node.Expression.Type == typeof(TestEntity) &&
                rewrittenExpr.Type == typeof(MongoTest))
            {
                var memberName = node.Member.Name;

                var mongoProperty = typeof(MongoTest)
                    .GetProperty(memberName);
                if (mongoProperty is not null)
                {
                    return Expression.MakeMemberAccess(rewrittenExpr, mongoProperty);
                }
            }

            return base.VisitMember(node);
        }

        public Expression<Func<MongoTest, bool>> Rewrite(
            Expression<Func<TestEntity, bool>> original)
        {
            var newBody = Visit(original.Body);
            return Expression.Lambda<Func<MongoTest, bool>>(newBody, mongoParam);
        }
    }
}
