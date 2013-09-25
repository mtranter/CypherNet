namespace CypherNet.Queries
{
    #region

    using System.Linq;
    using Dynamic;
    using Graph;

    #endregion

    public static class GraphEntityExtensions
    {
        [ParseToCypher("{0}.{1}")]
        public static TProp Get<TProp>(
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] this IGraphEntity entity, string propName)
        {
            throw new ExpressionTreeOnlyUsageException();
        }

        [ParseToCypher("{0}.{1}")]
        public static object Get(
            [ArgumentEvaluator(typeof(MemberNameArgumentEvaluator))] this IGraphEntity entity, string propName)
        {
            throw new ExpressionTreeOnlyUsageException();
        }

        [ParseToCypher("{0}.{1} = {2}")]
        public static void Set<TProp>(
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] this IGraphEntity entity, string propName,
            [ArgumentEvaluator(typeof (StringWrapperArgumentEvaluator))] TProp value)
        {
            throw new ExpressionTreeOnlyUsageException();
        }
    }
}