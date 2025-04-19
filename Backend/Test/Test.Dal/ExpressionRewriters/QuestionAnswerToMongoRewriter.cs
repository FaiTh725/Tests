using System.Linq.Expressions;
using Test.Dal.Persistences;
using Test.Domain.Entities;

namespace Test.Dal.ExpressionRewriters
{
    public class QuestionAnswerToMongoRewriter: ExpressionVisitor
    {
        private readonly ParameterExpression mongoParam = Expression.Parameter(typeof(MongoQuestionAnswer));

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if(node.Type == typeof(QuestionAnswer))
            {   
                return mongoParam;
            }

            return base.VisitParameter(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            var rewriteExpr = Visit(node.Expression);

            if(node.Expression is not null &&
                node.Expression.Type == typeof(QuestionAnswer) &&
                rewriteExpr.Type == typeof(MongoQuestionAnswer))
            {
                var memberName = node.Member.Name;

                var mongoProperty = typeof(MongoQuestionAnswer).GetProperty(memberName);
                if(mongoProperty is not null)
                { 
                    return Expression.MakeMemberAccess(rewriteExpr, mongoProperty);
                }
            }

            return base.VisitMember(node);
        }

        public Expression<Func<MongoQuestionAnswer, bool>> Rewrite(
            Expression<Func<QuestionAnswer, bool>> original)
        {
            var newBody = Visit(original.Body);
            return Expression.Lambda<Func<MongoQuestionAnswer, bool>>(newBody, mongoParam);
        }
    }
}
