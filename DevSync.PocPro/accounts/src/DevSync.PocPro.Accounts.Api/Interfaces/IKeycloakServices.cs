namespace DevSync.PocPro.Accounts.Api.Interfaces;

public interface IKeycloakServices
{
    Task RegisterUserLoginAsync(string username, string email);
}