using ClickMarket.Spa.Services;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ClickMarket.Spa.Security;

public class JWTAuthenticationStateProvider(AccessTokenService accessTokenService) : AuthenticationStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var token = await accessTokenService.ObterToken();

            if (string.IsNullOrWhiteSpace(token))
            {
                return await MarcarComoNaoAutorizado();
            }

            var lerJwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var identiy = new ClaimsIdentity(lerJwt.Claims, "jwt");
            var principal = new ClaimsPrincipal(identiy);

            return await Task.FromResult(new AuthenticationState(principal));
        }
        catch
        {
            return await MarcarComoNaoAutorizado();
        }
    }

    private async Task<AuthenticationState> MarcarComoNaoAutorizado()
    {
        try
        {
            var state = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            NotifyAuthenticationStateChanged(Task.FromResult(state));

            return await Task.FromResult(state);
        }
        catch
        {
            return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
        }
    }
}
