using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaldykNews.WebUI.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [DisplayName("Порядок вывода")]
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Порядок вывода должен быть больше 0")]
        public int DisplayOrder { get; set; }
    }
}
