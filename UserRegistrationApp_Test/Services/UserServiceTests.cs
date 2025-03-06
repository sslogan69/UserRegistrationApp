using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using UserRegistrationApp.DbContext;
using UserRegistrationApp.Dtos;
using UserRegistrationApp.Models;
using UserRegistrationApp.Services.Implementations;
using Xunit;

namespace UserRegistrationApp.Tests.Services
{
    public class UserServiceTests
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<ILogger<UserService>> _mockLogger;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _mockLogger = new Mock<ILogger<UserService>>();
            _userService = new UserService(_context, _mockLogger.Object);
        }

        [Fact]
        public async Task GetUserAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var userId = "1";
            var user = new User
            {
                UserId = 1,
                UserName = "testuser",
                Email = "testuser@example.com",
                PasswordHash = "hashedpassword",
                RefreshToken = Guid.NewGuid().ToString(),
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7)
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userService.GetUserAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);
        }

        [Fact]
        public async Task GetUserAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = "1";

            // Act
            var result = await _userService.GetUserAsync(userId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task InsertUserAsync_ShouldReturnInsertedUser()
        {
            // Arrange
            var userDto = new UserDto
            {
                UserId = "1",
                UserName = "testuser",
                Email = "testuser@example.com",
                PasswordHash = "hashedpassword"
            };

            // Act
            var result = await _userService.InsertUserAsync(userDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userDto.UserId, result.UserId);
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldReturnTrue_WhenUserIsUpdated()
        {
            // Arrange
            var userDto = new UserDto
            {
                UserId = "1",
                UserName = "updateduser",
                Email = "updateduser@example.com",
                PasswordHash = "hashedpassword"
            };
            var user = new User
            {
                UserId = 1,
                UserName = "testuser",
                Email = "testuser@example.com",
                PasswordHash = "hashedpassword",
                RefreshToken = Guid.NewGuid().ToString(),
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7)
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userService.UpdateUserAsync(userDto);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldReturnFalse_WhenUserIsNotFound()
        {
            // Arrange
            var userDto = new UserDto
            {
                UserId = "1",
                UserName = "updateduser",
                Email = "updateduser@example.com",
                PasswordHash = "hashedpassword"
            };

            // Act
            var result = await _userService.UpdateUserAsync(userDto);

            // Assert
            Assert.False(result);
        }
    }
}
