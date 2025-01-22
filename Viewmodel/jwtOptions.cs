using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace back.Viewmodel
{
    public class jwtOptions
    {
         public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public string? SecretKey { get; set; }
    }
}