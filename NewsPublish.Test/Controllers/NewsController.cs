using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsPublish.Model.Entity;
using NewsPublish.Service;

namespace NewsPublish.Test.Controllers
{
    public class NewsController : Controller
    {
        // GET: NewsController
        private NewsService _newsService;
        public NewsController(NewsService newsService)
        {
            this._newsService = newsService;
        }
        public ActionResult Classify(int id)
        {
            if(id<=0)
                Response.Redirect("/Home/Index");
            var classify = _newsService.GetOneNewsClassify(id);
            ViewData["ClassifyName"] = classify.data.Name;
            ViewData["Title"] = classify.data.Name;
            var newsList = _newsService.GetNewsList(c=>c.NewsClassifyid==id,6);
            ViewData["NewsList"] = newsList;

            return View(_newsService.GetClassifyList());            
        }

        // GET: NewsController/Details/5
        public ActionResult Show(int id)
        {
            if(id<=0)
                Response.Redirect("/Home/Index");
            var news = _newsService.GetOneNews(id);
            if (news.code == 200)
            {
                ViewData["Title"] = "新闻详情" + "[" + news.data.Title + "]";
                ViewData["news"] = news;
            }
            else
            {
                Response.Redirect("/Home/Index");
            }


            return View(_newsService.GetClassifyList());
        }

        // GET: NewsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: NewsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: NewsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: NewsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: NewsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: NewsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
