namespace ClickMarket.Business.Models;

public class Cliente : EntityBase
{
    public string Nome { get; set; }
    public string Email { get; set; }
    public string Telefone { get; set; }
    public bool Ativo { get; set; } = true;
}
