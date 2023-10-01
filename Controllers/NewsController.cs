using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaldykNews.Data;
using TaldykNews.Models;
using TaldykNews.WebUI.Models;
using TaldykNews.WebUI.Models.ViewModel.News;

namespace TaldykNews.Controllers
{
    public class NewsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public NewsController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]
        public IActionResult Index()
        {
            IndexVM indexVM = new()
            {
                News = _db.News.Include(x => x.Author).Include(x => x.Category)
            };
            return View(indexVM);
        }
        [HttpGet]
        public IActionResult Article(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }
            var obj = _db.News
                .Include(x => x.Author)
                .Include(x => x.Category)
                .FirstOrDefault(x => x.Id == Id);
            if (obj == null)
            {
                return NotFound();
            }
            IEnumerable<News> news = _db.News
                .Where(x => x.Id != Id)
                .Take(4);
            ArticleVM articleVM = new ArticleVM()
            {
                Author = obj.Author.FullName,
                Title = obj.Title,
                Image = obj.Image,
                Text = obj.Text,
                PreviewText = obj.PreviewText,
                CreationDate = obj.CreationDate,
                SidebarList = news
            };
            return View(articleVM);
        }

        [HttpGet]
        public IActionResult ArticleByCategory(string category)
        {
            var obj = _db.News
                .OrderByDescending(x => x.CreationDate)
                .Include(x => x.Category)
                .Include(x => x.Author)
                .FirstOrDefault(x => x.Category.Name == category);
            if (obj == null)
            {
                return NotFound();
            }
            IEnumerable<News> news = _db.News
                .Where(x => x != obj)
                .Take(4);
            ArticleVM articleVM = new ArticleVM()
            {
                Author = obj.Author.FullName,
                Title = obj.Title,
                Image = obj.Image,
                Text = obj.Text,
                PreviewText = obj.PreviewText,
                CreationDate = obj.CreationDate,
                SidebarList = news
            };
            return View(articleVM);
        }
        public IActionResult NewsList(string sortOrder = "", int page = 1, int pageSize = 10)
        {
            NewsListVM newsListVM = new()
            {
                Pages = (_db.News.Count() + pageSize - 1) / pageSize,
                CurrentPage = page
            };
            if (page > 1)
            {
                newsListVM.News = _db.News.Include(u => u.Category)
                    .Include(u => u.Author)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize);
            }
            else
            {
                newsListVM.News = _db.News
                    .Include(u => u.Category)
                    .Include(u => u.Author)
                    .Take(pageSize);
            }
            switch (sortOrder)
            {
                case "Category":
                    newsListVM.News = newsListVM.News.OrderByDescending(u => u.Category.Name);
                    break;
                case "Author":
                    newsListVM.News = newsListVM.News.OrderByDescending(u => u.Author.FullName);
                    break;
                case "Date":
                    newsListVM.News = newsListVM.News.OrderByDescending(u => u.CreationDate);
                    break;
                default:
                    break;
            }
            return View(newsListVM);
        }

        [HttpGet]
        public IActionResult Upsert(int? id)
        {            
            UpsertVM obj = new UpsertVM();
            obj.CategoriesList = _db.Categories;
            if (id == null || id == 0)
            {
                obj.Article = new News();
                obj.Article.CreationDate = DateTime.Now;
                obj.Article.Author = _db.Users.FirstOrDefault(x => x.Email == User.Identity.Name);
                obj.Article.AuthorId = obj.Article.Author.Id;
                return View(obj);
            }
            else
            {
                obj.Article = _db.News.Include(u => u.Author).FirstOrDefault(u => u.Id == id);
                if (obj == null)
                {
                    return NotFound();
                }
                if (!(obj.Article.Author.Email == HttpContext.User.Identity.Name) || HttpContext.User.IsInRole("Admin"))
                {
                    return Forbid();
                }
                return View(obj);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(UpsertVM obj)
        {
            if (ModelState.IsValid)
            {
                obj.Article.Author = _db.Users.Find(obj.Article.AuthorId);
                obj.Article.Category = _db.Categories.Find(obj.Article.CategoryId);
                if (obj.Article.Author.Email != HttpContext.User.Identity.Name || !HttpContext.User.IsInRole("Admin"))
                {
                    return Forbid();
                }
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;
                if (obj.Article.Id == 0)
                {
                    string upload = webRootPath + WebConstants.ImagePath;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);
                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }
                    obj.Article.Image = fileName + extension;
                    obj.Article.CreationDate = DateTime.Now;
                    _db.News.Add(obj.Article);
                }
                else
                {
                    var objFromDb = _db.News.AsNoTracking().FirstOrDefault(x => x.Id == obj.Article.Id);
                    if (files.Count > 0)
                    {
                        string upload = webRootPath + WebConstants.ImagePath;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);
                        var oldFile = Path.Combine(upload, objFromDb.Image);
                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }
                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }
                        obj.Article.Image = fileName + extension;
                    }
                    else
                    {
                        obj.Article.Image = objFromDb.Image;
                    }
                    _db.News.Update(obj.Article);
                }
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _db.News
                .Include(u => u.Author)
                .FirstOrDefault(x=>x.Id==id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult DeletePost(int? id)
        {
            var obj = _db.News.Include(u => u.Author).FirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            if (!(obj.Author.Email == HttpContext.User.Identity.Name || HttpContext.User.IsInRole("Admin")))
            {
                return Forbid();
            }
            string upload = _webHostEnvironment.WebRootPath + WebConstants.ImagePath;
            var oldFile = Path.Combine(upload, obj.Image);
            if (System.IO.File.Exists(oldFile))
            {
                System.IO.File.Delete(oldFile);
            }
            _db.News.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
