using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RestLibrary.Models
{
    public class RestResponse
    {
        public bool IsSuccessful { get; }

        public HttpStatusCode StatusCode { get; }

        public string RawContent { get; }

        internal RestResponse(HttpResponseMessage response, string content)
        {
            IsSuccessful = response.IsSuccessStatusCode;
            StatusCode = response.StatusCode;
            RawContent = content;
        }
    }
}
