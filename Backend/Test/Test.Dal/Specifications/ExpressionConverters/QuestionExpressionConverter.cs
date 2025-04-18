using MongoDB.Driver;
using System.Linq.Expressions;
using Test.Dal.Persistences;
using Test.Domain.Entities;

namespace Test.Dal.Specifications.ExpressionConverters
{
    public static class QuestionExpressionConverter
    {
        public static Expression<Func<MongoQuestion, bool>> Convert(
            Expression<Func<Question, bool>> expression)
        {
            var newParameter = Expression.Parameter(typeof(MongoQuestion), "mongoQuestion");
            var visitor = new ParameterReplacer(newParameter);

            var newBody = visitor.Visit(expression.Body);
            return Expression.Lambda<Func<MongoQuestion, bool>>(newBody, newParameter);
        }
    }
}
