using System.ComponentModel.DataAnnotations;

namespace ClickMarket.Api.ViewModels
{
    public class VendedorViewModel
    {
        public VendedorViewModel()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
    }
}
