using Newtonsoft.Json;
using RestLibrary.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestLibrary.Models
{
    public class ServiceError
    {
        public string Message { get; set; }

        public string ExceptionMessage { get; set; }

        public string ExceptionType { get; set; }

        public string StackTrace { get; set; }

        [JsonConverter(typeof(ModelStateConverter))]
        public IEnumerable<string> ModelState { get; set; }
    }
}
