using AuthService.Domain.ReadModels;
using AuthService.Domain.WriteModels;

namespace AuthService.Application.Mappers;

public static class UserMapper
{
    public static UserCollection ToReadModel(User user)
    {
        return new UserCollection
        {
            Id = user.Id,
            RoleId = user.RoleId,
            Email = user.Email,
            EmailConfirmed = user.EmailConfirmed,
            PhoneNumber = user.PhoneNumber,
            FirstName = user.FirstName,
            LastName = user.LastName,
            DateOfBirth = user.DateOfBirth,
            Address = user.Address,
            Gender = user.Gender,
            Key = user.Key,
            AvatarUrl = user.AvatarUrl,
            CreatedAt = user.CreatedAt,
            CreatedBy = user.CreatedBy,
            IsActive = user.IsActive,
            LockoutEnd = user.LockoutEnd,
            PasswordHash = user.PasswordHash,
            UpdatedAt = user.UpdatedAt,
            UpdatedBy = user.UpdatedBy,
            AccessFailedCount = user.AccessFailedCount,
            RoleInf = new UserCollection.Role
            {
                Id = user.Role.Id,
                Name = user.Role.Name,
                NormalizedName = user.Role.NormalizedName
            }
        };
    }
}