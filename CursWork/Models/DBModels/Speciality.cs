using System;
using System.Collections.Generic;

#nullable disable

namespace CursWork.Models.DBModels
{
    public partial class Speciality
    {
        public Speciality()
        {
            Groups = new HashSet<Group>();
        }

        public int IdSpeciality { get; set; }
        public string SpecialityName { get; set; }

        public virtual ICollection<Group> Groups { get; set; }
    }
}
