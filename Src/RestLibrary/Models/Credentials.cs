using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestLibrary.Models
{
    public class Credentials
    {
        public string UserName { get; }

        public string Password { get; }

        public Credentials(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }
    }
}
