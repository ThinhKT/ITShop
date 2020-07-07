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
            Session["View"] = "Order";
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
        //warehouse là kho hàng
        public ActionResult WareHouse(int id)
        {
            var query = (from x in db.OrderDetails
                         where x.OrderID == id
                         select x).ToList();

            string str = "select * from Products where ID = ";
            for (int i = 0; i < query.Count - 1; i++)
            {
                str = str + query[i].ProductID.ToString() + " or ID = ";
            }
            str = str + query[query.Count - 1].ProductID.ToString();
            var query2 = db.Products.SqlQuery(str);

            ViewBag.Quantity = query;
            ViewBag.Product = query2.ToList();
            ViewBag.OrderNo = id;
            return View();
        }
        public ActionResult Wrapper(int id)
        {
            var query = (from x in db.OrderDetails
                         where x.OrderID == id
                         select x).ToList();
            string str;
            foreach (var item in query)
            {
                str = "update Products set Quantity = Quantity - " +
                    item.Quantity.ToString() + "where ID = " + item.ProductID.ToString();
                var query2 = db.Database.ExecuteSqlCommand(str);
            }
            str = "update Orders set Status = 4 where ID = " + id.ToString();
            var query3 = db.Database.ExecuteSqlCommand(str);
            return RedirectToAction("Dashboard","Shipper");
        }
        public ActionResult Ship(int id)
        {
            string str = "update Orders set Status = 5 where ID = " + id.ToString();
            var query = db.Database.ExecuteSqlCommand(str);
            return RedirectToAction("Dashboard", "Shipper");
        }
        public ActionResult Checkout(int id)
        {
            string str = "update Orders set Status = 6 where ID = " + id.ToString();
            var query = db.Database.ExecuteSqlCommand(str);
            return RedirectToAction("Dashboard", "Shipper");
        }
        public ActionResult MyAccount()
        {
            Session["View"] = "MyAccount";
            return View();
        }
    }
}