namespace ARIBApp.Core.Interfaces.Services;
public interface IAuthService
{
    Task<bool> ValidateLoginAsync(LoginDTO login);

}
