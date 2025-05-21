using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.services.Email
{
    public interface IEmailService
    {
        public Task Sendmail(string ToEmail, string Subject, string Body, bool IsBodyHtml = false);
    }
}