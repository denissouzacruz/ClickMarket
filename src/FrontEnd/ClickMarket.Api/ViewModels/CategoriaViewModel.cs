using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ClickMarket.Api.ViewModels
{
    public class CategoriaViewModel
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

        public CategoriaViewModel()
        {
            Id = Guid.NewGuid();
        }
    }
}
