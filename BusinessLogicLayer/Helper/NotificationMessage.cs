namespace BusinessLogicLayer.Helper;

public class NotificationMessage
{
    // Success Messages
    public const string LoginSuccess = "Logged In Successfully.";
    public const string LogoutSuccess = "Logged Out successfully .";
    public const string RegistrationSuccess = "Registered Successfully.";
    public const string CreateSuccess = "{0} added successfully.";
    public const string UpdateSuccess = "{0} updated successfully.";
    public const string DeleteSuccess = "{0} deleted successfully.";
    public const string EmailSentSuccessfully = "Email has been sent successfully!";
    public const string PasswordChanged = "Your password has been changed successfully.";

    // Failure Messages
    public const string RegistrationFailed = "Registration failed. Email already exists.";
    public const string LogoutFailed = "Logout failed. Please try again.";
    public const string InvalidCredentials = "Invalid credentials. Please try again.";
    public const string TokenExpired = "Invalid or expired token.";
    public const string AlreadyExists = "{0} already exists.";
    public const string DoesNotExist = "{0} does not exist.";
    public const string CreateFailure = "Failed to add {0}. Please try again.";
    public const string UpdateFailure = "Failed to update {0}. Please try again.";
    public const string DeleteFailure = "Failed to delete {0}. Please try again.";
    public const string ImageFormat = "Image format is not supported. Please upload a valid image file.";
    public const string EmailSendingFailed = "Failed to send the email. Please try again.";
    public const string ResetPasswordChangedError = "You have already changed the Password once.";
    public const string PasswordChangeFailed = "Failed to change the password. Please try again.";

}