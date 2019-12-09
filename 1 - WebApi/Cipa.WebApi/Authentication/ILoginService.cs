using Cipa.WebApi.ViewModels;

namespace Cipa.WebApi.Authentication
{
    public interface ILoginService
    {
        AuthInfoViewModel Login(string email, string senha);
    }
}