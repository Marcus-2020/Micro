using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Security.Claims;
using FluentResults;
using Micro.Identity.Data;
using Micro.Identity.Entities;
using Micro.Identity.Providers;
using Micro.Identity.Requests;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace Micro.Identity.Extensions;

public static class CustomIdentityApiEndpointRouteBuilderExtensions
{
    // Validate the email address using DataAnnotations like the UserValidator does when RequireUniqueEmail = true.
    private static readonly EmailAddressAttribute _emailAddressAttribute = new();

    /// <summary>
    /// Add endpoints for registering, logging in, and logging out using ASP.NET Core Identity.
    /// </summary>
    /// <typeparam name="TUser">The type describing the user. This should match the generic parameter in <see cref="UserManager{TUser}"/>.</typeparam>
    /// <param name="endpoints">
    /// The <see cref="IEndpointRouteBuilder"/> to add the identity endpoints to.
    /// Call <see cref="EndpointRouteBuilderExtensions.MapGroup(IEndpointRouteBuilder, string)"/> to add a prefix to all the endpoints.
    /// </param>
    /// <returns>An <see cref="IEndpointConventionBuilder"/> to further customize the added endpoints.</returns>
    public static IEndpointConventionBuilder MapCustomIdentityApi<TUser>(this IEndpointRouteBuilder endpoints)
        where TUser : class, new()
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        // We'll figure out a unique endpoint name based on the final route pattern during endpoint generation.
        string? confirmEmailEndpointName = null;

        var routeGroup = endpoints.MapGroup("");

        routeGroup.MapPost("/register", async Task<Results<Ok, ValidationProblem>>
            ([FromBody] CustomRegisterRequest registration, HttpContext context, [FromServices] IServiceProvider sp) =>
        {
            var userManager = sp.GetRequiredService<UserManager<TUser>>();
            var dbContext = sp.GetRequiredService<ApplicationDbContext>();

            if (!userManager.SupportsUserEmail)
            {
                throw new NotSupportedException($"{nameof(MapCustomIdentityApi)} requires a user store with email support.");
            }

            var userStore = sp.GetRequiredService<IUserStore<TUser>>();
            var emailStore = (IUserEmailStore<TUser>)userStore;
            var email = registration.Email;

            if (string.IsNullOrEmpty(email) || !_emailAddressAttribute.IsValid(email))
            {
                return CreateValidationProblem(IdentityResult.Failed(userManager.ErrorDescriber.InvalidEmail(email)));
            }

            var user = new TUser();
            await userStore.SetUserNameAsync(user, email, CancellationToken.None);
            await emailStore.SetEmailAsync(user, email, CancellationToken.None);
            var result = await userManager.CreateAsync(user, registration.Password);

            if (!result.Succeeded)
            {
                return CreateValidationProblem(result);
            }

            await InsertAppUserAsync(registration, dbContext, (user as IdentityUser).Id);

            return TypedResults.Ok();
        });

        routeGroup.MapPost("/login", async Task<Results<Ok<object>, EmptyHttpResult, ProblemHttpResult>>
            ([FromBody] CustomLoginRequest login, [FromServices] IServiceProvider sp, [FromServices] IConfiguration configuration) =>
        {
            var signInManager = sp.GetRequiredService<SignInManager<TUser>>();
            var userManager = sp.GetRequiredService<UserManager<TUser>>();
            var tokenProvider = sp.GetRequiredService<TokenProvider>();
            var dbContext = sp.GetRequiredService<ApplicationDbContext>();

            var user = await userManager.FindByEmailAsync(login.Email);
            if (user is null)
            {
                return TypedResults.Problem("Email or password was incorrect", statusCode: StatusCodes.Status401Unauthorized);
            }

            var result = await signInManager.PasswordSignInAsync(user, login.Password, false, false);
            if (!result.Succeeded)
            {
                return TypedResults.Problem("Email or password was incorrect", statusCode: StatusCodes.Status401Unauthorized);
            }

            var getUser = await GetAppUserAsync((user as IdentityUser).Id, dbContext);
            if (getUser.IsFailed)
            {
                return TypedResults.Problem(getUser.Errors.First().Message, statusCode: StatusCodes.Status500InternalServerError);
            }

            if (getUser.ValueOrDefault is null)
            {
                await InsertAppUserAsync(new CustomRegisterRequest
                {
                    Name = (user as IdentityUser).UserName,
                    Email = (user as IdentityUser).Email,
                    BirthDate = DateTime.UtcNow.AddYears(-18),
                    Password = string.Empty,
                }, dbContext, (user as IdentityUser).Id);
            }

            var tokenResult = tokenProvider.CreateToken(user as IdentityUser);
            return TypedResults.Ok<object>(new
            {
                accessToken = tokenResult.Token,
                expiresIn = tokenResult.ExpiresInSeconds,
            });
        });
        

        var accountGroup = routeGroup.MapGroup("/manage").RequireAuthorization();
        
        accountGroup.MapGet("/info", async Task<Results<Ok<InfoResponse>, ValidationProblem, NotFound>>
            (ClaimsPrincipal claimsPrincipal, [FromServices] IServiceProvider sp) =>
        {
            var userManager = sp.GetRequiredService<UserManager<TUser>>();
            if (await userManager.GetUserAsync(claimsPrincipal) is not { } user)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(await CreateInfoResponseAsync(user, userManager));
        });

        accountGroup.MapPatch("/update-me", async Task<Results<Ok, ProblemHttpResult>>
            ([FromBody] UpdateMeRequest update, HttpContext context, [FromServices] IServiceProvider sp) =>
        {
            var userManager = sp.GetRequiredService<UserManager<TUser>>();
            var dbContext = sp.GetRequiredService<ApplicationDbContext>();

            var authenticatedUserId =
                context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value;

            var user = await userManager.FindByIdAsync(authenticatedUserId);
            if (user is null)
            {
                return TypedResults.Problem("User not found", statusCode: StatusCodes.Status404NotFound);
            }

            var getUser = await GetAppUserAsync((user as IdentityUser).Id, dbContext);
            if (getUser.IsFailed)
            {
                return TypedResults.Problem(getUser.Errors[0].Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }

            if (getUser.ValueOrDefault is null)
            {
                return TypedResults.Problem("User not found", statusCode: StatusCodes.Status404NotFound);
            }

            var appUser = getUser.Value!;
            appUser.Name = update.Name;
            if (update.BirthDate is not null &&
                update.BirthDate.Value > DateTime.UtcNow.Date.AddYears(-100) &&
                update.BirthDate.Value < DateTime.UtcNow.Date)
            {
                appUser.BirthDate = update.BirthDate.Value.Date.ToUniversalTime();
            }

            var updateResult = await UpdateAppUserAsync(appUser, dbContext);
            if (updateResult.IsFailed)
            {
                return TypedResults.Problem(getUser.Errors[0].Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }

            return TypedResults.Ok();
        });

        return new IdentityEndpointsConventionBuilder(routeGroup);
    }
    
    private static async Task InsertAppUserAsync(CustomRegisterRequest registerRequest, ApplicationDbContext context,
        string identityUserId)
    {
        var logger = Serilog.Log.Logger.ForContext<Program>();
        try
        {
            await context.Users.AddAsync(new User
            {
                Id = Guid.NewGuid(),
                Name = registerRequest.Name,
                Email = registerRequest.Email,
                BirthDate = registerRequest.BirthDate,
                IdentityUserId = identityUserId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
            });
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logger.Error(ex, "An error occurred while trying to insert the application user");
        }
    }
    
    private static async Task<Result> UpdateAppUserAsync(User user, ApplicationDbContext context)
    {
        var logger = Serilog.Log.Logger.ForContext<Program>();
        try
        {
            context.Users.Update(user);
            await context.SaveChangesAsync();
            return Result.Ok();
        }
        catch (Exception ex)
        {
            logger.Error(ex, "An error occurred while trying to update the application user");
            return Result.Fail("An error occurred while trying to update the application user");
        }
    }

    private static async Task<Result<User?>> GetAppUserAsync(string identityUserId, ApplicationDbContext dbContext)
    {
        var logger = Serilog.Log.Logger.ForContext<Program>();
        try
        {
            return await dbContext.Users.FirstOrDefaultAsync(x => x.IdentityUserId == identityUserId);
        }
        catch (Exception ex)
        {
            logger.Error(ex, "An error occurred while trying to recover the application user");
            return Result.Fail("An error occured when trying to recover the application user");
        }
    }

    private static ValidationProblem CreateValidationProblem(IdentityResult result)
    {
        // We expect a single error code and description in the normal case.
        // This could be golfed with GroupBy and ToDictionary, but perf! :P
        Debug.Assert(!result.Succeeded);
        var errorDictionary = new Dictionary<string, string[]>(1);

        foreach (var error in result.Errors)
        {
            string[] newDescriptions;

            if (errorDictionary.TryGetValue(error.Code, out var descriptions))
            {
                newDescriptions = new string[descriptions.Length + 1];
                Array.Copy(descriptions, newDescriptions, descriptions.Length);
                newDescriptions[descriptions.Length] = error.Description;
            }
            else
            {
                newDescriptions = [error.Description];
            }

            errorDictionary[error.Code] = newDescriptions;
        }

        return TypedResults.ValidationProblem(errorDictionary);
    }

    private static async Task<InfoResponse> CreateInfoResponseAsync<TUser>(TUser user, UserManager<TUser> userManager)
        where TUser : class
    {
        return new()
        {
            Email = await userManager.GetEmailAsync(user) ?? throw new NotSupportedException("Users must have an email."),
            IsEmailConfirmed = await userManager.IsEmailConfirmedAsync(user),
        };
    }

    // Wrap RouteGroupBuilder with a non-public type to avoid a potential future behavioral breaking change.
    private sealed class IdentityEndpointsConventionBuilder(RouteGroupBuilder inner) : IEndpointConventionBuilder
    {
        private IEndpointConventionBuilder InnerAsConventionBuilder => inner;

        public void Add(Action<EndpointBuilder> convention) => InnerAsConventionBuilder.Add(convention);
        public void Finally(Action<EndpointBuilder> finallyConvention) => InnerAsConventionBuilder.Finally(finallyConvention);
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    private sealed class FromBodyAttribute : Attribute, IFromBodyMetadata
    {
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    private sealed class FromServicesAttribute : Attribute, IFromServiceMetadata
    {
    }
}