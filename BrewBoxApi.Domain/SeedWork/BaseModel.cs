using Microsoft.AspNetCore.Identity;

namespace BrewBoxApi.Domain.SeedWork;

public abstract class BaseModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public IdentityUser User { get; set; } = null!;
    public string? CreatedBy { get; set; }
    public DateTime? CreatedOn { get; set; }
}
