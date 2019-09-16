using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Collectiblox.Model;
namespace Collectiblox.EditorExtensions
{
    public class AttributeFormatConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(AttributeFormat);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            AttributeFormat output = new AttributeFormat();
            JToken obj = JToken.Load(reader);
            output.type = serializer.Deserialize<string>(obj["type"].CreateReader());
            output.nameToType = new Dictionary<string, AttributeType>();
            JObject attributes = JObject.Load(obj["attributes"].CreateReader());
            foreach(JProperty property in attributes.Properties())
            {
                    output.nameToType.Add(property.Name, serializer.Deserialize<AttributeType>(property.Value.CreateReader()));
            }

            return output;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
