using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestLibrary.Models
{
    public class AuthenticationError
    {
        public string Message { get; }

        public string Description { get; }

        public AuthenticationError(string message, string description)
        {
            Message = message;
            Description = description;
        }
    }
}
