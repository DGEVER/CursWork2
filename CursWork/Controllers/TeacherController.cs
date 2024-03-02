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
using TemplateEngine.Docx;
using Microsoft.Office.Interop.Word;
using Microsoft.AspNetCore.Hosting;

namespace CursWork.Controllers
{
    public class TeacherController : Controller
    {

        private readonly IWebHostEnvironment _appEnvironment;

        private TeacherContext db;
        private readonly ILogger<TeacherController> _logger;

        public TeacherController(TeacherContext tc, ILogger<TeacherController> logger, IWebHostEnvironment appEnvironment)
        {
            _logger = logger;
            db = tc;
            _appEnvironment = appEnvironment;
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

        public class PGroupPredmet
        {
            public int Id { get; set; }
            public string NameGroup { get; set; }
            public string NamePredmet { get; set;}

            public PGroupPredmet(int id, string nameG, string nameP)
            {
                Id = id;
                NameGroup = nameG;
                NamePredmet = nameP;
            }
        }

        public class CurGrade
        {
            public string Surname { get; set; }
            public string Name { get; set; }
            public string SecondName { get; set; }
            public string? Mark { get; set; }
            public string? Date { get; set; }
            public CurGrade(string surName, string name, string secondName, string mark, string date)
            {
                Surname = surName;
                Name = name;
                SecondName = secondName;
                Mark = mark;
                Date = date;
            }
        }

        public class CurGrade2
        {
            public int ID_student { get; set; }
            public string Surname { get; set; }
            public string Name { get; set; }
            public string SecondName { get; set; }
            public string? Mark { get; set; }
            public CurGrade2(int id, string surName, string name, string secondName, string mark)
            {
                ID_student = id;
                Surname = surName;
                Name = name;
                SecondName = secondName;
                Mark = mark;
            }
        }

        public class PExam
        {
            public int ID { get; set; }
            public string NameGroup { get; set; }
            public string NamePredmet { get; set; }
            public string NameType { get; set; }
            public PExam(int id, string ng, string np, string nt)
            {
                ID = id;
                NameGroup=ng;
                NamePredmet = np;
                NameType = nt;
            }
        }
        public IActionResult Mainpage_Teacher()
        {
            string type = HttpContext.Session.GetString("type");
            if (type != "teacher") return RedirectToAction("Index", "Home");
            int id = Convert.ToInt32(HttpContext.Session.GetString("id"));
            var teacher = from Teacher in db.Teachers
                          join User in db.Users on Teacher.IdUserNavigation.IdUser equals User.IdUser
                          where User.IdUser == id
                          select new
                          {
                              ID = User.IdUser,
                              Surname = Teacher.Surname,
                              Name = Teacher.Name,
                              SecondName = Teacher.SecondName,
                              ID_Teacher = Teacher.IdTeacher
                          };
            
            var curTeacher = teacher.FirstOrDefault();
            HttpContext.Session.SetString("id_teacher", curTeacher.ID_Teacher.ToString());
            ViewBag.Name = curTeacher.Name;
            ViewBag.Surname = curTeacher.Surname;
            ViewBag.SecondName = curTeacher.SecondName;
            _logger.LogInformation("Открытие страницы: Mainpage_Teacher");
            return View();
        }

        public IActionResult ShowPredmTeach()
        {
            int id = Convert.ToInt32(HttpContext.Session.GetString("id_teacher"));
            var predm = from UplanUnit in db.UplanUnits
                        join TypeOfControl in db.TypeOfControls on UplanUnit.IdTypeOfControlNavigation.IdTypeOfControl equals TypeOfControl.IdTypeOfControl
                        join Predmet in db.Predmets on UplanUnit.IdPredmetNavigation.IdPredmet equals Predmet.IdPredmet
                        where UplanUnit.IdTeacher == id
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
            _logger.LogInformation("Открытие страницы: ShowPredmTeach");
            return View();
        }

        public IActionResult ShowGrade1()
        {
            int id = Convert.ToInt32(HttpContext.Session.GetString("id_teacher"));
            var groupAndpredmet = from UplanUnit in db.UplanUnits
                                  join Group in db.Groups on UplanUnit.IdGroupNavigation.IdGroup equals Group.IdGroup
                                  join Predmet in db.Predmets on UplanUnit.IdPredmetNavigation.IdPredmet equals Predmet.IdPredmet
                                  where UplanUnit.IdTeacher == id
                                  select new
                                  {
                                      ID = UplanUnit.IdUplanUnit,
                                      NameGroup = Group.GroupName,
                                      NamePredmet = Predmet.PredmetName
                                  };
            groupAndpredmet = groupAndpredmet.OrderBy(p => p.NamePredmet);
            List<PGroupPredmet> pgp = new List<PGroupPredmet>();
            foreach (var g in groupAndpredmet)
            {
                pgp.Add(new PGroupPredmet(g.ID, g.NameGroup, g.NamePredmet));
            }
            ViewBag.GP = pgp;
            _logger.LogInformation("Открытие страницы: ShowGrade1");
            return View();
        }

        public IActionResult ShowGrade2([FromQuery(Name = "id")] int Id)
        {
            int id_teacher = Convert.ToInt32(HttpContext.Session.GetString("id_teacher"));
            var grade = from Uspevaemost in db.Uspevaemosts
                        join Exam in db.Exams on Uspevaemost.IdExamNavigation.IdExam equals Exam.IdExam
                        join UplanUnit in db.UplanUnits on Exam.IdPlanUnitNavigation.IdUplanUnit equals UplanUnit.IdUplanUnit
                        join Student in db.Students on Uspevaemost.IdStudentNavigation.IdStudent equals Student.IdStudent
                        where UplanUnit.IdUplanUnit == Id
                        orderby Student.Surname
                        select new
                        {
                            Surname = Student.Surname,
                            Name = Student.Name,
                            SecondName = Student.SecondName,
                            Mark = Uspevaemost.Mark,
                            Date = Exam.Date
                        };
            
            List<CurGrade> cg = new List<CurGrade>();

            foreach(var g in grade)
            {
                cg.Add(new CurGrade(g.Surname, g.Name, g.SecondName, g.Mark, g.Date.ToShortDateString()));
            }
            string nameGroup = (from UplanUnit in db.UplanUnits
                               join Group in db.Groups on UplanUnit.IdGroupNavigation.IdGroup equals Group.IdGroup
                               where UplanUnit.IdUplanUnit == Id
                               select Group.GroupName).First();

            string namePredmet = (from UplanUnit in db.UplanUnits
                                  join Predmet in db.Predmets on UplanUnit.IdPredmetNavigation.IdPredmet equals Predmet.IdPredmet
                                  where UplanUnit.IdUplanUnit == Id
                                  select Predmet.PredmetName).First();


            var teacher = db.Teachers.Where(t => t.IdTeacher == id_teacher).First();
            string nameTeacher = teacher.Surname + ' ' + teacher.Name + ' ' + teacher.SecondName;
            string date = cg.Last().Date;

            System.IO.File.Delete("OutputFile.docx");
            System.IO.File.Copy("TemplateListGroupWithGrade.docx", "OutputFile.docx");

            TableContent table = new TableContent();
            table.Name = "tableVed";
            int n = 0;
            foreach (var i in cg)
            {
                n++;
                table.AddRow(new FieldContent("number", n.ToString()),
                    new FieldContent("FIO", i.Surname + ' ' + i.Name + ' ' + i.SecondName),
                    new FieldContent("mark", i.Mark));
            }

            var valuesToFile = new Content(
                new FieldContent("group", nameGroup),
                new FieldContent("predmet", namePredmet),
                new FieldContent("teacher", nameTeacher),
                new FieldContent("date", date),
                table
                );

            using (var ouputDocumet = new TemplateProcessor("OutputFile.docx").SetRemoveContentControls(true))
            {
                ouputDocumet.FillContent(valuesToFile);
                ouputDocumet.SaveChanges();
            }



            ViewBag.CG = cg;
            ViewBag.NameGroup = nameGroup;
            ViewBag.NamePredmet = namePredmet;
            ViewBag.ID = Id;
            _logger.LogInformation("Открытие страницы: ShowGrade2");
            return View();
        }


        public IActionResult GetFileClient()
        {
            string file_path = Path.Combine(_appEnvironment.ContentRootPath, "OutputFile.docx");
            string file_type = "application/docx";
            string file_name = "OutputFile.docx";

            _logger.LogInformation("Вызов функции скачивания файла");
            return PhysicalFile(file_path, file_type, file_name);
        }

        public IActionResult SetGrade1()
        {
            int id = Convert.ToInt32(HttpContext.Session.GetString("id_teacher"));
            var exams = from Exam in db.Exams
                        join UplanUnit in db.UplanUnits on Exam.IdPlanUnitNavigation.IdUplanUnit equals UplanUnit.IdUplanUnit
                        join Group in db.Groups on UplanUnit.IdGroupNavigation.IdGroup equals Group.IdGroup
                        join Predmet in db.Predmets on UplanUnit.IdPredmetNavigation.IdPredmet equals Predmet.IdPredmet
                        join TypeOfControl in db.TypeOfControls on UplanUnit.IdTypeOfControlNavigation.IdTypeOfControl equals TypeOfControl.IdTypeOfControl
                        where UplanUnit.IdTeacher == id
                        select new
                        {
                            ID = Exam.IdExam,
                            NameGroup = Group.GroupName,
                            NamePredmet = Predmet.PredmetName,
                            NameType = TypeOfControl.TypeOfControlName
                        };
            List<PExam> pe = new List<PExam>();
            foreach (var exam in exams)
            {
                pe.Add(new PExam(exam.ID, exam.NameGroup, exam.NamePredmet, exam.NameType));
            }

            ViewBag.Exams = pe;
            _logger.LogInformation("Открытие страницы: SetGrade1");
            return View();
        }

        public IActionResult SetGrade2([FromQuery(Name = "id")] int Id)
        {
            
            var uspev = from Uspevaemost in db.Uspevaemosts where Uspevaemost.IdExam == Id select Uspevaemost;

            int id_group = (from Exam in db.Exams
                           join UplanUnit in db.UplanUnits on Exam.IdPlanUnitNavigation.IdUplanUnit equals UplanUnit.IdUplanUnit
                           where Exam.IdExam == Id
                           select UplanUnit.IdGroup).First();

            var students = from Student in db.Students where Student.IdGroup == id_group select Student;

            int id_typeControl = (from Exam in db.Exams
                                 join UplanUnit in db.UplanUnits on Exam.IdPlanUnitNavigation.IdUplanUnit equals UplanUnit.IdUplanUnit
                                 where Exam.IdExam == Id
                                 select UplanUnit.IdTypeOfControl).First();
            List < CurGrade2 > cg = new List<CurGrade2>();
            foreach(var student in students)
            {
                string Mark = null;
                
                foreach(var u in uspev)
                {
                    if (u.IdStudent == student.IdStudent)
                    {
                        Mark = u.Mark;
                    }
                }
                cg.Add(new CurGrade2(student.IdStudent, student.Surname, student.Name, student.SecondName, Mark));
            }

            ViewBag.Course = cg;
            ViewBag.Type = id_typeControl;
            ViewBag.id_exam = Id;
            _logger.LogInformation("Открытие страницы: SetGrade2");
            return View();
        }

        public IActionResult SetGrade3([FromQuery(Name = "id")] int id_exam, [FromQuery(Name = "id_student")] int id_student, [FromQuery(Name = "id_type")] int id_type)
        {
            ViewBag.ID_EXAM = id_exam;
            ViewBag.ID_STUDENT = id_student;
            ViewBag.ID_TYPE = id_type;
            _logger.LogInformation("Открытие страницы: SetGrade3");
            return View();
        }

        public IActionResult SetGrade4(string mark, int id_student, int id_exam)
        {
            _logger.LogInformation("Открытие страницы: SetGrade4");
            Uspevaemost uspevaemost = new Uspevaemost();
            uspevaemost.Mark = mark;
            uspevaemost.IdStudent = id_student;
            uspevaemost.IdExam = id_exam;
            var transaction = db.Database.BeginTransaction();
            try
            {
                db.Uspevaemosts.Add(uspevaemost);

                db.SaveChanges();
                transaction.Commit();
                _logger.LogInformation("Транзакция завершена успешно");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.LogError("Ошибка при добавлении данных");
            }
            return RedirectToAction("Mainpage_Teacher", "Teacher");
        }

        public IActionResult ChangeGrade1([FromQuery(Name = "id")] int id_exam, [FromQuery(Name = "id_student")] int id_student, [FromQuery(Name = "id_type")] int id_type)
        {
            ViewBag.ID_EXAM = id_exam;
            ViewBag.ID_STUDENT = id_student;
            ViewBag.ID_TYPE = id_type;
            _logger.LogInformation("Открытие страницы: ChangeGrade1");
            return View();
        }

        public IActionResult ChangeGrade2(string mark, int id_student, int id_exam)
        {
            _logger.LogInformation("Открытие страницы: ChangeGrade1");
            var uspevaemost = db.Uspevaemosts.Where(p => p.IdStudent == id_student && p.IdExam == id_exam).FirstOrDefault();
            uspevaemost.Mark = mark;
            var transaction = db.Database.BeginTransaction();
            try
            {
                db.Uspevaemosts.Update(uspevaemost);

                db.SaveChanges();
                transaction.Commit();
                _logger.LogInformation("Транзакция завершена успешно");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.LogError("Ошибка при обновлении данных");
            }
            return RedirectToAction("Mainpage_Teacher", "Teacher");
        }

        public IActionResult ShowGroups()
        {
            var groups = (from Groups in db.Groups select Groups).ToList();
            ViewBag.Groups = groups;
            _logger.LogInformation("Открытие страницы: ShowGroups");
            return View();
        }

        public IActionResult ShowStudent([FromQuery(Name = "id")] int id_group)
        {
            
            string nameGroup = db.Groups.Where(p => p.IdGroup == id_group).Select(p => p.GroupName).FirstOrDefault();
            var students = db.Students.Where(p => p.IdGroup == id_group).OrderBy(p => p.Surname).ToList();

            //Подготовка файла
            System.IO.File.Delete("OutputFile.docx");
            System.IO.File.Copy("TemplateListStudents.docx", "OutputFile.docx");
            TableContent table = new TableContent();
            table.Name = "table";
            int n = 0;
            foreach (var i in students)
            {
                n++;
                table.AddRow(new FieldContent("number", n.ToString()),
                    new FieldContent("FIO", i.Surname + ' ' + i.Name + ' ' + i.SecondName));
            }

            var valuesToFile = new Content(
                new FieldContent("nameGroup", nameGroup),
                new FieldContent("count", students.Count().ToString()),
                table
                );

            using (var ouputDocumet = new TemplateProcessor("OutputFile.docx").SetRemoveContentControls(true))
            {
                ouputDocumet.FillContent(valuesToFile);
                ouputDocumet.SaveChanges();
            }


            ViewBag.Students = students;
            ViewBag.NameGroup = nameGroup;
            _logger.LogInformation("Открытие страницы: ShowStudent");
            return View();
        }
    }
}
