using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEB.Assets.User;

namespace WEB.Controllers
{
    public class ProductController : Controller
    {
        public ShopEntities db = new ShopEntities();
        // GET: Product
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Detail(int id)
        {
            var query = (from pd in db.Products
                        where pd.ID == id
                        select pd).ToList();
            ViewBag.Detail = query;//chắc không cần đâu
            string str = "select * from Products where CategoryID = " + query[0].CategoryID.ToString();
            var query2 = db.Products.SqlQuery(str);
            ViewBag.MoreProduct = query2.ToList();
            return View();
        }
    }
}