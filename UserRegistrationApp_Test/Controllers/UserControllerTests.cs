using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using UserRegistrationApp.Controllers;
using UserRegistrationApp.Dtos;
using UserRegistrationApp.Models;
using UserRegistrationApp.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace UserRegistrationApp.Tests.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<ILogger<UserController>> _mockLogger;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly UserController _userController;

        public UserControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _mockLogger = new Mock<ILogger<UserController>>();
            _mockConfiguration = new Mock<IConfiguration>();
            _userController = new UserController(_mockUserService.Object, _mockLogger.Object, _mockConfiguration.Object);

        }

        [Fact]
        public async Task GetUser_ShouldReturnOk_WhenUserExists()
        {
            // Arrange
            var userId = "1";
            var userDto = new UserDto
            {
                UserId = userId,
                UserName = "testuser",
                Email = "testuser@example.com"
            };
            _mockUserService.Setup(s => s.GetUserAsync(userId)).ReturnsAsync(userDto);

            // Act
            var result = await _userController.GetUser(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ApiResponse<UserDto>>(okResult.Value);
            Assert.Equal(userId, returnValue.Data.UserId);
        }

        [Fact]
        public async Task GetUser_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = "1";
            _mockUserService.Setup(s => s.GetUserAsync(userId)).ReturnsAsync((UserDto)null);

            // Act
            var result = await _userController.GetUser(userId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var returnValue = Assert.IsType<UserRegistrationApp.Models.ErrorResponse>(notFoundResult.Value);
            Assert.Equal("USER_NOT_FOUND", returnValue.Error.Code);
        }

        [Fact]
        public async Task InsertUser_ShouldReturnCreated_WhenUserIsInserted()
        {
            // Arrange
            var userDto = new UserDto
            {
                UserId = "1",
                UserName = "testuser",
                Email = "testuser@example.com",
                PasswordHash = "hashedpassword"
            };
            _mockUserService.Setup(s => s.InsertUserAsync(userDto)).ReturnsAsync(userDto);

            // Act
            var result = await _userController.InsertUser(userDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnValue = Assert.IsType<ApiResponse<UserDto>>(createdResult.Value);
            Assert.Equal(userDto.UserId, returnValue.Data.UserId);
        }

        [Fact]
        public async Task InsertUser_ShouldReturnBadRequest_WhenUserAlreadyExists()
        {
            // Arrange
            var userDto = new UserDto
            {
                UserId = "1",
                UserName = "testuser",
                Email = "testuser@example.com",
                PasswordHash = "hashedpassword"
            };
            _mockUserService.Setup(s => s.InsertUserAsync(userDto)).ThrowsAsync(new InvalidOperationException("User already exists"));

            // Act
            var result = await _userController.InsertUser(userDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var returnValue = Assert.IsType<UserRegistrationApp.Models.ErrorResponse>(badRequestResult.Value);
            Assert.Equal("USER_ALREADY_EXISTS", returnValue.Error.Code);
        }

        [Fact]
        public async Task UpdateUser_ShouldReturnOk_WhenUserIsUpdated()
        {
            // Arrange
            var userDto = new UserDto
            {
                UserId = "1",
                UserName = "updateduser",
                Email = "updateduser@example.com"
            };
            _mockUserService.Setup(s => s.UpdateUserAsync(userDto)).ReturnsAsync(true);

            // Act
            var result = await _userController.UpdateUser(userDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ApiResponse<UserDto>>(okResult.Value);
            Assert.Equal(userDto.UserId, returnValue.Data.UserId);
        }

        [Fact]
        public async Task UpdateUser_ShouldReturnBadRequest_WhenUserIsNotUpdated()
        {
            // Arrange
            var userDto = new UserDto
            {
                UserId = "1",
                UserName = "updateduser",
                Email = "updateduser@example.com"
            };
            _mockUserService.Setup(s => s.UpdateUserAsync(userDto)).ReturnsAsync(false);

            // Act
            var result = await _userController.UpdateUser(userDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var returnValue = Assert.IsType<UserRegistrationApp.Models.ErrorResponse>(badRequestResult.Value);
            Assert.Equal("UPDATE_FAILED", returnValue.Error.Code);
        }
    }
}







