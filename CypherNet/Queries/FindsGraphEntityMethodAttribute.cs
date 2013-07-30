
namespace CypherNet.Queries
{
    #region

    using System;

    #endregion

    [AttributeUsage(AttributeTargets.Method)]
    public class ParseToCypherAttribute : Attribute
    {
        public ParseToCypherAttribute(string format)
        {
            Format = format;
        }

        public string Format { get; private set; }
    }
}