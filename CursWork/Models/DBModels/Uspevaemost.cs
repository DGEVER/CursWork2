using System;
using System.Collections.Generic;

#nullable disable

namespace CursWork.Models.DBModels
{
    public partial class Uspevaemost
    {
        public Uspevaemost()
        {
            MarkLogs = new HashSet<MarkLog>();
        }

        public int IdUspevaemost { get; set; }
        public int IdStudent { get; set; }
        public int IdExam { get; set; }
        public string Mark { get; set; }

        public virtual Exam IdExamNavigation { get; set; }
        public virtual Student IdStudentNavigation { get; set; }
        public virtual ICollection<MarkLog> MarkLogs { get; set; }
    }
}
