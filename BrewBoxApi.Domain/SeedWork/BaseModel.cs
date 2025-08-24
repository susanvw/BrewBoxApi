using BrewBoxApi.Domain.Aggregates.Identity;
using Microsoft.AspNetCore.Identity;

namespace BrewBoxApi.Domain.SeedWork;

public abstract class BaseModel
{

    [PersonalData]
    public string Id { get; } = Guid.NewGuid().ToString();

    public ApplicationUser CreatedBy { get; set; } = null!;
    public string CreatedById { get; set; } = null!;
    public DateTime CreatedOn { get; set; }

    public ApplicationUser? ModifiedBy { get; set; }
    public string? ModifiedById { get; set; }
    public DateTime? ModifiedOn { get; set; }

    public bool IsDeleted { get; set; } = false;
    public ApplicationUser? DeletedBy { get; set; }
    public string? DeletedById { get; set; }
    public DateTime? DeletedOn { get; set; }

}
