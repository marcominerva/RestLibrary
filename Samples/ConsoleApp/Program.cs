using RestLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new RestClient("http://localhost:37773");
            client.OAuthLoginAsync("demo", "password").Wait();
        }
    }
}
