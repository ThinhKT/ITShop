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
                }
                else
                {
                    ViewBag.None = "Giỏ hàng không có sản phẩm nào";
                }
                return View();
            } 
        }
        public ActionResult MakeOrder()
        {
            return View();
        }
    }
}