using System;

namespace Remp.Remp.Models.Interfaces.Services;

public interface IEmailService
{
    Task SendEmailAsync(string receiver, string title, string body);
}
