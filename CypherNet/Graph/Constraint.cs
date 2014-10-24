namespace CypherNet.Graph
{
    public class Constraint
    {
        internal Constraint(string label, string[] propertyKeys)
        {
            this.Label = label;
            this.PropertyKeys = propertyKeys;
        }

        public string Label { get; private set; }

        public string[] PropertyKeys { get; private set; }
    }
}
