using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaldykNews.Data;
using TaldykNews.Models;
using TaldykNews.WebUI.Models;
using TaldykNews.WebUI.Models.ViewModel.Account;

namespace TaldykNews.WebUI.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        ApplicationDbContext _db;
        public AccountController(ApplicationDbContext context)
        {
            _db = context;
        }
        [HttpGet]
        public IActionResult Index()
        {
            User user = _db.Users
                .Include(u => u.Role)
                .SingleOrDefault(u => u.Email == HttpContext.User.Identity.Name);
            IndexVM userVM = new IndexVM()
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.Name
            };
            return View(userVM);
        }
        [HttpPost]
        public IActionResult Index(IndexVM obj)
        {
            if (ModelState.IsValid & obj.Id != 0)
            {
                User user = _db.Users.Find(obj.Id);
                user.Password = obj.Password;
                _db.SaveChanges();
            }
            return View();
        }
        [HttpGet]
        public IActionResult Users(string sortOrder = "", int page = 1, int pageSize = 10)
        {
            UsersVM usersList = new()
            {
                Pages = (_db.Users.Count() + pageSize - 1)/pageSize,
                CurrentPage = page
            };
            if (page > 1)
            {
                usersList.Users = _db.Users.Include(u => u.Role).Skip((page-1) * pageSize).Take(pageSize);
            }
            else
            {
                usersList.Users = _db.Users.Include(u => u.Role).Take(pageSize);
            }
            switch (sortOrder)
            {
                case "Role":
                    usersList.Users = usersList.Users.OrderByDescending(u => u.Role.Name);
                    break;
                case "Name":
                    usersList.Users = usersList.Users.OrderByDescending(u => u.FullName);
                    break;
                default:
                    break;
            }
            return View(usersList);
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                User user = await _db.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);
                if (user != null)
                {
                    await Authenticate(user);

                    return RedirectToAction("Index", "News");
                }
                ModelState.AddModelError("", "Некорректные логин и/или пароль");
            }
            return View(model);
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                User user = await _db.Users
                    .FirstOrDefaultAsync(u => u.Email == model.Email);
                if (user == null)
                {
                    user = new User
                    {
                        Email = model.Email,
                        FullName = model.FullName,
                        Password = model.Password
                    };
                    Role userRole = await _db.Roles
                        .FirstOrDefaultAsync(u => u.Name == model.Role);
                    if (userRole != null)
                    {
                        user.Role = userRole;
                    }
                    else
                    {
                        ModelState.AddModelError("", "Роль пользователя указана некорректно");
                        return View(model);
                    }
                    _db.Users.Add(user);
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Users", "Account");
                }
                else
                {
                    ModelState.AddModelError("", "Пользователь с таким e-mail уже существует");
                }
            }
            return View(model);
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult ChangeRole(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            User user = _db.Users.Include(u => u.Role).FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            var obj = new ChangeRoleVM
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.Name
            };
            return View(obj);
        }
        [HttpPost, ActionName("ChangeRole")]
        [ValidateAntiForgeryToken]
        public IActionResult ChangeRolePost(ChangeRoleVM changeRoleVM)
        {
            if (changeRoleVM == null)
            {
                return BadRequest();
            }
            if (changeRoleVM.Id == 0)
            {
                return NotFound();
            }
            User user = _db.Users.Include(u => u.Role).FirstOrDefault(u => u.Id == changeRoleVM.Id);
            user.Role = _db.Roles.FirstOrDefault(u => u.Name == changeRoleVM.Role);
            if (user.Role == null)
            {
                return BadRequest();
            }
            _db.Users.Update(user);
            _db.SaveChanges();
            return RedirectToAction("Users");
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _db.Users.Include(u => u.Role).FirstOrDefault(u => u.Id == id);
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
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _db.Users.Include(u => u.Role).FirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            if (obj.Email == HttpContext.User.Identity.Name)
            {
                return BadRequest();
            }
            _db.Users.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("Users");
        }
        [AllowAnonymous]
        private async Task Authenticate(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.Name)
            };
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}
