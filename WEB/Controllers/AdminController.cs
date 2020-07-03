using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WEB.Controllers
{
    public class AdminController : Controller
    {
        public ShopEntities db = new ShopEntities();
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection fc)
        {
            return RedirectToAction("Dashboard", "Admin");
        }
        public ActionResult Dashboard()
        {
            Session["View"] = "Dashboard";
            return View();
        }
        public ActionResult Product()
        {
            Session["View"] = "Product";
            var query = from pd in db.Products
                        select pd;
            ViewBag.ProductLoad = query.ToList();
            var query2 = from pd in db.ProductCategories
                        select pd;
            ViewBag.ProductCategoryLoad = query2.ToList();
            return View();
        }
        public ActionResult ProductCate()
        {
            Session["View"] = "Other";
            return View();
        }
        public ActionResult ListUser()
        {
            Session["View"] = "ListUser";
            var query = from pd in db.ApplicationUsers
                        select pd;
            ViewBag.UserList = query.ToList();
            return View();
        }
        public ActionResult Manager()
        {
            Session["View"] = "Other";
            return View();
        }
        public ActionResult Chart()
        {
            Session["View"] = "Other";
            return View();
        }
        public ActionResult Order()
        {
            Session["View"] = "Order";
            //var query = from od in db.Orders
            //            join odd in db.OrderDetails on od.ID equals odd.OrderID
            //            select new { Table1 = od, Table2 = odd };
            //ViewBag.OrderList = query.ToList();
            return View();
        }
        public ActionResult Income()
        {
            Session["View"] = "Income";
            return View();
        }
    }
}