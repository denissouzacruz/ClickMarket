using ClickMarket.Business.Models;

namespace ClickMarket.Business.Interfaces
{
    public interface IProdutoRepository : IRepository<Produto>
    {
        Task<IEnumerable<Produto>> ObterProdutoCategoria(Guid? clienteId = null);
        Task<Produto> ObterProdutoCategoria(Guid id);
        Task<IEnumerable<Produto>> ObterProdutoCategoriaPorVendedor(Guid vendedorId);
        Task<IEnumerable<Produto>> ObterProdutosPorCategoriaId(Guid categoriaId);
        Task<List<Produto>> ObterTodosIncluindoFavoritos(Guid? clienteId = null);
        Task<List<Produto>> ObterTodosApenasFavoritos(Guid clienteId);
        Task<Produto> ObterProdutoFavorito(Guid produtoId, Guid clienteId);
    }
}
