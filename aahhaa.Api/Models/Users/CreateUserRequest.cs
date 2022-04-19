using aahhaa.Core.Models;

namespace aahhaa.Api.Models.Users;

public class CreateUserRequest
{
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? UserName { get; set; }

    public User MapToUser()
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Email = Email,
            FirstName = FirstName,
            LastName = LastName,
            UserName = UserName
        };
    }
}