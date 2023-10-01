using TaldykNews.Models;

namespace TaldykNews.WebUI.Models.ViewModel.Account
{
    public class UsersVM
    {
        public int CurrentPage { get; set; }
        public int Pages { get; set; }
        public IEnumerable<User> Users { get; set; }
    }
}
