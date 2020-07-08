using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEB.Assets.User;
using WEB.Dao;

namespace WEB.Controllers
{
    public class DashboardController : Controller
    {
        public ShopEntities db = new ShopEntities();
        // GET: Dashboard
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Dashboard()
        {
            var query = from pd in db.Products
                        select pd;
            ViewBag.ProductLoad = query.Take(12).ToList();
            return View();
        }
        [HttpPost]
        public ActionResult Search(FormCollection fc)
        {
            var str = fc.ToString();
            var str = fc["input"].ToString();
            var query = from pd in db.Products
                        where pd.Name.Contains(str)
                        select pd;
            ViewBag.SearchResult = query.ToList();
            return View();
        }
        public ActionResult Category(int id)
        {
            var query = from pd in db.Products
                        where pd.CategoryID == id
                        select pd;
            var categoryName = from cate in db.ProductCategories
                               where cate.ID == id
                               select cate;
            ViewBag.Category = query.ToList();
            ViewBag.Name = categoryName.ToList();
            return View();
        }
        public JsonResult ListName(string q)
        {
            var data = new ProductDao().ListName(q);
            return Json(new
            {
                data = data,
                status = true
            }, JsonRequestBehavior.AllowGet);
        }

    }
}