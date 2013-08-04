using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CypherNet.Serialization
{

    [AttributeUsage(AttributeTargets.Constructor)]
    internal class DeserializeUsingAttribute : Attribute
    {
    }
}
