namespace DevSync.PocPro.Accounts.Api.Interfaces;

public interface IIdentityServices
{
    Task<string> RegisterUserLoginAsync(string username, string email, string password, string accessToken);
}