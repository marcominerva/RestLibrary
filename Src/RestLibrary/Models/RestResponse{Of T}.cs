using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RestLibrary.Models
{
    public class RestResponse<T> : RestResponse
    {
        public T Content { get; } = default(T);

        internal RestResponse(HttpResponseMessage response, string content)
            : base(response, content)
        {
            if (response.IsSuccessStatusCode)
            {
                if (typeof(T) == typeof(string))
                {
                    Content = (T)Convert.ChangeType(content, typeof(string));
                }
                else
                {
                    Content = JsonConvert.DeserializeObject<T>(content);
                }
            }
        }
    }
}
