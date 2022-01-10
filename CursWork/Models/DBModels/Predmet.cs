using System;
using System.Collections.Generic;

#nullable disable

namespace CursWork.Models.DBModels
{
    public partial class Predmet
    {
        public Predmet()
        {
            UplanUnits = new HashSet<UplanUnit>();
        }

        public int IdPredmet { get; set; }
        public string PredmetName { get; set; }

        public virtual ICollection<UplanUnit> UplanUnits { get; set; }
    }
}
