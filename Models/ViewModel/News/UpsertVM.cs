namespace TaldykNews.WebUI.Models.ViewModel.News
{
    public class UpsertVM
    {
        public TaldykNews.Models.News Article { get; set; }
        public IEnumerable<Models.Category> CategoriesList { get; set; }
    }
}
