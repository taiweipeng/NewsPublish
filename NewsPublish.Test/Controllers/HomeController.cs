using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NewsPublish.Service;
using NewsPublish.Test.Models;

namespace NewsPublish.Test.Controllers
{
    public class HomeController : Controller
    {
        private NewsService _newService;
        private BannerService _bs;

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, NewsService newsService, BannerService bannerService)
        {
            _logger = logger;
            this._newService = newsService;
            this._bs = bannerService;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "首页";
            Model.Response.ResponseModel model = _newService.GetClassifyList();
            return base.View(model);
        }
        [HttpGet]
        public JsonResult GetBannerList()
        {
            return Json(_bs.GetBannerList());
        }
        [HttpGet]
        public JsonResult GetNewsCount()
        {
            return Json(_newService.GetNewsCount(c=>true));
        }

        [HttpGet]
        public JsonResult GetHomeNewsList()
        {
            return Json(_newService.GetNewsList(c => true,6));
        }
        //[HttpGet]
        //public JsonResult GetNewCommentNewsList()
        //{
        //    return Json(_newService.GetRecommendNewsList);
        //}
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
