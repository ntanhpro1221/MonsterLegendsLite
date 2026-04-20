using System.Threading.Tasks;
using Firebase.Auth;

namespace MonsterLegendsLite.Auth {
    public interface IAuthenticator {
        Task<Credential> AuthenticateAsync(AllAuthInput allAuthInput);
    }
}