using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace PersonalCodeWiki.AutchonBackend
{
    public class RuleEnginePredicateCompiler
    {
        private readonly ExpressionType[] _nestedOperators =
        {
            ExpressionType.And,
            ExpressionType.AndAlso,
            ExpressionType.Or,
            ExpressionType.OrElse
        };

        public bool PassesRules<T>(IList<Rule> rules, T toInspect)
        {
            return CompileRules<T>(rules).Invoke(toInspect);
        }

        public Func<T, bool> CompileRules<T>(IList<Rule> rules)
        {
            var parameter = Expression.Parameter(typeof(T));
            var expression = BuildNestedExpression<T>(rules, parameter, ExpressionType.And);
            return Expression.Lambda<Func<T, bool>>(expression, parameter).Compile();
        }

        private Expression GetExpressionForRule<T>(Rule rule, ParameterExpression parameter)
        {
            if (ExpressionType.TryParse(rule.Operator, out var nestedOperator)
                && _nestedOperators.Contains(nestedOperator)
                && rule.Rules != null
                && rule.Rules.Any())
            {
                return BuildNestedExpression<T>(rule.Rules, parameter, nestedOperator);
            }

            return BuildExpr<T>(rule, parameter);
        }

        private Expression BuildNestedExpression<T>(IList<Rule> rules, ParameterExpression parameter, ExpressionType operation)
        {
            var expressions = new List<Expression>();
            foreach (var rule in rules)
            {
                expressions.Add(GetExpressionForRule<T>(rule, parameter));
            }

            return BuildBinaryExpression(expressions, operation);
        }

        private Expression BuildBinaryExpression(IList<Expression> expressions, ExpressionType operationType)
        {
            Func<Expression, Expression, Expression> combiner = (left, right) => Expression.And(left, right);
            switch (operationType)
            {
                case ExpressionType.Or:
                    combiner = (left, right) => Expression.Or(left, right);
                    break;
                case ExpressionType.OrElse:
                    combiner = (left, right) => Expression.OrElse(left, right);
                    break;
                case ExpressionType.AndAlso:
                    combiner = (left, right) => Expression.AndAlso(left, right);
                    break;
            }

            if (expressions.Count == 1)
            {
                return expressions[0];
            }

            var expression = combiner(expressions[0], expressions[1]);
            for (var i = 2; i < expressions.Count; i++)
            {
                expression = combiner(expression, expressions[i]);
            }

            return expression;
        }

        private Expression BuildExpr<T>(Rule rule, ParameterExpression parameter)
        {
            Expression propertyExpression = string.IsNullOrEmpty(rule.MemberName)
                ? parameter
                : Expression.PropertyOrField(parameter, rule.MemberName);
            var propertyType = propertyExpression.Type;

            if (ExpressionType.TryParse(rule.Operator, out var binaryOperator))
            {
                return Expression.MakeBinary(binaryOperator, propertyExpression, Expression.Constant(Convert.ChangeType(rule.TargetValue, propertyType)));
            }

            if (rule.Operator == "IsMatch")
            {
                return Expression.Call(
                    typeof(Regex).GetMethod("IsMatch", new[] { typeof(string), typeof(string), typeof(RegexOptions) }),
                    propertyExpression,
                    Expression.Constant(rule.TargetValue, typeof(string)),
                    Expression.Constant(RegexOptions.IgnoreCase, typeof(RegexOptions)));
            }

            throw new NotSupportedException(rule.Operator);
        }
    }

    public class Rule
    {
        public string MemberName { get; set; }
        public string Operator { get; set; }
        public string TargetValue { get; set; }
        public List<Rule> Rules { get; set; } = new List<Rule>();
    }
}
