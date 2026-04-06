using AspNetCoreCrudDemo.Models;

namespace AspNetCoreCrudDemo.Interfaces
{
    public interface IJwtTokenService
    {
        TokenResponse GenerateTokenResponse(User user);
    }
}
