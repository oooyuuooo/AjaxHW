using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
	public class ApiController : Controller
	{
		private readonly MyDBContext _dbContext;
        public ApiController(MyDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
		{
			return View();
		}

		public IActionResult Cities()
		{
			var cities = _dbContext.Addresses.Select(c => c.City).Distinct();
			return Json(cities);
		}
	}
}
