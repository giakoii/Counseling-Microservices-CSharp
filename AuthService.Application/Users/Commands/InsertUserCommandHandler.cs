using AuthService.Application.Mappers;
using AuthService.Domain.WriteModels;
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
) : ICommand<BaseResponse>;

internal class InsertUserCommandHandler : ICommandHandler<InsertUserCommand, BaseResponse>
{
    private readonly ICommandRepository<User> _userRepository;
    private readonly ICommandRepository<Role> _roleRepository;

    public InsertUserCommandHandler(ICommandRepository<User> userRepository, ICommandRepository<Role> roleRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
    }

    public async Task<BaseResponse> Handle(InsertUserCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseResponse {Success = false};
        
        // Validate email
        var emailExist = await _userRepository.Find(x => x.Email == request.Email && x.IsActive, true).FirstOrDefaultAsync(cancellationToken);
        if (emailExist != null)
        {
            response.SetMessage(MessageId.E11004);
            return response;
        }

        // Check role in the database
        var role = await _roleRepository.Find(x => x.Name == ConstantEnum.Role.Student.ToString()).FirstOrDefaultAsync(cancellationToken);
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
                RoleId = role.Id,
            };
            
            // Save changes
            await _userRepository.AddAsync(newUser);
            await _userRepository.SaveChangesAsync(newUser.Email);
            
            // Map user to UserWriteModel
            _userRepository.Store(UserMapper.ToReadModel(newUser), newUser.Email);
            await _userRepository.SessionSavechanges();
            
            response.Success = true;
            response.SetMessage(MessageId.I00001);
            return true;
        });
        return response;
    }
    
}