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
using Microsoft.EntityFrameworkCore;

namespace CursWork.Controllers
{
    public class StudentController : Controller
    {
        private readonly ILogger<StudentController> _logger;

        private StudentContext db;

        public StudentController(StudentContext sc, ILogger<StudentController> logger)
        {
            _logger = logger;
            db = sc;
        }

        public class PGrade
        {
            public PGrade(string date, string namePredmet, string surname, string typeControl, string curGrade)
            {
                Date = date;
                NamePredmet = namePredmet;
                SurnameTeacher = surname;
                TypeControl = typeControl;
                CurGrade = curGrade;
            }
            public string Date { get; set; }
            public string NamePredmet { get; set; }
            public string SurnameTeacher { get; set; }
            public string TypeControl { get; set; }
            public string CurGrade { get; set; }
        }

        public class PPredmet
        {
            public PPredmet(string name, string type, int count, int semestr)
            {
                NamePredmet = name;
                TypeControl = type;
                Count = count;
                Semestr = semestr;
            }
            public string NamePredmet { get; set; }
            public string TypeControl { get; set; }
            public int Count { get; set; }
            public int Semestr { get; set; }
        }
        public IActionResult Mainpage_Student()
        {
            var student = from Student in db.Students
                          join User in db.Users on Student.IdUserNavigation.IdUser equals User.IdUser
                          join Group in db.Groups on Student.IdGroupNavigation.IdGroup equals Group.IdGroup
                          join Speciality in db.Specialities on Group.IdSpecialityNavigation.IdSpeciality equals Speciality.IdSpeciality
                          join FormOfEducat in db.FormOfEducats on Group.IdFormOfEducationNavigation.IdForm equals FormOfEducat.IdForm
                          select new
                          {
                              Name = Student.Name,
                              Surname = Student.Surname,
                              SecondName = Student.SecondName,
                              Group = Group.GroupName,
                              Spec = Speciality.SpecialityName,
                              Form = FormOfEducat.FormName,
                              ID = User.IdUser
                          };
            int id = Convert.ToInt32(HttpContext.Session.GetString("id"));
            var curStudent = student.Where(p => p.ID == id);
            ViewBag.Name = curStudent.First().Name;
            ViewBag.Surname = curStudent.First().Surname;
            ViewBag.SecondName = curStudent.First().SecondName;
            ViewBag.Group = curStudent.First().Group;
            ViewBag.Spec = curStudent.First().Spec;
            ViewBag.Form = curStudent.First().Form;
            _logger.LogInformation("Открытие страницы: Mainpage_Student");
            return View();
        }
        public IActionResult Grade()
        {
            int id = Convert.ToInt32(HttpContext.Session.GetString("id"));
            var grade = from Uspevaemost in db.Uspevaemosts
                        join Student in db.Students on Uspevaemost.IdStudentNavigation.IdStudent equals Student.IdStudent
                        join Exam in db.Exams on Uspevaemost.IdExamNavigation.IdExam equals Exam.IdExam
                        join UplanUnit in db.UplanUnits on Exam.IdPlanUnitNavigation.IdUplanUnit equals UplanUnit.IdUplanUnit
                        join Predmet in db.Predmets on UplanUnit.IdPredmetNavigation.IdPredmet equals Predmet.IdPredmet
                        join Teacher in db.Teachers on UplanUnit.IdTeacherNavigation.IdTeacher equals Teacher.IdTeacher
                        join TypeOfControl in db.TypeOfControls on UplanUnit.IdTypeOfControlNavigation.IdTypeOfControl equals TypeOfControl.IdTypeOfControl
                        //where Uspevaemost.IdStudent == id
                        where Student.IdUser == id
                        select new
                        {
                            Date = Exam.Date.ToShortDateString(),
                            NamePr = Predmet.PredmetName,
                            Surname = Teacher.Surname,
                            TypeControl = TypeOfControl.TypeOfControlName,
                            CurGrade = Uspevaemost.Mark

                        };
            List<PGrade> pg = new List<PGrade>();
            foreach (var i in grade)
            {
                pg.Add(new PGrade(i.Date, i.NamePr, i.Surname, i.TypeControl, i.CurGrade));
            }

            ViewBag.Grades = pg;
            _logger.LogInformation("Открытие страницы: Grade");
            return View();
        }

        public IActionResult GradeSearch(string namePredmet)
        {
            int id = Convert.ToInt32(HttpContext.Session.GetString("id"));
            var grade = from Uspevaemost in db.Uspevaemosts
                        join Student in db.Students on Uspevaemost.IdStudentNavigation.IdStudent equals Student.IdStudent
                        join Exam in db.Exams on Uspevaemost.IdExamNavigation.IdExam equals Exam.IdExam
                        join UplanUnit in db.UplanUnits on Exam.IdPlanUnitNavigation.IdUplanUnit equals UplanUnit.IdUplanUnit
                        join Predmet in db.Predmets on UplanUnit.IdPredmetNavigation.IdPredmet equals Predmet.IdPredmet
                        join Teacher in db.Teachers on UplanUnit.IdTeacherNavigation.IdTeacher equals Teacher.IdTeacher
                        join TypeOfControl in db.TypeOfControls on UplanUnit.IdTypeOfControlNavigation.IdTypeOfControl equals TypeOfControl.IdTypeOfControl
                        where Student.IdUser == id
                        select new
                        {
                            Date = Exam.Date.ToShortDateString(),
                            NamePr = Predmet.PredmetName,
                            Surname = Teacher.Surname,
                            TypeControl = TypeOfControl.TypeOfControlName,
                            CurGrade = Uspevaemost.Mark

                        };
            grade = grade.Where(p => EF.Functions.Like(p.NamePr, namePredmet + '%'));
            List<PGrade> pg = new List<PGrade>();
            foreach (var i in grade)
            {
                pg.Add(new PGrade(i.Date, i.NamePr, i.Surname, i.TypeControl, i.CurGrade));
            }

            ViewBag.Grades = pg;
            _logger.LogInformation("Вызов функции поиска");
            return View("Grade");
        }

        public IActionResult ShowPredmets()
        {
            int id_group;
            int id = Convert.ToInt32(HttpContext.Session.GetString("id"));
            var student = from Student in db.Students
                          where Student.IdUser == id
                          select new
                          {
                              IdGroup = Student.IdGroup
                          };
            
            id_group = student.First().IdGroup;

            var predm = from UplanUnit in db.UplanUnits
                        join TypeOfControl in db.TypeOfControls on UplanUnit.IdTypeOfControlNavigation.IdTypeOfControl equals TypeOfControl.IdTypeOfControl
                        join Predmet in db.Predmets on UplanUnit.IdPredmetNavigation.IdPredmet equals Predmet.IdPredmet
                        where UplanUnit.IdGroup == id_group
                        orderby UplanUnit.Semestr
                        select new
                        {
                            IdGroup = UplanUnit.IdGroup,
                            NamePred = Predmet.PredmetName,
                            TypeControl = TypeOfControl.TypeOfControlName,
                            Count = UplanUnit.CounterOfHours,
                            Sem = UplanUnit.Semestr
                        };

            List<PPredmet> pred = new List<PPredmet>();

            foreach (var i in predm)
            {
                pred.Add(new PPredmet(i.NamePred, i.TypeControl, i.Count, i.Sem));
            }
            ViewBag.Predmets = pred;
            _logger.LogInformation("Открытие страницы: ShowPredmets");
            return View();
        }
    }
}
