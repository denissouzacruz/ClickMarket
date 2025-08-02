using ClickMarket.Business.Interfaces;
using ClickMarket.Business.Models;

namespace ClickMarket.Business.Services
{
    public class CategoriaService(INotificador notificador,
        ICategoriaRepository categoriaRepository,
        IProdutoRepository produtoRepository) : BaseService(notificador), ICategoriaService
    {
        private readonly ICategoriaRepository _categoriaRepository = categoriaRepository;
        private readonly IProdutoRepository _produtoRepository = produtoRepository;

        public async Task Adicionar(Categoria categoria)
        {
            //regra de negócio
            await _categoriaRepository.Adicionar(categoria);
        }

        public async Task Atualizar(Categoria categoria)
        {
            //Regra de negócio
            await _categoriaRepository.Atualizar(categoria);
        }

        public async Task Remover(Guid id)
        {
            var produtos = await _produtoRepository.ObterProdutosPorCategoriaId(id);

            if(produtos.Any())
            {
                Notificar("Não é possível remover essa categoria, pois te produtos adicionados.");
                return;
            }

            await _categoriaRepository.Remover(id);
        }

        public void Dispose()
        {
            _categoriaRepository?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
