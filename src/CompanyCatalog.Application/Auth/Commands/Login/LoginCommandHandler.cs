using CompanyCatalog.Application.Abstractions.Authentication;
using CompanyCatalog.Application.Abstractions.Persistence;
using CompanyCatalog.Application.Abstractions.Results;
using MediatR;

namespace CompanyCatalog.Application.Auth.Commands.Login;

internal sealed class LoginCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtService jwtService)
    : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null)
        {
            return Result.Failure<LoginResponse>(Error.Unauthorized("Auth.InvalidCredentials",
                "Email veya şifre hatalı."));
        }

        if (!user.IsActive)
        {
            return Result.Failure<LoginResponse>(
                Error.Unauthorized("Auth.AccountInactive", "Your account is inactive."));
        }

        var passwordValid = passwordHasher.Verify(request.Password, user.PasswordHash);
        if (!passwordValid)
        {
            return Result.Failure<LoginResponse>(Error.Unauthorized("Auth.InvalidCredentials",
                "Email veya şifre hatalı."));
        }

        var token = jwtService.GenerateToken(user);

        var response = new LoginResponse(
            AccessToken: token,
            UserId: user.Id,
            Email: user.Email,
            FullName: user.FullName,
            Role: user.Role.ToString()
        );

        return Result.Success(response);
    }
}