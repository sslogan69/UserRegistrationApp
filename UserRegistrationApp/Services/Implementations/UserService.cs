using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserRegistrationApp.DbContext;
using UserRegistrationApp.Dtos;
using UserRegistrationApp.Services.Interfaces;
using UserRegistrationApp.Models;

namespace UserRegistrationApp.Services.Implementations
{
    /// <summary>
    /// Service class for handling user-related operations.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        /// <param name="logger">The logger instance.</param>
        public UserService(ApplicationDbContext context, ILogger<UserService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Gets a user by their ID.
        /// </summary>
        /// <param name="userId">The ID of the user to retrieve.</param>
        /// <returns>A <see cref="UserDto"/> representing the user, or null if the user is not found.</returns>
        public async Task<UserDto> GetUserAsync(string userId)
        {
            try
            {
                _logger.LogInformation("Getting user with ID {UserId}", userId);
                var user = await _context.Users.FindAsync(int.Parse(userId));
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found", userId);
                    return null;
                }

                _logger.LogInformation("User with ID {UserId} found", userId);
                return new UserDto
                {
                    UserId = user.UserId.ToString(),
                    UserName = user.UserName,
                    Email = user.Email,
                    PasswordHash = user.PasswordHash
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting user with ID {UserId}", userId);
                throw;
            }
        }

        /// <summary>
        /// Updates a user with the provided data.
        /// </summary>
        /// <param name="userDto">The data to update the user with.</param>
        /// <returns>True if the user was updated successfully, false otherwise.</returns>
        public async Task<bool> UpdateUserAsync(UserDto userDto)
        {
            try
            {
                _logger.LogInformation("Updating user with ID {UserId}", userDto.UserId);
                var user = await _context.Users.FindAsync(int.Parse(userDto.UserId));
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found", userDto.UserId);
                    return false;
                }

                user.UserName = userDto.UserName;
                user.Email = userDto.Email;
                user.PasswordHash = PasswordHelper.HashPassword(userDto.PasswordHash);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User with ID {UserId} updated successfully", userDto.UserId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating user with ID {UserId}", userDto.UserId);
                throw;
            }
        }

        /// <summary>
        /// Inserts a new user.
        /// </summary>
        /// <param name="userDto">The data of the user to insert.</param>
        /// <returns>The inserted <see cref="UserDto"/>.</returns>
        public async Task<UserDto> InsertUserAsync(UserDto userDto)
        {
            try
            {
                _logger.LogInformation("Inserting new user with UserName {UserName}", userDto.UserName);
                var user = new User
                {
                    UserName = userDto.UserName,
                    Email = userDto.Email,
                    PasswordHash = PasswordHelper.HashPassword(userDto.PasswordHash),
                    CreatedDate = DateTime.UtcNow,
                    RefreshToken = Guid.NewGuid().ToString(),
                    RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7)
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                userDto.UserId = user.UserId.ToString();
                _logger.LogInformation("User with ID {UserId} inserted successfully", user.UserId);
                return userDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while inserting a new user with UserName {UserName}", userDto.UserName);
                throw;
            }
        }

        /// <summary>
        /// Sets the workflow state to "Started" for the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>True if the workflow state was set successfully, false otherwise.</returns>
        public async Task<bool> SetWorkflowStateStartedAsync(int userId)
        {
            var workflowState = new WorkflowState
            {
                UserId = userId,
                State = "Started",
                CreatedDate = DateTime.UtcNow
            };

            _context.WorkflowStates.Add(workflowState);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Sets the workflow state to "Completed" for the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>True if the workflow state was set successfully, false otherwise.</returns>
        public async Task<bool> SetWorkflowStateCompletedAsync(int userId)
        {
            var workflowState = new WorkflowState
            {
                UserId = userId,
                State = "Completed",
                CreatedDate = DateTime.UtcNow
            };

            _context.WorkflowStates.Add(workflowState);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Saves the refresh token for the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="refreshToken">The refresh token to save.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task SaveRefreshTokenAsync(string userId, string refreshToken)
        {
            var user = await _context.Users.FindAsync(int.Parse(userId));
            if (user != null)
            {
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // Set refresh token expiry time
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Gets the user ID by the refresh token.
        /// </summary>
        /// <param name="refreshToken">The refresh token.</param>
        /// <returns>The user ID if found; otherwise, null.</returns>
        public async Task<string> GetUserIdByRefreshTokenAsync(string refreshToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && u.RefreshTokenExpiryTime > DateTime.UtcNow);
            return user?.UserId.ToString();
        }

        /// <summary>
        /// Updates the refresh token for the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="newRefreshToken">The new refresh token to save.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task UpdateRefreshTokenAsync(string userId, string newRefreshToken)
        {
            var user = await _context.Users.FindAsync(int.Parse(userId));
            if (user != null)
            {
                user.RefreshToken = newRefreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // Set new refresh token expiry time
                await _context.SaveChangesAsync();
            }
        }
    }
}
