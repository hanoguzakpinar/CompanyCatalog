namespace CompanyCatalog.Application.Auth.Commands.Login;

public sealed record LoginResponse(string AccessToken, Guid UserId, string Email, string FullName, string Role);