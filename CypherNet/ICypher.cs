
namespace CypherNet
{
    using System.Collections.Generic;

    public interface ICypher
    {
        IEnumerable<TResult> ExecuteQuery<TResult>(string cypher);
    }
}
