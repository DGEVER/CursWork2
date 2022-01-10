using CursWork.Models;
using CursWork.Models.DBModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

namespace CursWork.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private StudentContext db;
        public HomeController(ILogger<HomeController> logger, StudentContext sc)
        {
            _logger = logger;
            db = sc;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.Keys.Contains("id") && HttpContext.Session.Keys.Contains("type"))
            {
                if (HttpContext.Session.GetString("type") == "student")
                    return RedirectToAction("Mainpage_Student", "Student");

                if (HttpContext.Session.GetString("type") == "teacher")
                    return RedirectToAction("Mainpage_Teacher", "Teacher");

                if (HttpContext.Session.GetString("type") == "admin")
                    return RedirectToAction("Mainpage_Admin", "Admin");
            }
            return View();
        }

        public IActionResult Login(string login, string password)
        {
            if (login == null || password == null)
            {
                return View("NoLogin");
            }
            byte[] data = Encoding.Default.GetBytes(password);
            var result = new SHA256Managed().ComputeHash(data);
            string hash = BitConverter.ToString(result).Replace("-", "").ToUpper();

            var findUser = db.Users.Where(p => p.Login == login && p.HashPassword == hash);

            if (!findUser.Any()) return View("NoLogin");

            var cur = findUser.First();
            HttpContext.Session.SetString("id", cur.IdUser.ToString());
            HttpContext.Session.SetString("type", cur.Type);


            if (cur.Type == "student")
            {
                return RedirectToAction("Mainpage_student", "Student");
            }

            if (cur.Type == "teacher")
            {
                return RedirectToAction("Mainpage_teacher", "Teacher");
            }

            if (cur.Type == "admin")
            {
                return RedirectToAction("Mainpage_admin", "Admin");
            }
            return View("NoLogin");
        }

        public IActionResult Exit()
        {
            HttpContext.Session.Clear();
            return View("Index");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
