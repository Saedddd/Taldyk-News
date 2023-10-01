using Microsoft.Build.Framework;
using TaldykNews.Models;

namespace TaldykNews.WebUI.Models
{
    public class Role
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public List<User> Users { get; set; }
        public Role()
        {
            Users = new List<User>();
        }
    }
}
