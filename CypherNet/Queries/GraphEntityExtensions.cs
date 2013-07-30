

namespace CypherNet.Queries
{
    using System.Linq;
    using Dynamic;
    using Graph;

    static class GraphEntityExtensions
    {
        [ParseToCypherAttribute("{0}.{1}")]
        public static TProp Prop<TProp>([ArgumentEvaluator(typeof(MemberNameArgumentEvaluator))] this IGraphEntity entity, string propName)
        {
            var dynamicEntity = entity as IDynamicMetaData;
            var prop = dynamicEntity.GetAllValues().Where(kvp => kvp.Key == propName);
            return (TProp)prop;
        }
    }
}
