namespace Micro.Identity.Requests;

public sealed class UpdateMeRequest
{
    public required string Name { get; init; }
    public DateTime? BirthDate { get; init; }
}