using Microsoft.EntityFrameworkCore;
using System;
using UserRegistrationApp.DbContext;

namespace UserRegistrationApp.Tests.TestUtilities
{
    public static class InMemoryDbContextFactory
    {
        public static ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }
    }
}



