using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet.Actions;

namespace back.services
{
    public interface ICloundinaryService
    {
        Task<string> uploadImage(string base64);
    }
}