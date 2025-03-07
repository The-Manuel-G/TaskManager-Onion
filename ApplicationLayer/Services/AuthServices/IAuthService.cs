using DomainLayer.DTO;
using System.Threading.Tasks;

using System.Text.Json.Serialization;

namespace ApplicationLayer.Services.AuthServices
{

    public interface IAuthService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest request);
        Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest request);

        AuthenticateResponse RefreshToken(string refreshToken);
        Task<AuthenticateResponse> RefreshTokenAsync(string refreshToken);

        AuthenticateResponse RegisterUser(string username, string password, string email);
        Task<AuthenticateResponse> RegisterUserAsync(string username, string password, string email);
    }

    


      
    }



