using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CypherNet.Queries
{
    using System.Linq.Expressions;

    public static class ExpressionExtensions
    {
        public static string ToCypherString(this ConstantExpression expression)
        {
            return WrapConstant(expression.Value);
        }

        private static string WrapConstant(object value)
        {
            if (value == null)
            {
                return "null";
            }
            var val = "";
            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.Boolean:
                    val = (((bool)value) ? "true" : "false");
                    break;
                case TypeCode.String:
                    val = string.Format("'{0}'", value);
                    break;
                case TypeCode.Object:
                    throw new NotSupportedException(string.Format("The constant for '{0}' is not supported", value));
                default:
                    val = value.ToString();
                    break;
            }
            return val;
        }
    }
}
