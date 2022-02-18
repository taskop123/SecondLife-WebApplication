using SecondLife.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SecondLife.Service.Interface
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailMessage email);
    }
}
