using ClickMarket.Api.Extensions;
using ClickMarket.Api.ViewModels;
using ClickMarket.Business.Interfaces;
using ClickMarket.Business.Requests;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ClickMarket.Api.Controllers;

[ApiController]
[Route("api/conta")]
public class AuthController(SignInManager<IdentityUser> signInManager,
                       UserManager<IdentityUser> userManager,
                       IOptions<JwtSettings> jwtSettings,
                       IClienteService clienteService,
                       INotificador notificador,
                       IUser user) : MainController(notificador, user)
{
    private readonly SignInManager<IdentityUser> _signInManager = signInManager;
    private readonly UserManager<IdentityUser> _userManager = userManager;
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;
    private readonly IClienteService _clienteService = clienteService;

    [HttpPost("registrar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> Registrar(RegisterUserViewModel registerUserViewModel)
    {
        if (!ModelState.IsValid)
        {
            return CustomResponse(ModelState);
        }

        var user = new IdentityUser()
        {
            UserName = registerUserViewModel.Email,
            Email = registerUserViewModel.Email,
            EmailConfirmed = true
        };

        IdentityResult result = await _userManager.CreateAsync(user, registerUserViewModel.Password);
        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(user, false);

            await _userManager.AddToRoleAsync(user, "Cliente");

            var cliente = await _clienteService.Adicionar(new ClienteRequest
            {
                Id = Guid.Parse(user.Id),
                Nome = registerUserViewModel.Name,
                Email = registerUserViewModel.Email
            });

            if (cliente == null)
            {
                AdicionarErroProcessamento("Ocorreu um erro ao salvar os dados do Cliente.");
                await _userManager.DeleteAsync(user);
                return CustomResponse();
            }

            return CustomResponse(GerarJwt(registerUserViewModel.Email));
        }

        var errors = result.Errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.Description).ToArray()
                    );

        return CustomResponse(errors);
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> Login(UserLoginViewModel userLoginViewModel)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(new ValidationProblemDetails(ModelState)
            { Title = "Ocorreu um erro ao cadastrar o usuário!" });


        var result = await _signInManager.PasswordSignInAsync(userLoginViewModel.Email, userLoginViewModel.Password, false, true);

        if (!result.Succeeded)
        {
            AdicionarErroProcessamento("Usuário ou senha inválidos!");
            return CustomResponse();
        }

        return CustomResponse(GerarJwt(userLoginViewModel.Email));
    }

    private string GerarJwt(string email)
    {
        var user = _userManager.FindByEmailAsync(email).Result;

        var claims = CarregarClaimsUsuario(user).Result;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Segredo ?? string.Empty);

        var token = tokenHandler.CreateToken(new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = _jwtSettings.Emissor,
            Audience = _jwtSettings.Audiencia,
            Expires = DateTime.UtcNow.AddHours(_jwtSettings.ExpiracaoHoras),
            SigningCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(
                new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)

        });

        var encodedToken = tokenHandler.WriteToken(token);
        return encodedToken;
    }

    private async Task<IList<Claim>> CarregarClaimsUsuario(IdentityUser user)
    {
        var claims = await _userManager.GetClaimsAsync(user);
        var userRoles = await _userManager.GetRolesAsync(user);

        claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
        claims.Add(new Claim(ClaimTypes.Email, user.Email));
        claims.Add(new Claim(ClaimTypes.Role, userRoles.FirstOrDefault() ?? string.Empty));

        return claims;
    }
}
