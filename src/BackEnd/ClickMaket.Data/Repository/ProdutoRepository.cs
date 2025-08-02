using ClickMarket.Business.Interfaces;
using ClickMarket.Business.Models;
using ClickMarket.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace ClickMarket.Data.Repository
{
    public class ProdutoRepository(ClickDbContext clickDbContext) : Repository<Produto>(clickDbContext), IProdutoRepository
    {
        public async Task<IEnumerable<Produto>> ObterProdutoCategoria(Guid? clienteId = null)
        {
            return await _dbSet.AsNoTracking()
                    .Include(x => x.Categoria)
                    .Include(x => x.Vendedor)
                    .Include(x => x.Favorito)
                    .Where(x => !clienteId.HasValue || x.Favorito.ClienteId == clienteId || x.Favorito == null)
                    .ToListAsync();
        }

        public async Task<Produto> ObterProdutoCategoria(Guid id)
        {
            return await _dbSet
                        .AsNoTracking()
                        .Include(x => x.Categoria)
                        .Include(v => v.Vendedor)
                        .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Produto>> ObterProdutoCategoriaPorVendedor(Guid vendedorId)
        {
            return await _dbSet
                        .AsNoTracking()
                        .Include(x => x.Categoria)
                        .Include(v => v.Vendedor)
                        .Where(x => x.VendedorId == vendedorId).ToListAsync();
        }

        public async Task<IEnumerable<Produto>> ObterProdutosPorCategoriaId(Guid categoriaId)
        {
            return await _dbSet
                        .AsNoTracking()
                        .Include(x => x.Categoria)
                        .Include(v => v.Vendedor)
                        .Where(x => x.CategoriaId == categoriaId).ToListAsync();
        }

        public async Task<List<Produto>> ObterTodosIncluindoFavoritos(Guid? clienteId = null)
        {
            var produtos = new List<Produto>();
            if (clienteId.HasValue)
            {
                var produtosFavoritados = await _dbSet
                                        .AsNoTracking()
                                        .Include(x => x.Favorito)
                                        .Where(a => a.Favorito.ClienteId == clienteId)
                                        .ToListAsync();

                var idsFavoritados = produtosFavoritados.Select(x => x.Id).ToList();

                var produtosTotais = await _dbSet
                                        .AsNoTracking()
                                        .Where(a => !idsFavoritados.Contains(a.Id))
                                        .ToListAsync();

                produtos.AddRange(produtosFavoritados);
                produtos.AddRange(produtosTotais);

                return [.. produtos.OrderBy(a => a.Nome)];
            }

            produtos.AddRange(await _dbSet
                                        .AsNoTracking()
                                        .ToListAsync());

            return produtos;
        }

        public async Task<List<Produto>> ObterTodosApenasFavoritos(Guid clienteId)
        {
            if (clienteId == Guid.Empty)
                return [];

            return await _dbSet
                            .AsNoTracking()
                            .Include(x => x.Favorito)
                            .Where(a => a.Favorito.ClienteId == clienteId)
                            .ToListAsync();
        }

        public async Task<Produto> ObterProdutoFavorito(Guid produtoId, Guid clienteId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(x => x.Favorito)
                .FirstOrDefaultAsync(a => a.Favorito.ClienteId == clienteId && a.Id == produtoId);
        }
    }
}
