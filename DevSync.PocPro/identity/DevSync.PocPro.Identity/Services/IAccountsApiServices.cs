using DevSync.PocPro.Identity.Dtos;

namespace DevSync.PocPro.Identity.Services;

public interface IAccountsApiServices
{
    Task<int> AddUserToAccountsAsync(AddUserToAccountRequest request);
}