using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlazorERP.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser, ApplicationRole, int>(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            var adminRole = new ApplicationRole
            {
                Id = 1,
                Name = "Administrator",
                NormalizedName = "ADMINISTRATOR"
            };

            modelBuilder.Entity<ApplicationRole>().HasData(adminRole);

            var adminUser = new ApplicationUser
            {
                Id = 1,
                UserName = "erp_admin",
                NormalizedUserName = "ERP_ADMIN",
                EmailConfirmed = true,
                SecurityStamp = "",
                Email = string.Empty,
                NormalizedEmail = string.Empty,
                PhoneNumber = string.Empty,
            };

            var hasher = new PasswordHasher<ApplicationUser>();
            adminUser.PasswordHash = hasher.HashPassword(adminUser, "12Tester34#");
            modelBuilder.Entity<ApplicationUser>().HasData(adminUser);


            //Seeding the relation between our user and role to AspNetUserRoles table
            modelBuilder.Entity<IdentityUserRole<int>>()
                .HasData
                (
                new IdentityUserRole<int>
                    {
                        RoleId = 1,
                        UserId = 1
                    }
                );

        }
    }

}