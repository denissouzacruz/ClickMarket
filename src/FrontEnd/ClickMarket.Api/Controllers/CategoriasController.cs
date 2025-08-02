using AutoMapper;
using ClickMarket.Api.ViewModels;
using ClickMarket.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClickMarket.Api.Controllers
{
    [Authorize(Roles = "Administrador")]
    [ApiController]
    [Route("api/categorias")]
    public class CategoriasController(ICategoriaRepository categoriaRepository,
                                ICategoriaService categoriaService,
                                IMapper mapper,
                                IUser user,
                                INotificador notificador) : MainController(notificador, user)
    {
        private readonly ICategoriaRepository _categoriaRepository = categoriaRepository;
        private readonly IMapper _mapper = mapper;

        [AllowAnonymous]
        [HttpGet("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<CategoriaViewModel>> ObterPorId(Guid id)
        {
            var categoria = await _categoriaRepository.ObterPorId(id);

            if (categoria == null)
            {
                return NotFound();
            }

            var categoriaModel = _mapper.Map<CategoriaViewModel>(categoria);
            return categoriaModel;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaViewModel>>> ObterTodas()
        {
            var categorias = await _categoriaRepository.ObterTodos();
            var categoriasModel = _mapper.Map<IEnumerable<CategoriaViewModel>>(categorias);

            return categoriasModel.ToList();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<CategoriaViewModel>> Criar(CategoriaViewModel categoria)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(new ValidationProblemDetails(ModelState)
                {
                    Title = "Ocorreu um ou mais erros ao tentar inserir a categoria!"
                });
            }

            var categoriaModel = _mapper.Map<Business.Models.Categoria>(categoria);
            await _categoriaRepository.Adicionar(categoriaModel);

            return CreatedAtAction(nameof(ObterPorId), new { categoriaModel.Id }, categoria);
        }

        [HttpPut("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Alterar(Guid id, CategoriaViewModel categoria)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(new ValidationProblemDetails(ModelState)
                {
                    Title = "Ocorreu um ou mais erros ao tentar editar a categoria!"
                });
            }

            var categoriaBd = await _categoriaRepository.ObterPorId(id);
            if (categoriaBd == null)
                return NotFound();

            _mapper.Map(categoria, categoriaBd);

            try
            {
                await _categoriaRepository.Atualizar(categoriaBd);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (await _categoriaRepository.ObterPorId(id) == null)
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Remover(Guid id)
        {
            var categoriaBd = await _categoriaRepository.ObterCategoriaProduto(id);
            if (categoriaBd == null)
                return NotFound();

            await categoriaService.Remover(id);

            if (!OperacaoValida())
                return CustomResponse();

            return NoContent();
        }
    }

}
