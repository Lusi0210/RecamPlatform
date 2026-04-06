using System;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Remp.Remp.Models.Interfaces.Services;

namespace Remp.Remp.Service;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string receiver, string title, string body)
    {
        var emailSettings = _configuration.GetSection("EmailSettings");

        string smtpServer = emailSettings["SmtpServer"]!;
        int smtpPort = int.Parse(emailSettings["SmtpPort"]!);
        string senderEmail = emailSettings["SenderEmail"]!;
        string senderPassword = emailSettings["SenderPassword"]!;
        string senderName = emailSettings["SenderName"]!;

        MailMessage mailMessage = new MailMessage
        {
            From = new MailAddress(senderEmail, senderName),
            Subject = title,
            Body = body,
            IsBodyHtml = true
        };

        mailMessage.To.Add(receiver);

        SmtpClient smtpClient = new SmtpClient(smtpServer)
        {
            Port = smtpPort,
            Credentials = new NetworkCredential(senderEmail, senderPassword),
            EnableSsl = true
        };

        await smtpClient.SendMailAsync(mailMessage);
    }
}
