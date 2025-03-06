using CoreWCF;
using System.Threading.Tasks;
using UserRegistrationApp.Dtos;

namespace UserRegistrationApp.Services.Interfaces
{
    /// <summary>
    /// Interface for user service operations.
    /// </summary>
    [ServiceContract]
    public interface IUserService
    {
        /// <summary>
        /// Gets a user by their ID.
        /// </summary>
        /// <param name="userId">The ID of the user to retrieve.</param>
        /// <returns>A <see cref="UserDto"/> representing the user, or null if the user is not found.</returns>
        [OperationContract]
        Task<UserDto> GetUserAsync(string userId);

        /// <summary>
        /// Inserts a new user.
        /// </summary>
        /// <param name="userDto">The data of the user to insert.</param>
        /// <returns>The inserted <see cref="UserDto"/>.</returns>
        [OperationContract]
        Task<UserDto> InsertUserAsync(UserDto userDto);

        /// <summary>
        /// Updates a user with the provided data.
        /// </summary>
        /// <param name="userDto">The data to update the user with.</param>
        /// <returns>True if the user was updated successfully, false otherwise.</returns>
        [OperationContract]
        Task<bool> UpdateUserAsync(UserDto user);
        /// <summary>
        /// Sets the workflow state to "Started" for the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>True if the workflow state was set successfully, false otherwise.</returns>
        [OperationContract]
        Task<bool> SetWorkflowStateStartedAsync(int userId);

        /// <summary>
        /// Sets the workflow state to "Completed" for the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>True if the workflow state was set successfully, false otherwise.</returns>
        [OperationContract]
        Task<bool> SetWorkflowStateCompletedAsync(int userId);

        Task SaveRefreshTokenAsync(string userId, string refreshToken);
        Task<string> GetUserIdByRefreshTokenAsync(string refreshToken);
        Task UpdateRefreshTokenAsync(string userId, string newRefreshToken);


    }
}


