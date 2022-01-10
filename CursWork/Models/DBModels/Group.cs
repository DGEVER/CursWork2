using System;
using System.Collections.Generic;

#nullable disable

namespace CursWork.Models.DBModels
{
    public partial class Group
    {
        public Group()
        {
            Students = new HashSet<Student>();
            UplanUnits = new HashSet<UplanUnit>();
        }

        public int IdGroup { get; set; }
        public string GroupName { get; set; }
        public int IdFormOfEducation { get; set; }
        public int IdSpeciality { get; set; }

        public virtual FormOfEducat IdFormOfEducationNavigation { get; set; }
        public virtual Speciality IdSpecialityNavigation { get; set; }
        public virtual ICollection<Student> Students { get; set; }
        public virtual ICollection<UplanUnit> UplanUnits { get; set; }
    }
}
