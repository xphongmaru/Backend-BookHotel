using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace BookHotel.Services.Mail
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string message);
        Task SendEmailForgotPassword(string toEmail, string subject, string message);

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
                    <p style='font-size: 24px'>Xin chào,</p>
                    <p style='font-size: 18px'>Cảm ơn bạn đã đăng ký! Để có thể tạo tài khoản lưu trữ đầu tiên, vui lòng xác minh địa chỉ email của bạn.</p>
                    <p style='font-size: 20px'>Mã OTP: {message}</p>
                    <p style='font-size: 18px'>Nếu bạn không đăng ký, bạn không cần thực hiện thêm hành động nào nữa, địa chỉ email của bạn sẽ tự động bị xóa sau vài ngày.</p>
                    <p style='font-size: 18px'>Xin cảm ơn,</p>
                    <p style='font-size: 18px'>BookHotel Team</p>
                "
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }

        public async Task SendEmailForgotPassword(string toEmail, string subject, string message)
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
                    <p style='font-size: 24px'>Xin chào,</p>
                    <p style='font-size: 18px'>Bạn đã yêu cầu quên mật khẩu, vui lòng xác minh để lấy lại mật khẩu của bạn.</p>
                    <p style='font-size: 20px'>Mã OTP: {message}</p>
                    <p style='font-size: 18px'>Nếu bạn thực hiện yêu cầu quên mật khẩu, bạn không cần thực hiện thêm hành động nào nữa, địa chỉ email của bạn sẽ tự động bị xóa sau vài ngày.</p>
                    <p style='font-size: 18px'>Xin cảm ơn,</p>
                    <p style='font-size: 18px'>BookHotel Team</p>
                "
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
