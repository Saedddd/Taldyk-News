namespace TaldykNews.WebUI.Models.ViewModel.News
{
    public class NewsListVM
    {
        public int CurrentPage { get; set; }
        public int Pages { get; set; }
        public IEnumerable<TaldykNews.Models.News> News { get; set; }
    }
}
