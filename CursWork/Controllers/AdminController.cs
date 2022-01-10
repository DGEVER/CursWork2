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
    public class AdminController : Controller
    {
        private AdminContext db;
        private readonly ILogger<AdminController> _logger;

        public AdminController(AdminContext ac, ILogger<AdminController> logger)
        {
            _logger = logger;
            db = ac;
        }

        public IActionResult Mainpage_Admin()
        {
            return View();
        }

        public IActionResult Show()
        {
            return View();
        }

        public IActionResult Add()
        {
            return View();
        }

        public IActionResult ShowGroups()
        {
            var groups = (from Groups in db.Groups select Groups).ToList();
            ViewBag.Groups = groups;
            return View();
        }

        public IActionResult ShowStudent([FromQuery(Name = "id")] int id_group)
        {
            string nameGroup = db.Groups.Where(p => p.IdGroup == id_group).Select(p => p.GroupName).FirstOrDefault();
            var students = db.Students.Where(p => p.IdGroup == id_group).ToList();
            ViewBag.Students = students;
            ViewBag.NameGroup = nameGroup;
            return View();
        }

        public IActionResult ShowTeachers()
        {
            var teachers = (from Teacher in db.Teachers select Teacher).ToList();
            ViewBag.Teachers = teachers;
            return View();
        }

        public IActionResult AddGroup1()
        {
            //var form = (from FormOfEducat in db.FormOfEducats select FormOfEducat).ToList();
            var form = db.FormOfEducats.ToList();
            //var spec = (from Speciality in db.Specialities select Speciality).ToList();
            var spec = db.Specialities.ToList();
            ViewBag.Specialities = spec;
            ViewBag.Form = form;
            return View();
        }

        public IActionResult AddGroup2(int id_form, int id_spec, string nameGroup)
        {
            Group group = new Group();
            group.GroupName = nameGroup;
            group.IdFormOfEducation = id_form;
            group.IdSpeciality = id_spec;

            db.Groups.Add(group);
            
            db.SaveChanges();
            return RedirectToAction("Mainpage_Admin", "Admin");
        }

        public IActionResult AddStudent1()
        {
            var groups = db.Groups.ToList();
            ViewBag.Groups = groups;
            return View();
        }

        public IActionResult AddStudent2(int id_group, string surname, string name, string secondName, string year, string number, string email,
            string city, string street, string house, string flat, string login, string password)
        {
            byte[] data = Encoding.Default.GetBytes(password);
            var result = new SHA256Managed().ComputeHash(data);
            string hash = BitConverter.ToString(result).Replace("-", "").ToUpper();

            User user = new User();
            user.Login = login;
            user.HashPassword = hash;
            user.Type = "student";

            db.Users.Add(user);
            db.SaveChanges();
            Student student = new Student();
            student.IdUser = db.Users.OrderByDescending(p => p.IdUser).First().IdUser;
            student.IdGroup = id_group;
            student.Name = name;
            student.Surname = surname;
            student.SecondName = secondName;
            student.YearOfAppl = year;
            student.ContactNumber = number;
            student.ContactEmail = email;
            student.City = city;
            student.Street = street;
            student.House = house;
            student.Flat = flat;
            
            db.Students.Add(student);

            db.SaveChanges();
            return RedirectToAction("Mainpage_Admin", "Admin");
        }
    }
}
