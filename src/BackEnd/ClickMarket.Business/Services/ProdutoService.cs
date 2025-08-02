using AutoMapper;
using ClickMarket.Business.Dtos;
using ClickMarket.Business.Interfaces;
using ClickMarket.Business.Models;

namespace ClickMarket.Business.Services;

public class ProdutoService(
    INotificador notificador,
    IProdutoRepository produtoRepository,
    IFavoritoRepository favoritoRepository,
    IMapper mapper) : BaseService(notificador), IProdutoService
{
    public async Task Adicionar(Produto produto)
    {
        await produtoRepository.Adicionar(produto);
    }

    public async Task Atualizar(Produto produto)
    {
        await produtoRepository.Atualizar(produto);
    }

    public async Task Remover(Guid id)
    {
        await produtoRepository.Remover(id);
    }

    public async Task<Favorito> AdicionarFavorito(Guid produtoId, Guid clienteId)
    {
        var favorito = new Favorito
        {
            Id = Guid.NewGuid(),
            ProdutoId = produtoId,
            ClienteId = clienteId
        };

        var modelExistente = await produtoRepository.ObterProdutoFavorito(produtoId, clienteId);

        if (modelExistente != null)
        {
            Notificar("Este produto já está na lista de favoritos.");
            return null;
        }

        var produto = await produtoRepository.ObterPorId(produtoId);

        if (produto == null)
        {
            Notificar("Produto não encontrado ou não está ativo.");
            return null;
        }

        await favoritoRepository.Adicionar(favorito);

        return favorito;
    }

    public async Task RemoverFavorito(Guid produtoId, Guid clienteId)
    {
        var modelExistente = await produtoRepository.ObterProdutoFavorito(produtoId, clienteId);

        if (modelExistente == null)
        {
            Notificar("Favorito não encontrado.");
            return;
        }

        await favoritoRepository.Remover(modelExistente.Favorito.Id);
    }

    public void Dispose()
    {
        produtoRepository?.Dispose();
        GC.SuppressFinalize(this);
    }

    public async Task Ativar(Guid id)
    {
        var produto = await produtoRepository.ObterPorId(id);
        if (produto == null)
        {
            Notificar("Produto não encontrado.");
            return;
        }
        produto.Ativo = true;
        await produtoRepository.Atualizar(produto);
    }

    public async Task Inativar(Guid id)
    {
        var produto = await produtoRepository.ObterPorId(id);
        if (produto == null)
        {
            Notificar("Produto não encontrado.");
            return;
        }
        produto.Ativo = false;
        await produtoRepository.Atualizar(produto);
    }
}
