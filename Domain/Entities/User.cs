using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class User : IdentityUser<Guid>
{
    [Required, MaxLength(255)]
    public string FullName { get; set; } = null!;
}