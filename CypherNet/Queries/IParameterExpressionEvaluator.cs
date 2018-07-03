using System.IO;
using Newtonsoft.Json;

namespace CypherNet.Queries
{
    #region

    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    using Dynamic;

    #endregion

    internal interface IArgumentEvaluator
    {
        object Evaluate(Expression argument, ParameterInfo paramInfo);
        bool CanEvaluate(Expression argument, ParameterInfo parameterInfo);
    }

    internal class ValueArgumentEvaluator : IArgumentEvaluator
    {
        public virtual object Evaluate(Expression argument, ParameterInfo paramInfo)
        {
            var val = ExpressionEvaluator.PartialEval(argument) as ConstantExpression;
            return val == null ? null : val.Value;
        }

        public bool CanEvaluate(Expression argument, ParameterInfo parameterInfo)
        {
            return true;
        }
    }

    internal class MemberNameArgumentEvaluator : IArgumentEvaluator
    {
        public object Evaluate(Expression argument, ParameterInfo paramInfo)
        {
            var member = argument as MemberExpression;
            return member.Member.Name;
        }

        public bool CanEvaluate(Expression argument, ParameterInfo parameterInfo)
        {
            return argument.NodeType == ExpressionType.MemberAccess;
        }
    }

    internal class StringWrapperArgumentEvaluator : ValueArgumentEvaluator
    {
        private static readonly Type[] WrappedTypes = new[] {typeof (string), typeof (char)};

        public override object Evaluate(Expression argument, ParameterInfo paramInfo)
        {
            var value = base.Evaluate(argument, paramInfo);
            if (value == null)
            {
                return "Null";
            }
            return WrapValue(value);
        }

        internal static object WrapValue(object value)
        {
            return WrappedTypes.Contains(value.GetType()) ? String.Format(@"""{0}""", value) : value;
        }
    }

    internal class JsonArgumentEvaluator : ValueArgumentEvaluator
    {
        public override object Evaluate(Expression argument, ParameterInfo paramInfo)
        {
            var value = base.Evaluate(argument, paramInfo);
            if (value == null)
            {
                return "";
            }

            JsonSerializer serializer = JsonSerializer.Create(Configuration.CypherJsonSerializerSettings.DefaultSerializerSettings?.Invoke());
            var stringWriter = new StringWriter();
            using (var writer = new JsonTextWriter(stringWriter))
            {
                writer.QuoteName = false;
                serializer.Serialize(writer, value);
            }

            return stringWriter.ToString();
        }
    }

    internal static class ArgumentEvaluatorFactory
    {
        internal static IArgumentEvaluator GetEvaluator(ParameterInfo paramInfo)
        {
            var att = paramInfo.GetCustomAttribute<ArgumentEvaluatorAttribute>();
            if (att == null)
            {
                return new ValueArgumentEvaluator();
            }
            return (IArgumentEvaluator) Activator.CreateInstance(att.ArgumentEvaluatorType);
        }
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    internal class ArgumentEvaluatorAttribute : Attribute
    {
        public ArgumentEvaluatorAttribute(Type argumentEvaluatorType)
        {
            ArgumentEvaluatorType = argumentEvaluatorType;
            if (!typeof (IArgumentEvaluator).IsAssignableFrom(argumentEvaluatorType))
            {
                throw new Exception("argumentEvaluatorType must implement " + typeof (IArgumentEvaluator).Name);
            }
        }

        public Type ArgumentEvaluatorType { get; private set; }
    }

    internal class MethodExpressionArgumentEvaluator
    {
        internal static object[] EvaluateArguments(MethodCallExpression exp)
        {
            var argsAndParams = exp.Arguments.Zip(exp.Method.GetParameters(), (a, p) => new {Arg = a, Param = p});
            return
                (argsAndParams.Select(ap => ArgumentEvaluatorFactory.GetEvaluator(ap.Param).Evaluate(ap.Arg, ap.Param)))
                    .ToArray();
        }
    }
}