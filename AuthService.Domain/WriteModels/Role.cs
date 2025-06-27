﻿namespace AuthService.Domain.WriteModels;

public partial class Role
{
    public Guid RoleId { get; set; }

    public string Name { get; set; } = null!;

    public string NormalizedName { get; set; } = null!;
    
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
