using System;
using System.Collections.Generic;
using System.Data.Entity;
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

        //đăng nhập
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
                         where x.IsAdmin == true && x.UserName == username
                         select x.PasswordHash).ToList();
            if (query.Count == 0)
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
                          where x.IsAdmin == true && x.UserName == username
                          select x).ToList();
            Session["Name"] = query2[0].FullName;
            Session["ID"] = query2[0].Id;
            return RedirectToAction("Dashboard", "Admin");
        }
        public ActionResult Logout()
        {
            Session["Name"] = null;
            Session["ID"] = null;
            return View("Login");
        }

        //trang chủ
        public ActionResult Dashboard()
        {
            //lấy các đơn hàng không bị hủy để show ra
            string str = "select * from Orders where Status = 4 or Status = 5 or Status = 6 order by ID DESC";
            var query = db.Orders.SqlQuery(str);
            ViewBag.OrderList = query.ToList();
            Session["View"] = "Dashboard";

            return View();
        }

        #region product
        //Load Sản Phẩm và Danh Mục Sản Phẩm
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

        //Thêm Sản Phẩm
        public ActionResult AddProduct()
        {
            Session["View"] = "Product";
            var query = from pd in db.ProductCategories
                         select pd.ID;
            ViewBag.ProductCategoryLoad = new SelectList(query.ToList());
            return View();
        }
        [HttpPost]
        public ActionResult AddProduct(Product product)
        {
            Session["View"] = "Product";
            db.Products.Add(product);
            db.SaveChanges();
            return RedirectToAction("Product");
        }

        //Sửa Sản Phẩm
        public ActionResult EditProduct(int? Id)
        {
            Session["View"] = "Product";
            var query = from pd in db.ProductCategories
                        select pd.ID;
            ViewBag.ProductCategoryLoad = new SelectList(query.ToList());

            Product product = new Product();
            product = db.Products.Find(Id);
            return View(product);
        }
        [HttpPost]
        public ActionResult EditProduct(Product product)
        {
            Session["View"] = "Product";
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Product");
        }

        //Xóa Sản Phẩm
        public ActionResult DeleteProduct(int? Id)
        {
            Product products = new Product();
            products = db.Products.Find(Id);
            db.Products.Remove(products);
            db.SaveChanges();
            return RedirectToAction("Product");
        }
        #endregion

        #region productCate
        //Thêm Danh Mục Sản Phẩm
        public ActionResult AddProductCate()
        {
            Session["View"] = "Product";
            return View();
        }
        [HttpPost]
        public ActionResult AddProductCate(ProductCategory productCategory)
        {
            Session["View"] = "Product";
            db.ProductCategories.Add(productCategory);
            db.SaveChanges();
            return RedirectToAction("Product");
        }

        //Sửa Danh Mục Sản Phẩm
        public ActionResult EditProductCate(int? Id)
        {
            Session["View"] = "Product";
            ProductCategory productCategory = new ProductCategory();
            productCategory = db.ProductCategories.Find(Id);
            return View(productCategory);
        }
        [HttpPost]
        public ActionResult EditProductCate(ProductCategory productCategory)
        {
            Session["View"] = "Product";
            if (ModelState.IsValid)
            {
                db.Entry(productCategory).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Product");
        }

        //Xóa Danh Mục Sản Phẩm
        public ActionResult DeleteProductCate(int? Id)
        {
            ProductCategory productCategory = new ProductCategory();
            productCategory = db.ProductCategories.Find(Id);
            db.ProductCategories.Remove(productCategory);
            db.SaveChanges();
            return RedirectToAction("Product");
        }
        public ActionResult ProductCate()
        {
            Session["View"] = "Other";
            return View();
        }
        #endregion

        #region User 
        //Load Người Dùng
        public ActionResult ListUser()
        {
            Session["View"] = "ListUser";
            var query = from pd in db.ApplicationUsers
                        where pd.IsAdmin == false && pd.IsShipper == false
                        select pd;
            ViewBag.UserList = query.ToList();
            return View();
        }

        //Thêm Người Dùng
        public ActionResult AddUser()
        {
            Session["View"] = "ListUser";
            return View();
        }
        [HttpPost]
        public ActionResult AddUser(ApplicationUser applicationUser)
        {
            Session["View"] = "ListUser";
            db.ApplicationUsers.Add(applicationUser);
            db.SaveChanges();
            return RedirectToAction("ListUser");
        }

        //Sửa Người Dùng
        public ActionResult EditUser(int? Id)
        {
            Session["View"] = "ListUser";
            ApplicationUser applicationUser = new ApplicationUser();
            applicationUser = db.ApplicationUsers.Find(Id);
            return View(applicationUser);
        }
        [HttpPost]
        public ActionResult EditUser(ApplicationUser applicationUser)
        {
            Session["View"] = "ListUser";
            if (ModelState.IsValid)
            {
                db.Entry(applicationUser).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("ListUser");
        }

        //Xóa Người Dùng
        public ActionResult DeleteUser(int? Id)
        {
            ApplicationUser applicationUser = new ApplicationUser();
            applicationUser = db.ApplicationUsers.Find(Id);
            db.ApplicationUsers.Remove(applicationUser);
            db.SaveChanges();
            return RedirectToAction("ListUser");
        }

        public ActionResult Purchase()
        {
            Session["View"] = "Other";
            var query = from x in db.PurchaseHistories
                        orderby x.ID descending
                        select x;
            ViewBag.PurchaseInfo = query.ToList();
            return View();
        }
        public ActionResult DoPurchase(int id)
        {
            var query = (from x in db.PurchaseHistories
                        where x.ID == id
                        select x).ToList()[0];
            string str = "update ApplicationUsers set Money = Money + " + query.Value.ToString() + " where Id = " + query.UserID.ToString();
            var query2 = db.Database.ExecuteSqlCommand(str);
            string str2 = "update PurchaseHistory set Status = 1 where ID = " + query.ID.ToString();
            var query3 = db.Database.ExecuteSqlCommand(str2);

            return RedirectToAction("Purchase","Admin");
        }

        public ActionResult Manager()
        {
            Session["View"] = "Other";
            return View();
        }
        #endregion

        #region Shipper
        //xem danh sách Shipper
        public ActionResult Shipper()
        {
            Session["View"] = "Other";
            var query = from pd in db.ApplicationUsers
                        where pd.IsShipper == true
                        select pd;
            ViewBag.UserList = query.ToList();
            return View();
        }

        //Thêm Shipper
        public ActionResult AddShipper()
        {
            Session["View"] = "Other";
            return View();
        }
        [HttpPost]
        public ActionResult AddShipper(FormCollection fc)
        {
            //kiểm tra trước
            //kiểm tra password và re-pasword
            if (fc["password"] != fc["re_password"])
            {
                ViewBag.PassNotMatch = "Mật khẩu nhập lại không trùng khớp !";
                return View();
            }
            //kiểm tra trùng username
            string username = fc["username"].ToString();
            var query = (from x in db.ApplicationUsers
                        where x.UserName == username && x.IsShipper == true
                        select x).ToList();
            if(query.Count != 0)
            {
                ViewBag.ExistUsername = "Tên đăng nhập đã tồn tại";
                return View();
            }
            string str = "INSERT INTO ApplicationUsers (FullName,BirthDay,Email,PhoneNumber,UserName,IsAdmin,IsShipper,Money,PasswordHash,Money)" +
                " VALUES (N'" + fc["name"] + "', " + fc["birthday"].ToString() + ", '" + fc["email"] + "', '" + fc["phone"] +
                "', '" + fc["username"] + "', 0, 1, 0,'" + fc["password"].ToString() + "', 0)";
            var query2 = db.Database.ExecuteSqlCommand(str);
            ViewBag.Sucsess = "Thêm shipper thành công !";
            return View();
        }

        //Xóa Shipper
        public ActionResult DeleteShipper(int id)
        {
            string str = "update ApplicationUsers set IsShipper = 0 where ID = " + id.ToString();
            var query = db.Database.ExecuteSqlCommand(str);
            return RedirectToAction("Shipper", "Admin");
        }
        #endregion

        #region order
        //xem danh sách đơn hàng
        public ActionResult Order()
        {
            Session["View"] = "Order";
            string str = "select * from Orders order by ID DESC";
            var query = db.Orders.SqlQuery(str);
            ViewBag.OrderList = query.ToList();
            return View();
        }

        //xem chi tiết đơn hàng
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

        //xem kho
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
            ViewBag.AbleToSale = true;
            return View();
        }

        //tiếp nhận, duyệt đơn hàng
        public ActionResult TakeOrder(int id)
        {
            string str = "update Orders set Status = 2 where ID = " + id.ToString();
            var query = db.Database.ExecuteSqlCommand(str);
            return RedirectToAction("Order","Admin");
        }

        //kho không đủ, hủy đơn hàng
        public ActionResult RemoveOrder(int id)
        {
            string str = "update Orders set Status = -1 where ID = " + id.ToString();
            var query = db.Database.ExecuteSqlCommand(str);
            return RedirectToAction("Order", "Admin");
        }

        public ActionResult Pending(int id)
        {
            //không dùng
            string str = "update Orders set Status = 3 where ID = " + id.ToString();
            var query = db.Database.ExecuteSqlCommand(str);
            return RedirectToAction("Order", "Admin");
        }

        //admin đóng gói để giao cho shipper
        public ActionResult Checkout(int id)
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

            //string str = "update Orders set Status = 4 where ID = " + id.ToString();
            //var query = db.Database.ExecuteSqlCommand(str);
            //return RedirectToAction("Order", "Admin");
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
            return RedirectToAction("Order", "Admin");
        }
        #endregion

        public ActionResult Income()
        {
            Session["View"] = "Income";
            return View();
        }

        //lịch sử bán hàng
        public ActionResult ShipHistory()
        {
            Session["View"] = "Other";
            var query = from x in db.ShipHistories
                        select x;
            ViewBag.ShipHistory = query.ToList();

            return View();
        }

        public ActionResult Chart()
        {
            Session["View"] = "Other";
            return View();
        }
    }
}