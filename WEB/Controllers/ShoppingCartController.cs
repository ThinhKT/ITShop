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
            string sql = "Select * from Products where ID = ";
            for (int i = 0; i < TempCart.count; i++)
            {
                sql = sql + TempCart.ID[i].ToString() + " or ID = ";
            }
            sql = sql + TempCart.ID[TempCart.count].ToString();
            var query = db.Products.SqlQuery(sql).ToList();
            foreach (var item in query)
            {
                for (int i = 0; i < TempCart.count; i++)
                {
                    if (item.ID == TempCart.ID[i])
                    {
                        if (item.Quantity < TempCart.ammount[i])
                        {
                            Session["CartOverload"] = "Không đủ sản phẩm" + item.Name.ToString();
                            return RedirectToAction("ViewCart", "ShoppingCart");
                        }
                    }
                }
            }
            //nếu không return ở trên, thì còn hàng, trừ hàng ra
            foreach (var item in query)
            {
                for (int i = 0; i < TempCart.count; i++)
                {
                    if (item.ID == TempCart.ID[i])
                    {
                        string strin = "update Products set Quantity = Quantity - " + TempCart.ammount[i].ToString() +
                            "where ID = " + TempCart.ID[i].ToString();
                        var queryy = db.Database.ExecuteSqlCommand(strin);
                    }
                }
            }
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
            return RedirectToAction("ViewCart", "ShoppingCart");
        }
        public ActionResult MyAccount()
        {
            int id = int.Parse(Session["UserID"].ToString());
            var query = from o in db.Orders
                        where o.CustomerId == id
                        select o;
            ViewBag.Order = query.ToList();

            var query2 = from us in db.ApplicationUsers
                         where us.Id == id
                         select us;
            ViewBag.Info = query2.ToList();
            return View();
        }
        public ActionResult OrderDetail(int id)
        {
            return View();
        }
        public ActionResult DeleteOrder(int id)
        {
            string sql = "Delete from Orders where ID = " + id.ToString();
            var query = db.Database.ExecuteSqlCommand(sql);
            return RedirectToAction("MyAccount","ShoppingCart");
        }
    }
}