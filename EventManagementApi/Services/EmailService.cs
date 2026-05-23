using System.Net;
using System.Net.Mail;

namespace EventManagementApi.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendResetCodeAsync(string toEmail, string code)
        {
            var smtpHost = _config["Gmail:SmtpHost"]!;
            var smtpPort = int.Parse(_config["Gmail:SmtpPort"]!);
            var senderEmail = _config["Gmail:SenderEmail"]!;
            var appPassword = _config["Gmail:AppPassword"]!;
            var senderName = _config["Gmail:SenderName"]!;

            var smtpClient = new SmtpClient(smtpHost)
            {
                Port = smtpPort,
                Credentials = new NetworkCredential(senderEmail, appPassword),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail, senderName),
                Subject = "Şifrə Sıfırlama Kodu",
                Body = $@"
                    <h2>Şifrə Sıfırlama</h2>
                    <p>Şifrənizi sıfırlamaq üçün aşağıdakı kodu istifadə edin:</p>
                    <h1 style='color: #4CAF50; letter-spacing: 5px;'>{code}</h1>
                    <p>Bu kod <strong>10 dəqiqə</strong> ərzində keçərlidir.</p>
                    <p>Əgər bu sorğunu siz etməmisinizsə, bu e-poçtu nəzərə almayın.</p>
                ",
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
