using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using dotenv.net;
namespace BackEnd.services
{
    public class CloudinaryService:ICloundinaryService
    {
        private readonly Cloudinary cloudinary;
        public CloudinaryService()
        {
            DotEnv.Load(options: new DotEnvOptions(probeForEnv: true));
            cloudinary = new Cloudinary(Environment.GetEnvironmentVariable("CLOUDINARY_URL"));
            cloudinary.Api.Secure = true;
        }

        public async Task<string> uploadImage(string base64)
        {
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(base64),
                UseFilename = true,
                UniqueFilename = false,
                Overwrite = true
            };
            var uploadResult = await cloudinary.UploadAsync(uploadParams);
            
            return uploadResult.Url.ToString();
        }
    }
}