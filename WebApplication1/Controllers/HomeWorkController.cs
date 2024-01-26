using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
	public class HomeWorkController : Controller
	{
		public IActionResult Index() // hw1
		{
			return View();
		}

		public IActionResult CheckAccount() // hw2
		{
			return View();
		}

		public IActionResult Register() //hw3
		{
			return View();
		}

		public IActionResult Spots() //hw4
		{
			return View();
		}
	}
}
