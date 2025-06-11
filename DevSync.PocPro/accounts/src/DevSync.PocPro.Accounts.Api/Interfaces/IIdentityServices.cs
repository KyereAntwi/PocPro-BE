namespace DevSync.PocPro.Accounts.Api.Interfaces;

public interface IIdentityServices
{
    Task RegisterUserLoginAsync(string username, string email, string password, string accessToken);
}