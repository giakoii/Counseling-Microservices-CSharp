using AuthService.Domain.ReadModels;
using AuthService.Domain.WriteModels;
using BackEnd.Utils.Const;
using BuildingBlocks.CQRS;
using Common;
using Common.Utils.Const;
using Microsoft.EntityFrameworkCore;
using Shared.Application.Repositories;

namespace AuthService.Application.Users.Commands;

public record InsertUserCommand (
    string Email,
    string Password,
    string FirstName,
    string LastName
) : ICommand<UserCreateResponse>;

internal class InsertUserCommandHandler : ICommandHandler<InsertUserCommand, UserCreateResponse>
{
    private readonly ICommandRepository<User> _userRepository;
    private readonly ICommandRepository<Role> _roleRepository;
    private readonly INoSqlQueryRepository<UserMongo> _userMongoRepository;

    public InsertUserCommandHandler(ICommandRepository<User> userRepository, ICommandRepository<Role> roleRepository, INoSqlQueryRepository<UserMongo> userMongoRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userMongoRepository = userMongoRepository;
    }

    public async Task<UserCreateResponse> Handle(InsertUserCommand request, CancellationToken cancellationToken)
    {
        var response = new UserCreateResponse {Success = false};
        
        // Validate email
        var emailExist = await _userRepository.Find(x => x.Email == request.Email && x.IsActive, true).FirstOrDefaultAsync(cancellationToken);
        if (emailExist != null)
        {
            response.SetMessage(MessageId.E11004);
            return response;
        }

        // Check role in the database
        var role = await _roleRepository.Find(x => x.Name == ConstantEnum.Role.Customer.ToString()).FirstOrDefaultAsync(cancellationToken);
        if (role == null)
        {
            response.SetMessage(MessageId.E99999);
            return response;
        }

        await _userRepository.ExecuteInTransactionAsync(async () =>
        {
            // Insert new user
            var newUser = new User
            {
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, workFactor: 12),
                LastName = request.LastName,
                FirstName = request.FirstName,
                RoleId = role.RoleId,
            };
        
            // Save changes
            await _userRepository.AddAsync(newUser);
            await _userRepository.SaveChangesAsync(newUser.Email);
            
            // Insert into MongoDB
            var newUserMongo = new UserMongo
            {
                UserId = newUser.UserId,
                Email = newUser.Email,
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, workFactor: 12),
                RoleId = newUser.RoleId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = newUser.Email,
                UpdatedBy = newUser.Email,
                IsActive = true,
            };
            
            await _userMongoRepository.AddAsync(newUserMongo);
            
            response.Success = true;
            response.SetMessage(MessageId.I00001);
            return true;
        });
        return response;
    }
    
}

public class UserCreateResponse : AbstractResponse<string>
{
    public override string Response { get; set; }
}