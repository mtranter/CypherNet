namespace CypherNet.Serialization
{
    #region

    using System;
    using System.Collections.Generic;
    using Graph;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    #endregion

    internal class RestNodeConverter : CustomCreationConverter<Node>
    {
        public override bool CanWrite
        {
            get { return false; }
        }

        public override Node Create(Type objectType)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                                        JsonSerializer serializer)
        {
            var values = serializer.Deserialize<Dictionary<string, object>>(reader);
            return base.ReadJson(reader, objectType, existingValue, serializer);
        }
    }
}