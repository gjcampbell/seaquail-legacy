using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;

namespace SeaQuailDiagramTool.Infrastructure
{
    public class TableEntityExpressionAdapter
    {
        private ParameterExpression param;
        public string Adapt<T>(Expression<Func<T, bool>> expression)
        {
            param = expression.Parameters.First();
            return Eval(expression.Body);
        }

        private string Eval(Expression node)
            => node switch
            {
                MemberExpression me => EvalMember(me),
                BinaryExpression be => EvalBinary(be),
                UnaryExpression ue => EvalUnary(ue),
                ConstantExpression ce => EvalConstant(ce),
                _ => throw new NotImplementedException($"Some other kind of expression {node.GetType()}")
            };

        private string EvalMember(MemberExpression me)
        {
            if (me.Expression == param)
            {
                return me.Member.Name;
            }            
            else if (me.Member is FieldInfo)
            {
                return CreateLiteral(GetValue(me));
            }
            throw new NotImplementedException($"What kind of member expression is this? {me}");
        }

        private object GetValue(MemberExpression me)
        {
            if (me.Member is FieldInfo fi)
            {
                if (me.Expression is ConstantExpression ce)
                {
                    return fi.GetValue(ce.Value);
                }
                else if (me.Expression is MemberExpression me2)
                {
                    return fi.GetValue(GetValue(me2));
                }
            }
            throw new NotImplementedException($"Not sure about this member expression {me}");
        }

        private string EvalBinary(BinaryExpression be)
        {
            var left = Eval(be.Left);
            var right = Eval(be.Right);
            return be.NodeType switch
            {
                ExpressionType.Equal => CreateBinary(left, right, "eq"),
                ExpressionType.NotEqual => CreateBinary(left, right, "ne"),
                ExpressionType.GreaterThan => CreateBinary(left, right, "gt"),
                ExpressionType.GreaterThanOrEqual => CreateBinary(left, right, "gte"),
                ExpressionType.LessThan => CreateBinary(left, right, "lt"),
                ExpressionType.LessThanOrEqual => CreateBinary(left, right, "lte"),
                ExpressionType.AndAlso => CreateBinary(left, right, "and", true),
                ExpressionType.OrElse => CreateBinary(left, right, "or", true),
                _ => throw new NotImplementedException($"I don't support {be.NodeType} yet")
            };
        }

        private string CreateBinary(string left, string right, string comp, bool paren = false)
        {
            var result = $"{left} {comp} {right}";
            return paren ? $"({result})" : result;
        }

        private string EvalConstant(ConstantExpression ce)
        {
            return CreateLiteral(ce.Value);
        }

        private string CreateLiteral(object? value)
        {
            if (value == null) return "''";
            return value switch
            {
                Guid g => $"guid'{g}'",
                string vs => $"'{vs.Replace("'", "''")}'",
                DateTime d => JsonSerializer.Serialize(d),
                DateTimeOffset d => JsonSerializer.Serialize(d),
                bool b => b.ToString().ToLower(),
                _ => value.ToString()
            };
        }

        private string EvalUnary(UnaryExpression ue)
        {
            if (ue.NodeType == ExpressionType.Convert)
            {
                return Eval(ue.Operand);
            }
            throw new NotImplementedException($"Not sure what to do with this {ue.NodeType}");
        }
    }
}
