

namespace CypherNet.Queries
{
    using System.Linq;
    using Dynamic;
    using Graph;

    static class GraphEntityExtensions
    {
        [ParseToCypherAttribute("{0}.{1}")]
        public static TProp Get<TProp>([ArgumentEvaluator(typeof(MemberNameArgumentEvaluator))] this IGraphEntity entity, string propName)
        {
            var dynamicEntity = entity as IDynamicMetaData;
            var prop = dynamicEntity.GetAllValues().Where(kvp => kvp.Key == propName);
            return (TProp)prop;
        }

        [ParseToCypherAttribute("{0}.{1} = {2}")]
        public static TProp Set<TProp>([ArgumentEvaluator(typeof(MemberNameArgumentEvaluator))] this IGraphEntity entity, string propName, [ArgumentEvaluator(typeof(StringWrapperArgumentEvaluator))] TProp value)
        {
            var dynamicEntity = entity as IDynamicMetaData;
            var prop = dynamicEntity.GetAllValues().Where(kvp => kvp.Key == propName);
            return (TProp)prop;
        }
    }
}
