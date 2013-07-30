

namespace CypherNet.Configuration
{
    using System;
    using System.Linq;

    class ScalarTypes
    {
        private static readonly Type[] _all = { 
                                                typeof(bool), typeof(byte), 
                                                typeof(ushort), typeof(uint), typeof(ulong), 
                                                typeof(short), typeof(int), typeof(long),
                                                typeof(float), typeof(decimal), typeof(double),
                                                typeof(char), typeof(string), typeof(DateTimeOffset)
                                              };

        private static readonly Lazy<Type[]> _allAndNullables = new Lazy<Type[]>(AllAndNullable);

        internal static Type[] All {get { return _allAndNullables.Value; }}

        private static Type[] AllAndNullable()
        {
            return
                _all.Union(_all.Where(t => t.IsValueType).Select(s => typeof(Nullable<>).MakeGenericType(s))).ToArray();
        }
         
    }
}
