using System;
using System.Collections.Generic;

#nullable disable

namespace CursWork.Models.DBModels
{
    public partial class User
    {
        public User()
        {
            Students = new HashSet<Student>();
            Teachers = new HashSet<Teacher>();
        }

        public int IdUser { get; set; }
        public string Login { get; set; }
        public string HashPassword { get; set; }
        public string Type { get; set; }

        public virtual ICollection<Student> Students { get; set; }
        public virtual ICollection<Teacher> Teachers { get; set; }
    }
}
