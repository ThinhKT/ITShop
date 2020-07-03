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
            var query = from pd in db.Products
                        where pd.ID == id
                        select pd;
            ViewBag.Detail = query.ToList();//chắc không cần đâu
            return View();
        }
    }
}