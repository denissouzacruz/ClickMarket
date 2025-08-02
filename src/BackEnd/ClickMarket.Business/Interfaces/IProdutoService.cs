using ClickMarket.Business.Dtos;
using ClickMarket.Business.Models;

namespace ClickMarket.Business.Interfaces;

public interface IProdutoService : IDisposable
{
    public Task Adicionar(Produto produto);
    public Task Remover(Guid id);
    public Task Atualizar(Produto produto);

    public Task Ativar(Guid id);
    public Task Inativar(Guid id);


    Task<Favorito> AdicionarFavorito(Guid produtoId, Guid clienteId);
    Task RemoverFavorito(Guid produtoId, Guid clienteId);
}
