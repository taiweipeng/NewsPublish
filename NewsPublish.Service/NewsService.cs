using Microsoft.EntityFrameworkCore;
using NewsPublish.Model.Entity;
using NewsPublish.Model.Request;
using NewsPublish.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;


namespace NewsPublish.Service
{
    public class NewsService
    {
        private Db_Help _db;
        public NewsService(Db_Help db)
        {
            this._db = db;
        }
        /// <summary>
        /// 添加一个新闻类别
        /// </summary>
        /// <param name="newsClassify"></param>
        /// <returns></returns>
        public ResponseModel AddNewsClassify(AddNewsClassify newsClassify)
        {
            //判断这个新闻类别是否存在
            var exit = _db.NewsClassify.FirstOrDefault(c => c.Name == newsClassify.Name) != null;
            //如果存在，就返回已存在
            if (exit)
                return new ResponseModel { code = 0, result = "该类别已存在" };
            //不存在就返回添加的newsclassify的实体
            var classify = new NewsClassify {Name=newsClassify.Name,Sort=newsClassify.Sort,Remark=newsClassify.Remark };
            _db.NewsClassify.Add(classify);
            //判断数据库返回值
            int i = _db.SaveChanges();
            if (i > 0)
                return new ResponseModel { code = 200, result = "类别添加成功" };
            return new ResponseModel { code = 0, result = "类别添加失败！" };
        }
        /// <summary>
        /// 获取一个新闻类别
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel GetOneNewsClassify(int id)
        {
            var classify = _db.NewsClassify.Find(id);
            if (classify==null)
                return new ResponseModel { code = 0, result = "该类别不存在！" };
            return new ResponseModel {
                code = 200,
                result = "新闻类别获取成功！",
                data=new NewsClassifyModel { 
                    id=classify.id,
                    Name=classify.Name,
                    Sort=classify.Sort,
                    Remark=classify.Remark
                }
            };
        }
        /// <summary>
        /// 根据条件获取一个NewsClassify实体
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        private NewsClassify GetOneNewsClassify(Expression<Func<NewsClassify,bool>> where)
        {
            return _db.NewsClassify.FirstOrDefault(where);
        }
        /// <summary>
        /// 编辑一个新闻类别
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel EditNewsClassify(EditNewsClassify newsClassify)
        {
            var classify = this.GetOneNewsClassify(c=>c.id==newsClassify.id);
            if (classify == null)
                return new ResponseModel { code = 0, result = "该类别不存在！" };
            classify.Name = newsClassify.Name;
            classify.Sort = newsClassify.Sort;
            classify.Remark = newsClassify.Remark;
            _db.NewsClassify.Update(classify);
            int i = _db.SaveChanges();
            if (i > 0)
                return new ResponseModel { code = 200, result = "类别修改成功" };
            return new ResponseModel { code = 0, result = "类别修改失败！" };
        }

        public ResponseModel GetClassifyList()
        {
            var Classifies = _db.NewsClassify.ToList().OrderByDescending(c => c.Sort).ToList();
            var response = new ResponseModel();
            response.code = 200;
            response.result = "获取类别集合成功";
            response.data = new List<NewsClassifyModel>();
            foreach (var classify in Classifies)
            {
                response.data.Add(new NewsClassifyModel
                {
                    id = classify.id,
                    Name = classify.Name,
                    Sort = classify.Sort,
                    Remark = classify.Remark
                });
            }
            return response;
        }
        public ResponseModel AddNews(AddNews news)
        {
            //判断这个新闻是否存在
            var exit = _db.News.FirstOrDefault(c=>c.Title== news.Title)!=null;
            //如果存在，就返回已存在
            if (exit)
                return new ResponseModel { code = 0, result = "该新闻已存在" };
            //不存在就返回添加的newsclassify的实体
            var insert_news = new News { 
                Title = news.Title, 
                NewsClassifyid = news.Classifyid, 
                Remark = news.Remark,
                Image= news.Image,
                Contents= news.Contents,
                PublishDate=DateTime.Now 
            };
            _db.News.Add(insert_news);
            //判断数据库返回值
            int i = _db.SaveChanges();
            if (i > 0)
                return new ResponseModel { code = 200, result = "新闻添加成功" };
            return new ResponseModel { code = 0, result = "新闻添加失败！" };
        }
        /// <summary>
        /// 获取一个新闻
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel GetOneNews(int id)
        {
            var news = _db.News.Include("NewsClassify").Include("NewsComment").FirstOrDefault(c=>c.id==id);
            if (news == null)
                return new ResponseModel { code = 0, result = "该新闻不存在！" };
            return new ResponseModel
            {
                code = 200,
                result = "新闻获取成功！",
                data = new NewsModel
                {
                    id = news.id,
                    Title = news.Title,
                    Remark = news.Remark,
                    Contents = news.Contents,
                    Image = news.Image,
                    PublishDate = news.PublishDate.ToString("yyyy-MM-dd"),
                    ClassifyName = news.NewsClassify.Name,
                    CommentCount=news.NewsComment.Count()
                }
            };
        }

        /// <summary>
        /// 删除新闻
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteOneNews(int id)
        {
            var news = _db.News.FirstOrDefault(c => c.id == id);
            if (news == null)
                return new ResponseModel { code = 0, result = "该新闻不存在！" };
            _db.News.Remove(news);
            int i = _db.SaveChanges();
            if (i > 0)
                return new ResponseModel { code = 200, result = "一条新闻删除成功！" };
            return new ResponseModel { code = 0, result = "一条新闻删除失败！" };
        }

        /// <summary>
        /// 分页查询新闻
        /// </summary>
        /// <param name="pagesize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="total"></param>
        /// <param name="wheres"></param>
        /// <returns></returns>
        public ResponseModel NewsPageQuery(int pagesize,int pageIndex,out int total,List<Expression<Func<News,bool>>>wheres)
        {
            var list = _db.News.Include("NewsClassify").Include("NewsComment");            
            foreach (var item in wheres)
            {
                list = list.Where(item);               
            }
            total = list.Count();

            var pageData = list.OrderByDescending(c => c.PublishDate).Skip(pagesize * (pageIndex - 1)).Take(pagesize).ToList();
            var response = new ResponseModel {
                code = 200,
                result="新闻获取成功"
            };
            response.data = new List<NewsModel>();
            foreach (var model in pageData)
            {
                response.data.Add(new NewsModel
                {
                    id = model.id,
                    ClassifyName = model.NewsClassify.Name,
                    Title = model.Title,
                    Image = model.Image,
                    CommentCount = model.NewsComment.Count,
                    Contents = model.Contents.Length > 50 ? model.Contents.Substring(0, 50) + "..." : model.Contents.Trim(),
                    PublishDate = model.PublishDate.ToString("yyyy-MM-dd"),
                    Remark = model.Remark
                }); 
            }
            return response;
        }
        /// <summary>
        /// 查询新闻列表，首页要使用
        /// </summary>
        /// <param name="where"></param>
        /// <param name="topCount"></param>
        /// <returns></returns>
        public ResponseModel GetNewsList(Expression<Func<News,bool>>where,int topCount)
        {
            var list = _db.News.Include("NewsClassify").Include("NewsComment").Where(where).OrderByDescending(c=>c.PublishDate).Take(topCount);
            var response = new ResponseModel
            {
                code = 200,
                result = "新闻获取成功"
            };
            response.data = new List<NewsModel>();
            foreach (var model in list)
            {
                response.data.Add(new NewsModel
                {
                    id = model.id,
                    ClassifyName = model.NewsClassify.Name,
                    Title = model.Title,
                    Image = model.Image,
                    CommentCount = model.NewsComment.Count,
                    Contents = model.Contents.Length>50?model.Contents.Substring(0,50):model.Contents,//截取新闻内容的前50个字
                    PublishDate = model.PublishDate.ToString("yyyy-MM-dd"),
                    Remark = model.Remark
                });
            }
            return response;
        }
        /// <summary>
        /// 获取最新评论的新闻集合
        /// </summary>
        /// <param name="topCount"></param>
        /// <returns></returns>
        public ResponseModel GetCommentNewsList(int topCount)
        {
            var newsids = _db.NewsComment.OrderByDescending(c => c.AddTime).GroupBy(c => c.Newsid).Select(c => c.Key).Take(topCount);
            var list = _db.News.Include("NewsClassify").Include("NewsComment").Where(c=>newsids.Contains(c.id)).OrderByDescending(c=>c.PublishDate);
            var response = new ResponseModel
            {
                code = 200,
                result = "最新评论新闻获取成功"
            };
            response.data = new List<NewsModel>();
            foreach (var model in list)
            {
                response.data.Add(new NewsModel
                {
                    id = model.id,
                    ClassifyName = model.NewsClassify.Name,
                    Title = model.Title,
                    Image = model.Image,
                    CommentCount = model.NewsComment.Count,
                    Contents = model.Contents.Length > 50 ? model.Contents.Substring(0, 50) : model.Contents,//截取新闻内容的前50个字
                    PublishDate = model.PublishDate.ToString("yyyy-MM-dd"),
                    Remark = model.Remark
                });
            }
            return response;
        }

        /// <summary>
        /// 搜索一条新闻
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public ResponseModel GetSearchNews(Expression<Func<News, bool>> where)
        {
            var news = _db.News.Where(where).FirstOrDefault();
            if (news == null)
                return new ResponseModel {code=0,result="新闻搜索失败" };
            return new ResponseModel { code = 200, result = "新闻搜索成功",data=news.id };
        }

        /// <summary>
        /// 获取新闻数量
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public ResponseModel GetNewsCount(Expression<Func<News, bool>> where)
        {
            int count = _db.News.Where(where).Count();
            return new ResponseModel { code = 200, result = "获取新闻数量成功", data = count };
        }

        /// <summary>
        /// 获取推荐新闻
        /// </summary>
        /// <param name="newsid"></param>
        /// <returns></returns>
        public ResponseModel GetRecommendNewsList(int newsid)
        {
            var news = _db.News.FirstOrDefault(c => c.id == newsid);
            if (news == null)
                return new ResponseModel { code = 0, result = "新闻不存在" };
            var newsList = _db.News.Include("NewsComment").Where(c => c.NewsClassifyid == news.NewsClassifyid && c.id != newsid).OrderByDescending(c=>c.PublishDate).OrderByDescending(c=>c.NewsComment.Count).Take(6).ToList();
            var response = new ResponseModel
            {
                code = 200,
                result = "最新评论新闻获取成功"
            };
            response.data = new List<NewsModel>();
            foreach (var model in newsList)
            {
                response.data.Add(new NewsModel
                {
                    id = model.id,
                    ClassifyName = model.NewsClassify.Name,
                    Title = model.Title,
                    Image = model.Image,
                    CommentCount = model.NewsComment.Count,
                    Contents = model.Contents.Length > 50 ? model.Contents.Substring(0, 50) : model.Contents,//截取新闻内容的前50个字
                    PublishDate = model.PublishDate.ToString("yyyy-MM-dd"),
                    Remark = model.Remark
                });
            }
            return response;
        }
    }
}
