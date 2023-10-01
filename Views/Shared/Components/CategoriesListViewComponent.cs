using Microsoft.AspNetCore.Mvc;
using TaldykNews.Data;

namespace TaldykNews.WebUI.Views.Shared.Components
{
    public class CategoriesListViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;
        public CategoriesListViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            IEnumerable<Models.Category> CategoriesList = _db.Categories.OrderBy(u => u.DisplayOrder);
            return View(CategoriesList);
        }
    }
}
