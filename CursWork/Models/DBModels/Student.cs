using System;
using System.Collections.Generic;

#nullable disable

namespace CursWork.Models.DBModels
{
    public partial class Student
    {
        public Student()
        {
            Uspevaemosts = new HashSet<Uspevaemost>();
        }

        public int IdStudent { get; set; }
        public int IdGroup { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string SecondName { get; set; }
        public string YearOfAppl { get; set; }
        public string ContactNumber { get; set; }
        public string ContactEmail { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string House { get; set; }
        public string Flat { get; set; }
        public int IdUser { get; set; }

        public virtual Group IdGroupNavigation { get; set; }
        public virtual User IdUserNavigation { get; set; }
        public virtual ICollection<Uspevaemost> Uspevaemosts { get; set; }
    }
}
