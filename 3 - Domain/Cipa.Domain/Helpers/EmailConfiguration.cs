using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cipa.Domain.Helpers
{
    public class EmailConfiguration
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public bool EnabledSSL { get; set; }
        public bool UseDefaultCredentials { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
