using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WEB.Controllers
{
    public class ShipperController : Controller
    {
        // GET: Shipper
        public ShopEntities db = new ShopEntities();
        public ActionResult Dashboard()
        {
            string str = "select * from Orders order by ID DESC";
            var query = db.Orders.SqlQuery(str);
            ViewBag.OrderList = query.ToList();
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection fc)
        {
            return RedirectToAction("Dashboard","Shipper");
        }
        public ActionResult Logout()
        {
            return View("Login");
        }
    }
}