using System.Linq.Expressions;
using System.Reflection;
using static System.Linq.Expressions.Expression;
using System.Collections.ObjectModel;

namespace POEMgr.Repository.CommonQuery
{
    public class CommonQueryBase
    {
        public enum Operators
        {
            Contains = 0,
            Equal = 1,
            GreaterThan = 2,
            GreaterThanOrEqual = 3,
            LessThan = 4,
            LessThanOrEqual = 5,
            None = 6,
            StartWith = 7,
            EndWidth = 8,
            Range = 9,
            NotEqual = 10
        }
        public enum Condition
        {
            AndAlso = 0,
            OrElse = 1
        }
        public string Name { get; set; }
        public Operators Operator { get; set; }
        public object Value { get; set; }
        public object ValueMin { get; set; }
        public object ValueMax { get; set; }
    }
    public class CustomQueryCollection : Collection<CommonQueryBase>
    {
        public List<Type> TypeList { get; set; } = new List<Type>();
        public Expression<Func<T, bool>> AsExpression<T>(CommonQueryBase.Condition? condition = CommonQueryBase.Condition.AndAlso) where T : class
        {
            Type targetType = typeof(T);
            TypeInfo typeInfo = targetType.GetTypeInfo();
            var parameter = Expression.Parameter(targetType, "m");
            Expression expression = null;
            Func<Expression, Expression, Expression> Append = (exp1, exp2) =>
            {
                if (exp1 == null)
                {
                    return exp2;
                }
                return (condition ?? CommonQueryBase.Condition.OrElse) == CommonQueryBase.Condition.OrElse ? Expression.OrElse(exp1, exp2) : Expression.AndAlso(exp1, exp2);
            };
            foreach (var item in this)
            {
                var property = typeInfo.GetProperty(item.Name);
                if (property == null ||
                    !property.CanRead ||
                    (item.Operator != CommonQueryBase.Operators.Range && item.Value == null) ||
                    (item.Operator == CommonQueryBase.Operators.Range && item.ValueMin == null && item.ValueMax == null))
                {
                    continue;
                }
                Type realType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                if (item.Value != null)
                {
                    bool enumFlag = true;
                    foreach (var enumType in TypeList)
                    {
                        if (realType == enumType)
                        {
                            item.Value = Enum.Parse(enumType, item.Value.ToString());
                            enumFlag = false;
                            break;
                        }
                    }
                    if (enumFlag) item.Value = System.Convert.ChangeType(item.Value, realType);
                }
                Expression<Func<object>> valueLamba = () => item.Value;
                switch (item.Operator)
                {
                    case CommonQueryBase.Operators.Equal:
                        {
                            expression = Append(expression, Expression.Equal(Expression.Property(parameter, item.Name),
                                Convert(valueLamba.Body, property.PropertyType)));
                            break;
                        }
                    case CommonQueryBase.Operators.NotEqual:
                        {
                            expression = Append(expression, Expression.NotEqual(Expression.Property(parameter, item.Name),
                                Convert(valueLamba.Body, property.PropertyType)));
                            break;
                        }
                    case CommonQueryBase.Operators.GreaterThan:
                        {
                            expression = Append(expression, Expression.GreaterThan(Expression.Property(parameter, item.Name),
                                Expression.Convert(valueLamba.Body, property.PropertyType)));
                            break;
                        }
                    case CommonQueryBase.Operators.GreaterThanOrEqual:
                        {
                            expression = Append(expression, Expression.GreaterThanOrEqual(Expression.Property(parameter, item.Name),
                                Expression.Convert(valueLamba.Body, property.PropertyType)));
                            break;
                        }
                    case CommonQueryBase.Operators.LessThan:
                        {
                            expression = Append(expression, Expression.LessThan(Expression.Property(parameter, item.Name),
                                Expression.Convert(valueLamba.Body, property.PropertyType)));
                            break;
                        }
                    case CommonQueryBase.Operators.LessThanOrEqual:
                        {
                            expression = Append(expression, Expression.LessThanOrEqual(Expression.Property(parameter, item.Name),
                                Expression.Convert(valueLamba.Body, property.PropertyType)));
                            break;
                        }
                    case CommonQueryBase.Operators.Contains:
                        {
                            var nullCheck = Expression.Not(Expression.Call(typeof(string), "IsNullOrEmpty", null, Expression.Property(parameter, item.Name)));
                            var contains = Expression.Call(Expression.Property(parameter, item.Name), "Contains", null,
                                Expression.Convert(valueLamba.Body, property.PropertyType));
                            expression = Append(expression, Expression.AndAlso(nullCheck, contains));
                            break;
                        }
                    case CommonQueryBase.Operators.StartWith:
                        {
                            var nullCheck = Expression.Not(Expression.Call(typeof(string), "IsNullOrEmpty", null, Expression.Property(parameter, item.Name)));
                            var startsWith = Expression.Call(Expression.Property(parameter, item.Name), "StartsWith", null,
                                Expression.Convert(valueLamba.Body, property.PropertyType));
                            expression = Append(expression, Expression.AndAlso(nullCheck, startsWith));
                            break;
                        }
                    case CommonQueryBase.Operators.EndWidth:
                        {
                            var nullCheck = Expression.Not(Expression.Call(typeof(string), "IsNullOrEmpty", null, Expression.Property(parameter, item.Name)));
                            var endsWith = Expression.Call(Expression.Property(parameter, item.Name), "EndsWith", null,
                                Expression.Convert(valueLamba.Body, property.PropertyType));
                            expression = Append(expression, Expression.AndAlso(nullCheck, endsWith));
                            break;
                        }
                    case CommonQueryBase.Operators.Range:
                        {
                            Expression minExp = null, maxExp = null;
                            if (item.ValueMin != null)
                            {
                                var minValue = System.Convert.ChangeType(item.ValueMin, realType);
                                Expression<Func<object>> minValueLamda = () => minValue;
                                minExp = Expression.GreaterThanOrEqual(Expression.Property(parameter, item.Name), Expression.Convert(minValueLamda.Body, property.PropertyType));
                            }
                            if (item.ValueMax != null)
                            {
                                var maxValue = System.Convert.ChangeType(item.ValueMax, realType);
                                Expression<Func<object>> maxValueLamda = () => maxValue;
                                maxExp = Expression.LessThanOrEqual(Expression.Property(parameter, item.Name), Expression.Convert(maxValueLamda.Body, property.PropertyType));
                            }

                            if (minExp != null && maxExp != null)
                            {
                                expression = Append(expression, Expression.AndAlso(minExp, maxExp));
                            }
                            else if (minExp != null)
                            {
                                expression = Append(expression, minExp);
                            }
                            else if (maxExp != null)
                            {
                                expression = Append(expression, maxExp);
                            }

                            break;
                        }
                }
            }
            if (expression == null)
            {
                return null;
            }
            return ((Expression<Func<T, bool>>)Expression.Lambda(expression, parameter));
        }
    }
}