using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Exceptions
{
    public class NotFoundException(string message) : Exception(message)
    {
        
    }
}