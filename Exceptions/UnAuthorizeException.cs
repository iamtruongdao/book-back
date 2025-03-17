using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Exceptions
{
    public class UnAuthorizeException(string message) : Exception(message)
    {
        
    }
}