using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaldykNews.Data;
using TaldykNews.Models;
using TaldykNews.WebUI.Models;
using TaldykNews.WebUI.Models.ViewModel;
using TaldykNews.WebUI.Models.ViewModel.Category;
using TaldykNews.WebUI.Models.ViewModel.News;
using IndexVM = TaldykNews.WebUI.Models.ViewModel.Category.IndexVM;

namespace TaldykNews.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public CategoryController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            IEnumerable<Category> categories = _db.Categories;
            IndexVM indexVM = new IndexVM()
            {
                Categories = categories
            };
            return View(indexVM);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category obj)
        {
            if (ModelState.IsValid)
            {
                var name = _db.Categories.FirstOrDefault(u => u.Name == obj.Name);
                if (name == null)
                {
                    _db.Categories.Add(obj);
                    _db.SaveChanges();
                }
                else
                {
                    return View(obj);
                }
            }
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _db.Categories.FirstOrDefault(x=>x.Id==id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = _db.Categories.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            _db.Categories.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
