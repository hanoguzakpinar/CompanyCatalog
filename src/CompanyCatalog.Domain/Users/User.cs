using CompanyCatalog.Domain.Common;

namespace CompanyCatalog.Domain.Users;

public sealed class User : AggregateRoot<Guid>
{
    private User()
    {
    }

    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; }

    private User(Guid id, string email, string passwordHash, string fullName, UserRole role)
    {
        Id = id;
        Email = email;
        PasswordHash = passwordHash;
        FullName = fullName;
        Role = role;
        IsActive = true;
    }

    public static User Create(string email, string passwordHash, string fullName, UserRole role = UserRole.User)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email zorunludur.");

        if (!email.Contains("@"))
            throw new DomainException("Email formatı hatalı.");

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("Password hash zorunludur.");

        if (string.IsNullOrWhiteSpace(fullName) || fullName.Length < 2)
            throw new DomainException("Full name minimum 2 karakter olmalıdır.");

        return new User(
            id: Guid.CreateVersion7(),
            email: email.ToLowerInvariant().Trim(),
            passwordHash: passwordHash,
            fullName: fullName.Trim(),
            role: role);
    }

    public void UpdatePasswordHash(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new DomainException("Password hash zorunludur.");

        PasswordHash = newPasswordHash;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateProfile(string newFullName)
    {
        if (string.IsNullOrWhiteSpace(newFullName) || newFullName.Length < 2)
            throw new DomainException("Full name minimum 2 karakter olmalıdır.");

        FullName = newFullName.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        if (!IsActive)
            throw new DomainException("Kullanıcı zaten inaktif durumdadır.");

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        if (IsActive)
            throw new DomainException("Kullanıcı zaten aktif durumdadır.");

        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }
}