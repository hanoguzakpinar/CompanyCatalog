using CompanyCatalog.Application.Abstractions.Authentication;
using CompanyCatalog.Application.Abstractions.Persistence;
using CompanyCatalog.Application.Abstractions.Results;
using CompanyCatalog.Domain.Common;
using CompanyCatalog.Domain.Users;
using MediatR;

namespace CompanyCatalog.Application.Auth.Commands.Register;

internal sealed class RegisterCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork) : IRequestHandler<RegisterCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var emailExists = await userRepository.ExistsByEmailAsync(request.Email, cancellationToken);
        if (emailExists)
        {
            return Result.Failure<Guid>(Error.Conflict("User.DuplicateEmail", "Bu email sistemizde zaten kayıtlıdır."));
        }

        var passwordHash = passwordHasher.Hash(request.Password);

        User user;
        try
        {
            user = User.Create(request.Email, passwordHash, request.FullName);
        }
        catch (DomainException ex)
        {
            return Result.Failure<Guid>(Error.Validation("User.Invalid", ex.Message));
        }

        userRepository.Add(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(user.Id);
    }
}