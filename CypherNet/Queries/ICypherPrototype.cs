namespace CypherNet.Queries
{
    #region

    using System;
    using Graph;

    #endregion

    public interface ICypherPrototype
    {
        Node Node { get; }
        Relationship Rel { get; }
    }
}