using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace CursWork.Models.DBModels
{
    public partial class TeacherContext : DbContext
    {
        public TeacherContext()
        {
        }

        public TeacherContext(DbContextOptions<TeacherContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Exam> Exams { get; set; }
        public virtual DbSet<FormOfEducat> FormOfEducats { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<MarkLog> MarkLogs { get; set; }
        public virtual DbSet<Predmet> Predmets { get; set; }
        public virtual DbSet<Speciality> Specialities { get; set; }
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<Teacher> Teachers { get; set; }
        public virtual DbSet<TypeOfControl> TypeOfControls { get; set; }
        public virtual DbSet<UplanUnit> UplanUnits { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Uspevaemost> Uspevaemosts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=DESKTOP-T1HQMAS\\SQLEXPRESS;Database=performance2;User id=teacher;Password=teacher;TrustServerCertificate=true;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Cyrillic_General_CI_AS");

            modelBuilder.Entity<Exam>(entity =>
            {
                entity.HasKey(e => e.IdExam);

                entity.ToTable("Exam");

                entity.Property(e => e.IdExam).HasColumnName("id_exam");

                entity.Property(e => e.Date)
                    .HasColumnType("date")
                    .HasColumnName("date");

                entity.Property(e => e.IdPlanUnit).HasColumnName("id_planUnit");

                entity.HasOne(d => d.IdPlanUnitNavigation)
                    .WithMany(p => p.Exams)
                    .HasForeignKey(d => d.IdPlanUnit)
                    .HasConstraintName("FK_Exam_UPlanUnit");
            });

            modelBuilder.Entity<FormOfEducat>(entity =>
            {
                entity.HasKey(e => e.IdForm);

                entity.ToTable("FormOfEducat");

                entity.Property(e => e.IdForm).HasColumnName("id_form");

                entity.Property(e => e.FormName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("form_name");
            });

            modelBuilder.Entity<Group>(entity =>
            {
                entity.HasKey(e => e.IdGroup);

                entity.ToTable("Group");

                entity.Property(e => e.IdGroup).HasColumnName("id_group");

                entity.Property(e => e.GroupName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("group_name");

                entity.Property(e => e.IdFormOfEducation).HasColumnName("id_formOfEducation");

                entity.Property(e => e.IdSpeciality).HasColumnName("id_speciality");

                entity.HasOne(d => d.IdFormOfEducationNavigation)
                    .WithMany(p => p.Groups)
                    .HasForeignKey(d => d.IdFormOfEducation)
                    .HasConstraintName("FK_Group_FormOfEducat");

                entity.HasOne(d => d.IdSpecialityNavigation)
                    .WithMany(p => p.Groups)
                    .HasForeignKey(d => d.IdSpeciality)
                    .HasConstraintName("FK_Group_Speciality");
            });

            modelBuilder.Entity<MarkLog>(entity =>
            {
                entity.HasKey(e => e.IdMarkLog);

                entity.ToTable("MarkLog");

                entity.Property(e => e.IdMarkLog).HasColumnName("id_markLog");

                entity.Property(e => e.DateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("dateTime");

                entity.Property(e => e.IdTeacher).HasColumnName("id_teacher");

                entity.Property(e => e.IdUspevaemost).HasColumnName("id_uspevaemost");

                entity.Property(e => e.NewMark)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("newMark");

                entity.Property(e => e.OldMark)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("oldMark");

                entity.HasOne(d => d.IdTeacherNavigation)
                    .WithMany(p => p.MarkLogs)
                    .HasForeignKey(d => d.IdTeacher)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MarkLog_Teacher");

                entity.HasOne(d => d.IdUspevaemostNavigation)
                    .WithMany(p => p.MarkLogs)
                    .HasForeignKey(d => d.IdUspevaemost)
                    .HasConstraintName("FK_MarkLog_Uspevaemost");
            });

            modelBuilder.Entity<Predmet>(entity =>
            {
                entity.HasKey(e => e.IdPredmet);

                entity.ToTable("Predmet");

                entity.Property(e => e.IdPredmet).HasColumnName("id_predmet");

                entity.Property(e => e.PredmetName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("predmet_name");
            });

            modelBuilder.Entity<Speciality>(entity =>
            {
                entity.HasKey(e => e.IdSpeciality);

                entity.ToTable("Speciality");

                entity.Property(e => e.IdSpeciality).HasColumnName("id_speciality");

                entity.Property(e => e.SpecialityName)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("speciality_name");
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.IdStudent);

                entity.ToTable("Student");

                entity.Property(e => e.IdStudent).HasColumnName("id_student");

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("city");

                entity.Property(e => e.ContactEmail)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("contactEmail");

                entity.Property(e => e.ContactNumber)
                    .IsRequired()
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("contactNumber");

                entity.Property(e => e.Flat)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("flat");

                entity.Property(e => e.House)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("house");

                entity.Property(e => e.IdGroup).HasColumnName("id_group");

                entity.Property(e => e.IdUser).HasColumnName("id_user");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("name");

                entity.Property(e => e.SecondName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("second_name");

                entity.Property(e => e.Street)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("street");

                entity.Property(e => e.Surname)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("surname");

                entity.Property(e => e.YearOfAppl)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("yearOfAppl");

                entity.HasOne(d => d.IdGroupNavigation)
                    .WithMany(p => p.Students)
                    .HasForeignKey(d => d.IdGroup)
                    .HasConstraintName("FK_Student_Group");

                entity.HasOne(d => d.IdUserNavigation)
                    .WithMany(p => p.Students)
                    .HasForeignKey(d => d.IdUser)
                    .HasConstraintName("FK_Student_Users");
            });

            modelBuilder.Entity<Teacher>(entity =>
            {
                entity.HasKey(e => e.IdTeacher);

                entity.ToTable("Teacher");

                entity.Property(e => e.IdTeacher).HasColumnName("id_teacher");

                entity.Property(e => e.ContactNumb)
                    .IsRequired()
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("contactNumb");

                entity.Property(e => e.IdUser).HasColumnName("id_user");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("name");

                entity.Property(e => e.SecondName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("second_name");

                entity.Property(e => e.Surname)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("surname");

                entity.HasOne(d => d.IdUserNavigation)
                    .WithMany(p => p.Teachers)
                    .HasForeignKey(d => d.IdUser)
                    .HasConstraintName("FK_Teacher_Users");
            });

            modelBuilder.Entity<TypeOfControl>(entity =>
            {
                entity.HasKey(e => e.IdTypeOfControl);

                entity.ToTable("TypeOfControl");

                entity.Property(e => e.IdTypeOfControl).HasColumnName("id_typeOfControl");

                entity.Property(e => e.TypeOfControlName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("typeOfControl_name");
            });

            modelBuilder.Entity<UplanUnit>(entity =>
            {
                entity.HasKey(e => e.IdUplanUnit);

                entity.ToTable("UPlanUnit");

                entity.Property(e => e.IdUplanUnit).HasColumnName("id_uplanUnit");

                entity.Property(e => e.CounterOfHours).HasColumnName("counterOfHours");

                entity.Property(e => e.IdGroup).HasColumnName("id_group");

                entity.Property(e => e.IdPredmet).HasColumnName("id_predmet");

                entity.Property(e => e.IdTeacher).HasColumnName("id_teacher");

                entity.Property(e => e.IdTypeOfControl).HasColumnName("id_typeOfControl");

                entity.Property(e => e.Semestr).HasColumnName("semestr");

                entity.HasOne(d => d.IdGroupNavigation)
                    .WithMany(p => p.UplanUnits)
                    .HasForeignKey(d => d.IdGroup)
                    .HasConstraintName("FK_UPlanUnit_Group");

                entity.HasOne(d => d.IdPredmetNavigation)
                    .WithMany(p => p.UplanUnits)
                    .HasForeignKey(d => d.IdPredmet)
                    .HasConstraintName("FK_UPlanUnit_Predmet");

                entity.HasOne(d => d.IdTeacherNavigation)
                    .WithMany(p => p.UplanUnits)
                    .HasForeignKey(d => d.IdTeacher)
                    .HasConstraintName("FK_UPlanUnit_Teacher");

                entity.HasOne(d => d.IdTypeOfControlNavigation)
                    .WithMany(p => p.UplanUnits)
                    .HasForeignKey(d => d.IdTypeOfControl)
                    .HasConstraintName("FK_UPlanUnit_TypeOfControl");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.IdUser);

                entity.Property(e => e.IdUser).HasColumnName("id_user");

                entity.Property(e => e.HashPassword)
                    .IsRequired()
                    .HasMaxLength(65)
                    .IsUnicode(false)
                    .HasColumnName("hashPassword");

                entity.Property(e => e.Login)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("login");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("type");
            });

            modelBuilder.Entity<Uspevaemost>(entity =>
            {
                entity.HasKey(e => e.IdUspevaemost);

                entity.ToTable("Uspevaemost");

                entity.Property(e => e.IdUspevaemost).HasColumnName("id_uspevaemost");

                entity.Property(e => e.IdExam).HasColumnName("id_exam");

                entity.Property(e => e.IdStudent).HasColumnName("id_student");

                entity.Property(e => e.Mark)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("mark");

                entity.HasOne(d => d.IdExamNavigation)
                    .WithMany(p => p.Uspevaemosts)
                    .HasForeignKey(d => d.IdExam)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Uspevaemost_Exam");

                entity.HasOne(d => d.IdStudentNavigation)
                    .WithMany(p => p.Uspevaemosts)
                    .HasForeignKey(d => d.IdStudent)
                    .HasConstraintName("FK_Uspevaemost_Student");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
