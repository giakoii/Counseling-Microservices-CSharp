using Microsoft.EntityFrameworkCore;

namespace Shared.Infrastructure.Context;

public abstract class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }
    
    /// <summary>
    /// Save changes async with common value
    /// </summary>
    /// <param name="updateUserId"></param>
    /// <param name="needLogicalDelete"></param>
    /// <returns></returns>
    public　async Task<int> SaveChangesAsync(string updateUserId, bool needLogicalDelete = false)
    {
        this.SetCommonValue(updateUserId, needLogicalDelete);
        return await base.SaveChangesAsync();
    }
    
    /// <summary>
    /// Save changes with common value
    /// </summary>
    /// <param name="updateUserId"></param>
    /// <param name="needLogicalDelete"></param>
    /// <returns></returns>
    public int SaveChanges(string updateUserId, bool needLogicalDelete = false)
    {
        this.SetCommonValue(updateUserId, needLogicalDelete);
        return base.SaveChanges();
    }
    
    /// <summary>
    /// Set common value for all entities
    /// </summary>
    /// <param name="updateUser"></param>
    /// <param name="needLogicalDelete"></param>
    private void SetCommonValue(string updateUser, bool needLogicalDelete = false)
    {
        // Register
        var newEntities = ChangeTracker.Entries()
            .Where(
                x => x.State == EntityState.Added &&
                x.Entity != null
                )
            .Select(e => e.Entity);

        // Modify
        var modifiedEntities = ChangeTracker.Entries()
            .Where(
                x => x.State == EntityState.Modified &&
                    x.Entity != null
                    )
                .Select(e => e.Entity);

        // Get current time
        var now = DateTime.UtcNow;
        // Add
        foreach (dynamic newEntity in newEntities)
        {
            try
            {
                newEntity.IsActive = true;
                newEntity.CreatedAt = now;
                newEntity.CreatedBy = updateUser;
                newEntity.UpdatedBy = updateUser;
                newEntity.UpdatedAt = now;
            }
            catch (IOException e)
            {
                // There may be no elements, so don't throw an error
            }
        }

        // Set modifiedEntities
        foreach (dynamic modifiedEntity in modifiedEntities)
        {
            try
            {
                if (needLogicalDelete)
                {
                    // Delete
                    modifiedEntity.IsActive = false;
                    modifiedEntity.UpdatedBy = updateUser;
                }
                else
                {
                    // Normal
                    modifiedEntity.IsActive = true;
                    modifiedEntity.UpdatedBy = updateUser;
                }
                modifiedEntity.UpdatedAt = now;
            }
            catch (IOException e)
            {
                // There may be no elements, so don't throw an error
            }
        }

    }
}