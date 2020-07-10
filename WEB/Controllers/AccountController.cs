using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Hosting;
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
        //[HttpPost]
        //public ActionResult Login(FormCollection fc)
        //{
        //    string username = fc["UserName"].ToString();
        //    string password = fc["Password"].ToString();
        //    var query = from u in db.ApplicationUsers
        //                where u.UserName == username
        //                && u.PasswordHash == password
        //                select u.Id;
        //    var query2 = from u in db.ApplicationUsers
        //                where u.UserName == username
        //                && u.PasswordHash == password // First() để lấy phần tử đầu tiên
        //                 select u.FullName;
        //    if (query.Count() == 0)
        //    {
        //        ViewBag.Err = "Đăng nhập sai";
        //        return View();
        //    }
        //    else
        //    {
        //        //khởi tạo giỏ hàng
        //        TempCart.count = 1;
        //        TempCart.ID = new int[100];
        //        TempCart.ammount = new int[100];
        //        //lưu lại tên user
        //        Session["UserID"] = query.First();
        //        Session["UserName"] = query2.First();
        //        return RedirectToAction("Dashboard", "Dashboard");
        //    }
        //}
        public ActionResult GetPassword(int regId)

        {
            ViewBag.regID = regId;
            return View();
        }
        public JsonResult ResetPassword(int regId, string password)
        {
            // string message = "Tai khoan";
            ApplicationUser DataItem = db.ApplicationUsers.Where(x => x.Id == regId).FirstOrDefault();
            DataItem.PasswordHash = password;
            db.SaveChanges();
            string message = "Tài khoản của bạn đã được thay đổi mật khẩu";

            return Json(message, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveData(ApplicationUser model)
        {
            model.LockoutEnabled = false;
            db.ApplicationUsers.Add(model);
            db.SaveChanges();
            BuildEmailTemplate(model.Id);
            return Json("Registration Successfull", JsonRequestBehavior.AllowGet);
        }
        public ActionResult Confirm(int regId)
        {
            ViewBag.regID = regId;
            return View();
        }
        public JsonResult RegisterConfirm(int regId)
        {
            ApplicationUser Data = db.ApplicationUsers.Where(x => x.Id == regId).FirstOrDefault();
            Data.LockoutEnabled = true;
            db.SaveChanges();
            var msg = "Địa chỉ của bạn đã được xác nhận!";
            return Json(msg, JsonRequestBehavior.AllowGet);
            //string result = "success";
            //return Json(result, JsonRequestBehavior.AllowGet);
        }
        public void BuildEmailTemplate(int regID)
        {
            string body = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailTemplate/") + "Text" + ".cshtml ");
            var regInfo = db.ApplicationUsers.Where(x => x.Id == regID).FirstOrDefault();
            var url = "http://localhost:4039/" + "Account/Confirm?regId=" + regID;
            body = body.Replace("@ViewBag.ConfirmationLink", url);
            body = body.ToString();
            BuildEmailTemplate("Tài khoản của bạn đã được đăng ký.", body, regInfo.Email);
        }
        //đặt lại pass
        public void BuildEmailTemplate(int regID, int a)
        {
            string body = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailTemplate/") + "capmatkhau" + ".cshtml ");
            var regInfo = db.ApplicationUsers.Where(x => x.Id == regID).FirstOrDefault();
            var url = "http://localhost:4039/" + "Account/GetPassword/?regId=" + regID;
            body = body.Replace("@ViewBag.ConfirmationLink1", url);
            body = body.ToString();
            BuildEmailTemplate("Tạo lại mật khẩu", body, regInfo.Email);
        }
        //public ActionResult Confirm(int regId, int a)
        //{
        //    ViewBag.regID = regId;
        //    return View();
        //}
        public static void BuildEmailTemplate(string subjectText, string bodyText, string sendTo)
        {
            string from, to, bcc, cc, subject, body;
            from = "duchuypham200@gmail.com";
            to = sendTo.Trim();
            bcc = "";
            cc = "";
            subject = subjectText;
            StringBuilder sb = new StringBuilder();
            sb.Append(bodyText);
            body = sb.ToString();
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(from);
            mail.To.Add(new MailAddress(to));
            if (!string.IsNullOrEmpty(bcc))
            {
                mail.Bcc.Add(new MailAddress(bcc));
            }
            if (!string.IsNullOrEmpty(cc))
            {
                mail.CC.Add(new MailAddress(cc));
            }
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;
            SendEmail(mail);
        }

        public static void SendEmail(MailMessage mail)
        {
            SmtpClient client = new SmtpClient();
            client.Host = "smtp.gmail.com";
            client.Port = 587;
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Credentials = new System.Net.NetworkCredential("duchuypham200@gmail.com", "0334591287");
            try
            {
                client.Send(mail);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult CheckValidUser(ApplicationUser model)
        {
            string result = "1";
            var DataItem = db.ApplicationUsers.Where(x => x.Email == model.Email && x.PasswordHash == model.PasswordHash && x.LockoutEnabled == true).FirstOrDefault();
            if (DataItem != null)
            {
                Session["UserID"] = DataItem.Id.ToString();
                Session["UserName"] = DataItem.UserName.ToString();
                //khởi tạo giỏ hàng
                TempCart.count = 1;
                TempCart.ID = new int[100];
                TempCart.ammount = new int[100];
                result = "2";
            }
            else
                result = "1";
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ChangePassWord(string Email1)
        {
            string result = "Fail";
            var DataItem = db.ApplicationUsers.Where(x => x.Email == Email1).FirstOrDefault();
            //Kiem tra khi tạo mk mới tk có dag hd ko true or F
            if (DataItem != null)
            {
                Session["UserID"] = DataItem.Id.ToString();
                BuildEmailTemplate(DataItem.Id, 1);

                // Session["UserName"] = DataItem.UserName.ToString();
                result = "Success";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
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