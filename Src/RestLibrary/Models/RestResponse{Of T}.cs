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
        public T Content { get; internal set; }

        internal RestResponse(HttpResponseMessage response, T content = default(T))
            : base(response)
        {
            Content = content;
        }
    }
}
