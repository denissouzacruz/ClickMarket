﻿namespace ClickMarket.Spa.Models;

public class ProdutoListViewModel
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string Descricao { get; set; }
    public decimal Valor { get; set; }
    public string Imagem { get; set; }
    public Guid CategoriaId { get; set; }
    public bool AddListaDesejos { get; set; }
}