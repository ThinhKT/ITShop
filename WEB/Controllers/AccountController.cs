using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEB.Assets.User;

namespace WEB.Controllers
{
    public class AccountController : Controller
    {
        public ShopEntities db = new ShopEntities();
        // GET: Account
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
            string username = fc["UserName"].ToString();
            string password = fc["Password"].ToString();
            var query = from u in db.ApplicationUsers
                        where u.UserName == username
                        && u.PasswordHash == password
                        select u;
            if (query.Count() == 0)
            {
                ViewBag.Err = "Đăng nhập sai";
                return View();
            }
            else
            {
                //khởi tạo giỏ hàng
                TempCart.count = 1;
                TempCart.ID = new int[100];
                TempCart.ammount = new int[100];
                //lưu lại tên user
                Session["UserID"] = 1;
                Session["UserName"] = "Thịnh";
                return RedirectToAction("Dashboard", "Dashboard");
            }
        }
        public ActionResult Logout()
        {
            Session["UserID"] = null;
            Session["UserName"] = null;
            return RedirectToAction("Dashboard", "Dashboard");
        }
        public ActionResult Register()
        {
            return View();
        }
        public ActionResult Detail()
        {
            return View();
        }
    }
}