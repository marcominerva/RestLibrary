using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace RestLibrary.Converters
{
    internal class ModelStateConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var content = serializer.Deserialize(reader);
            var obj = new Dictionary<string, string[]>();
            var x = JsonConvert.DeserializeAnonymousType(content.ToString(), obj);

            return x.SelectMany(m => m.Value).ToList();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}