using Blezzenger.Models;

namespace Blezzenger.Services
{
    public interface IAuthenticationService
    {
        User CurrentUser { get; set; }
    }

    public class AuthenticationService : IAuthenticationService
    {
        public User CurrentUser { get; set; }
    }
}
