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
    public class AdminController : Controller
    {
        private AdminContext db;
        private readonly ILogger<AdminController> _logger;

        private readonly IWebHostEnvironment _appEnvironment;

        public AdminController(AdminContext ac, ILogger<AdminController> logger, IWebHostEnvironment appEnvironment)
        {
            _logger = logger;
            db = ac;
            _appEnvironment = appEnvironment;
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
            string type = HttpContext.Session.GetString("type");
            if (type != "admin") return RedirectToAction("Index", "Home");

            _logger.LogInformation("Открытие страницы: Mainpage_Admin");
            return View();
        }

        public IActionResult Show()
        {
            _logger.LogInformation("Открытие страницы: Show");
            return View();
        }

        public IActionResult Add()
        {
            _logger.LogInformation("Открытие страницы: Add");
            return View();
        }

        public IActionResult ShowGroups()
        {
            var groups = (from Groups in db.Groups select Groups).ToList();
            ViewBag.Groups = groups;

            _logger.LogInformation("Открытие страницы: ShowGroups");

            return View();
        }
        public IActionResult ShowInfoAboutStudent([FromQuery(Name = "id")] int id_student)
        {
            var cur_student = (from Student in db.Students where Student.IdStudent == id_student select Student).First();
            string nameGroup = (from Group in db.Groups where Group.IdGroup == cur_student.IdGroup select Group.GroupName).First();

            //Подготовка файла
            System.IO.File.Delete("OutputFile.docx");
            System.IO.File.Copy("TemplateInfoAboutStudent.docx", "OutputFile.docx");
            var valuesToFile = new Content(
                new FieldContent("firstName", cur_student.Surname),
                new FieldContent("name", cur_student.Name),
                new FieldContent("secondName", cur_student.SecondName),
                new FieldContent("year", cur_student.YearOfAppl),
                new FieldContent("group", nameGroup),
                new FieldContent("number", cur_student.ContactNumber),
                new FieldContent("email", cur_student.ContactEmail),
                new FieldContent("city", cur_student.City),
                new FieldContent("street", cur_student.Street),
                new FieldContent("house", cur_student.House),
                new FieldContent("flat", cur_student.Flat)
                );
            using (var ouputDocumet = new TemplateProcessor("OutputFile.docx").SetRemoveContentControls(true))
            {
                ouputDocumet.FillContent(valuesToFile);
                ouputDocumet.SaveChanges();
            }

            _logger.LogInformation("Открытие страницы: ShowInfoAboutStudent");
            ViewBag.Student = cur_student;
            ViewBag.NameGroup = nameGroup;
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

            _logger.LogInformation("Открытие страницы: ShowStudent");

            ViewBag.Students = students;
            ViewBag.NameGroup = nameGroup;
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

        public IActionResult ShowTeachers()
        {
            var teachers = (from Teacher in db.Teachers select Teacher).ToList();
            ViewBag.Teachers = teachers;

            _logger.LogInformation("Открытие страницы: ShowTeachers");

            return View();
        }

        public IActionResult AddGroup1()
        {
            var form = db.FormOfEducats.ToList();
            var spec = db.Specialities.ToList();
            ViewBag.Specialities = spec;
            ViewBag.Form = form;

            _logger.LogInformation("Открытие страницы: AddGroup1");

            return View();
        }

        public IActionResult AddGroup2(int id_form, int id_spec, string nameGroup)
        {
            Group group = new Group();
            group.GroupName = nameGroup;
            group.IdFormOfEducation = id_form;
            group.IdSpeciality = id_spec;

            var transaction = db.Database.BeginTransaction();
            try
            {


                db.Groups.Add(group);

                db.SaveChanges();
                transaction.Commit();
                _logger.LogInformation("Транзакция завершена успешно");

            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.LogError("Ошибка при добавлении данных" + ex.Message);
            }

            _logger.LogInformation("Открытие страницы: AddGroup2");

            return RedirectToAction("Mainpage_Admin", "Admin");
        }

        public IActionResult AddStudent1()
        {
            var groups = db.Groups.ToList();
            ViewBag.Groups = groups;

            _logger.LogInformation("Открытие страницы: AddStudent1");

            return View();
        }

        public IActionResult AddStudent2(int id_group, string surname, string name, string secondName, string year, string number, string email,
            string city, string street, string house, string flat, string login, string password)
        {
            byte[] data = Encoding.Default.GetBytes(password);
            var result = new SHA256Managed().ComputeHash(data);
            string hash = BitConverter.ToString(result).Replace("-", "").ToUpper();

            _logger.LogInformation("Открытие страницы: AddStudent2");

            User user = new User();
            user.Login = login;
            user.HashPassword = hash;
            user.Type = "student";

            var transaction = db.Database.BeginTransaction();
            try
            {
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
                transaction.Commit();
                _logger.LogInformation("Транзакция завершена успешно");
            }
            catch(Exception ex)
            {
                transaction.Rollback();
                _logger.LogError("Ошибка при добавлении данных");
            }
            return RedirectToAction("Mainpage_Admin", "Admin");
        }

        public IActionResult AddTeacher1()
        {
            var predmets = db.Predmets.ToList();
            ViewBag.Predmets = predmets;
            _logger.LogInformation("Открытие страницы: AddTeacher1");
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

            _logger.LogInformation("Открытие страницы: AddTeacher1");

            var transaction = db.Database.BeginTransaction();
            try
            {

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
                transaction.Commit();
                _logger.LogInformation("Транзакция завершена успешно");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.LogError("Ошибка при добавлении данных");
            }
            return RedirectToAction("Mainpage_Admin", "Admin");
        }

        public IActionResult AddPredmet1()
        {
            _logger.LogInformation("Открытие страницы: AddPredmet1");
            return View();
        }

        public IActionResult AddPredmet2(string namePredmet)
        {
            Predmet predmet = new Predmet();
            predmet.PredmetName = namePredmet;

            _logger.LogInformation("Открытие страницы: AddPredmet1");


            var transaction = db.Database.BeginTransaction();
            try
            {
                db.Predmets.Add(predmet);
                db.SaveChanges();
                transaction.Commit();
                _logger.LogInformation("Транзакция завершена успешно");
            }
            catch(Exception ex)
            {
                transaction.Rollback();
                _logger.LogError("Ошибка при добавлении данных");
            }
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
            _logger.LogInformation("Открытие страницы: AddUPlanUnit1");
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

            _logger.LogInformation("Открытие страницы: AddUPlanUnit2");
            var transaction = db.Database.BeginTransaction();
            try
            {
                db.UplanUnits.Add(uplanUnit);
                db.SaveChanges();
                transaction.Commit();
                _logger.LogInformation("Транзакция завершена успешно");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.LogError("Ошибка при добавлении данных");
            }
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
            _logger.LogInformation("Открытие страницы: AddExam1");
            return View();
        }

        public IActionResult AddExam2(int id, DateTime dateEx)
        {
            Exam exam = new Exam();
            exam.IdPlanUnit = id;
            exam.Date = dateEx;
            
            _logger.LogInformation("Открытие страницы: AddExam2");

            var transaction = db.Database.BeginTransaction();
            try
            {
                db.Exams.Add(exam);
                db.SaveChanges();
                transaction.Commit();
                _logger.LogInformation("Транзакция завершена успешно");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.LogError("Ошибка при добавлении данных");
            }
            return RedirectToAction("Mainpage_Admin", "Admin");
        }

        public IActionResult ShowMarkLog()
        {
            var markLog = from MarkLog in db.MarkLogs
                          join Teacher in db.Teachers on MarkLog.IdTeacherNavigation.IdTeacher equals Teacher.IdTeacher
                          join Uspevaemost in db.Uspevaemosts on MarkLog.IdUspevaemostNavigation.IdUspevaemost equals Uspevaemost.IdUspevaemost
                          join Student in db.Students on Uspevaemost.IdStudentNavigation.IdStudent equals Student.IdStudent
                          join Exam in db.Exams on Uspevaemost.IdExamNavigation.IdExam equals Exam.IdExam
                          join UplanUnit in db.UplanUnits on Exam.IdPlanUnitNavigation.IdUplanUnit equals UplanUnit.IdUplanUnit
                          join TypeOfControl in db.TypeOfControls on UplanUnit.IdTypeOfControlNavigation.IdTypeOfControl equals TypeOfControl.IdTypeOfControl
                          join Predmet in db.Predmets on UplanUnit.IdPredmetNavigation.IdPredmet equals Predmet.IdPredmet
                          join Group in db.Groups on UplanUnit.IdGroupNavigation.IdGroup equals Group.IdGroup
                          orderby MarkLog.IdMarkLog descending
                          select new
                          {
                              Date = MarkLog.DateTime,
                              NameGroup = Group.GroupName,
                              NameStudent = Student.Surname + ' ' + Student.Name + ' ' + Student.SecondName,
                              NameTeacher = Teacher.Surname + ' ' + Teacher.Name + ' ' + Teacher.SecondName,
                              NamePredmet = Predmet.PredmetName,
                              NameType = TypeOfControl.TypeOfControlName,
                              OldMark = MarkLog.OldMark,
                              NewMark = MarkLog.NewMark
                          };

            List<PMarkLog> list = new List<PMarkLog>();
            foreach(var i in markLog)
            {
                list.Add(new PMarkLog(i.Date, i.NameGroup, i.NameStudent, i.NameTeacher, i.NamePredmet, i.NameType, i.OldMark, i.NewMark));
            }
            ViewBag.List = list;
            _logger.LogInformation("Открытие страницы: ShowMarkLog");
            return View();
        }

        public class PMarkLog
        {
            public DateTime Date  { get; set; }
            public string NameGroup { get; set; }
            public string NameStudent { get; set; }
            public string NameTeacher { get; set; }
            public string NamePredmet { get; set; }
            public string NameType { get; set; }
            public string? OldMark { get; set; }
            public string? NewMark { get; set; }
            public PMarkLog(DateTime date, string nameGroup, string nameStudent, string nameTeacher, string namePredmet, string nameType,
                string? oldMark, string? newMark)
                {
                Date = date;
                NameGroup = nameGroup;
                NameStudent = nameStudent;
                NameTeacher = nameTeacher;
                NamePredmet = namePredmet;
                NameType = nameType;
                OldMark = oldMark;
                NewMark = newMark;
                }
        
        }
    }
}
