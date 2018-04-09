using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestLibrary.Models
{
    public class AuthenticationResult
    {
        public bool Succeeded { get; internal set; }

        public AuthenticationError Error { get; internal set; }
    }
}
