using ClickMarket.Spa.Models;
using System.Text.Json;

namespace ClickMarket.Spa.Services;

public class ProdutoService(AccessTokenService accessTokenService, IHttpClientFactory httpClientFactory, IWebHostEnvironment webHostEnvironment)
{
    private readonly IWebHostEnvironment _env = webHostEnvironment;
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("ClickMarketAPI");

    public async Task<List<ProdutoViewModel>> ObterProdutos()
    {
        try
        {
            var token = await accessTokenService.ObterToken();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var produtos = await _httpClient.GetFromJsonAsync<List<ProdutoViewModel>>("produtos");
            return produtos ?? [];
        }
        catch
        {
            return [];
        }
    }
    public async Task<List<ProdutoViewModel>> ObterProdutosFavoritos()
    {
        try
        {
            var token = await accessTokenService.ObterToken();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var produtos = await _httpClient.GetFromJsonAsync<List<ProdutoViewModel>>("produtos/favoritos");
            return produtos ?? [];
        }
        catch
        {
            return [];
        }
    }

    public string ObterImagem(string imagem)
    {
        var path = Path.Combine(_env.WebRootPath, @"imagens");

        var fileExists = Path.Exists($"{path}\\{imagem}");

        if (!fileExists)
        {
            return @"imagens/" + Path.GetFileName($"{path}\\no-image.png");
        }

        return @"imagens/" + Path.GetFileName($"{path}\\{imagem}");
    }

    public async Task<RetornoViewModel> SalvarNaLista(Guid produtoId)
    {
        var token = await accessTokenService.ObterToken();

        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.PostAsJsonAsync($"produtos/favoritos/{produtoId}", new { });
        var retornoJson = await response.Content.ReadAsStringAsync();
        try
        {
            var retornoViewModel = JsonSerializer.Deserialize<RetornoViewModel>(retornoJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (!retornoViewModel.Success)
            {
                return new RetornoViewModel
                {
                    Success = false,
                    Errors = retornoViewModel.Errors ?? ["Erro ao registrar o produto como favorito."]
                };
            }

            return retornoViewModel;

        }
        catch
        {
            return new RetornoViewModel
            {
                Success = false,
                Errors = ["Erro ao processar a resposta do servidor."]
            };
        }
    }

    public async Task<RetornoViewModel> RemoverDaLista(Guid produtoId)
    {
        var token = await accessTokenService.ObterToken();
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.DeleteAsync($"produtos/favoritos/{produtoId}");
        var retornoJson = await response.Content.ReadAsStringAsync();
        try
        {
            var retornoViewModel = JsonSerializer.Deserialize<RetornoViewModel>(retornoJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (!retornoViewModel.Success)
            {
                return new RetornoViewModel
                {
                    Success = false,
                    Errors = retornoViewModel.Errors ?? ["Erro ao remover o produto da lista de favoritos."]
                };
            }

            return retornoViewModel;

        }
        catch
        {
            return new RetornoViewModel
            {
                Success = false,
                Errors = ["Erro ao processar a resposta do servidor."]
            };
        }
    }
}
