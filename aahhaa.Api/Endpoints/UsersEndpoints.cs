using aahhaa.Api.Models.Users;
using aahhaa.Core.Models;
using aahhaa.Shared.Data.Interfaces;
using FluentValidation;
using MinimalApis.Extensions.Results;

namespace aahhaa.Api.Endpoints;

public class UsersEndpoints : IEndpoints
{
    public void Register(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet("v1/user/{id}", GetByIdAsync);
        routeBuilder.MapPost("v1/users", CreateUserAsync);
    }

    public async Task<IResult> GetByIdAsync(Guid id, IRepository<User> userRepository)
    {
        return await userRepository.GetAsync(id) switch
        {
            User user => Results.Extensions.Ok(UserResponse.From(user)),
            null => Results.Extensions.NotFound()
        };
    }

    public async Task<IResult> CreateUserAsync(
        CreateUserRequest createUserRequest,
        IValidator<CreateUserRequest> validator,
        IRepository<User> userRepository)
    {
        // Validate the request
        var validationResult = validator.Validate(createUserRequest);
        if (!validationResult.IsValid)
        {
            return Results.Extensions.BadRequest(validationResult.ToString());
        }

        // Check if the user has already registered an account
        var existingUser = await userRepository.FindOneAsync(u => u.UserName == createUserRequest.UserName);
        if (existingUser != null)
        {
            return Results.Extensions.Conflict($"The username '{createUserRequest.UserName}' is already taken.");
        }

        // Create the user and return the UserResponse object
        var user = createUserRequest.MapToUser();
        await userRepository.CreateAsync(user);

        return Results.Extensions.Created($"v1/users/{user.Id}", UserResponse.From(user));
    }
}