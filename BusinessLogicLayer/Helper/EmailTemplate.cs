namespace BusinessLogicLayer.Helper;

public class EmailTemplate
{
    // Template for Sending the Reset Password Email
    public static string ResetPasswordEmail(string resetLink, string userName)
    {
        string body = $@"<body style='font-family: 'Segoe UI' , sans-serif; background-color: #f0f9f8; padding: 0; margin: 0; color: #333;'>

        <div style=' max-width: 600px; margin: 30px auto; background-color: #e4e4e48e; border-radius: 12px; overflow: hidden;
        box-shadow: 0 8px 24px rgba(0, 0, 0, 0.05);'>

        <div style=' background-color: rgb(54, 130, 133); color: white; padding: 40px 30px; text-align: center;'>
            <h1 style=' margin: 0; font-size: 24px;'>Item Order Management</h1>
        </div>

        <div style='padding: 30px; padding-top: 10px'>

            <p><strong style='display: flex; justify-content: center;font-size: 23px;'>Forgot Password</strong></p>

            <p><strong>Hello {10}</strong><span style='margin-left: 3px; '>,</span></p>

            <p>Please click on this link<a href={123}
                    style='color: #1a73e8; font-weight: bold; margin-left: 8px; margin-right: 6px;'>Reset Password
                </a>
                to reset your account password.</p>
            <p style='font-size: 16px; line-height: 1.6;'><strong style='color: rgb(255, 115, 0);'>Important
                    Note:</strong> For security reasons, the link will be used only
                once.
                If you did not request a password reset, please ignore this email or contact our support team
                immediately.
            </p>
        </div>

        <div style=' text-align: center; padding: 25px; font-size: 13px; color: #666; background-color: #d8d8d8;'>
            &copy; Item Order Management
        </div>
    </div>
    </body>
";
        body = body.Replace("123", resetLink);
        body = body.Replace("10", userName);
        return body;
    }
}
