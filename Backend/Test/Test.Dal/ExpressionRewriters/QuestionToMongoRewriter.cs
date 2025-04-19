using System.Linq.Expressions;
using Test.Dal.Persistences;
using Test.Domain.Entities;

namespace Test.Dal.ExpressionRewriters
{
    public  class QuestionToMongoRewriter: ExpressionVisitor
    {
        private readonly ParameterExpression mongoParam = Expression.Parameter(typeof(MongoQuestion), "mq");

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (node.Type == typeof(Question))
            {
                return mongoParam;
            }
            return base.VisitParameter(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            var rewrittenExpr = Visit(node.Expression);

            if (node.Expression is not null &&
                node.Expression.Type == typeof(Question) &&
                rewrittenExpr.Type == typeof(MongoQuestion))
            {
                var memberName = node.Member.Name;

                var mongoProperty = typeof(MongoQuestion)
                    .GetProperty(memberName);
                if (mongoProperty is not null)
                {
                    return Expression.MakeMemberAccess(rewrittenExpr, mongoProperty);
                }
            }

            return base.VisitMember(node);
        }

        public Expression<Func<MongoQuestion, bool>> Rewrite(
            Expression<Func<Question, bool>> original)
        {
            var newBody = Visit(original.Body);
            return Expression.Lambda<Func<MongoQuestion, bool>>(newBody, mongoParam);
        }
    }
}
