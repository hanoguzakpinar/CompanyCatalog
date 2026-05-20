using CompanyCatalog.Domain.Users;

namespace CompanyCatalog.Application.Abstractions.Authentication;

public interface IJwtService
{
    string GenerateToken(User user);
}