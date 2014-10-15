namespace CypherNet.Queries
{
    using CypherNet.Graph;

    public interface IQueryContext<out TVariables>
    {
        TVariables Vars { get; }
    }

    public interface IReturnQueryContext<out TVariables> : IQueryContext<TVariables>, IEntityPropertyAccessor
    {
    }

    public interface IWhereQueryContext<out TVariables> : IMatchQueryContext<TVariables>, IEntityPropertyAccessor
    {

    }

    public interface IEntityPropertyAccessor
    {
        [ParseToCypher("{0}.{1}")]
        TProp Prop<TProp>(
            [ArgumentEvaluator(typeof (MemberNameArgumentEvaluator))] IGraphEntity entity,
            [ArgumentEvaluator(typeof (ValueArgumentEvaluator))] string property);

        [ParseToCypher("{0}.{1}")]
        object Prop(
            [ArgumentEvaluator(typeof(MemberNameArgumentEvaluator))] IGraphEntity entity,
            [ArgumentEvaluator(typeof(ValueArgumentEvaluator))] string property);

        [ParseToCypher("has({0}.{1})")]
        bool Has(
            [ArgumentEvaluator(typeof(MemberNameArgumentEvaluator))] IGraphEntity entity,
            [ArgumentEvaluator(typeof(ValueArgumentEvaluator))] string property);

        [ParseToCypher("{0}")]
        bool Clause([ArgumentEvaluator(typeof(ValueArgumentEvaluator))] string clause);
    }

    public interface IUpdateQueryContext<out TVariables> : IQueryContext<TVariables>
    {
        [ParseToCypher("{0}.{1} = {2}")]
        ISetResult Set<TEntity, TValue>(
            [ArgumentEvaluator(typeof(MemberNameArgumentEvaluator))]TEntity entity,
            [ArgumentEvaluator(typeof(ValueArgumentEvaluator))]string property,
            [ArgumentEvaluator(typeof(StringWrapperArgumentEvaluator))]TValue newValue) where TEntity : IGraphEntity;
    }

    public interface ISetResult
    {
    }
}