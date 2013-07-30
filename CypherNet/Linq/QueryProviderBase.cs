namespace CypherNet.Linq
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    public class CypherQueryProvider : IQueryProvider
    {
        protected CypherQueryProvider()
        {
        }

        IQueryable<TResult> IQueryProvider.CreateQuery<TResult>(Expression expression)
        {
            return new CypherQuery<TResult>(this, expression);
        }

        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            Type elementType = TypeSystem.GetElementType(expression.Type);
            try
            {
                return (IQueryable)Activator.CreateInstance(typeof(CypherQuery<>).MakeGenericType(elementType), new object[] { this, expression });
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        TRetval IQueryProvider.Execute<TRetval>(Expression expression)
        {
            return (TRetval)this.Execute(expression);
        }

        object IQueryProvider.Execute(Expression expression)
        {
            return this.Execute(expression);
        }

        public string GetQueryText(Expression expression)
        {
            throw new NotImplementedException();
        }

        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }
    }
}