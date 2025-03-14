using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace BookHotel.Services.Mail
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string message);
    }

    public class EmailService: IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var smtpConfig = _configuration.GetSection("Smtp");
            var smtpClient = new SmtpClient(smtpConfig["Host"])
            {
                Port = int.Parse(smtpConfig["Port"]),
                Credentials = new NetworkCredential(smtpConfig["UserName"], smtpConfig["Password"]),
                EnableSsl = bool.Parse(smtpConfig["EnableSsl"]),
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpConfig["UserName"]),
                Subject = subject,
                IsBodyHtml = true,
                Body = $@"
                    <p>Xin chào,</p>
                    <p>Cảm ơn bạn đã đăng ký! Để có thể tạo tài khoản lưu trữ đầu tiên, vui lòng nhấp vào nút bên dưới để xác minh địa chỉ email của bạn.</p>
                    <a href='{message}' style='padding: 10px 20px; color: white; background-color: blue; text-decoration: none;'>Xác minh email</a>
                    <p>Nếu bạn không đăng ký, bạn không cần thực hiện thêm hành động nào nữa, địa chỉ email của bạn sẽ tự động bị xóa sau vài ngày.</p>
                    <p>Nếu bạn gặp vấn đề khi nhấp vào nút, vui lòng sao chép và dán URL sau vào trình duyệt của bạn: {message  }</p>
                    <p>Xin cảm ơn,</p>
                    <p>BookHotel Team</p>
                "
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
