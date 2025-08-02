using ClickMarket.Business.Dtos;
using ClickMarket.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClickMarket.Api.Controllers
{
    [Authorize(Roles = "Administrador")]
    [ApiController]
    [Route("api/clientes")]
    public class ClientesController(IClienteService clienteService, INotificador notificador, IUser user) : MainController(notificador, user)
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ClienteDto>))]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<IEnumerable<ClienteDto>>> ObterTodos()
        {
            var clientes = await clienteService.ObterTodos();

            return CustomResponse(clientes);
        }

        [HttpGet("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ClienteDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            var cliente = await clienteService.ObterPorId(id);

            if (cliente == null)
            {
                return NotFound();
            }

            return CustomResponse(cliente);
        }
    }

}
