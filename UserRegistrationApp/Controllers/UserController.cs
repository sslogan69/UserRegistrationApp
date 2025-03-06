using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using UserRegistrationApp.Dtos;
using ModelsErrorResponse = UserRegistrationApp.Models.ErrorResponse;
using ModelsErrorDetails = UserRegistrationApp.Models.ErrorDetails;
using UserRegistrationApp.Services.Interfaces;
using UserRegistrationApp.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace UserRegistrationApp.Controllers
{
    /// <summary>
    /// Controller for handling user-related operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="userService">The user service instance.</param>
        /// <param name="logger">The logger instance.</param>
        public UserController(IUserService userService, ILogger<UserController> logger, IConfiguration configuration)
        {
            _userService = userService;
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// Gets a user by their ID.
        /// </summary>
        /// <param name="userId">The ID of the user to retrieve.</param>
        /// <returns>An <see cref="IActionResult"/> containing the user data or an error response.</returns>
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUser(string userId)
        {
            try
            {
                _logger.LogInformation("Getting user with ID {UserId}", userId);
                var user = await _userService.GetUserAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found", userId);
                    return NotFound(new ModelsErrorResponse
                    {
                        Success = false,
                        Error = new ModelsErrorDetails
                        {
                            Code = "USER_NOT_FOUND",
                            Message = "User not found",
                            Details = $"No user found with ID {userId}"
                        }
                    });
                }
                return Ok(new ApiResponse<UserDto>
                {
                    Success = true,
                    Data = user,
                    Message = "User retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the user with ID {UserId}", userId);
                return StatusCode(500, new ModelsErrorResponse
                {
                    Success = false,
                    Error = new ModelsErrorDetails
                    {
                        Code = "INTERNAL_SERVER_ERROR",
                        Message = "An error occurred while retrieving the user",
                        Details = ex.Message
                    }
                });
            }
        }

        /// <summary>
        /// Inserts a new user.
        /// </summary>
        /// <param name="userDto">The data of the user to insert.</param>
        /// <returns>An <see cref="IActionResult"/> containing the inserted user data or an error response.</returns>
        [HttpPost]
        public async Task<IActionResult> InsertUser([FromBody] UserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ModelsErrorResponse
                {
                    Success = false,
                    Error = new ModelsErrorDetails
                    {
                        Code = "VALIDATION_ERROR",
                        Message = "Validation failed",
                        Details = "Invalid user data"
                    }
                });
            }

            try
            {
                _logger.LogInformation("Inserting new user with UserName {UserName}", userDto.UserName);
                var insertedUser = await _userService.InsertUserAsync(userDto);
                return CreatedAtAction(nameof(GetUser), new { userId = insertedUser.UserId }, new ApiResponse<UserDto>
                {
                    Success = true,
                    Data = insertedUser,
                    Message = "User inserted successfully"
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "User with email {Email} already exists", userDto.Email);
                return BadRequest(new ModelsErrorResponse
                {
                    Success = false,
                    Error = new ModelsErrorDetails
                    {
                        Code = "USER_ALREADY_EXISTS",
                        Message = "User already exists",
                        Details = ex.Message
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while inserting a new user with UserName {UserName}", userDto.UserName);
                return StatusCode(500, new ModelsErrorResponse
                {
                    Success = false,
                    Error = new ModelsErrorDetails
                    {
                        Code = "INTERNAL_SERVER_ERROR",
                        Message = "An error occurred while inserting the user",
                        Details = ex.Message
                    }
                });
            }
        }

        /// <summary>
        /// Updates a user with the provided data.
        /// </summary>
        /// <param name="userDto">The data to update the user with.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the update operation.</returns>
        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ModelsErrorResponse
                {
                    Success = false,
                    Error = new ModelsErrorDetails
                    {
                        Code = "VALIDATION_ERROR",
                        Message = "Validation failed",
                        Details = "Invalid user data"
                    }
                });
            }

            try
            {
                _logger.LogInformation("Updating user with ID {UserId}", userDto.UserId);
                var result = await _userService.UpdateUserAsync(userDto);
                if (!result)
                {
                    _logger.LogWarning("Failed to update user with ID {UserId}", userDto.UserId);
                    return BadRequest(new ModelsErrorResponse
                    {
                        Success = false,
                        Error = new ModelsErrorDetails
                        {
                            Code = "UPDATE_FAILED",
                            Message = "Failed to update user",
                            Details = "The user could not be updated. Please check the provided data."
                        }
                    });
                }
                return Ok(new ApiResponse<UserDto>
                {
                    Success = true,
                    Data = userDto,
                    Message = "User updated successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the user with ID {UserId}", userDto.UserId);
                return StatusCode(500, new ModelsErrorResponse
                {
                    Success = false,
                    Error = new ModelsErrorDetails
                    {
                        Code = "INTERNAL_SERVER_ERROR",
                        Message = "An error occurred while updating the user",
                        Details = ex.Message
                    }
                });
            }
        }
        /// <summary>
        /// Authenticates a user and generates JWT and refresh tokens.
        /// </summary>
        /// <param name="loginDto">The login data containing user ID and password.</param>
        /// <returns>An <see cref="IActionResult"/> containing the JWT and refresh tokens or an error response.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _userService.GetUserAsync(loginDto.UserId);
            if (user == null || !PasswordHelper.VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            // Generate JWT token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserId.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // Generate refresh token
            var refreshToken = Guid.NewGuid().ToString();
            // Store refresh token securely (e.g., in the database)
            await _userService.SaveRefreshTokenAsync(user.UserId, refreshToken);

            // Return the tokens
            return Ok(new
            {
                Token = tokenString,
                RefreshToken = refreshToken,
                Message = "Login successful"
            });
        }
        /// <summary>
        /// Refreshes the JWT and refresh tokens.
        /// </summary>
        /// <param name="refreshTokenDto">The refresh token data.</param>
        /// <returns>An <see cref="IActionResult"/> containing the new JWT and refresh tokens or an error response.</returns>
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto refreshTokenDto)
        {
            var userId = await _userService.GetUserIdByRefreshTokenAsync(refreshTokenDto.RefreshToken);
            if (userId == null)
            {
                return Unauthorized(new { message = "Invalid refresh token" });
            }

            // Generate new JWT token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, userId.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // Generate new refresh token
            var newRefreshToken = Guid.NewGuid().ToString();
            await _userService.UpdateRefreshTokenAsync(userId, newRefreshToken);

            // Return the tokens
            return Ok(new
            {
                Token = tokenString,
                RefreshToken = newRefreshToken,
                Message = "Token refreshed successfully"
            });
        }
    }
}


