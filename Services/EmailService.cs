﻿

using Azure;
using Azure.Communication.Email;
using Azure.Messaging.ServiceBus;
using EmailProvider.Functions;
using EmailProvider.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EmailProvider.Services;

public class EmailService(EmailClient emailClient, ILogger<EmailSender> logger) : IEmailService
{
    private readonly EmailClient _emailClient = emailClient;
    private readonly ILogger<EmailSender> _logger = logger;



    public EmailRequest UnPackEmailRequest(ServiceBusReceivedMessage message)
    {
        try
        {

            var request = JsonConvert.DeserializeObject<EmailRequest>(message.Body.ToString());
            if (request != null)
            {
                return request;
            }

        }
        catch (Exception ex)
        {
            _logger.LogError($"ERROR : EmailSender.UnPackEmailRequest() :: {ex.Message}");
        }
        return null!;
    }


    public bool SendEmail(EmailRequest emailRequest)
    {
        try
        {
            var result = _ = _emailClient.Send(
               WaitUntil.Completed,

               senderAddress: Environment.GetEnvironmentVariable("SenderAddress"),
               recipientAddress: emailRequest.To,
               subject: emailRequest.Subject,
               htmlContent: emailRequest.HtmlBody,
               plainTextContent: emailRequest.PlainText);

            if (result.HasCompleted)
            {
                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"ERROR : EmailSender.SendEmailAsync() :: {ex.Message}");
        }
        return false;
    }



}
