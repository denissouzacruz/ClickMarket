using System.ComponentModel.DataAnnotations;

namespace ClickMarket.Api.ViewModels
{
    public class UserLoginViewModel
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório!")]
        [EmailAddress(ErrorMessage = "O campo {0} é inválido!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório!")]
        [StringLength(100, ErrorMessage = "O campo {0} precisa estar entre {2} e {1} caracteres!", MinimumLength = 6)]
        public string Password { get; set; }
    }
}
