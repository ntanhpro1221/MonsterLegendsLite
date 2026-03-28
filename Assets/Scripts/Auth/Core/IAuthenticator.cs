using System.Threading.Tasks;
using Firebase.Auth;

namespace NGDtuanh.Auth {
    public interface IAuthenticator {
        Task<Credential> AuthenticateAsync(AllAuthInput allAuthInput);
    }
}