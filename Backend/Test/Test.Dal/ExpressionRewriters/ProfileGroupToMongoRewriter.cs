using System.Linq.Expressions;
using Test.Dal.Persistences;
using Test.Domain.Entities;

namespace Test.Dal.ExpressionRewriters
{
    public class ProfileGroupToMongoRewriter : ExpressionVisitor
    {
        private readonly ParameterExpression mongoParam = Expression.Parameter(typeof(MongoProfileGroup));

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (node.Type == typeof(ProfileGroup))
            {
                return mongoParam;
            }

            return base.VisitParameter(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            var rewriteExpr = Visit(node.Expression);

            if (node.Expression is not null &&
                node.Expression.Type == typeof(ProfileGroup) &&
                rewriteExpr.Type == typeof(MongoProfileGroup))
            {
                var memberName = node.Member.Name;

                var mongoProperty = typeof(MongoProfileGroup).GetProperty(memberName);
                if (mongoProperty is not null)
                {
                    return Expression.MakeMemberAccess(rewriteExpr, mongoProperty);
                }
            }

            return base.VisitMember(node);
        }

        public Expression<Func<MongoProfileGroup, bool>> Rewrite(
            Expression<Func<ProfileGroup, bool>> original)
        {
            var newBody = Visit(original.Body);
            return Expression.Lambda<Func<MongoProfileGroup, bool>>(newBody, mongoParam);
        }
    }
}
