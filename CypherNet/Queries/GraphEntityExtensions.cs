namespace CypherNet.Queries
{
    #region

    using System.Linq;
    using Dynamic;
    using Graph;

    #endregion

    internal static class GraphEntityExtensions
    {
        [ParseToCypher("{0}.{1}")]
        public static TProp Get<TProp>(
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] this IGraphEntity entity, string propName)
        {
            var dynamicEntity = entity as IDynamicMetaData;
            var prop = dynamicEntity.GetAllValues().First(kvp => kvp.Key == propName).Value;
            return (TProp) prop;
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