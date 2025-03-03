using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Import.DB
{
    public class RequestsDG : Requests
    {
        public string Status { get; set; }
        public string Client { get; set; }
        public string Master { get; set; }
        public string Comment { get; set; }
    }
}
