using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using WebApplication1.Models.DTO;

namespace WebApplication1.Controllers
{
    public class ApiController : Controller
    {
        private readonly MyDBContext _dbContext;
        private readonly IWebHostEnvironment _host;

        public ApiController(MyDBContext dbContext, IWebHostEnvironment host)
        {
            _dbContext = dbContext;
            _host = host;
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

        public IActionResult Register(Member member, IFormFile Picture)
        {

            //todo 檔案存在?
            //todo 限制上傳的檔案類型
            //todo 限制上傳的檔案大小

            string fileName = "empty.jpg";
            if (Picture != null)
            {
                fileName = Picture.FileName;
            };
            //取得實際路徑
            string uploadPath = Path.Combine(_host.WebRootPath, "uploads", fileName);
            //檔案上傳
            using (var stream = new FileStream(uploadPath, FileMode.Create))
            {
                Picture?.CopyTo(stream);
            }

            //轉成二進位
            byte[]? imgByte = null;
            using (var memoryStream = new MemoryStream())
            {
                Picture?.CopyTo(memoryStream);
                imgByte = memoryStream.ToArray();
            }

            member.FileName = fileName;
            member.FileData = imgByte;

            _dbContext.Members.Add(member);
            _dbContext.SaveChanges();

            return Content("新增成功");
        }

        //public IActionResult Register(string name, int age)
        //public IActionResult Register(UserDto _user)
        //{
        //    if (string.IsNullOrEmpty(_user.Name))
        //    {
        //        _user.Name = "Guest";
        //    }

        //    //todo 檔案存在?
        //    //todo 限制上傳的檔案類型
        //    //todo 限制上傳的檔案大小

        //    string fileName = "empty.jpg";
        //    if (_user.Picture != null)
        //    {
        //        fileName = _user.Picture.FileName;
        //    };
        //    //取得實際路徑
        //    string uploadPath = Path.Combine(_host.WebRootPath, "uploads", fileName);
        //    //檔案上傳
        //    using (var stream = new FileStream(uploadPath, FileMode.Create))
        //    {
        //        _user.Picture?.CopyTo(stream);
        //    }

        //    //轉成二進位
        //    byte[]? imgByte = null;
        //    using (var memoryStream = new MemoryStream())
        //    {
        //        _user.Picture?.CopyTo(memoryStream);
        //        imgByte = memoryStream.ToArray();
        //    }
        //    //return Content(Path.Combine(_host.WebRootPath, "uploads", fileName));

        //    //return Content($"Hello {_user.Name}, You are {_user.Age} years old, Your Email is {_user.Email}. The Picture info is {_user.Picture?.FileName} - {_user.Picture?.Length} - {_user.Picture?.ContentType}");
        //    return Content($"The Picture info is {_user.Picture?.FileName} - {_user.Picture?.Length} - {_user.Picture?.ContentType}");
        //    //F:\全端工程師\後端\Ajax\AjaxHW\WebApplication1\wwwroot\uploads
        //}

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

        public IActionResult CheckAccount(string name) //HW2
        {
            var member = _dbContext.Members.Any(a => a.Name == name);
            return Content(member.ToString());
        }

        [HttpPost]
        public IActionResult Spots([FromBody] SearchDTO _search)
        {
            //根據分類編號讀取景點資料
            var spots = _search.categoryId == 0 ? _dbContext.SpotImagesSpots : _dbContext.SpotImagesSpots.Where(s => s.CategoryId == _search.categoryId);

            //根據關鍵字搜尋
            if (!string.IsNullOrEmpty(_search.keyword))
            {
                spots = spots.Where(s => s.SpotTitle.Contains(_search.keyword) || s.SpotDescription.Contains(_search.keyword));
            }

            //排序
            switch (_search.sortBy)
            {
                case "spotTitle":
                    spots = _search.sortType == "asc" ? spots.OrderBy(s => s.SpotTitle) : spots.OrderByDescending(s => s.SpotTitle);
                    break;
                case "categoryId":
                    spots = _search.sortType == "asc" ? spots.OrderBy(s => s.CategoryId) : spots.OrderByDescending(s => s.CategoryId);
                    break;
                default:
                    spots = _search.sortType == "asc" ? spots.OrderBy(s => s.SpotId) : spots.OrderByDescending(s => s.SpotId);
                    break;
            }

            //分頁
            int TotalCount = spots.Count();//總共有多少筆資料

            int pageSize = _search.PageSize ?? 9;//設定每頁顯示多少筆資料

            int page = _search.Page ?? 1;//目前要顯示第幾頁

            int TotalPages = (int)Math.Ceiling((decimal)TotalCount / pageSize);//計算總共有幾頁

            //取出分頁資料
            spots = spots.Skip((int)((page - 1) * pageSize)).Take(pageSize);


            SpotsPagingDTO spotsPaging = new SpotsPagingDTO
            {
                TotalPages = TotalPages,
                SpotsResult = spots.ToList()
            };
            return Json(spotsPaging);
        }
    }
}
