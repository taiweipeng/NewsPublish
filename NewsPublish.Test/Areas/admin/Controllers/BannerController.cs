using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsPublish.Model.Request;
using NewsPublish.Model.Response;
using NewsPublish.Service;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace NewsPublish.Test.Areas.admin.Controllers
{
    [Area("admin")]
    public class BannerController : AreaController
    {
        private BannerService _bs;
        [Obsolete]
        private IHostingEnvironment _host;

        [Obsolete]
        public BannerController(BannerService bannerService,IHostingEnvironment host)
        {
            this._bs = bannerService;
            this._host = host;
        }
        // GET: BannerController
        public ActionResult Index()
        {
            var banner = _bs.GetBannerList();
            return View(banner);
        }

        // GET: BannerController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: BannerController/Create
        public ActionResult BannerAdd()
        {
            return View();
        }
        [HttpPost]
        [Obsolete]
        public async Task<JsonResult> AddBanner(AddBanner banner,IFormCollection collection)
        {
            var files = collection.Files;
            if (files.Count > 0)
            {
                //获取主机地址
                var WebRootPath = _host.WebRootPath;
                string relativeDirPath = "\\BannerPic";
                string absolutionPath = WebRootPath + relativeDirPath;

                string[] fileTypes = new string[] { ".gif",".jpg",".jpeg",".png",".bmp"};
                string extension = Path.GetExtension(files[0].FileName);
                if (fileTypes.Contains(extension.ToLower()))
                {
                    if (!Directory.Exists(absolutionPath)) 
                        Directory.CreateDirectory(absolutionPath);
                    string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
                    var filePath = absolutionPath + "\\" + fileName;
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await files[0].CopyToAsync(stream);
                    }
                    banner.Image = "/BannerPic/" + fileName;
                    return Json(_bs.AddBanner(banner));
                }
                return Json(new ResponseModel { code = 0,result="图片格式错误！" });
;            }
            return Json(new ResponseModel { code = 0, result = "请上传图片文件！" });
        }
        [HttpPost]
        public JsonResult DeleteBanner(int id)
        {
            if (id <= 0)
                return Json(new ResponseModel { code=0,result="参数有误！"});
            return Json(_bs.DeleteBanner(id));
        }        
    }
}
