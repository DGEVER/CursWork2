using System;
using System.Collections.Generic;

#nullable disable

namespace CursWork.Models.DBModels
{
    public partial class UplanUnit
    {
        public UplanUnit()
        {
            Exams = new HashSet<Exam>();
        }

        public int IdUplanUnit { get; set; }
        public int IdGroup { get; set; }
        public int IdTypeOfControl { get; set; }
        public int IdTeacher { get; set; }
        public int IdPredmet { get; set; }
        public int CounterOfHours { get; set; }
        public int Semestr { get; set; }

        public virtual Group IdGroupNavigation { get; set; }
        public virtual Predmet IdPredmetNavigation { get; set; }
        public virtual Teacher IdTeacherNavigation { get; set; }
        public virtual TypeOfControl IdTypeOfControlNavigation { get; set; }
        public virtual ICollection<Exam> Exams { get; set; }
    }
}
