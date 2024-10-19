using System.ComponentModel.DataAnnotations;

namespace Micro.Identity.Requests;

public record CustomLoginRequest(
    [property:Required(ErrorMessage = "Email is required for login")]
    [property:EmailAddress(ErrorMessage = "Must be a valid e-mail address")]
    string Email, 
    [property:Required(ErrorMessage = "Password is required for login")]
    string Password);