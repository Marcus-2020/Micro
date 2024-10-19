using Microsoft.AspNetCore.Identity;

namespace Micro.Identity.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime BirthDate { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; }

    public string IdentityUserId { get; set; }
    public IdentityUser IdentityUser { get; set; }
}