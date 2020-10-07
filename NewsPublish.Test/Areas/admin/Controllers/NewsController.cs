using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsPublish.Service;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using NewsPublish.Model.Request;
using NewsPublish.Model.Response;
using System.Linq.Expressions;
using NewsPublish.Model.Entity;

namespace NewsPublish.Test.Areas.admin.Controllers
{
    [Area("admin")]
    public class NewsController : AreaController
    {
        private NewsService _newsService;
        private IHostingEnvironment _host;

        public NewsController(NewsService newsService, IHostingEnvironment host)
        {
            this._newsService = newsService;
            this._host = host;
        }
        // GET: ClassifyController
        public ActionResult Index()
        {
            var newsClassifies = _newsService.GetClassifyList();
            return View(newsClassifies);
        }

        // GET: ClassifyController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        #region 新闻类别
        public ActionResult NewsClassify()
        {
            var newsClassifies = _newsService.GetClassifyList(); 
            return View(newsClassifies);
        }
        public ActionResult NewsClassifyAdd()
        {
            return View();
        }
        public ActionResult NewsClassifyEdit(int id)
        {
            return View(_newsService.GetOneNewsClassify(id));
        }
        [HttpPost]
        public JsonResult AddNewsClassify(AddNewsClassify newsClassify)
        {
            //标题名称不能为空
            if (string.IsNullOrEmpty(newsClassify.Name))
                return Json(new ResponseModel { code = 0, result = "类别标题不能为空！" });
            return Json(_newsService.AddNewsClassify(newsClassify));
        }

        [HttpPost]
        public JsonResult EditNewsClassify(EditNewsClassify edit_newsClassify)
        {
            //标题名称不能为空
            if (string.IsNullOrEmpty(edit_newsClassify.Name))
                return Json(new ResponseModel { code = 0, result = "类别标题不能为空！" });
            return Json(_newsService.EditNewsClassify(edit_newsClassify));
        }
        #endregion


    
        public JsonResult GetNews(int pageIndex, int pageSize, int classifyID, string keyWord)
        {
            List<Expression<Func<News, bool>>> wheres = new List<Expression<Func<News, bool>>>();
            if (classifyID > 0)
            {
                wheres.Add(c => c.NewsClassifyid == classifyID);
            }
            if (!string.IsNullOrEmpty(keyWord))
            {
                wheres.Add(c => c.Title.Contains(keyWord));
            }
            int total = 0;
            var news = _newsService.NewsPageQuery(pageSize, pageIndex, out total, wheres);
            return Json(new { total=total,data=news.data});
        }
        public ActionResult NewsAdd()
        {
            var newsClassifies = _newsService.GetClassifyList();
            return View(newsClassifies);
        }
        [HttpPost]
        public async Task<JsonResult> AddNews(AddNews news, IFormCollection collection)
        {
            if (news.Classifyid <= 0 || string.IsNullOrEmpty(news.Title) || string.IsNullOrEmpty(news.Contents))
                return Json(new ResponseModel { code = 0, result = "参数有误！" });
            var files = collection.Files;
            if (files.Count > 0)
            {
                //获取主机地址
                var WebRootPath = _host.WebRootPath;
                string relativeDirPath = "\\NewsPic";
                string absolutionPath = WebRootPath + relativeDirPath;

                string[] fileTypes = new string[] { ".gif", ".jpg", ".jpeg", ".png", ".bmp" };
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
                    news.Image = "/NewsPic/" + fileName;
                    return Json(_newsService.AddNews(news));
                }
                return Json(new ResponseModel { code = 0, result = "图片格式错误！" });                
            }
            return Json(new ResponseModel { code = 0, result = "请上传图片文件！" });
        }

    }
}
