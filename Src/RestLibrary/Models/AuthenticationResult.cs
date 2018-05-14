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

        public AuthenticationResult(bool succeded = false, string error = null, string errorDescription = null)
        {
            Succeeded = succeded;
            if (!string.IsNullOrWhiteSpace(error) || !string.IsNullOrWhiteSpace(errorDescription))
            {
                Error = new AuthenticationError(error, errorDescription);
            }
        }
    }
}
