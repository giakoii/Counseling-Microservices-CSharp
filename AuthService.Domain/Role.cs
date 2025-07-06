using System.Text.Json.Serialization;

namespace AuthService.Domain;

public partial class Role
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string NormalizedName { get; set; } = null!;
    
    [JsonIgnore]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
