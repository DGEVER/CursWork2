using System;
using System.Collections.Generic;

#nullable disable

namespace CursWork.Models.DBModels
{
    public partial class TypeOfControl
    {
        public TypeOfControl()
        {
            UplanUnits = new HashSet<UplanUnit>();
        }

        public int IdTypeOfControl { get; set; }
        public string TypeOfControlName { get; set; }

        public virtual ICollection<UplanUnit> UplanUnits { get; set; }
    }
}
