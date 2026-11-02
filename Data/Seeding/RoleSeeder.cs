
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Constants;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;

namespace MovieTicketBooking.Data.Seeding
{
    public static class RoleSeeder
    {
        public static async Task SeedAsync(
            ApplicationDbContext context,
            IConfiguration configuration)
        {
            var hasher = new PasswordHasher<User>();

            await SeedUserAsync(
                context,
                hasher,
                configuration,
                "Admin");

            await SeedUserAsync(
                context,
                hasher,
                configuration,
                "TheatreManager");
        }

        private static async Task SeedUserAsync(
            ApplicationDbContext context,
            IPasswordHasher<User> hasher,
            IConfiguration configuration,
            string role)
        {
            string prefix = $"SeedUsers:{role}";

            string fullName = configuration[$"{prefix}:FullName"]
                ?? throw new Exception($"{role} name is missing.");

            string email = configuration[$"{prefix}:Email"]
                ?? throw new Exception($"{role} email is missing.");

            string password = configuration[$"{prefix}:Password"]
                ?? throw new Exception($"{role} password is missing.");

            if (await context.Users.AnyAsync(u => u.Email == email))
                return;

            var user = new User
            {
                FullName = fullName,
                Email = email.ToLower(),
                Role = role == "Admin"
                    ? UserRoles.Admin
                    : UserRoles.TheatreManager,
                CreatedAt = DateTime.UtcNow
            };

            user.PasswordHash = hasher.HashPassword(user, password);

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
        }
    }
}
