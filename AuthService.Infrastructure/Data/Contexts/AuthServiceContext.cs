using AuthService.Domain;
using Common.Utils.Const;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Context;

namespace AuthService.Infrastructure.Data.Contexts.CommandDbContext;

public class AuthServiceContext : AppDbContext
{
    public virtual DbSet<User> Users { get; set; }
    
    public virtual DbSet<Role> Roles { get; set; }
    
    public AuthServiceContext(DbContextOptions<AuthServiceContext> options)
        : base(options) {}
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            DotNetEnv.Env.Load(); 

            var connectionString = Environment.GetEnvironmentVariable(ConstEnv.AuthServiceDB);

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("Missing CONNECTION_STRING environment variable");

            optionsBuilder.UseNpgsql(connectionString);
        }
    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.UseOpenIddict();

        builder.Entity<User>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.Email).IsUnique();
            entity.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(x => x.LastName).HasMaxLength(100).IsRequired();
            entity.Property(x => x.PasswordHash).HasMaxLength(512);
            entity.Property(x => x.PhoneNumber).HasMaxLength(15);
            entity.Property(x => x.Address).HasMaxLength(500);
            entity.Property(x => x.AvatarUrl).HasMaxLength(500);
            entity.Property(x => x.CreatedBy).HasMaxLength(256).IsRequired();
            entity.Property(x => x.UpdatedBy).HasMaxLength(256).IsRequired();
            entity.Property(x => x.Key).HasMaxLength(256);
            entity.Property(x => x.LockoutEnd);
            entity.Property(x => x.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(x => x.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(x => x.IsActive).HasDefaultValue(true);
            entity.Property(x => x.AccessFailedCount).HasDefaultValue(0);
            entity.Property(x => x.DateOfBirth).HasColumnType("date");
            entity.Property(x => x.Gender);
            entity.Property(x => x.EmailConfirmed).HasDefaultValue(false);

            entity.HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        builder.Entity<Role>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(256).IsRequired();
            entity.Property(x => x.NormalizedName).HasMaxLength(256).IsRequired();
        });
    }
}