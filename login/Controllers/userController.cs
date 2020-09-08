using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using login.Models;
using System.Web.Security;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace login.Controllers
{
    public class userController : Controller
    {
        private usercontext db = new usercontext();
        // GET: Users
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
        //[Authorize]
        public ActionResult Register()
        {
            return View();
        }
        //[Authorize]
        [HttpPost]
        public ActionResult Register(user user)
        {
            var obj = db.Users.Where(u => u.Id.Equals(user.Id)).FirstOrDefault();
            if (obj == null)
            {
                if (ModelState.IsValid)
                {
                    // user.Password = FormsAuthentication.HashPasswordForStoringInConfigFile(user.Password, "sha1");
                    // user.ConfirmPassword= FormsAuthentication.HashPasswordForStoringInConfigFile(user.ConfirmPassword, "sha1");
                    user.Password = encrypt(user.Password);
                    user.ConfirmPassword = encrypt(user.ConfirmPassword);
                    db.Users.Add(user);
                    db.SaveChanges();
                    return RedirectToAction("Index");

                }
                else
                {
                    ModelState.AddModelError("", "Error Occured! Try again!!");
                }
            }
            else
                ModelState.AddModelError("", "User already Exists");
            return View(user);



        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(user user)
        {
            using (usercontext db = new usercontext())
            {
                
                //user.Password = FormsAuthentication.HashPasswordForStoringInConfigFile(user.Password, "sha1");
                user.Password= encrypt(user.Password);
                var obj = db.Users.Where(u => u.Id.Equals(user.Id) && u.Password.Equals(user.Password)).FirstOrDefault();
                if (obj != null)
                {
                    FormsAuthentication.SetAuthCookie(user.Id, false);
                    Session["UserId"] = obj.Id.ToString();
                    Session["Username"] = obj.Name.ToString();
                    return RedirectToAction("Index");
                }
            }
            return View(user);
        }
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Login");
        }

        public string encrypt(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (System.IO.MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }
        
    }
}

