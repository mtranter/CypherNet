namespace CypherNet.Serialization
{
    #region

    using System;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    #endregion

    public class IsoDateConverter :
        CustomCreationConverter<DateTime>
    {
        public override DateTime Create(Type objectType)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                                        JsonSerializer serializer)
        {
            var sdate = reader.Value.ToString();
            return DateTime.Parse(sdate).ToLocalTime();
        }
    }
}