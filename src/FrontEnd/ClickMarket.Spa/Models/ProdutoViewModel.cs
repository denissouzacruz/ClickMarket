using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ClickMarket.Spa.Models;

public class ProdutoViewModel
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string Descricao { get; set; }
    public decimal Valor { get; set; }
    public string Imagem { get; set; }
    public Guid CategoriaId { get; set; }
    public bool NaListaDesejos { get; set; }
    public int? QuantidadeEstoque { get; set; }
}