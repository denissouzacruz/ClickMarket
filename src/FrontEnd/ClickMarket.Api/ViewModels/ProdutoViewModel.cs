using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ClickMarket.Api.ViewModels
{
    public class ProdutoViewModel
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Preencha o campo {0}.")]
        [StringLength(100, ErrorMessage = "O campo {0} precisa ter no mínimo {2} caracteres e no máximo {1}", MinimumLength = 2)]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Preencha o campo {0}.")]
        [StringLength(200, ErrorMessage = "O campo {0} precisa ter no mínimo {2} caracteres e no máximo {1}", MinimumLength = 2)]
        [DisplayName("Descrição")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [DisplayName("Categoria")]
        public Guid CategoriaId { get; set; }

        [Required(ErrorMessage = "Preencha o campo {0}.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor precisa ser maior que 0")]
        [DisplayName("Preço")]
        public decimal Valor { get; set; }

        [Required(ErrorMessage = "Preencha o campo {0}.")]
        [Range(1, int.MaxValue, ErrorMessage = "O valor precisa ser maior que 0")]
        [DisplayName("Qtd em Estoque")]
        public int? QuantidadeEstoque { get; set; }

        [Required(ErrorMessage = "Preencha o campo {0}.")]
        public IFormFile ImagemUpload { get; set; }
        public string Imagem { get; set; }
        public bool NaListaDesejos { get; set; }
        public CategoriaViewModel Categoria { get; set; }
        public VendedorViewModel Vendedor { get; set; }

    }
}
