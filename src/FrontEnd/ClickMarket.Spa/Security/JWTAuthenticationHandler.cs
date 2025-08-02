using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace ClickMarket.Spa.Security;

public class JWTAuthenticationHandler : AuthenticationHandler<CustomOptions>
{
    public JWTAuthenticationHandler(IOptionsMonitor<CustomOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        try
        {
            var token = Request.Cookies["access_token"];
            if (string.IsNullOrWhiteSpace(token))
                return AuthenticateResult.NoResult();


            var lerJwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var identiy = new ClaimsIdentity(lerJwt.Claims, "jwt");
            var principal = new ClaimsPrincipal(identiy);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
        catch (Exception ex)
        {
            return AuthenticateResult.NoResult();
        }
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.Redirect("/login");
        return Task.CompletedTask;
    }

    protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
    {
        Response.Redirect("/access-denied");
        return Task.CompletedTask;
    }
}

public class CustomOptions : AuthenticationSchemeOptions
{

}
