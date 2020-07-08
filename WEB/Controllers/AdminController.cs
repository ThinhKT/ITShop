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

        //Load Ngưới Dùng
        public ActionResult ListUser()
        {
            Session["View"] = "ListUser";
            var query = from pd in db.ApplicationUsers
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
        public ActionResult Manager()
        {
            Session["View"] = "Other";
            return View();
        }
        #region Shipper
        public ActionResult Shipper()
        {
            Session["View"] = "Other";
            var query = from pd in db.ApplicationUsers
                        where pd.IsShipper == true
                        select pd;
            ViewBag.UserList = query.ToList();
            return View();
        }
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
            string str = "INSERT INTO ApplicationUsers (FullName,BirthDay,Email,PhoneNumber,UserName,IsAdmin,IsShipper,Money,PasswordHash)" +
                " VALUES (N'" + fc["name"] + "', " + fc["birthday"].ToString() + ", '" + fc["email"] + "', '" + fc["phone"] +
                "', '" + fc["username"] + "', 0, 1, 0,'" + fc["password"].ToString() + "')";
            var query2 = db.Database.ExecuteSqlCommand(str);
            ViewBag.Sucsess = "Thêm shipper thành công !";
            return View();
        }
        public ActionResult DeleteShipper(int id)
        {
            string str = "update ApplicationUsers set IsShipper = 0 where ID = " + id.ToString();
            var query = db.Database.ExecuteSqlCommand(str);
            return RedirectToAction("Shipper", "Admin");
        }
        #endregion
        public ActionResult Chart()
        {
            Session["View"] = "Other";
            return View();
        }
        public ActionResult Order()
        {
            Session["View"] = "Order";
            string str = "select * from Orders order by ID DESC";
            var query = db.Orders.SqlQuery(str);
            ViewBag.OrderList = query.ToList();
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
        public ActionResult TakeOrder(int id)
        {
            string str = "update Orders set Status = 2 where ID = " + id.ToString();
            var query = db.Database.ExecuteSqlCommand(str);
            return RedirectToAction("Order","Admin");
        }
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
        public ActionResult Checkout(int id)
        {
            string str = "update Orders set Status = 4 where ID = " + id.ToString();
            var query = db.Database.ExecuteSqlCommand(str);
            return RedirectToAction("Order", "Admin");
        }
        public ActionResult Income()
        {
            Session["View"] = "Income";
            return View();
        }
    }
}