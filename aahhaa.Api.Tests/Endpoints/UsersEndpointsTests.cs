using aahhaa.Api.Endpoints;
using aahhaa.Api.Models.Users;
using aahhaa.Api.Models.Users.Validators;
using aahhaa.Core.Models;
using aahhaa.Shared.Data.Interfaces;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MinimalApis.Extensions.Results;
using Moq;
using NUnit.Framework;
using System.Linq.Expressions;

namespace aahhaa.Api.Tests.Apis
{
    [TestFixture]
    public class UsersEndpointsTests
    {
        private UsersEndpoints? _usersEndpoints;
        private Mock<IRepository<User>>? _mockUserRepository;
        private Mock<IValidator<CreateUserRequest>>?_mockValidator;

        [SetUp]
        public void Setup()
        {
            _usersEndpoints = new UsersEndpoints();
            _mockUserRepository = new Mock<IRepository<User>>();
            _mockValidator = new Mock<IValidator<CreateUserRequest>>();
        }

        [Test]
        public async Task GetById_ReturnsOk()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var expectedUser = new User { Id = userId, Email = "test@unit.com", UserName = "test", FirstName = "Test", LastName = "Unit" };
            
            _mockUserRepository
                .Setup(r => r.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(expectedUser);

            // Act
            var result = await _usersEndpoints.GetByIdAsync(userId, _mockUserRepository.Object);

            // Assert
            result.Should().NotBeNull().And.BeOfType<Ok<UserResponse>>();

            var actualUserResponse = result as Ok<UserResponse>;
            actualUserResponse.Value.Id.Should().Be(userId);
            actualUserResponse.Value.UserName.Should().Be(expectedUser.UserName);
            actualUserResponse.Value.FirstName.Should().Be(expectedUser.FirstName);
            actualUserResponse.Value.LastName.Should().Be(expectedUser.LastName);
            actualUserResponse.Value.Email.Should().Be(expectedUser.Email);
        }

        [Test]
        public async Task GetById_ReturnsNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            User? expectedUser = null;

            _mockUserRepository
                .Setup(r => r.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(expectedUser);

            // Act
            var result = await _usersEndpoints.GetByIdAsync(userId, _mockUserRepository.Object);

            // Assert
            result.Should().NotBeNull().And.BeOfType<NotFound>();
            result.As<NotFound>().ResponseContent.Should().BeNullOrEmpty();
        }

        [Test]
        public async Task CreateUser_ReturnsOk()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var createUserRequest = new CreateUserRequest { Email = "test@unit.com", UserName = "test", FirstName = "Test", LastName = "Unit" };

            _mockValidator
                .Setup(v => v.Validate(It.IsAny<CreateUserRequest>()))
                .Returns(new ValidationResult());

            _mockUserRepository
                .Setup(r => r.CreateAsync(It.IsAny<User>()));

            // Act
            var result = await _usersEndpoints.CreateUserAsync(createUserRequest, _mockValidator.Object, _mockUserRepository.Object);

            // Assert
            result.Should().NotBeNull().And.BeOfType<Created<UserResponse>>();
        }

        [Test]
        public async Task CreateUser_ReturnsBadRequest_WhenModelFailsValidation()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var createUserRequest = new CreateUserRequest { Email = "", UserName = "test", FirstName = "Test", LastName = "Unit" };

            var validator = new CreateUserRequestValidator();

            _mockUserRepository
                .Setup(r => r.CreateAsync(It.IsAny<User>()));

            // Act
            var result = await _usersEndpoints.CreateUserAsync(createUserRequest, validator, _mockUserRepository.Object);

            // Assert
            result.Should().NotBeNull().And.BeOfType<BadRequest>();
            result.As<BadRequest>().ResponseContent.Should().Be("'Email' is not a valid email address.");
        }

        [Test]
        public async Task CreateUser_ReturnsConflict_WhenUserAlreadyExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var createUserRequest = new CreateUserRequest { Email = "test@unit.com", UserName = "test", FirstName = "Test", LastName = "Unit" };
            var existingUser = new User { Email = "test@unit.com", UserName = "test", FirstName = "Test", LastName = "Unit" };

            _mockValidator
                .Setup(v => v.Validate(It.IsAny<CreateUserRequest>()))
                .Returns(new ValidationResult());

            _mockUserRepository
                .Setup(r => r.FindOneAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(existingUser);

            // Act
            var result = await _usersEndpoints.CreateUserAsync(createUserRequest, _mockValidator.Object, _mockUserRepository.Object);

            // Assert
            result.Should().NotBeNull().And.BeOfType<Conflict>();
            result.As<Conflict>().ResponseContent.Should().Be($"The username '{createUserRequest.UserName}' is already taken.");
        }
    }
}
