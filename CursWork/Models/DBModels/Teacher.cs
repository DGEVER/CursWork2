using System;
using System.Collections.Generic;

#nullable disable

namespace CursWork.Models.DBModels
{
    public partial class Teacher
    {
        public Teacher()
        {
            MarkLogs = new HashSet<MarkLog>();
            UplanUnits = new HashSet<UplanUnit>();
        }

        public int IdTeacher { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string SecondName { get; set; }
        public string ContactNumb { get; set; }
        public int IdUser { get; set; }

        public virtual User IdUserNavigation { get; set; }
        public virtual ICollection<MarkLog> MarkLogs { get; set; }
        public virtual ICollection<UplanUnit> UplanUnits { get; set; }
    }
}
