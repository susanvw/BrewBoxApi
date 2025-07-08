using BrewBoxApi.Domain.Aggregates.Identity;
using Microsoft.AspNetCore.Identity;

namespace BrewBoxApi.Domain.SeedWork;

public  class BaseModel
{
    public BaseModel()
    {
        Id = Guid.NewGuid().ToString();
    }


    [PersonalData]
    public string Id { get; }

    public ApplicationUser CreatedBy { get; set; } = null!;
    public string? CreatedById { get; set; }
    public DateTime? CreatedOn { get; set; }
}
