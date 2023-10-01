using System.ComponentModel.DataAnnotations;

namespace TaldykNews.WebUI.Models.ViewModel.News
{
    public class ArticleVM
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        public string Image { get; set; }
        [Required]
        public string Text { get; set; }
        [Required]
        public string PreviewText { get; set; }
        [Required]
        public DateTime CreationDate { get; set; }
        public IEnumerable<TaldykNews.Models.News> SidebarList { get; set; }
    }
}
