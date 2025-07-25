using System.Security.Cryptography;
using AuthService.Application.Mappers;
using AuthService.Domain.WriteModels;
using BuildingBlocks.CQRS;
using BuildingBlocks.Messaging.Events.CounselorScheduleEvents;
using Common;
using Common.Utils.Const;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Application.Interfaces;

namespace AuthService.Application.Users.Commands;

public record InsertCounselorCommand(
    string Email,
    string FirstName,
    string LastName
) : ICommand<BaseCommandResponse>;

public class InsertCounselorCommandHandler : ICommandHandler<InsertCounselorCommand, BaseCommandResponse>
{
    private readonly ICommandRepository<User> _userRepository;
    private readonly ICommandRepository<Role> _roleRepository;
    private readonly IRequestClient<UserInformationRequest> _requestClient;
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="userRepository"></param>
    /// <param name="roleRepository"></param>
    /// <param name="requestClient"></param>
    public InsertCounselorCommandHandler(ICommandRepository<User> userRepository, ICommandRepository<Role> roleRepository, IRequestClient<UserInformationRequest> requestClient)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _requestClient = requestClient;
    }

    public async Task<BaseCommandResponse> Handle(InsertCounselorCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseCommandResponse {Success = false};
        
        // Validate email
        var emailExist = await _userRepository.Find(x => x.Email == request.Email && x.IsActive).FirstOrDefaultAsync(cancellationToken);
        if (emailExist != null)
        {
            response.SetMessage(MessageId.E11004);
            return response;
        }

        // Check role in the database
        var role = await _roleRepository.Find(x => x.Name == nameof(ConstantEnum.Role.Consultant)).FirstOrDefaultAsync(cancellationToken);
        if (role == null)
        {
            response.SetMessage(MessageId.E99999);
            return response;
        }

        await _userRepository.ExecuteInTransactionAsync(async () =>
        {
            var password = GenerateRandomPassword();
            
            // Insert new user
            var newUser = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12),
                LastName = request.LastName,
                FirstName = request.FirstName,
                RoleId = role.Id,
            };
        
            // Save changes
            await _userRepository.AddAsync(newUser);
            await _userRepository.SaveChangesAsync(newUser.Email);
            
            _userRepository.Store(UserMapper.ToReadModel(newUser), newUser.Email);
            await _userRepository.SessionSavechanges();
            
            // Publish event to notify other services
            var @event = new UserInformationRequest 
                { 
                    Email = newUser.Email,
                    FirstName = newUser.FirstName,
                    CounselorId = newUser.Id,
                    LastName = newUser.LastName,
                };

            var responseInsertCounselorSchedule = await _requestClient.GetResponse<BaseCommandResponse>(@event, cancellationToken);
            
            // If response from UserInformationRequest is not successful, set error message
            if (!responseInsertCounselorSchedule.Message.Success)
            {
                response.SetMessage(MessageId.E99999);
                return false;
            }
            
            response.Success = true;
            response.SetMessage(MessageId.I00001);
            return true;
        });
        return response;
    }
    
    /// <summary>
    /// Generates a random password that meets complexity requirements
    /// </summary>
    /// <param name="length">Password length (minimum 8)</param>
    /// <returns>A secure random password</returns>
    private static string GenerateRandomPassword(int length = 12)
    {
        // Ensure minimum length of 8
        length = Math.Max(8, length);
        
        // Define character sets
        const string lowerChars = "abcdefghijklmnopqrstuvwxyz";
        const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string numberChars = "0123456789";
        const string specialChars = "!@#$%^&*()-_+[]{}|;:,.<>?";
        
        // Create a random number generator
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[4];
        
        // Generate one character from each required set
        var password = new char[length];
        password[0] = GetRandomChar(lowerChars, rng, bytes);
        password[1] = GetRandomChar(upperChars, rng, bytes);
        password[2] = GetRandomChar(numberChars, rng, bytes);
        password[3] = GetRandomChar(specialChars, rng, bytes);
        
        // Combine all character sets for remaining positions
        string allChars = lowerChars + upperChars + numberChars + specialChars;
        
        // Fill remaining positions with random characters
        for (int i = 4; i < length; i++)
        {
            password[i] = GetRandomChar(allChars, rng, bytes);
        }
        
        // Shuffle the password to avoid predictable patterns
        ShuffleArray(password, rng, bytes);
        
        return new string(password);
    }
    
    /// <summary>
    /// Gets a random character from the provided character set
    /// </summary>
    private static char GetRandomChar(string charSet, RandomNumberGenerator rng, byte[] bytes)
    {
        rng.GetBytes(bytes);
        uint num = BitConverter.ToUInt32(bytes, 0);
        return charSet[(int)(num % charSet.Length)];
    }
    
    /// <summary>
    /// Fisher-Yates shuffle algorithm to randomize character positions
    /// </summary>
    private static void ShuffleArray(char[] array, RandomNumberGenerator rng, byte[] bytes)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            rng.GetBytes(bytes);
            uint num = BitConverter.ToUInt32(bytes, 0);
            int j = (int)(num % (i + 1));
            
            // Swap elements
            (array[i], array[j]) = (array[j], array[i]);
        }
    }
}