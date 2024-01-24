using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using WebApplication1.Models.DTO;

namespace WebApplication1.Controllers
{
    public class ApiController : Controller
    {
        private readonly MyDBContext _dbContext;
        public ApiController(MyDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        //[HttpPost] error 505
        public IActionResult Index()
        {
            System.Threading.Thread.Sleep(5000);
            return Content("Hello Content");
            //return Content("<h2>Hello!!!</h2>", "text/html");
            //return Content("Hello 你好 我是歐歐歐","text/plain",System.Text.Encoding.UTF8);
        }

        public IActionResult Cities()
        {
            var cities = _dbContext.Addresses.Select(c => c.City).Distinct();
            return Json(cities);
        }

        public IActionResult Avatar(int id = 1)
        {
            Member? member = _dbContext.Members.Find(id);
            if (member != null)
            {
                byte[] img = member.FileData;
                return File(img, "image/jpeg");
            }
            return NotFound();
        }

        //public IActionResult Register(string name, int age)
        public IActionResult Register(UserDto _user)
        {
            if (string.IsNullOrEmpty(_user.Name))
            {
                _user.Name = "Guest";
            }
            return Content($"Hello {_user.Name}, You are {_user.Age} years old, Your Email is ${_user.Email}");
        }

        public IActionResult Districts(string city)
        {
            var distircts = _dbContext.Addresses.Where(a => a.City == city).Select(a => a.SiteId).Distinct();
            return Json(distircts);
        }

        public IActionResult Roads(string siteId)
        {
            var road = _dbContext.Addresses.Where(a => a.SiteId == siteId).Select(a => a.Road).Distinct();
            return Json(road);
        }

        public IActionResult CheckAccount(string name)
        {
            var member = _dbContext.Members.FirstOrDefault(a => a.Name == name);
            if (member != null)
            {
                return Content("帳號已存在");
            }
            return Content("帳號可使用");
        }
    }
}
