using AuthService.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AuthService.Infrastructure.Factories;

public class AuthServiceContextFactory : IDesignTimeDbContextFactory<AuthServiceContext>
{
    public AuthServiceContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AuthServiceContext>();

        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=AuthServiceDB;Username=root;Password=Gi@khoi221203");

        return new AuthServiceContext(optionsBuilder.Options);
    }
}