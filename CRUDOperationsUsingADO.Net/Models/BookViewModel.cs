using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CRUDOperationsUsingADO.Net.Models
{
    public class BookViewModel
    {
        [Required]
        [Key]
        public int BookID { get; set; }

        [Required(ErrorMessage ="This Field is required")]
        [DisplayName("Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "This Field is required")]
        [DisplayName("Author")]
        public string Author { get; set; }

        [Required(ErrorMessage = "This Field is required")]
        [DisplayName("Price")]
        [Range(1, int.MaxValue,ErrorMessage = "Should be greater than 1 or equal to 1")]
        public int Price { get; set; }
    }
}
