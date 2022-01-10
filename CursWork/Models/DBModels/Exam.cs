using System;
using System.Collections.Generic;

#nullable disable

namespace CursWork.Models.DBModels
{
    public partial class Exam
    {
        public Exam()
        {
            Uspevaemosts = new HashSet<Uspevaemost>();
        }

        public int IdExam { get; set; }
        public DateTime Date { get; set; }
        public int IdPlanUnit { get; set; }

        public virtual UplanUnit IdPlanUnitNavigation { get; set; }
        public virtual ICollection<Uspevaemost> Uspevaemosts { get; set; }
    }
}
