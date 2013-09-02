namespace CypherNet.Queries
{
    internal class CypherQueryParameter
    {
        internal CypherQueryParameter(string name, string values)
        {
            Name = name;
            Values = values;
        }

        public string Name { get; private set; }
        public string Values { get; private set; }
    }
}