using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace BrewBoxApi.Domain.Aggregates.Identity;

public class ApplicationUser : IdentityUser<string>
{
    public ApplicationUser(string displayName, string email)
    {
        ArgumentException.ThrowIfNullOrEmpty(displayName);
        DisplayName = displayName;
        Email = email;
        UserName = email;
    }

    [MaxLength(50)]
    [PersonalData]
    public string DisplayName { get; set; }
}
