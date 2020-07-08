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
            string username = fc["username"].ToString();
            string pass = fc["password"].ToString();
            var query = (from x in db.ApplicationUsers
                        where x.IsShipper == true && x.UserName == username
                        select x.PasswordHash).ToList();
            if(query.Count == 0)
            {
                ViewBag.WrongAccount = "Tài khoản không tồn tại !";
                return View();
            }
            if (query[0].ToString() != pass)
            {
                ViewBag.WrongPass = "Sai mật khẩu !";
                return View();
            }
            var query2 = (from x in db.ApplicationUsers
                         where x.IsShipper == true && x.UserName == username
                         select x).ToList();
            Session["Name"] = query2[0].FullName;
            Session["ID"] = query2[0].Id;
            return RedirectToAction("Dashboard","Shipper");
        }
        public ActionResult Logout()
        {
            Session["Name"] = null;
            Session["ID"] = null;
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
            int id = int.Parse(Session["ID"].ToString());
            var query = from x in db.ApplicationUsers
                        where x.Id == id
                        select x;
            ViewBag.MyInfo = query.ToList();
            Session["View"] = "MyAccount";
            return View();
        }
        [HttpPost]
        public ActionResult ChangeInfo(FormCollection fc)
        {
            try
            {
                string str = "update ApplicationUsers set FullName = N'" + fc["name"] +"', " +
                    "PhoneNumber = '" + fc["phone"]+ "' " +
                    ", Email = '" + fc["email"] + "'";
                var query = db.Database.ExecuteSqlCommand(str);
                Session["Sucsess"] = "Sửa thông tin thành công";
                return RedirectToAction("MyAccount", "Shipper");
            }
            catch
            {
                Session["Sucsess"] = "Đã xảy ra lỗi !";
                return RedirectToAction("MyAccount", "Shipper");
            }
        }
        public ActionResult ChangePassword()
        {
            Session["View"] = "MyAccount";
            return View();
        }
        [HttpPost]
        public ActionResult ChangePassword(FormCollection fc)
        {
            int id = int.Parse(Session["ID"].ToString());
            var query = (from x in db.ApplicationUsers
                        where x.Id == id
                        select x).ToList();
            //kiểm tra mật khẩu cũ
            if(query[0].PasswordHash != fc["password"])
            {
                ViewBag.WrongPass = "Nhập sai mật khẩu !";
                return View();
            }
            //kiểm tra trùng lặp mật khẩu
            if(query[0].PasswordHash == fc["new_password"])
            {
                ViewBag.PassEqual = "Mật khẩu mới trùng với mật khẩu cũ !";
                return View();
            }
            //Kiểm tra mật khẩu nhập lại
            if (fc["re_password"] != fc["new_password"])
            {
                ViewBag.PassNotMatch = "Mật khẩu nhập lại không trùng khớp !";
                return View();
            }
            string str = "update ApplicationUsers set PasswordHash = '" + fc["new_password"].ToString() +"' where id = " + id.ToString();
            var query2 = db.Database.ExecuteSqlCommand(str);
            ViewBag.Sucsess = "Đổi mật khẩu thành công !";
            return View();
        }
    }
}