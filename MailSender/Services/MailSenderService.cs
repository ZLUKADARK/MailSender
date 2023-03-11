using Newtonsoft.Json;
using Publisher.Dto;
using System.Net;
using System.Net.Mail;

public class MailSenderService : IMailSenderService 
{
    public async Task SendMessageAsync(string jsonMessage)
    {
        var message = JsonMessageConvert(jsonMessage);
        MailAddress from = new MailAddress("damha2@mail.ru", message.SenderName);
        MailAddress to = new MailAddress(message.Receiver.Address, message.Receiver.DisplayName);
        MailMessage mailMessage = new MailMessage(from, to);
        mailMessage.Subject = message.Subject;

        if (message.ContentHtml != null)
        {
            mailMessage.Body = message.ContentHtml;
            mailMessage.IsBodyHtml = true;
        }
        else
        {
            mailMessage.Body = message.ContentPlain;
            mailMessage.IsBodyHtml = false;
        }

        await SendAsync(mailMessage);        
    }

    private EmailMessage JsonMessageConvert(string jsonMessage) 
    {
        return JsonConvert.DeserializeObject<EmailMessage>(jsonMessage);
    }

    private async Task SendAsync(MailMessage message)
    {
        try
        {
            SmtpClient smtpClient = new SmtpClient("smtp.mail.ru", 25);
            smtpClient.Credentials = new NetworkCredential("damha2@mail.ru", "ZyPyiebZeXvVXtsqctEP");
            smtpClient.EnableSsl = true;
            await smtpClient.SendMailAsync(message);
            Console.WriteLine($"Доставлено | {Thread.CurrentThread.ManagedThreadId} | {message.To}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка отправки | Поток Id: {Thread.CurrentThread.ManagedThreadId} | {message.To} {message.Body}");
            Console.WriteLine($"Ошибка | Поток Id: {Thread.CurrentThread.ManagedThreadId} | {ex.Message}");
            throw new Exception();
        }
    }
}
