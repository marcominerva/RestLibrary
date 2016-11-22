using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestLibrary.Extensions
{
    internal static class JsonExtensions
    {
        private static JsonSerializerSettings settings;

        static JsonExtensions()
        {
            settings = new JsonSerializerSettings();

            //settings.Formatting = Formatting.Indented;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            settings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            settings.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat;
            settings.Converters.Add(new StringEnumConverter());
        }

        public static string ToJson(this object value)
        {
            var serialized = JsonConvert.SerializeObject(value, settings);
            return serialized;
        }
    }
}
