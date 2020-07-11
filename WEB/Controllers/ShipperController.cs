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

        //trang chủ
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

        //Warehouse là hàm xem kho hàng
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

        //Xem chi tiết đơn hàng
        public ActionResult OrderDetail(int id)
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

            var query3 = from x in db.Orders
                         where x.ID == id
                         select x;

            ViewBag.Quantity = query;
            ViewBag.Product = query2.ToList();
            ViewBag.OrderNo = id;
            ViewBag.Status = query3.ToList()[0];
            return View();
        }

        //hàm xuất kho + đóng gói
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

        //hàm bắt đầu ship
        public ActionResult Ship(int id)
        {
            string str = "update Orders set Status = 5 where ID = " + id.ToString();
            var query = db.Database.ExecuteSqlCommand(str);
            return RedirectToAction("Dashboard", "Shipper");
        }

        //checkout - hoàn thành giao hàng
        public ActionResult Checkout(int id)
        {
            //shipper nhận tiền và thêm vào lịch sử ship
            var order = (from x in db.Orders
                         where x.ID == id
                         select x).ToList()[0];
            string strr = "INSERT INTO ShipHistory (ShipperID,OrderID,RecievedMoney,Status,Date) " +
                "VALUES (" + Session["ID"].ToString() + ", " + id.ToString() + ", " 
                + order.TotalPrice.ToString() + ", 0, GETDATE() )";
            var queryy = db.Database.ExecuteSqlCommand(strr);

            //thay đổi trạng thái đơn hàng
            string str = "update Orders set Status = 6 where ID = " + id.ToString();
            var query = db.Database.ExecuteSqlCommand(str);
            return RedirectToAction("Dashboard", "Shipper");
        }

        //xem thông tin đơn hàng định hủy bỏ
        public ActionResult Cancelled(int id)
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

            var query3 = from x in db.Orders
                         where x.ID == id
                         select x;

            ViewBag.Quantity = query;
            ViewBag.Product = query2.ToList();
            ViewBag.OrderNo = id;
            ViewBag.Status = query3.ToList()[0];
            return View();
        }

        //trả hàng về kho
        public ActionResult ReturnPackage(int id)
        {
            //trả hàng về kho
            var query = (from x in db.OrderDetails
                         where x.OrderID == id
                         select x).ToList();
            string str;
            foreach (var item in query)
            {
                str = "update Products set Quantity = Quantity + " +
                    item.Quantity.ToString() + " where ID = " + item.ProductID.ToString();
                var query2 = db.Database.ExecuteSqlCommand(str);
            }

            //đổi trạng thái đơn hàng
            str = "update Orders set Status = 0 where ID = " + id.ToString();
            var query3 = db.Database.ExecuteSqlCommand(str);

            return RedirectToAction("OrderDetail", "Shipper", new { @id = id });
        }

        //hoàn tiền và trả hàng về kho
        public ActionResult Payback(int id)
        {
            //trả hàng về kho
            var query = (from x in db.OrderDetails
                         where x.OrderID == id
                         select x).ToList();
            string str;
            foreach (var item in query)
            {
                str = "update Products set Quantity = Quantity + " +
                    item.Quantity.ToString() + " where ID = " + item.ProductID.ToString();
                var query2 = db.Database.ExecuteSqlCommand(str);
            }

            //đổi trạng thái đơn hàng
            str = "update Orders set Status = 0 where ID = " + id.ToString();
            var query3 = db.Database.ExecuteSqlCommand(str);

            //hoàn tiền cho user
            var query4 = (from x in db.Orders
                         where x.ID == id
                         select x).ToList();
            string strr = "update ApplicationUsers set Money = Money + " + query4[0].TotalPrice.ToString() + " where Id = " + query4[0].CustomerId.ToString();
            var query5 = db.Database.ExecuteSqlCommand(strr);

            return RedirectToAction("OrderDetail", "Shipper", new { @id = id });
        }

        //vào trang thông tin cá nhân
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

        //hàm sửa thông tin cá nhân
        [HttpPost]
        public ActionResult ChangeInfo(FormCollection fc)
        {
            try
            {
                string str = "update ApplicationUsers set FullName = N'" + fc["name"] +"', " +
                    "PhoneNumber = '" + fc["phone"] + "', BirthDay = CONVERT(date,'" + fc["birthday"].ToString() +
                    "'), Email = '" + fc["email"] + "' where Id = " + Session["ID"].ToString();
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

        //hàm sửa password
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

        //xem tiền trong tài khoản (có từ các đơn hàng đã ship)
        public ActionResult MyShip()
        {
            Session["View"] = "MyShip";

            int id = int.Parse(Session["ID"].ToString());
            var query = from x in db.ShipHistories
                        where x.ShipperID == id
                        select x;
            ViewBag.ShipHistory = query.ToList();

            return View();
        }

        //chuyển khoản cho admin
        public ActionResult Transfer(int id)
        {
            //lấy thông tin lịch sử ship
            var query = (from x in db.ShipHistories
                         where x.OrderID == id
                         select x).ToList()[0];

            //thêm tiền cho admin
            string str2 = "update ApplicationUsers set Money = Money + " + query.RecievedMoney.ToString() + " where Id = 3";
            var query2 = db.Database.ExecuteSqlCommand(str2);

            //trừ tiền của shipper
            string str3 = "update ApplicationUsers set Money = Money - " + query.RecievedMoney.ToString() + " where Id = " + Session["ID"].ToString();
            var query3 = db.Database.ExecuteSqlCommand(str3);

            //thay đổi trạng thái của lịch sử ship
            string str4 = "update ShipHistory set Status = 1 where OrderID = " + id.ToString();
            var query4 = db.Database.ExecuteSqlCommand(str4);

            return RedirectToAction("MyShip","Shipper");
        }
    }
}