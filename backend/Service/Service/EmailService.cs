using GoWhere_Api.Service.IService;
using System.Net;
using System.Net.Mail;

namespace GoWhere_Api.Service.Service
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendPaymentConfirmationEmailAsync(string toEmail, string userName, int orderId, double amount)
        {
            var subject = "🎉 Thanh toán thành công - GoWhere AI Chat";
            var body = GetPaymentConfirmationTemplate(userName, orderId, amount);
            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendPaymentCreatedEmailAsync(string toEmail, string userName, int orderId, double amount, string qrCodeUrl, string paymentContent)
        {
            var subject = "💳 Hướng dẫn thanh toán - GoWhere AI Chat";
            var body = GetPaymentInstructionTemplate(userName, orderId, amount, qrCodeUrl, paymentContent);
            await SendEmailAsync(toEmail, subject, body);
        }

        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpHost = _configuration["EmailSettings:SmtpHost"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            var senderPassword = _configuration["EmailSettings:SenderPassword"];
            var senderName = _configuration["EmailSettings:SenderName"] ?? "GoWhere";

            if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(senderEmail) || string.IsNullOrEmpty(senderPassword))
            {
                throw new InvalidOperationException("Email settings not configured properly.");
            }

            using var smtpClient = new SmtpClient(smtpHost, smtpPort)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(senderEmail, senderPassword)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail, senderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }

        private string GetPaymentConfirmationTemplate(string userName, int orderId, double amount)
        {
            return $@"
<!DOCTYPE html>
<html lang='vi'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Thanh toán thành công</title>
</head>
<body style='margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #F5F5F5;'>
    <table width='100%' cellpadding='0' cellspacing='0' style='background-color: #F5F5F5; padding: 20px;'>
        <tr>
            <td align='center'>
                <table width='600' cellpadding='0' cellspacing='0' style='background-color: #FFFFFF; border-radius: 10px; overflow: hidden; box-shadow: 0 4px 6px rgba(0,0,0,0.1);'>
                    <!-- Header -->
                    <tr>
                        <td style='background: linear-gradient(135deg, #00A6FF 0%, #1E90FF 100%); padding: 40px 30px; text-align: center;'>
                            <h1 style='margin: 0; color: #FFFFFF; font-size: 28px; font-weight: bold;'>
                                <span style='display: inline-block; background-color: #FFFFFF; color: #00A6FF; width: 50px; height: 50px; line-height: 50px; border-radius: 50%; margin-bottom: 10px; font-size: 24px;'>G</span>
                                <br>GoWhere
                            </h1>
                            <p style='margin: 15px 0 0 0; color: #FFFFFF; font-size: 16px; opacity: 0.9;'>Trợ lý du lịch thông minh</p>
                        </td>
                    </tr>
                    
                    <!-- Success Icon -->
                    <tr>
                        <td style='padding: 40px 30px 20px; text-align: center;'>
                            <div style='width: 80px; height: 80px; margin: 0 auto; background-color: #10B981; border-radius: 50%; display: flex; align-items: center; justify-content: center;'>
                                <span style='font-size: 50px; color: #FFFFFF; line-height: 80px;'>✓</span>
                            </div>
                            <h2 style='margin: 20px 0 10px; color: #0B2447; font-size: 24px;'>Thanh toán thành công!</h2>
                            <p style='margin: 0; color: #6B7280; font-size: 16px;'>Cảm ơn bạn đã sử dụng dịch vụ GoWhere AI Chat</p>
                        </td>
                    </tr>
                    
                    <!-- Order Details -->
                    <tr>
                        <td style='padding: 20px 30px;'>
                            <table width='100%' cellpadding='15' cellspacing='0' style='background-color: #F9FAFB; border-radius: 8px;'>
                                <tr>
                                    <td style='color: #6B7280; font-size: 14px;'>Khách hàng:</td>
                                    <td style='color: #0B2447; font-size: 14px; font-weight: bold; text-align: right;'>{userName}</td>
                                </tr>
                                <tr>
                                    <td style='color: #6B7280; font-size: 14px; border-top: 1px solid #E5E7EB; padding-top: 15px;'>Mã đơn hàng:</td>
                                    <td style='color: #0B2447; font-size: 14px; font-weight: bold; text-align: right; border-top: 1px solid #E5E7EB; padding-top: 15px;'>#{orderId}</td>
                                </tr>
                                <tr>
                                    <td style='color: #6B7280; font-size: 14px; border-top: 1px solid #E5E7EB; padding-top: 15px;'>Số tiền:</td>
                                    <td style='color: #00A6FF; font-size: 18px; font-weight: bold; text-align: right; border-top: 1px solid #E5E7EB; padding-top: 15px;'>{amount:N0} VND</td>
                                </tr>
                                <tr>
                                    <td style='color: #6B7280; font-size: 14px; border-top: 1px solid #E5E7EB; padding-top: 15px;'>Trạng thái:</td>
                                    <td style='text-align: right; border-top: 1px solid #E5E7EB; padding-top: 15px;'>
                                        <span style='background-color: #10B981; color: #FFFFFF; padding: 5px 15px; border-radius: 20px; font-size: 12px; font-weight: bold;'>ĐÃ THANH TOÁN</span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    
                    <!-- Benefits -->
                    <tr>
                        <td style='padding: 20px 30px;'>
                            <div style='background: linear-gradient(135deg, #00A6FF 0%, #1E90FF 100%); border-radius: 8px; padding: 20px; text-align: center;'>
                                <p style='margin: 0; color: #FFFFFF; font-size: 16px; font-weight: bold;'>🎉 Bạn đã được kích hoạt</p>
                                <p style='margin: 10px 0 0; color: #FFFFFF; font-size: 24px; font-weight: bold;'>SỬ DỤNG KHÔNG GIỚI HẠN</p>
                                <p style='margin: 10px 0 0; color: #FFFFFF; font-size: 14px; opacity: 0.9;'>Tạo lịch trình du lịch AI bất cứ lúc nào bạn muốn</p>
                            </div>
                        </td>
                    </tr>
                    
                    <!-- Features -->
                    <tr>
                        <td style='padding: 20px 30px;'>
                            <h3 style='margin: 0 0 15px; color: #0B2447; font-size: 18px;'>🌟 Quyền lợi của bạn:</h3>
                            <table width='100%' cellpadding='0' cellspacing='0'>
                                <tr>
                                    <td width='50%' style='padding: 10px;'>
                                        <div style='text-align: center; padding: 15px; background-color: #F9FAFB; border-radius: 8px;'>
                                            <div style='font-size: 32px; margin-bottom: 10px;'>🤖</div>
                                            <p style='margin: 0; color: #0B2447; font-size: 14px; font-weight: bold;'>AI Thông minh</p>
                                            <p style='margin: 5px 0 0; color: #6B7280; font-size: 12px;'>Lịch trình tối ưu</p>
                                        </div>
                                    </td>
                                    <td width='50%' style='padding: 10px;'>
                                        <div style='text-align: center; padding: 15px; background-color: #F9FAFB; border-radius: 8px;'>
                                            <div style='font-size: 32px; margin-bottom: 10px;'>∞</div>
                                            <p style='margin: 0; color: #0B2447; font-size: 14px; font-weight: bold;'>Không giới hạn</p>
                                            <p style='margin: 5px 0 0; color: #6B7280; font-size: 12px;'>Tạo thoải mái</p>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td width='50%' style='padding: 10px;'>
                                        <div style='text-align: center; padding: 15px; background-color: #F9FAFB; border-radius: 8px;'>
                                            <div style='font-size: 32px; margin-bottom: 10px;'>🗺️</div>
                                            <p style='margin: 0; color: #0B2447; font-size: 14px; font-weight: bold;'>63 Tỉnh thành</p>
                                            <p style='margin: 5px 0 0; color: #6B7280; font-size: 12px;'>Khám phá Việt Nam</p>
                                        </div>
                                    </td>
                                    <td width='50%' style='padding: 10px;'>
                                        <div style='text-align: center; padding: 15px; background-color: #F9FAFB; border-radius: 8px;'>
                                            <div style='font-size: 32px; margin-bottom: 10px;'>⚡</div>
                                            <p style='margin: 0; color: #0B2447; font-size: 14px; font-weight: bold;'>Siêu nhanh</p>
                                            <p style='margin: 5px 0 0; color: #6B7280; font-size: 12px;'>60 giây có lịch</p>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    
                    <!-- CTA Button -->
                    <tr>
                        <td style='padding: 20px 30px 40px; text-align: center;'>
                            <a href='https://gowhere.com' style='display: inline-block; background: linear-gradient(135deg, #00A6FF 0%, #1E90FF 100%); color: #FFFFFF; text-decoration: none; padding: 15px 40px; border-radius: 25px; font-size: 16px; font-weight: bold; box-shadow: 0 4px 6px rgba(0,166,255,0.3);'>
                                Bắt đầu tạo lịch trình ngay
                            </a>
                        </td>
                    </tr>
                    
                    <!-- Footer -->
                    <tr>
                        <td style='background-color: #F9FAFB; padding: 30px; text-align: center; border-top: 1px solid #E5E7EB;'>
                            <p style='margin: 0 0 10px; color: #6B7280; font-size: 12px;'>Email này được gửi từ hệ thống GoWhere</p>
                            <p style='margin: 0; color: #6B7280; font-size: 12px;'>© 2026 GoWhere. All rights reserved.</p>
                            <p style='margin: 15px 0 0; color: #00A6FF; font-size: 12px;'>
                                <a href='#' style='color: #00A6FF; text-decoration: none; margin: 0 10px;'>Hỗ trợ</a> | 
                                <a href='#' style='color: #00A6FF; text-decoration: none; margin: 0 10px;'>Điều khoản</a> | 
                                <a href='#' style='color: #00A6FF; text-decoration: none; margin: 0 10px;'>Chính sách</a>
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
        }

        private string GetPaymentInstructionTemplate(string userName, int orderId, double amount, string qrCodeUrl, string paymentContent)
        {
            return $@"
<!DOCTYPE html>
<html lang='vi'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Hướng dẫn thanh toán</title>
</head>
<body style='margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #F5F5F5;'>
    <table width='100%' cellpadding='0' cellspacing='0' style='background-color: #F5F5F5; padding: 20px;'>
        <tr>
            <td align='center'>
                <table width='600' cellpadding='0' cellspacing='0' style='background-color: #FFFFFF; border-radius: 10px; overflow: hidden; box-shadow: 0 4px 6px rgba(0,0,0,0.1);'>
                    <!-- Header -->
                    <tr>
                        <td style='background: linear-gradient(135deg, #00A6FF 0%, #1E90FF 100%); padding: 40px 30px; text-align: center;'>
                            <h1 style='margin: 0; color: #FFFFFF; font-size: 28px; font-weight: bold;'>
                                <span style='display: inline-block; background-color: #FFFFFF; color: #00A6FF; width: 50px; height: 50px; line-height: 50px; border-radius: 50%; margin-bottom: 10px; font-size: 24px;'>G</span>
                                <br>GoWhere
                            </h1>
                            <p style='margin: 15px 0 0 0; color: #FFFFFF; font-size: 16px; opacity: 0.9;'>Hướng dẫn thanh toán</p>
                        </td>
                    </tr>
                    
                    <!-- Greeting -->
                    <tr>
                        <td style='padding: 40px 30px 20px;'>
                            <h2 style='margin: 0 0 15px; color: #0B2447; font-size: 24px;'>Xin chào {userName}! 👋</h2>
                            <p style='margin: 0; color: #6B7280; font-size: 16px; line-height: 1.6;'>
                                Cảm ơn bạn đã chọn GoWhere AI Chat. Vui lòng thanh toán để kích hoạt quyền sử dụng không giới hạn.
                            </p>
                        </td>
                    </tr>
                    
                    <!-- Order Info -->
                    <tr>
                        <td style='padding: 20px 30px;'>
                            <table width='100%' cellpadding='15' cellspacing='0' style='background-color: #F9FAFB; border-radius: 8px;'>
                                <tr>
                                    <td style='color: #6B7280; font-size: 14px;'>Mã đơn hàng:</td>
                                    <td style='color: #0B2447; font-size: 14px; font-weight: bold; text-align: right;'>#{orderId}</td>
                                </tr>
                                <tr>
                                    <td style='color: #6B7280; font-size: 14px; border-top: 1px solid #E5E7EB; padding-top: 15px;'>Số tiền cần thanh toán:</td>
                                    <td style='color: #00A6FF; font-size: 20px; font-weight: bold; text-align: right; border-top: 1px solid #E5E7EB; padding-top: 15px;'>{amount:N0} VND</td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    
                    <!-- QR Code -->
                    <tr>
                        <td style='padding: 20px 30px; text-align: center;'>
                            <div style='background-color: #F9FAFB; border-radius: 8px; padding: 30px;'>
                                <h3 style='margin: 0 0 20px; color: #0B2447; font-size: 18px;'>Quét mã QR để thanh toán</h3>
                                <img src='{qrCodeUrl}' alt='QR Code' style='max-width: 300px; width: 100%; border: 3px solid #00A6FF; border-radius: 8px;' />
                                <p style='margin: 20px 0 0; color: #6B7280; font-size: 14px;'>Mở app ngân hàng và quét mã QR này</p>
                            </div>
                        </td>
                    </tr>
                    
                    <!-- Payment Instructions -->
                    <tr>
                        <td style='padding: 20px 30px;'>
                            <h3 style='margin: 0 0 15px; color: #0B2447; font-size: 18px;'>📝 Hướng dẫn chuyển khoản</h3>
                            <div style='background-color: #FEF3C7; border-left: 4px solid #F59E0B; padding: 15px; border-radius: 4px; margin-bottom: 15px;'>
                                <p style='margin: 0; color: #92400E; font-size: 14px;'><strong>⚠️ Quan trọng:</strong> Vui lòng ghi đúng nội dung chuyển khoản để hệ thống tự động xác nhận</p>
                            </div>
                            <table width='100%' cellpadding='12' cellspacing='0' style='background-color: #F9FAFB; border-radius: 8px;'>
                                <tr>
                                    <td style='color: #6B7280; font-size: 14px;'>Nội dung chuyển khoản:</td>
                                    <td style='text-align: right;'>
                                        <span style='background-color: #00A6FF; color: #FFFFFF; padding: 8px 15px; border-radius: 4px; font-size: 16px; font-weight: bold; font-family: monospace;'>{paymentContent}</span>
                                    </td>
                                </tr>
                                <tr>
                                    <td style='color: #6B7280; font-size: 14px; border-top: 1px solid #E5E7EB; padding-top: 12px;'>Số tiền:</td>
                                    <td style='color: #0B2447; font-size: 16px; font-weight: bold; text-align: right; border-top: 1px solid #E5E7EB; padding-top: 12px;'>{amount:N0} VND</td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    
                    <!-- Steps -->
                    <tr>
                        <td style='padding: 20px 30px;'>
                            <h3 style='margin: 0 0 15px; color: #0B2447; font-size: 18px;'>🔢 Các bước thực hiện</h3>
                            <table width='100%' cellpadding='0' cellspacing='0'>
                                <tr>
                                    <td style='padding: 10px 0;'>
                                        <div style='display: flex; align-items: flex-start;'>
                                            <span style='display: inline-block; width: 30px; height: 30px; background-color: #00A6FF; color: #FFFFFF; border-radius: 50%; text-align: center; line-height: 30px; font-weight: bold; margin-right: 15px; flex-shrink: 0;'>1</span>
                                            <p style='margin: 5px 0 0; color: #4B5563; font-size: 14px;'>Mở app ngân hàng của bạn</p>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td style='padding: 10px 0;'>
                                        <div style='display: flex; align-items: flex-start;'>
                                            <span style='display: inline-block; width: 30px; height: 30px; background-color: #00A6FF; color: #FFFFFF; border-radius: 50%; text-align: center; line-height: 30px; font-weight: bold; margin-right: 15px; flex-shrink: 0;'>2</span>
                                            <p style='margin: 5px 0 0; color: #4B5563; font-size: 14px;'>Chọn tính năng quét mã QR</p>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td style='padding: 10px 0;'>
                                        <div style='display: flex; align-items: flex-start;'>
                                            <span style='display: inline-block; width: 30px; height: 30px; background-color: #00A6FF; color: #FFFFFF; border-radius: 50%; text-align: center; line-height: 30px; font-weight: bold; margin-right: 15px; flex-shrink: 0;'>3</span>
                                            <p style='margin: 5px 0 0; color: #4B5563; font-size: 14px;'>Quét mã QR ở trên hoặc nhập thông tin chuyển khoản</p>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td style='padding: 10px 0;'>
                                        <div style='display: flex; align-items: flex-start;'>
                                            <span style='display: inline-block; width: 30px; height: 30px; background-color: #00A6FF; color: #FFFFFF; border-radius: 50%; text-align: center; line-height: 30px; font-weight: bold; margin-right: 15px; flex-shrink: 0;'>4</span>
                                            <p style='margin: 5px 0 0; color: #4B5563; font-size: 14px;'>Kiểm tra nội dung: <strong style='color: #00A6FF;'>{paymentContent}</strong></p>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td style='padding: 10px 0;'>
                                        <div style='display: flex; align-items: flex-start;'>
                                            <span style='display: inline-block; width: 30px; height: 30px; background-color: #00A6FF; color: #FFFFFF; border-radius: 50%; text-align: center; line-height: 30px; font-weight: bold; margin-right: 15px; flex-shrink: 0;'>5</span>
                                            <p style='margin: 5px 0 0; color: #4B5563; font-size: 14px;'>Xác nhận thanh toán</p>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    
                    <!-- Benefits -->
                    <tr>
                        <td style='padding: 20px 30px;'>
                            <div style='background: linear-gradient(135deg, #00A6FF 0%, #1E90FF 100%); border-radius: 8px; padding: 25px; text-align: center;'>
                                <p style='margin: 0; color: #FFFFFF; font-size: 18px; font-weight: bold;'>✨ Sau khi thanh toán, bạn sẽ nhận được:</p>
                                <ul style='list-style: none; padding: 0; margin: 15px 0 0;'>
                                    <li style='color: #FFFFFF; font-size: 14px; padding: 8px 0;'>✓ Sử dụng AI Chat không giới hạn</li>
                                    <li style='color: #FFFFFF; font-size: 14px; padding: 8px 0;'>✓ Tạo lịch trình cho 63 tỉnh thành</li>
                                    <li style='color: #FFFFFF; font-size: 14px; padding: 8px 0;'>✓ Lịch trình chi tiết trong 60 giây</li>
                                    <li style='color: #FFFFFF; font-size: 14px; padding: 8px 0;'>✓ Email xác nhận ngay lập tức</li>
                                </ul>
                            </div>
                        </td>
                    </tr>
                    
                    <!-- Support -->
                    <tr>
                        <td style='padding: 20px 30px 40px;'>
                            <div style='background-color: #F9FAFB; border-radius: 8px; padding: 20px; text-align: center;'>
                                <p style='margin: 0 0 10px; color: #0B2447; font-size: 14px;'>Cần hỗ trợ?</p>
                                <p style='margin: 0; color: #6B7280; font-size: 14px;'>
                                    Email: <a href='mailto:support@gowhere.com' style='color: #00A6FF; text-decoration: none;'>support@gowhere.com</a><br>
                                    Hotline: <a href='tel:0123456789' style='color: #00A6FF; text-decoration: none;'>0123 456 789</a>
                                </p>
                            </div>
                        </td>
                    </tr>
                    
                    <!-- Footer -->
                    <tr>
                        <td style='background-color: #F9FAFB; padding: 30px; text-align: center; border-top: 1px solid #E5E7EB;'>
                            <p style='margin: 0 0 10px; color: #6B7280; font-size: 12px;'>Email này được gửi từ hệ thống GoWhere</p>
                            <p style='margin: 0; color: #6B7280; font-size: 12px;'>© 2026 GoWhere. All rights reserved.</p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
        }
    }
}
