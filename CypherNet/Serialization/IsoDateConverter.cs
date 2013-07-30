using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;

namespace CypherNet.Serialization
{
    public class IsoDateConverter :
            CustomCreationConverter<DateTime>
    {
        public override DateTime Create(Type objectType)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            var sdate = reader.Value.ToString();
            return DateTime.Parse(sdate).ToLocalTime();
        }
    }
}
