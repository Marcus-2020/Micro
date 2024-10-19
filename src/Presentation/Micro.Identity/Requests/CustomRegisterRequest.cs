namespace Micro.Identity.Requests;

public class CustomRegisterRequest
{
    /// <summary>
    /// The user's email address which acts as a user name.
    /// </summary>
    public required string Email { get; init; }
    
    /// <summary>
    /// The user's name.
    /// </summary>
    public required string Name { get; init; }
    
    /// <summary>
    /// The user's birth date which acts as a user name.
    /// </summary>
    public required DateTime BirthDate { get; init; }

    /// <summary>
    /// The user's password.
    /// </summary>
    public required string Password { get; init; }
}