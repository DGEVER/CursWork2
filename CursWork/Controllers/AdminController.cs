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
using System;
using System.IO;

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

        public class PUPlan
        {
            public int ID { get; set; }
            public string NameGroup { get; set; }
            public string NameType { get; set; }
            public string NamePredmet { get; set; }
            public string NameTeacher { get; set; }

            public PUPlan(int id, string nameGroup, string nameType, string namePredmet, string nameTeacher)
            {
                ID = id;
                NameGroup = nameGroup;
                NameType = nameType;
                NamePredmet = namePredmet;
                NameTeacher = nameTeacher;
            }
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

        public IActionResult AddTeacher1()
        {
            var predmets = db.Predmets.ToList();
            ViewBag.Predmets = predmets;
            return View();
        }

        public IActionResult AddTeacher2(int id_predm, string surname, string name, string secondName, string number, string login, string password)
        {
            byte[] data = Encoding.Default.GetBytes(password);
            var result = new SHA256Managed().ComputeHash(data);
            string hash = BitConverter.ToString(result).Replace("-", "").ToUpper();

            User user = new User();
            user.Login = login;
            user.HashPassword = hash;
            user.Type = "teacher";

            db.Users.Add(user);

            db.SaveChanges();

            Teacher teacher = new Teacher();
            teacher.Name = name;
            teacher.Surname = surname;
            teacher.SecondName = secondName;
            teacher.ContactNumb = number;
            teacher.IdUser = db.Users.OrderByDescending(x => x.IdUser).First().IdUser;

            db.Teachers.Add(teacher);

            db.SaveChanges();

            return RedirectToAction("Mainpage_Admin", "Admin");
        }

        public IActionResult AddPredmet1()
        {
            return View();
        }

        public IActionResult AddPredmet2(string namePredmet)
        {
            Predmet predmet = new Predmet();
            predmet.PredmetName = namePredmet;
            db.Predmets.Add(predmet);
            db.SaveChanges();
            return RedirectToAction("Mainpage_Admin", "Admin");
        }

        public IActionResult AddUPlanUnit1()
        {
            var teachers = db.Teachers.ToList();
            var groups = db.Groups.ToList();
            var predmets = db.Predmets.ToList();
            var types = db.TypeOfControls.ToList();

            ViewBag.Predmets = predmets;
            ViewBag.Types = types;
            ViewBag.Teachers = teachers;
            ViewBag.Groups = groups;
            return View();
        }

        public IActionResult AddUPlanUnit2(int id_type, int id_predm, int id_teacher, int id_group, int hourse, int semestr)
        {
            UplanUnit uplanUnit = new UplanUnit();
            uplanUnit.IdGroup = id_group;
            uplanUnit.IdTeacher = id_teacher;
            uplanUnit.IdPredmet = id_predm;
            uplanUnit.IdTypeOfControl = id_type;
            uplanUnit.CounterOfHours = hourse;
            uplanUnit.Semestr = semestr;

            db.UplanUnits.Add(uplanUnit);
            db.SaveChanges();
            return RedirectToAction("Mainpage_Admin", "Admin");
        }

        public IActionResult AddExam1()
        {
            var uplans = from UplanUnit in db.UplanUnits
                         join Group in db.Groups on UplanUnit.IdGroupNavigation.IdGroup equals Group.IdGroup
                         join Predmet in db.Predmets on UplanUnit.IdPredmetNavigation.IdPredmet equals Predmet.IdPredmet
                         join TypeOfControl in db.TypeOfControls on UplanUnit.IdTypeOfControlNavigation.IdTypeOfControl equals TypeOfControl.IdTypeOfControl
                         join Teacher in db.Teachers on UplanUnit.IdTeacherNavigation.IdTeacher equals Teacher.IdTeacher
                         select new
                         {
                             ID = UplanUnit.IdUplanUnit,
                             NameGroup = Group.GroupName,
                             NamePredmet = Predmet.PredmetName, 
                             NameType = TypeOfControl.TypeOfControlName,
                             NameTeacher = Teacher.Surname + ' ' + Teacher.Name + ' ' + Teacher.SecondName
                         };
            uplans = uplans.OrderBy(x => x.NameGroup);

            List<PUPlan> plan = new List<PUPlan>();
            foreach (var planItem in uplans)
            {
                plan.Add(new PUPlan(planItem.ID, planItem.NameGroup, planItem.NameType, planItem.NamePredmet, planItem.NameTeacher));
            }
            ViewBag.UPlanUnit = plan;
            return View();
        }

        public IActionResult AddExam2(int id, DateTime dateEx)
        {
            Exam exam = new Exam();
            exam.IdPlanUnit = id;
            exam.Date = dateEx;

            db.Exams.Add(exam);
            db.SaveChanges();
            return RedirectToAction("Mainpage_Admin", "Admin");
        }
    }
}
