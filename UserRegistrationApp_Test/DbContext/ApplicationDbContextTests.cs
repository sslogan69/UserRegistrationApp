using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using UserRegistrationApp.DbContext;
using UserRegistrationApp.Models;
using Xunit;

namespace UserRegistrationApp.Tests.DbContext
{
    public class ApplicationDbContextTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        [Fact]
        public async Task CanInsertUserIntoDatabase()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var user = new User
            {
                UserName = "testuser",
                Email = "testuser@example.com",
                PasswordHash = "hashedpassword",
                CreatedDate = DateTime.UtcNow,
                RefreshToken = Guid.NewGuid().ToString(),
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7)
            };

            // Act
            context.Users.Add(user);
            await context.SaveChangesAsync();

            // Assert
            var insertedUser = await context.Users.FirstOrDefaultAsync(u => u.UserName == "testuser");
            Assert.NotNull(insertedUser);
            Assert.Equal("testuser@example.com", insertedUser.Email);
        }

        [Fact]
        public async Task CanInsertWorkflowStateIntoDatabase()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var workflowState = new WorkflowState
            {
                UserId = 1,
                State = "Started",
                CreatedDate = DateTime.UtcNow
            };

            // Act
            context.WorkflowStates.Add(workflowState);
            await context.SaveChangesAsync();

            // Assert
            var insertedWorkflowState = await context.WorkflowStates.FirstOrDefaultAsync(ws => ws.State == "Started");
            Assert.NotNull(insertedWorkflowState);
            Assert.Equal(1, insertedWorkflowState.UserId);
        }
    }
}


