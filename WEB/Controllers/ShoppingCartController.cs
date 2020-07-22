using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEB.Assets.User;

namespace WEB.Controllers
{
    public class ShoppingCartController : Controller
    {
        public ShopEntities db = new ShopEntities();
        // GET: ShoppingCart
        public ActionResult Index()
        {
            return View();
        }

        //thêm sản phẩm vào giỏ hàng
        public ActionResult AddToCart(int id)
        {
            if (Session["UserName"] == null)
            {
                Session["NonLogin"] = "Bạn chưa đăng nhập";
                return RedirectToAction("Login", "Account");
            }
            else
            {
                for (int i = 0; i < TempCart.count; i++)
                {
                    if (TempCart.ID[i] == id)
                    {
                        TempCart.ammount[i] += 1;
                        return RedirectToAction("Dashboard", "Dashboard");
                    }
                }
                TempCart.ID[TempCart.count] = id;
                TempCart.ammount[TempCart.count] = 1;
                TempCart.count += 1;
                return RedirectToAction("Dashboard", "Dashboard");
            }
        }

        //xem giỏ hàng
        public ActionResult ViewCart()
        {
            if (Session["UserName"] == null)
            {
                Session["NonLogin"] = "Bạn chưa đăng nhập";
                return RedirectToAction("Login", "Account");
            }
            else
            {
                if (TempCart.count > 1)
                {
                    string sql = "Select * from Products where ID = ";
                    for (int i = 0; i < TempCart.count; i++)
                    {
                        sql = sql + TempCart.ID[i].ToString() + " or ID = ";
                    }
                    sql = sql + TempCart.ID[TempCart.count].ToString();
                    var query = db.Products.SqlQuery(sql).ToList();
                    ViewBag.Cart = query;

                    //tính tổng giá
                    decimal sum = 0;
                    foreach (var item in query)
                    {
                        for (int i = 0; i < TempCart.count; i++)
                        {
                            if (item.ID == TempCart.ID[i])
                            {
                                sum += item.Price * TempCart.ammount[i];
                            }
                        }
                    }
                    ViewBag.TotalPrice = sum;
                }
                else
                {
                    ViewBag.None = "Giỏ hàng không có sản phẩm nào";
                }
                return View();
            } 
        }

        //xóa sản phẩm khỏi giỏ hàng
        public ActionResult RemoveProduct(int id)
        {
            for(int i = 0; i < TempCart.count; i++)
            {
                if (TempCart.ID[i] == id)
                {
                    TempCart.ID[i] = 0;
                    break;
                }
            }
            return View("ViewCart");
        }

        //tạo đơn hàng
        public ActionResult MakeOrder()
        {
            int id = int.Parse(Session["UserID"].ToString());
            var query = from us in db.ApplicationUsers
                         where us.Id == id
                         select us;
            ViewBag.Info = query.ToList();

            if (TempCart.count > 1)
            {
                string sql = "Select * from Products where ID = ";
                for (int i = 0; i < TempCart.count; i++)
                {
                    sql = sql + TempCart.ID[i].ToString() + " or ID = ";
                }
                sql = sql + TempCart.ID[TempCart.count].ToString();
                var query2 = db.Products.SqlQuery(sql).ToList();
                ViewBag.Cart = query2;

                //tính tổng giá
                decimal sum = 0;
                foreach (var item in query2)
                {
                    for (int i = 0; i < TempCart.count; i++)
                    {
                        if (item.ID == TempCart.ID[i])
                        {
                            sum += item.Price * TempCart.ammount[i];
                        }
                    }
                }
                ViewBag.TotalPrice = sum;
            }
            return View();
        }
        [HttpPost]
        public ActionResult MakeOrder(FormCollection fc)
        {
            int id = int.Parse(Session["UserID"].ToString());
            string pass = fc["re_password"].ToString();
            //kiểm tra mật khẩu nhập lại
            var queryyy = (from x in db.ApplicationUsers
                          where x.Id == id && x.PasswordHash == pass
                          select x).ToList();
            if(queryyy.Count == 0)
            {
                Session["Message"] = "Nhập sai mật khẩu !";
                return RedirectToAction("MakeOrder", "ShoppingCart");
            }

            //lấy danh sách sản phẩm có trong giỏ hàng
            string sql = "Select * from Products where ID = ";
            for (int i = 0; i < TempCart.count; i++)
            {
                sql = sql + TempCart.ID[i].ToString() + " or ID = ";
            }
            sql = sql + TempCart.ID[TempCart.count].ToString();
            var query = db.Products.SqlQuery(sql).ToList();
            ////quy trình kiểm tra còn hàng không
            //foreach (var item in query)
            //{
            //    for (int i = 0; i < TempCart.count; i++)
            //    {
            //        if (item.ID == TempCart.ID[i])
            //        {
            //            if (item.Quantity < TempCart.ammount[i])
            //            {
            //                Session["CartOverload"] = "Không đủ sản phẩm" + item.Name.ToString();
            //                return RedirectToAction("ViewCart", "ShoppingCart");
            //            }
            //        }
            //    }
            //}
            ////nếu không return ở trên, thì còn hàng, trừ hàng ra
            //foreach (var item in query)
            //{
            //    for (int i = 0; i < TempCart.count; i++)
            //    {
            //        if (item.ID == TempCart.ID[i])
            //        {
            //            string strin = "update Products set Quantity = Quantity - " + TempCart.ammount[i].ToString() +
            //                "where ID = " + TempCart.ID[i].ToString();
            //            var queryy = db.Database.ExecuteSqlCommand(strin);
            //        }
            //    }
            //}
            //thêm đơn hàng
            string sql2 = "insert into Orders (CustomerName,CustomerAddress,CustomerEmail," +
                "CustomerMobile,CustomerMessage,PaymentMethod,CreatedDate,CreatedBy,PaymentStatus," +
                "Status,CustomerId,TotalPrice)" +
                "values" +
                "(N'"+ fc["fullname"].ToString() +"', N'" + fc["address"] + "', '" + fc["email"].ToString() + 
                "', '" + fc["phone"].ToString()+ "', N'" + fc["message"].ToString() +"', " +
                "N'" + fc["payment"].ToString() + "', GETDATE(), ' ', ' ', 1, " + Session["UserID"].ToString() + ", " + fc["price"] + ")";
            var query2 = db.Database.ExecuteSqlCommand(sql2);
            //thêm OrderDetail
            var RecentOrder = db.Orders.SqlQuery("select * from Orders order by ID DESC").First();
            foreach (var item in query)
            {
                for (int i = 0; i < TempCart.count; i++)
                {
                    if (item.ID == TempCart.ID[i])
                    {
                        string strin = "insert into OrderDetails(OrderID, ProductID, Quantity, Price) values " +
                            "(" + RecentOrder.ID.ToString() + ", " + item.ID.ToString() + ", " + TempCart.ammount[i].ToString() +
                            ", " + (TempCart.ammount[i] * item.Price).ToString() + ")";
                        var queryy = db.Database.ExecuteSqlCommand(strin);
                    }
                }
            }
            //sau khi thêm đơn hàng, xóa giỏ hàng đi
            TempCart.count = 1;
            TempCart.ID = new int[100];
            TempCart.ammount = new int[100];
            Session["Message"] = "Tạo đơn hàng thành công";
            return RedirectToAction("ViewCart", "ShoppingCart");
        }

        //xác nhận mua hàng
        public ActionResult Buy(int id)
        {
            string sql = "Update Orders set Status = 3 where ID = " + id.ToString();
            var query = db.Database.ExecuteSqlCommand(sql);
            return RedirectToAction("MyAccount", "ShoppingCart");
        }

        //thanh toán online
        public ActionResult Paid(int id)
        {
            //trừ tiền
            //string moneypay = Session["MoneyPay"].ToString();
            string sql = "Update ApplicationUsers set Money = Money - " + Session["MoneyPay"].ToString() + " where Id = " + Session["UserID"].ToString();
            var query = db.Database.ExecuteSqlCommand(sql);

            //cộng tiền cho admin
            string str2 = "update ApplicationUsers set Money = Money + " + Session["MoneyPay"].ToString() + " where Id = 3";
            var query2 = db.Database.ExecuteSqlCommand(str2);

            //thêm vào lịch sử ship
            string strr = "INSERT INTO ShipHistory (ShipperID,OrderID,RecievedMoney,Status,Date) " +
                "VALUES ( 3, " + id.ToString() + ", "
                + Session["MoneyPay"].ToString() + ", 1, GETDATE() )";
            var queryy = db.Database.ExecuteSqlCommand(strr);

            //thay đổi trạng thái đơn hàng
            string sql3 = "Update Orders set Status = 3 where ID = " + id.ToString();
            var query3 = db.Database.ExecuteSqlCommand(sql3);
            return RedirectToAction("MyAccount", "ShoppingCart");
        }

        //trang hiển thị thanh toán online
        public ActionResult Pay(int id)
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

            string n = Session["UserID"].ToString();
            var query3 = (from x in db.ApplicationUsers
                          where x.Id.ToString() == n
                          select x.Money).First();

            ViewBag.Quantity = query;
            ViewBag.Product = query2.ToList();
            ViewBag.OrderNo = id.ToString();
            ViewBag.MoneyPay = 0;
            ViewBag.MyMoney = query3;
            return View();
        }

        //xem danh sách đơn hàng và thông tin tài khoản
        public ActionResult MyAccount()
        {
            int id = int.Parse(Session["UserID"].ToString());
            var query = from o in db.Orders
                        where o.CustomerId == id
                        orderby o.ID descending
                        select o;
            ViewBag.Order = query.ToList();

            var query2 = from us in db.ApplicationUsers
                         where us.Id == id
                         select us;
            ViewBag.Info = query2.ToList();
            return View();
        }

        //xem chi tiết đơn hàng
        public ActionResult OrderDetail(int id)
        {
            var query = (from x in db.OrderDetails
                        where x.OrderID == id
                        select x).ToList();

            string str = "select * from Products where ID = ";
            for(int i = 0; i < query.Count - 1; i++)
            {
                str = str + query[i].ProductID.ToString() + " or ID = "; 
            }
            str = str + query[query.Count - 1].ProductID.ToString();
            var query2 = db.Products.SqlQuery(str);

            ViewBag.Quantity = query;
            ViewBag.Product = query2.ToList();
            ViewBag.OrderNo = id.ToString();

            return View();
        }

        //hủy đơn hàng
        public ActionResult DeleteOrder(int id)
        {
            string sql = "Update Orders set Status = 0 where ID = " + id.ToString();
            var query = db.Database.ExecuteSqlCommand(sql);
            return RedirectToAction("MyAccount","ShoppingCart");
        }

        //nạp tiền
        public ActionResult GetMoney()
        {
            //lấy thông tin người dùng
            int id = int.Parse(Session["UserID"].ToString());
            var query = from us in db.ApplicationUsers
                         where us.Id == id
                         select us;
            ViewBag.Info = query.ToList()[0];

            return View();
        }
        [HttpPost]
        public ActionResult GetMoney(FormCollection fc)
        {
            //gen mã dài loằng ngoằng
            string id = Session["UserID"].ToString();
            string name = Session["UserName"].ToString().ToUpper();
            //string myMoney = fc["MyMoney"].ToString();
            string newMoney = fc["money"].ToString();
            string bank = fc["bank"].ToString().ToUpper();
            string bankcode = fc["bankcode"].ToString();

            string code = name + id + newMoney + bankcode + bank;

            //thêm vào database
            string str = "INSERT INTO PurchaseHistory (UserID,Code,Bank,BankCode,Value,Status,Date)  " +
                "VALUES ("+ id + ", '" + code + "', '" + bank + "', '" + bankcode + "', " + newMoney + ", 0, GETDATE())";
            var query = db.Database.ExecuteSqlCommand(str);

            Session["code"] = code;
            Session["Message"] = "Yêu cầu đang được xử lý. Xem tại lịch sử nạp";

            return RedirectToAction("GetMoney","ShoppingCart");
            //try
            //{
            //    string str = "update ApplicationUsers set Money = Money + " + fc["money"].ToString() + " where Id = " + Session["UserID"].ToString();
            //    var query = db.Database.ExecuteSqlCommand(str);
            //    Session["Message"] = "Nạp thêm thành công !";
            //    return RedirectToAction("GetMoney","ShoppingCart");
            //}
            //catch
            //{
            //    Session["Message"] = "Nạp thêm thất bại !";
            //    return RedirectToAction("GetMoney", "ShoppingCart");
            //}
        }
        public ActionResult RequestInfo()
        {
            int id = int.Parse(Session["UserID"].ToString());
            var query = (from x in db.PurchaseHistories
                        where x.UserID == id
                        orderby x.ID descending
                        select x).ToList();
            ViewBag.RequestInfo = query;
            return View();
        }
    }
}