using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace BrewBoxApi.Domain.Aggregates.Identity;

public class ApplicationUser : IdentityUser<string>
{

    [MaxLength(50)]
    [PersonalData]
    public string DisplayName { get; set; } = string.Empty;
}
