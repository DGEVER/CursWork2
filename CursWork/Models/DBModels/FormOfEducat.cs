using System;
using System.Collections.Generic;

#nullable disable

namespace CursWork.Models.DBModels
{
    public partial class FormOfEducat
    {
        public FormOfEducat()
        {
            Groups = new HashSet<Group>();
        }

        public int IdForm { get; set; }
        public string FormName { get; set; }

        public virtual ICollection<Group> Groups { get; set; }
    }
}
